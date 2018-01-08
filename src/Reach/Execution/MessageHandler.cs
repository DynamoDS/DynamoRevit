using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Configuration;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.Search.SearchElements;
using PythonNodeModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Reach.Conversion;
using Reach.Messages;
using Reach.Messages.Data;
using Reach.Rendering;
using Reach.Responses;
using Dynamo.Visualization;
using Reach.Synchronization;
using Reach.Responses.Data;
using Reach.Serialization;
using Dynamo.Scheduler;
using System.Xml;
using Dynamo.Graph;
using Dynamo.Graph.Notes;
using Reach.Logging;
using Watch3DNodeModels;
using CoreNodeModels;
using Dynamo.Storage.Data;
using Dynamo.Storage.Upload;
using DynamoConversions;
using System.Text.RegularExpressions;
using Reach.Utilities;

namespace Reach.Execution
{
    public delegate void ResultReadyEventHandler(object sender, ResultReadyEventArgs e);

    public class ResultReadyEventArgs : EventArgs
    {
        public Response Response { get; private set; }
        public string SessionId { get; private set; }

        public ResultReadyEventArgs(Response response, string sessionId)
        {
            SessionId = sessionId;
            Response = response;
        }
    }

    public class MessageHandler : IMessageHandler
    {
        #region Events

        public event ResultReadyEventHandler ResultReady;

        #endregion

        #region Data Members

        private static readonly JsonSerializerSettings jsonSettings;
        private static readonly int maxRunTime;
        private static readonly IRenderPackageFactory renderPackageFactory;
        private const int floodMaxSize = 19000;

        private readonly FileUploader uploader;
        private readonly ICommandVerifier executer;
        private readonly ManualResetEvent nextRunAllowed = new ManualResetEvent(false);
        private readonly ManualResetEvent nodeGeometryComputed = new ManualResetEvent(true);
        private readonly Dictionary<Guid, IEnumerable<CameraData>> cameras = new Dictionary<Guid, IEnumerable<CameraData>>();

        /// <summary>
        /// First Guid is workspace guid. Second Guid is node guid. CameraData is position of the camera.
        /// </summary>
        private readonly Dictionary<Guid, Dictionary<Guid, CameraData>> watch3D_Cameras =
            new Dictionary<Guid, Dictionary<Guid, CameraData>>();
        private readonly ILogger logger;

        private DynamoModel dynamoModel;

        private IRenderPackageCache renderPackageCache;
        //For caching of search elements
        private List<LibraryItem> allLibraryItems;
        private Guid currentWorkspaceGuid = Guid.Empty;

        private IWebClient webClient;

        #endregion

        #region Properties

        /// <summary>
        /// The identifier of message that is currently being executed
        /// </summary>
        public int? CurrentMessageId { get; private set; }

        /// <summary>
        /// The identifier string that represents the current session
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Queue of messages to process
        /// </summary>
        public ActionQueue MessageQueue { get; set; }

        /// <summary>
        /// Calculated results for recently modified nodes
        /// </summary>
        public IEnumerable<ExecutedNode> ExecutedNodes { get; private set; }

        /// <summary>
        /// Collection of serializers which can represent node value in different forms
        /// </summary>
        public IEnumerable<NodeValueSerializerLibrary> NodeValueSerializerLibraries { get; private set; }

        #endregion

        #region Constructors

        static MessageHandler()
        {
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Binder = new MessageSerializationBinder("Dynamo.Models.DynamoModel+{0}, DynamoCore"),
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (!int.TryParse(ConfigurationManager.AppSettings[Constants.MaxRunTimeName], out maxRunTime))
            {
                maxRunTime = Constants.DefaultMaxRunTime;
            }

            renderPackageFactory = new DefaultRenderPackageFactory();
            {
                int maxTessellationDivisions;
                if (int.TryParse(ConfigurationManager.AppSettings["MaxTessellationDivisions"], out maxTessellationDivisions))
                    renderPackageFactory.TessellationParameters.MaxTessellationDivisions = maxTessellationDivisions;
            }
        }

        public MessageHandler(DynamoModel dynamoModel, ILogger logger, ICommandVerifier commandVerifier, IWebClient client)
        {
            this.dynamoModel = dynamoModel;
            executer = commandVerifier;

            var serializerPath = ConfigurationManager.AppSettings["NodeValueSerializerPath"];
            NodeValueSerializerLibraries = new NodeValueSerializerLoader().LoadDirectory(serializerPath);
            uploader = new FileUploader();
            this.logger = logger;
            MessageQueue = new ActionQueue();

            this.dynamoModel.EvaluationCompleted += RunCommandCompleted;
            this.dynamoModel.RefreshCompleted += RunRefreshCompleted;
            this.dynamoModel.WorkspaceOpening += ReadCameraDataFromFile;

            webClient = client;
        }

        public MessageHandler(DynamoModel dynamoModel, ILogger logger)
            : this(dynamoModel, logger, new TolerantCommandVerifier(dynamoModel), new SystemWebClient()) { }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            MessageQueue.Dispose();
            dynamoModel.EvaluationCompleted -= RunCommandCompleted;
            dynamoModel.RefreshCompleted -= RunRefreshCompleted;
            dynamoModel.WorkspaceOpening -= ReadCameraDataFromFile;
            dynamoModel.ShutDown(true);
            dynamoModel = null;

            GC.Collect();
        }

        /// <summary>
        /// This method serializes the Message object in the json form.
        /// </summary>
        /// <returns>The string can be used for reconstructing Message using Deserialize method</returns>
        public string SerializeMessage(Message message)
        {
            return message == null ? null : JsonConvert.SerializeObject(message, jsonSettings);
        }

        /// <summary>
        /// Call this method to reconstruct a Message from json string
        /// </summary>
        /// <param name="jsonString">Json string that contains all its arguments.</param>
        /// <returns>Reconstructed Message</returns>
        public Message DeserializeMessage(string jsonString)
        {
            Message result = null;
            try
            {
                var jObject = JObject.Parse(jsonString);
                var type = GetTypeFromString(jObject["type"].Value<string>());

                result = JsonConvert.DeserializeObject(jsonString, type, jsonSettings) as Message;
            }
            catch { }

            if (result == null)
            {
                throw new ArgumentException("Invalid jsonString for creating Message: '" + jsonString + "'");
            }

            return result;
        }

        /// <summary>
        /// Execute Message on current dynamoModel and session
        /// </summary>
        /// <param name="message">Message to execute</param>
        public void Execute(Message message)
        {
            CurrentMessageId = message.UniqueID;
            message.Execute(this);
        }

        /// <summary>
        /// Enqueue a message to execute it on current dynamoModel and session if it is specified
        /// Or execute immediately
        /// </summary>
        /// <param name="message">Message to execute</param>
        /// <param name="enqueue">Flag which indicates if it should be enqueued</param>
        public void ExecuteMessage(Message message, bool enqueue)
        {
            if (enqueue)
            {
                MessageQueue.Enqueue(() => Execute(message));
            }
            else
            {
                Execute(message);
            }
        }

        /// <summary>
        /// Retrieve a collection of all library items
        /// </summary>
        /// <returns>Collection of all library items</returns>
        public IEnumerable<LibraryItem> GetAllLibraryItemsByCategory()
        {
            if (!ListIsNullOrEmpty(allLibraryItems)) return allLibraryItems;

            allLibraryItems = new List<LibraryItem>();
            foreach (var elem in dynamoModel.SearchModel.SearchEntries)
            {
                //skip nodes that we should filter
                if (!NodeFilter.IsValidNode(elem.CreationName, elem.Assembly)) continue;

                //set keywords and weights on library item
                var libItem = new LibraryItem(elem,
                    keywords: dynamoModel.SearchModel.GetTags(elem),
                    keywordWeights: dynamoModel.SearchModel.GetWeights(elem));

                var functionItem = dynamoModel.LibraryServices.GetFunctionDescriptor(libItem.CreationName);
                //set input and output port data
                SetPorts(libItem, elem,functionItem);
                //set arguments
                SetArguments(libItem, functionItem);
                allLibraryItems.Add(libItem);
            }

            return allLibraryItems;
        }

        #endregion

        #region Message executers

        internal void SendSerializedNodeValue(GetSerializedNodeValuesMessage serializedMessage)
        {
            try
            {
                var format = serializedMessage.Format;
                var serializer = GetSerializer(format);

                if (serializer == null)
                {
                    SendResponse(new ContentResponse("Empty serializer", ResponseStatuses.Error));
                    return;
                }

                var nodeIds = new HashSet<string>(serializedMessage.NodeIds);
                var nodes = dynamoModel.CurrentWorkspace.Nodes.Where(n => nodeIds.Contains(n.GUID.ToString()));
                var parameters = serializedMessage.Parameters ?? new Dictionary<string, object>();
                var serializedNodes = new List<Responses.Data.SerializedNodeValueData>();

                foreach(var node in nodes)
                {
                    string nodeId = node.GUID.ToString();
                    string nodeValue = serializer.Serialize(node.CachedValue, parameters);

                    serializedNodes.Add(new Responses.Data.SerializedNodeValueData
                    {
                        NodeId = nodeId,
                        Value = nodeValue
                    });

                    nodeIds.Remove(nodeId);
                }

                foreach(var nodeId in nodeIds)
                {
                    serializedNodes.Add(new Responses.Data.SerializedNodeValueData
                    {
                        NodeId = nodeId,
                        Value = null
                    });
                }

                SendResponse(new SerializedNodeValuesResponse(serializedNodes, format));
            }
            catch (Exception ex)
            {
                SendResponse(new ContentResponse(ex.Message, ResponseStatuses.Error));
            }
        }

        internal void ExecuteCommands(RunCommandsMessage message)
        {
            var commands = message.Commands;
            if (!commands.Any()) return;

            var nodeCreationFailures = new List<string>();

            // If the first command is not a custom node creation command, and
            // you are not able to switch to that workspace (i.e. the workspace is not loaded in dynamo),
            // then do not attempt to execute any of the commands in this message.
            var createCustomNodeCommand = commands.ElementAt(0) as DynamoModel.CreateCustomNodeCommand;
            if (createCustomNodeCommand == null && !SetCurrentWorkspace(message.WorkspaceGuid))
            {
                return;
            }

            for (var i = 0; i < commands.Count(); i++)
            {
                // silence changes from custom node workspaces, we'll handle them in one single call at the end of the loop
                foreach (var ws in dynamoModel.Workspaces.OfType<CustomNodeWorkspaceModel>())
                {
                    ws.SilenceDefinitionUpdated = true;
                }

                var command = commands.ElementAt(i);
                if (command is DynamoModel.MakeConnectionCommand)
                {
                    var secondCommand = commands.ElementAt(++i);
                    if (secondCommand is DynamoModel.MakeConnectionCommand)
                    {
                        executer.ExecuteCommands(command as DynamoModel.MakeConnectionCommand,
                            secondCommand as DynamoModel.MakeConnectionCommand);
                    }
                    else
                    {
                        executer.ExecuteCommand(secondCommand);
                    }

                    continue;
                }

                if (command is DynamoModel.RunCancelCommand)
                {
                    ResetBeforeNewRun();
                }

                var updateCommand = command as DynamoModel.UpdateModelValueCommand;
                if (renderPackageCache != null && updateCommand != null && updateCommand.Name == "IsVisible")
                {
                    nodeGeometryComputed.Reset();
                }

                var commandWasAllowed = executer.ExecuteCommand(command);
                if (!commandWasAllowed)
                {
                    // If a node creation command was not allowed, add
                    // it to the node creation failures collection.
                    var cmd = command as DynamoModel.CreateNodeCommand;
                    if (cmd != null)
                    {
                        nodeCreationFailures.Add(cmd.Name);
                    }
                    continue;
                }

                if (command is DynamoModel.RunCancelCommand)
                {
                    WaitForRunCompletion();
                    NodeDataModified(message.SendGeometryImmediately);
                    continue;
                }

                if (updateCommand == null) continue;

                // if IsVisible is being changed,
                // it needs to wait for geometry re-computation is done
                if (commands.Count() == 1 && updateCommand.Name == "IsVisible")
                {
                    // if geometry re-computation task has not been scheduled
                    // nodeGeometryComputed will be signaled and no waiting will occur
                    // only if the event is reset.
                  //  if (nodeGeometryComputed.WaitOne(0))
                        nodeGeometryComputed.WaitOne(maxRunTime);
                }

                var model = dynamoModel.CurrentWorkspace.GetModelInternal(updateCommand.ModelGuid);

                var cbn = model as CodeBlockNodeModel;
                var pyn = model as PythonNode;
                var fn = model as CoreNodeModels.Formula;
                if (cbn != null && updateCommand.Name == PropertyNames.Code)
                // if a cbn is updated send back the data to redraw it in Flood
                {
                    var wsGuid = GetCurrentWorkspaceGuid();
                    var nodeGuid = updateCommand.ModelGuid.ToString();
                    var jData = cbn.GetInOutPortsData();

                    var data = JsonConvert.DeserializeObject<CodeBlockData>(jData);

                    SendResponse(new CodeBlockDataResponse(wsGuid, nodeGuid, data));
                }
                else if (pyn != null && updateCommand.Name == PropertyNames.ScriptContent)
                // Is PythonScript is updated call `OnNodeModified` method to update node
                {
                    pyn.OnNodeModified();
                }
                else if (fn != null && updateCommand.Name == PropertyNames.Formula)
                {
                    var wsGuid = GetCurrentWorkspaceGuid();
                    var nodeGuid = updateCommand.ModelGuid.ToString();
                    var jData = fn.GetInOutPortsData();
                    var data = JsonConvert.DeserializeObject<FormulaData>(jData);

                    var script = fn.FormulaString;

                    SendResponse(new FormulaResponse(wsGuid, nodeGuid, data, script));
                }
            }

            if (nodeCreationFailures.Any())
            {
                SendResponse(new NodeAvailabilityCheckResponse(nodeCreationFailures));
            }

            // update all custom node definitions
            foreach (var ws in this.dynamoModel.Workspaces.OfType<CustomNodeWorkspaceModel>())
            {
                ws.SilenceDefinitionUpdated = false;
                ws.OnDefinitionUpdated();
            }
        }

        internal void SendAllLibraryItems()
        {
            SendResponse(new LibraryItemsListResponse(GetAllLibraryItemsByCategory()));
        }

        internal void SaveFile(SaveFileMessage message)
        {
            var workspaceToSave = GetWorkspaceById(message.Guid);
            if (workspaceToSave == null) return;

            DependentFileStorage.Create();
            string tempPathForDependencies = DependentFileStorage.CreateLocalStorageForWorkspace(workspaceToSave.Guid);

            var guid = GetWorkspaceGuid(workspaceToSave);
            Action<XmlDocument> onSaving = doc =>
            {
                AddCameraDataToFile(doc, guid);
                AddWatch3DNodesCameraDataToFile(doc, guid);
            };
            try
            {
                var filePath = message.FilePath;
                workspaceToSave.Saving += onSaving;

                // if path was specified it means NWK is used and we need just to save file
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        workspaceToSave.Save(filePath);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Failed to save file: {0}", filePath));
                    }
                }
                else
                {
                    byte[] fileContent;
                    // Add to file name a correct extension
                    // dependently on its type (custom node or home)
                    var extension = workspaceToSave is CustomNodeWorkspaceModel
                        ? Constants.DyfFileExtension
                        : Constants.DynFileExtension;
                    var fileName = (message.FileName ?? Constants.DefaultCustomNodeName) + extension;

                    filePath = Path.Combine(tempPathForDependencies, fileName);

                    // Temporarily save workspaces into a drive
                    // using existing functionality for saving
                    try
                    {
                        workspaceToSave.Save(filePath);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Failed to save file: {0}", filePath));
                    }

                    // If the workspace has custom node dependencies then save them as well
                    // if the workspace has file dependencies, they will already be in the folder

                    WorkspaceDependencies deps =
                        WorkspaceDependencies.Collect(workspaceToSave, this.dynamoModel.CustomNodeManager);

                    if (deps != null && deps.CustomNodeWorkspaces.Any())
                    {
                        foreach (var cswkspace in deps.CustomNodeWorkspaces)
                        {
                            var currentFileName = (cswkspace.Name ?? Constants.DefaultCustomNodeName) + Constants.DyfFileExtension;
                            var currentFilePath = Path.Combine(tempPathForDependencies, currentFileName);

                            try
                            {
                                cswkspace.Save(currentFilePath);
                            }
                            catch
                            {
                                throw new Exception(string.Format("Failed to save file: {0}", currentFilePath));
                            }

                            //reset filepath for each dependency
                            cswkspace.FileName = null;
                        }
                    }

                    if (Directory.GetFileSystemEntries(tempPathForDependencies).Count() > 1)
                    {
                        // zip everything up and set the filepath to point to the final zipped file
                        fileName = Path.ChangeExtension(fileName, ".zip");
                        string zipFilePath = Path.GetTempPath() + fileName;
                        var zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                        zip.CreateZip(zipFilePath, tempPathForDependencies, true, null);
                        File.Delete(filePath);
                        filePath = zipFilePath;
                    }

                    // Get the file as byte array
                    fileContent = File.ReadAllBytes(filePath);
                    // delete the file first
                    File.Delete(filePath);

                    // Send to the Flood the file as byte array and its name
                    SendResponse(new SavedFileResponse(fileName,
                        fileContent,
                        string.Format("workspace {0} saved",workspaceToSave.Name)));
                }

                // reset file path
                workspaceToSave.FileName = null;
            }
            catch (Exception e)
            {
                // If there was something wrong
                SendResponse(new SavedFileResponse(
                    ResponseStatuses.Error,
                    string.Format("problem saving workspace {0}:{1}", workspaceToSave.Name, e.Message)));

                logger.Log(string.Format("problem saving workspace {0}:{1}", workspaceToSave.Name, e.Message));
            }
            finally
            {
                workspaceToSave.Saving -= onSaving;
            }
        }

        internal void UploadFile(UploadFileMessage message)
        {
            var result = uploader.ProcessFileData(message, dynamoModel);
            if (result != ProcessResult.Failed)
            {
                Guid.TryParse(GetCurrentWorkspaceGuid(), out currentWorkspaceGuid);

                SetCurrentWorkspaceToFirstHomeWorkspace();
                ResetBeforeNewRun();
                dynamoModel.ExecuteCommand(new DynamoModel.RunCancelCommand(false, false));
                WaitForRunCompletion();
                var respondPath = (result == ProcessResult.RespondWithPath) ? message.Path : null;
                NewWorkspaceCreated(respondPath);
            }
            else
            {
                var response = new UploadFileResponse(ResponseStatuses.Error, "Cannot upload the file",
                    uploader.InvalidNodeNames);
                SendResponse(response);
            }
        }

        internal void DownloadDependency(DownloadDependencyMessage message)
        {
            try
            {
                var workspace = dynamoModel.CurrentWorkspace;
                if (message.WorkspaceId != Guid.Empty && message.WorkspaceId != workspace.Guid)
                {
                    workspace = dynamoModel.CustomNodeManager.GetWorkspaceById(message.WorkspaceId);
                }

                // Establish where we're going to put the downloaded file
                DependentFileStorage.Create();

                // Set app current working directory so relative path resolution will work as expected
                string workspaceDirectory = DependentFileStorage.CreateLocalStorageForWorkspace(workspace.Guid);
                Directory.SetCurrentDirectory(workspaceDirectory);

                // Create an unique local path for this dependency
                string localFilePath = DependentFileStorage.CreateLocalPathForDependentFile(workspace.Guid, message.NodeIds, message.FileName);

                // Expected common case: standard HTTP URL
                if (message.Url.StartsWith("http", true, null))
                {
                    webClient.DownloadFile(message.Url, localFilePath);

                    SendResponse(new DownloadDependencyResponse(
                        ResponseStatuses.Success,
                        string.Format("Dependency {0} retrieved", message.FileName)));
                }
                else
                {
                    // Otherwise, check for RFC 2397 "data" URL scheme
                    var match = Regex.Match(message.Url, @"data:(?<mimetype>.+?),(?<data>.+)");
                    if (match.Success)
                    {
                        string base64Data = match.Groups["data"].Value;
                        byte[] binData = Convert.FromBase64String(base64Data);

                        using (var memoryStream = new MemoryStream(binData))
                        {
                            using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                            {
                                memoryStream.WriteTo(fileStream);
                                SendResponse(new DownloadDependencyResponse(
                                    ResponseStatuses.Success,
                                    string.Format("Dependency {0} retrieved", message.FileName)));
                            }
                        }
                    }
                    else
                    {
                        // Unsupported protocol
                        // Avoid logging or responding with a full giant malformed data URL
                        var urlExcerpt = message.Url.Length < 32 ? message.Url : message.Url.Substring(0, 32);
                        string strMessage = string.Format("Unrecognized URL format: {0}", urlExcerpt);
                        logger.Log(strMessage);

                        SendResponse(new DownloadDependencyResponse(
                            ResponseStatuses.Error,
                            strMessage));

                        return;
                    }
                }

                // Now update nodes in the workspace that depends on this file to point to the new local path

                var query = from node in workspace.Nodes
                            where message.NodeIds.OfType<Guid>().Contains(node.GUID)
                            select node;

                foreach (NodeModel node in query)
                {
                    string relativePath = localFilePath.Replace(workspaceDirectory, ".");
                    logger.Log("Changing path value of " + node.GetType().Name +
                               " node with id " + node.GUID.ToString() +
                               " from " + node.GetValue(dynamoModel.EngineController) +
                               " to " + relativePath);

                    node.UpdateValue(new UpdateValueParams("Value", relativePath));
                }

            }
            catch (Exception e)
            {
                string strMessage = string.Format("Problem retrieving dependency {0}: {1}", message.FileName, e.Message);
                logger.Log(strMessage);

                SendResponse(new DownloadDependencyResponse(
                   ResponseStatuses.Error,
                   strMessage
                   ));
            }
        }

        internal void RetrieveGeometry(GetNodeGeometryMessage message)
        {
            RetrieveGeometry(message.NodeId);
        }

        internal void RetrieveArrayItems(GetNodeArrayItemsMessage message)
        {
            Guid guid;
            var nodes = GetWorkspaceById(null).Nodes;
            if (!Guid.TryParse(message.NodeId, out guid)) return;

            var model = nodes.FirstOrDefault(n => n.GUID == guid);

            if (model != null)
            {
                SendResponse(new ArrayItemsDataResponse(model, message.IndexFrom, message.Length));
            }
        }

        /// <summary>
        /// Cleanup Home workspace and remove all custom nodes
        /// </summary>
        internal void ClearWorkspace(ClearWorkspaceMessage message)
        {
            SetCurrentWorkspaceToFirstHomeWorkspace();

            if (!message.ClearOnlyHome)
            {
                var allCustomNodeGuids = dynamoModel.CustomNodeManager.NodeInfos.Keys.ToList();
                foreach (var guid in allCustomNodeGuids)
                {
                    dynamoModel.CustomNodeManager.Remove(guid);
                }
            }

            dynamoModel.ClearCurrentWorkspace();
            dynamoModel.ResetEngine();
            (dynamoModel.CurrentWorkspace as HomeWorkspaceModel).RunSettings.RunType = RunType.Manual;

            if(renderPackageCache != null)
            {
                renderPackageCache.RequestNodeVisualUpdate -= RequestNodeVisualUpdate;
                renderPackageCache.RenderPackagesUpdated -= OnRenderPackagesUpdated;
                renderPackageCache.Dispose();
                renderPackageCache = null;
            }
        }

        internal void SendUnsavedChangesInfo(HasUnsavedChangesMessage message)
        {
            var guid = message.WorkspaceGuid;
            var workspace = GetWorkspaceById(guid);

            if (workspace != null)
            {
                SendResponse(new HasUnsavedChangesResponse(guid, workspace.HasUnsavedChanges));
            }
        }

        internal void SetCameraData(CameraDataMessage message)
        {
            if (message.CameraData == null) return;

            Guid guid;
            Guid.TryParse(message.WorkspaceGuid, out guid);
            cameras[guid] = message.CameraData;
        }

        /// <summary>
        /// On SetWatch3DCamerasMessage we save all cameras of watch 3d nodes.
        /// </summary>
        /// <param name="message">SetWatch3DCamerasMessage</param>
        internal void SetWatch3DCameras(SetWatch3DCameraMessage message)
        {
            if (message == null ||
                message.Cameras == null ||
                message.NodeGuids == null ||
                message.NodeGuids.Count != message.Cameras.Count)
            {
                return;
            }

            Guid wsGuid;
            if (Guid.TryParse(message.WorkspaceGuid, out wsGuid))
            {
                var temp = new Dictionary<Guid, CameraData>();
                for (int i = 0; i < message.NodeGuids.Count; i++)
                {
                    temp.Add(message.NodeGuids[i], message.Cameras[i]);
                }

                watch3D_Cameras.Add(wsGuid, temp);
            }
        }

        /// <summary>
        /// On WorkspaceSettingsMessage we save all workspace settings such as x, y offsets, zoom.
        /// </summary>
        /// <param name="message">Workspace settings message</param>
        internal void SetWorkspaceSettings(WorkspaceSettingsMessage message)
        {
            if (message == null) return;

            var workspace = dynamoModel.Workspaces.FirstOrDefault(ws => ws.Guid.ToString() == message.Guid);
            if (workspace == null && String.IsNullOrEmpty(message.Guid))
            {
                workspace = dynamoModel.Workspaces.First();
            }
            workspace.X = message.X;
            workspace.Y = message.Y;
            workspace.Zoom = message.Zoom;
        }

        /// <summary>
        /// Restores nodes' positions  in early uploaded .dyn file
        /// </summary>
        /// <param name="message">Restore positions message</param>
        internal void RestoreNodesPositions(RestoreNodesPositionsMessage message)
        {
            // when we save shared workspace back to .dyn file it is preferable
            // to restore initial nodes' coordinates on canvas.
            double minX, maxX, minY, maxY;
            var workspace = dynamoModel.Workspaces.FirstOrDefault(ws => ws.Guid.ToString() == message.WorkspaceGuid);
            if (workspace == null && String.IsNullOrEmpty(message.WorkspaceGuid))
            {
                workspace = dynamoModel.Workspaces.First();
            }
            minX = workspace.Nodes.Select(n => n.X).Min();
            maxX = workspace.Nodes.Select(n => n.X).Max();

            minY = workspace.Nodes.Select(n => n.Y).Min();
            maxY = workspace.Nodes.Select(n => n.Y).Max();
            var dynamoBox = message.BoundingBox;

            // next step is to compute bias for each node
            double biasX = 0, biasY = 0;
            if (dynamoBox.MinX < 0)
                biasX = dynamoBox.MinX;
            else if (dynamoBox.MaxX > floodMaxSize)
                biasX = floodMaxSize - dynamoBox.MaxX;

            if (dynamoBox.MinY < 0)
                biasY = dynamoBox.MinY;
            else if (dynamoBox.MaxY > floodMaxSize)
                biasY = floodMaxSize - dynamoBox.MaxY;

            // ultimatly we cast each node's position do correct dynamo representation
            // using pre-computed biases
            workspace.Nodes.ToList().ForEach((node) =>
            {
                node.X = (Math.Abs(dynamoBox.MinX - dynamoBox.MaxX) > floodMaxSize)
                    ? TransformCoordinates(minX, maxX, dynamoBox.MinX, dynamoBox.MaxX, node.X)
                    : node.X + biasX;

                node.Y = (Math.Abs(dynamoBox.MinY - dynamoBox.MaxY) > floodMaxSize)
                    ? TransformCoordinates(minY, maxY, dynamoBox.MinY, dynamoBox.MaxY, node.X)
                    : node.Y + biasY;
            });
        }

        // this is helper method which allows to compute node's coordinate in extended/compressed canvases
        private double TransformCoordinates(double r1, double l1, double r2, double l2, double x)
        {
            double res = r2*(x - l1) + l2*(r1 - x);
            return res / (r1 - l1);
        }

        internal void RestoreDynamoGroups(RestoreDynamoGroupsMessage message)
        {
            if (ListIsNullOrEmpty(message.Groups)) return;

            var workspace = GetWorkspaceById(message.WorkspaceGuid);
            if (workspace == null) return;

            workspace.ClearAnnotations();
            foreach (var group in message.Groups)
            {
                Guid groupId;
                if (!Guid.TryParse(group.GroupId, out groupId)) continue;

                // select group's nodes and notes
                workspace.Nodes.Concat<ModelBase>(workspace.Notes).ToList().ForEach(n => n.IsSelected = false);
                foreach (var id in group.Models)
                {
                    Guid guid;
                    if (!Guid.TryParse(id, out guid)) continue;

                    var model = workspace.GetModelInternal(guid);
                    if (!(model is NodeModel) && !(model is NoteModel)) continue;

                    model.IsSelected = true;
                }

                var addAnnotation = new DynamoModel.CreateAnnotationCommand(groupId, group.Text, 0, 0, false);
                dynamoModel.ExecuteCommand(addAnnotation);

                var annotation = workspace.Annotations.FirstOrDefault(a => a.GUID == groupId);
                // it won't be created if nothing is selected to add inside of it
                if (annotation == null) continue;

                group.SetPropertiesToAnnotation(annotation);
            }
        }

        #endregion

        #region Private instance methods

        /// <summary>
        /// It's called after all computations are done and Dynamo is ready
        /// to compute graphics data
        /// </summary>
        private void RunCommandCompleted(object sender, EvaluationCompletedEventArgs e)
        {
            if (e.EvaluationSucceeded)
            {
                // Send computation response for all nodes in error state
                SendComputationResponseForNodesInErrorState();
            }

            if (e.EvaluationTookPlace)
            {
                var workspace = GetWorkspaceById(null) as HomeWorkspaceModel;

                // only hold onto render packages for one run
                if (renderPackageCache != null)
                {
                    renderPackageCache.RequestNodeVisualUpdate -= RequestNodeVisualUpdate;
                    renderPackageCache.RenderPackagesUpdated -= OnRenderPackagesUpdated;
                    renderPackageCache.Dispose();
                }

                renderPackageCache = new WorkspaceRenderPackageCache(workspace);
                renderPackageCache.RequestNodeVisualUpdate += RequestNodeVisualUpdate;

                var updatedNodes = workspace.Nodes.Where(n => n.WasInvolvedInExecution).ToList();
                if (updatedNodes.Any())
                {
                    // Get each node in workspace to update their visuals.
                    foreach (var node in updatedNodes)
                    {
                        RequestNodeVisualUpdate(node);
                    }

                    return;
                }
            }

            RunWasNotCompleted();
        }

        /// <summary>
        /// It's called after all tasks in scheduler are finished and
        /// Dynamo is ready to send ComputationResponse
        /// </summary>
        private void RunRefreshCompleted(HomeWorkspaceModel model)
        {
            if (dynamoModel.CurrentWorkspace != model)
            {
                // It may happen when a new Home workspace is open,
                // so we do not need an old one anymore
                return;
            }

            var task = new GetExecutedNodesAsyncTask(dynamoModel.Scheduler,
                model, dynamoModel.EngineController, renderPackageCache);
            task.Completed += GetExecutedNodesCompleted;
            dynamoModel.Scheduler.ScheduleForExecution(task);
        }

        private void ReadCameraDataFromFile(object doc)
        {   
            if(!(doc is XmlDocument))
            {
                return;
            }
            var camerasElements = (doc as XmlDocument).GetElementsByTagName("Cameras");
            if (camerasElements.Count == 0) return;

            var wsCameras = new List<CameraData>();
            foreach (XmlNode cameraNode in camerasElements[0].ChildNodes)
            {
                wsCameras.Add(CameraData.DeserializeCamera(cameraNode));
            }

            // Get the id of the Workspace being opened.
            var guid = GetWorkspaceGuid(dynamoModel.Workspaces.Last());
            cameras[guid] = wsCameras;
        }

        private void AddCameraDataToFile(XmlDocument doc, Guid workspaceGuid)
        {
            var root = doc.DocumentElement;
            if (root == null || !cameras.ContainsKey(workspaceGuid)) return;

            var camerasElement = doc.CreateElement("Cameras");
            foreach (var c in cameras[workspaceGuid])
            {
                c.SerializeCamera(camerasElement);
            }

            root.AppendChild(camerasElement);
            cameras.Remove(workspaceGuid);
        }

        /// <summary>
        /// Adds watch 3d cameras information in xml file.
        /// </summary>
        /// <param name="doc">Xml file, which represents workspace.</param>
        /// <param name="workspaceGuid">Guid of workspace</param>
        private void AddWatch3DNodesCameraDataToFile(XmlDocument doc, Guid workspaceGuid)
        {
            var root = doc.DocumentElement;
            if (root == null || !watch3D_Cameras.ContainsKey(workspaceGuid)) return;

            var cameras = watch3D_Cameras[workspaceGuid];
            var nodes = root.SelectNodes("/Workspace/Elements/Watch3DNodeModels.Watch3D");

            foreach (XmlNode node in nodes)
            {
                var a = node.Attributes["guid"].Value;
                var nodeGuid = new Guid(a);
                if (!cameras.ContainsKey(nodeGuid))
                {
                    continue;
                }

                var camera = cameras[nodeGuid];
                camera.SerializeCamera(node["view"]);
            }

        }

        private void RequestNodeVisualUpdate(NodeModel node)
        {
            if (!node.RequestVisualUpdateAsync(dynamoModel.Scheduler,
                dynamoModel.EngineController,
                renderPackageFactory))
            {
                // if geometry re-computation has not been scheduled
                // notify calling thread to not wait for re-computation end
                nodeGeometryComputed.Set();
            }
        }

        private void GetExecutedNodesCompleted(AsyncTask task)
        {
            ExecutedNodes = ((GetExecutedNodesAsyncTask)task).Nodes;
            nextRunAllowed.Set();
            // subscribe after run is completed
            renderPackageCache.RenderPackagesUpdated += OnRenderPackagesUpdated;
        }

        private void OnRenderPackagesUpdated()
        {
            nodeGeometryComputed.Set();
        }

        /// <summary>
        /// Send the results of the execution
        /// </summary>
        private void OnResultReady(object sender, ResultReadyEventArgs e)
        {
            if (ResultReady == null) return;

            e.Response.UniqueID = CurrentMessageId;
            ResultReady(sender, e);
        }

        private void SendResponse(Response response)
        {
            OnResultReady(this, new ResultReadyEventArgs(response, SessionId));
        }

        private void SetArguments(LibraryItem item, Dynamo.Engine.FunctionDescriptor functionItem)
        {
            if (functionItem != null && functionItem.IsOverloaded)
            {
                item.Arguments = functionItem.Parameters.Select(x => x.Name);
            }
        }

        private void SetPorts(LibraryItem item, NodeSearchElement nodeEl, Dynamo.Engine.FunctionDescriptor functionItem)
        {

            if (functionItem != null)
            {
                item.DisplayName = functionItem.DisplayName;
            }


            var newElement = nodeEl.CreateNode();

            if (newElement == null)
                throw new TypeLoadException("Unable to create instance of NodeModel element by CreationName");

            if (newElement is DynamoConvert)
            {
                item.Extra = new
                {
                    UnitSource = Conversions.ConversionMetricLookup.Select(p =>
                            new { Metric = p.Key.ToString(), Values = p.Value.Select(v => v.ToString()) }).ToList(),
                    DefaultValues = Conversions.ConversionDefaults.Select(p =>
                            new { Metric = p.Key.ToString(), Value = p.Value.ToString() }).ToList()
                };
            }

            item.Parameters = newElement.InPorts.Select((elem, index) => 
            {
                var type = elem.ToolTip.Split('.').Last();
                //if functionItem exists, try to get any input type information that exists on it
                if (functionItem != null)
                {
                    if (!functionItem.InputParameters.Any())
                    {
                       type = functionItem.InputParameters.ElementAtOrDefault(index).Item2 ?? type;
                    }
                }
                    return new LibraryItem.PortInfo
                    {
                        Name = elem.Name,
                        Type = type,
                        DefaultValue = LookupPortDefaultValue(newElement, elem.Index),
                        ToolTip = elem.ToolTip
                    };
                });

            item.ReturnKeys = newElement.OutPorts.Select((elem, index) =>
            {
                //fallback to string manipulation
                var type = elem.ToolTip.Split('.').Last();
                //if functionItem exists, try to get any output information that exists on it
                if (functionItem != null)
                {
                    //if the outputs are typed use these type hints
                    if (functionItem.Returns.Any())
                    {
                       var tuple = functionItem.Returns.ElementAtOrDefault(index);
                       type =  tuple != null ? tuple.Item2 : type;
                    }
                    //if the node does not return a dictionary
                    //we can use the return type on the function description
                    else if(functionItem.ReturnKeys == null || functionItem.ReturnKeys.Any())
                    {
                        type = functionItem.ReturnType.Name;
                    }
                }

                return new LibraryItem.PortInfo
                {
                    Name = elem.Name,
                    ToolTip = elem.ToolTip,
                    Type = type
                };
            });
        }

        private INodeValueSerializer GetSerializer(string format)
        {
            var libs = NodeValueSerializerLibraries.Where(l => l.CanGetSerializer(format));
            return libs.Any() ? libs.First().GetSerializer() : null;
        }

        private void RunWasNotCompleted()
        {
            // ExecutedNodes=null means, the computation has not finished yet.
            // So ExecutedNodes=emptyList means, the computation has finished,
            // but there are no modified nodes
            ExecutedNodes = Enumerable.Empty<ExecutedNode>();
            nextRunAllowed.Set();
        }

        // Send computation response for all nodes in error state
        private void SendComputationResponseForNodesInErrorState()
        {
            var nodes = (from node in dynamoModel.CurrentWorkspace.Nodes
                    where node.IsInErrorState
                    let data = node.GetValue(dynamoModel.EngineController)
                    select new ExecutedNode(node, data, false)).ToList();

            if (!ListIsNullOrEmpty(nodes))
            {
                SendResponse(new ComputationResponse(nodes, false));
            }
        }


        private string GetCurrentWorkspaceGuid()
        {
            var cnModel = dynamoModel.CurrentWorkspace as CustomNodeWorkspaceModel;

            return cnModel != null ? cnModel.CustomNodeDefinition.FunctionId.ToString() : "";
        }

        private WorkspaceModel GetWorkspaceById(string idStr)
        {
            // Workspaces were previously put into the system with their 'guid' set to 
            // all zeros. For these workspaces, we'll default to using the first
            // HomeWorkspaceModel. 
            Guid guidValue;
            if (!Guid.TryParse(idStr, out guidValue) || guidValue.Equals(Guid.Empty))
            {
                return dynamoModel.Workspaces.First(w => w is HomeWorkspaceModel);
            }

            // If the guid is not empty, assume it's a custom node...
            var ws = dynamoModel.CustomNodeManager.GetWorkspaceById(guidValue) as WorkspaceModel;
            if (ws != null)
            {
                return ws;
            }

            // If we're here, the guid has a value, and it isn't a custom node.
            // Try to resolve it as a home workspace.
            ws = dynamoModel.Workspaces.FirstOrDefault(w => w.Guid == guidValue);
            if (ws != null)
            {
                return ws;
            }

            // If all else fails, just drop back to using the home workspace.
            return dynamoModel.Workspaces.First(w => w is HomeWorkspaceModel);
        }

        /// <summary>
        /// This method sends ComputationResponse when running is completed
        /// </summary>
        /// <param name="sendGeometry">Indicates if node geometry should be sent</param>
        private void NodeDataModified(bool sendGeometry)
        {
            SendResponse(new ComputationResponse(ExecutedNodes, sendGeometry));

            if (!sendGeometry || ListIsNullOrEmpty(ExecutedNodes))
                return;

            var allNodes = dynamoModel.CurrentWorkspace.Nodes;
            // send geometry for all visible nodes
            var guidsToSend = ExecutedNodes.Where(ex =>
                allNodes.Any(n => n.GUID.ToString() == ex.NodeId && n.IsVisible && ex.ContainsGeometryData))
                .Select(n => n.NodeId);

            foreach (var item in guidsToSend)
            {
                RetrieveGeometry(item);
            }
        }

        private void NewWorkspaceCreated(string respondPath)
        {
            var nodes = ExecutedNodes ?? new ExecutedNode[0];

            // switch back to the loaded workspace
            SetCurrentWorkspace(currentWorkspaceGuid);
            var currentWorkspace = dynamoModel.CurrentWorkspace;

            foreach (var node in currentWorkspace.Nodes)
            {
                string data;
                var exnode = nodes.FirstOrDefault(n => n.NodeId == node.GUID.ToString());
                if (node is Function || node is CodeBlockNodeModel || node is VariableInputNode)
                {
                    // include data about number of inputs and outputs
                    data = node.GetInOutPortsData();
                }
                else if (node is Watch3D)
                {
                    var watch = (Watch3D) node;
                    data = watch.GetExtraData(exnode.Data);
                }
                else if (node is DynamoConvert)
                {
                    data = ((DynamoConvert)node).GetExtraData(exnode.Data);
                }
                else if (exnode != null)
                {
                    // needed data is already computed and contains in executed nodes
                    data = exnode.Data;
                }
                else
                {
                    // all nodes in custom node workspace are always considered as not updated
                    // so node.IsUpdated=false and there are no executed nodes for them
                    data = node.GetValue(dynamoModel.EngineController);
                }

                uploader.AddCreationData(node, data);
            }

            var guid = GetWorkspaceGuid(currentWorkspace);
            var notes = currentWorkspace.Notes.Select(n => new
            {
                Id = n.GUID,
                NoteText = n.Text,
                n.X,
                n.Y
            });

            var groups = currentWorkspace.Annotations.Select(a => new GroupData(a));
            var response = new NewWorkspaceResponse(currentWorkspace.Name,
                GetCurrentWorkspaceGuid(),
                uploader.NodesToCreate,
                uploader.ConnectorsToCreate,
                nodes,
                cameras.ContainsKey(guid) ? cameras[guid] : null,
                notes,
                groups);

            var proxyNodesResponses = new List<UpdateProxyNodesResponse>();
            if (uploader.IsCustomNode)
            {
                // after uploading custom node definition there may be proxy nodes
                // that were updated
                var allWorkspaces = dynamoModel.Workspaces;
                foreach (var ws in allWorkspaces)
                {
                    // current workspace id
                    var wsId = ws is CustomNodeWorkspaceModel ?
                        ((CustomNodeWorkspaceModel)ws).CustomNodeDefinition.FunctionId.ToString() : "";
                    var nodeIds = new List<string>();

                    // foreach custom node within current workspace
                    foreach (var func in ws.Nodes.OfType<Function>())
                    {
                        // if this node was updated by uploading current custom node definition
                        if (func.Definition.FunctionId == guid)
                        {
                            nodeIds.Add(func.GUID.ToString());
                        }
                    }

                    // if there are updated nodes add the response data
                    if (nodeIds.Any())
                    {
                        proxyNodesResponses.Add(new UpdateProxyNodesResponse(wsId, response.WorkspaceId, nodeIds));
                    }
                }
            }

            SendResponse(response);

            foreach (var pnResponse in proxyNodesResponses)
            {
                SendResponse(pnResponse);
            }

            if (!string.IsNullOrEmpty(respondPath))
            {
                var wsResponse = new WorkspacePathResponse(response.WorkspaceId, respondPath);
                SendResponse(wsResponse);
            }
        }

        private void RetrieveGeometry(string nodeId)
        {
            Guid guid;
            if (!Guid.TryParse(nodeId, out guid) || renderPackageCache == null) return;
            if (!this.getGeoAndSend(guid))
            {
                //this node exists but it has no geo, lets try getting geo for it.
                //make sure the node is in the graph.
                var node = this.dynamoModel.CurrentWorkspace.Nodes.Where(n => n.GUID == guid).FirstOrDefault();
                if (node != null)
                {
                    //add a temporary handler which will send a geometry response when the render package is generated for a node.
                    this.renderPackageCache.RequestNodeVisualUpdate += handleNodeVisualUpdate;
                    //request a render package from dynamo.
                    node.RequestVisualUpdateAsync(this.dynamoModel.Scheduler, dynamoModel.EngineController, MessageHandler.renderPackageFactory, true);
                }
            }
        }

        /// <summary>
        /// attempts to get geometry from the cache, and if present, sends a geometry response.
        /// returns false if geometry was not present, returns true if response sent.
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private bool getGeoAndSend(Guid nodeID)
        {
            var geometry = renderPackageCache.GetRenderPackages(nodeID);
            if (geometry != null)
            {
                SendResponse(new GeometryDataResponse(new GeometryData(nodeID.ToString(), geometry)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// handle node visual update by regenerating renderpackage and sending a
        /// geometryResponse.
        /// </summary>
        /// <param name="obj"></param>
        private void handleNodeVisualUpdate(NodeModel obj)
        {   //try to send geometry again
            this.getGeoAndSend(obj.GUID);
            //unsubscribe from the request of visual updates.
            this.renderPackageCache.RequestNodeVisualUpdate -= handleNodeVisualUpdate;
        }

        private void WaitForRunCompletion()
        {
            if (nextRunAllowed.WaitOne(maxRunTime)) return;

            // if time is up before running is done
            SendResponse(new TimeoutResponse());
        }

        private void ResetBeforeNewRun()
        {
            ExecutedNodes = null;
            nextRunAllowed.Reset();
        }

        private void SetCurrentWorkspaceToFirstHomeWorkspace()
        {
            var hws = dynamoModel.Workspaces.First(ws => ws is HomeWorkspaceModel);
            SetCurrentWorkspace(hws.Guid);
        }

        private bool SetCurrentWorkspace(Guid guid)
        {
            var workspaceToSwitch = GetWorkspaceById(guid.ToString());
            if (workspaceToSwitch == null)
            {
                return false;
            }

            if (dynamoModel.CurrentWorkspace != workspaceToSwitch)
            {
                var index = dynamoModel.Workspaces.IndexOf(workspaceToSwitch);
                dynamoModel.ExecuteCommand(new DynamoModel.SwitchTabCommand(index));
            }

            return true;
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Lookup the default value of a node's port
        /// </summary>
        private static object LookupPortDefaultValue(NodeModel node, int portIndex)
        {
            if (portIndex >= node.InPorts.Count || portIndex < 0) return null;

            return node.InPorts[portIndex].DefaultValue != null ? node.InPorts[portIndex].DefaultValue.ToString() : null;
        }

        private static bool ListIsNullOrEmpty<T>(IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        private static Type GetTypeFromString(string type)
        {
            return Type.GetType(string.Format(Constants.MessageTypeFormat, type));
        }

        private static Guid GetWorkspaceGuid(WorkspaceModel workspace)
        {
            var customNodeWorkspace = workspace as CustomNodeWorkspaceModel;
            return customNodeWorkspace != null ? customNodeWorkspace.CustomNodeDefinition.FunctionId : workspace.Guid;
        }

        #endregion
    }
}
