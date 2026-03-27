using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using DSCPython;
using Dynamo.Applications;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.PythonServices;
using Dynamo.Scheduler;
using DynamoPlayer;
using Greg.AuthProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevitServices.Elements;
using RevitServices.Persistence;
using System.Reflection;
using System.Text.RegularExpressions;
using static Dynamo.Models.DynamoModel;

namespace DADynamoApp
{
    public class DAEntrypoint 
    {
        private DynamoModel model;
        internal static DynamoPlayerLoggerConfiguration logConfig = new DynamoPlayerLoggerConfiguration() { DynamoLogLevel = Dynamo.Logging.LogLevel.Console, LogLevel = DynamoPlayer.LogLevel.Information };
        private string DynamoPath;
        private string DynamoRevitPath;
        private ControlledApplication controlledApplication;

        // Store event handler references for cleanup
        private readonly Dictionary<HomeWorkspaceModel,
            (EventHandler<EventArgs> evaluationStartedHandler,
             EventHandler<Dynamo.Models.EvaluationCompletedEventArgs> evaluationCompletedHandler,
             Dictionary<NodeModel, Action<NodeModel>[]> nodeHandlers)>
            workspaceHandlers = [];

        private string LoadMessage;
        private string WorkItemFolder;
        private readonly string PythonDllFolder = "pythonDependencies";


        private List<IUpdater> Updaters = [];

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            model?.Dispose();

            controlledApplication.DocumentClosing -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosing;
            controlledApplication.DocumentClosed -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosed;
            controlledApplication.DocumentOpened -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentOpened;

            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            controlledApplication = application;

            WorkItemFolder = Directory.GetCurrentDirectory();
            Console.WriteLine($"<<!>> Work folder is '{WorkItemFolder}'");

            DynamoRevitPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DynamoPath = Directory.GetParent(DynamoRevitPath).FullName;

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var hostDir = Path.GetDirectoryName(hostloc);

            Console.WriteLine("<<!>> Starting to load D4DA");

            try
            {
                RevitServicesUpdater.Initialize(Updaters);
                Console.WriteLine("<<!>> D4DA Loaded");

                RevitServices.Transactions.TransactionManager.SetupManager(new RevitServices.Transactions.AutomaticTransactionStrategy());
                // TODO: do we need element binding in Design Automations?
                ElementBinder.IsEnabled = true;

                controlledApplication.DocumentClosing += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosing;
                controlledApplication.DocumentClosed += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosed;
                controlledApplication.DocumentOpened += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentOpened;

                DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;

                return ExternalDBApplicationResult.Succeeded;
            }
            catch (Exception ex)
            {
                return ExternalDBApplicationResult.Failed;
            }
        }

        private static string GetRevitContext(Autodesk.Revit.ApplicationServices.Application app)
        {
            var r = new Regex(@"\b(Autodesk |Structure |MEP |Architecture )\b");
            return r.Replace(app.VersionName, "");
        }

        /// <summary>
        /// Extracts the 'token' value from the work item JSON content.
        /// This is robust to casing differences and supports BoundArguments as an object.
        /// Returns null if token cannot be found.
        /// </summary>
        private string? ExtractTokenFromWorkItem(string workItemContent)
        {
            if (string.IsNullOrWhiteSpace(workItemContent)) return null;

            try
            {
                var job = JObject.Parse(workItemContent);

                // Find BoundArguments (case-insensitive)
                JToken? boundToken = null;
                if (!job.TryGetValue("BoundArguments", out boundToken))
                {
                    foreach (var prop in job.Properties())
                    {
                        if (string.Equals(prop.Name, "BoundArguments", StringComparison.OrdinalIgnoreCase))
                        {
                            boundToken = prop.Value;
                            break;
                        }
                    }
                }

                if (boundToken == null) return null;

                // If it's an object, try direct lookup
                if (boundToken.Type == JTokenType.Object)
                {
                    var boundObj = (JObject)boundToken;

                    // try exact key 'token' then case-insensitive
                    if (boundObj.TryGetValue("adsk3LeggedToken", out var tokVal))
                    {
                        return tokVal.Type == JTokenType.String ? tokVal.Value<string>() : tokVal.ToString();
                    }

                    foreach (var prop in boundObj.Properties())
                    {
                        if (string.Equals(prop.Name, "adsk3LeggedToken", StringComparison.OrdinalIgnoreCase))
                        {
                            return prop.Value.Type == JTokenType.String ? prop.Value.Value<string>() : prop.Value.ToString();
                        }
                    }
                }

                // Fallback: convert to dictionary and search case-insensitively
                var boundDict = boundToken.ToObject<Dictionary<string, JToken>>();
                if (boundDict != null)
                {
                    foreach (var kv in boundDict)
                    {
                        if (string.Equals(kv.Key, "adsk3LeggedToken", StringComparison.OrdinalIgnoreCase))
                        {
                            return kv.Value.Type == JTokenType.String ? kv.Value.Value<string>() : kv.Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExtractTokenFromWorkItem failed: {ex.Message}");
            }

            return null;
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            Console.WriteLine("<<!>> DA event raised.");
            
            var workItemId = Environment.ExpandEnvironmentVariables("%DAS_WORKITEM_ID%");

            Console.WriteLine($"<<!>> WorkItemId is '{workItemId}'");

            var workItemFileName = $"{workItemId}_job.das";
            var workItemExists = File.Exists(workItemFileName);
            Console.WriteLine($"Looking for WorkItem file at '{workItemFileName}', exists: {workItemExists}");
            if (workItemExists)
            {
                var workItemContent = File.ReadAllText(workItemFileName);

                var token = ExtractTokenFromWorkItem(workItemContent);
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"<<!>> Extracted token from workItem content.");
                }
            }

            // Local Change
            //WorkItemFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var dynTempDir = Path.Combine(WorkItemFolder, "dyn_tmp");
            var app = e.DesignAutomationData?.RevitApp;
            Console.WriteLine("<<!>> Preparing Dynamo model. Vers 1");

            SetupDARequest? setupReq = null;
            var setupReqPath = Path.Combine(WorkItemFolder, "setup.json");
            if (File.Exists(setupReqPath))
            {
                try
                {
                    var setupRequest = File.ReadAllText(setupReqPath);
                    Console.WriteLine(setupRequest);

                    setupReq = JsonConvert.DeserializeObject<SetupDARequest>(setupRequest, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // Ensure we have a pre-built graph output folder.
            try
            {
                var graphOutputFolder = "output";
                if (!Directory.Exists(graphOutputFolder))
                {
                    Directory.CreateDirectory(graphOutputFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create output folder. {ex.Message}");
            }

            Document? doc = null;
            var cModel = setupReq?.OpenCloudModelLocation;
            if (cModel != null)
            {
                try
                {
                    Console.WriteLine($"Opening cloud model with Region: {cModel.Region}, Project: {cModel.ProjectGuid}, Model: {cModel.ModelGuid}");

                    var cloudModelPath = ModelPathUtils.ConvertCloudGUIDsToCloudPath(cModel.Region, cModel.ProjectGuid, cModel.ModelGuid);
                    Console.WriteLine(doc == null ? $"Cloud model path is {JsonConvert.SerializeObject(cloudModelPath)}" : "doc is not null before opening cloud model");
                    doc = app?.OpenDocumentFile(cloudModelPath, new OpenOptions());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (setupReq?.LocalModelFileName != null)
            {
                try
                {
                    var localModelPath = Path.Combine(WorkItemFolder, setupReq.LocalModelFileName);
                    Console.WriteLine($"Opening local model at {localModelPath}");
                    doc = app?.OpenDocumentFile(localModelPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                doc = e.DesignAutomationData?.RevitDoc;
            }

            // Startup a new project, maybe an option we can have ?
            //app.NewProjectDocument(Autodesk.Revit.DB.UnitSystem.Metric);

            if (doc == null) throw new InvalidOperationException("Could not open revit document.");

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var asmLocation = controlledApplication.SharedComponentsLocation;
            Console.WriteLine($"using asm at location {asmLocation}");
            Console.WriteLine($"Is Loaded {LoadMessage}");

            // need this for cloud, does not work on local
            var userDataFolder = Path.Combine(dynTempDir, "Dynamo Revit");
            var commonDataFolder = Path.Combine(dynTempDir, "Dynamo");

            DocumentManager.Instance.PrepareForDesignAutomation(app);

            var loadedLibGVersion = ASMPrealoaderUtils.PreloadAsmFromRevit(asmLocation, DynamoPath);
            Console.WriteLine($"{loadedLibGVersion}");
            var geometryFactoryPath = ASMPrealoaderUtils.GetGeometryFactoryPath(DynamoPath, loadedLibGVersion);
            Console.WriteLine($"{geometryFactoryPath}");

            PreInstallPythonDependencies();

            model = Dynamo.Applications.Models.RevitDynamoModel.Start(
                new DefaultStartConfiguration
                {
                    DynamoCorePath = DynamoPath,
                    DynamoHostPath = DynamoRevitPath,
                    GeometryFactoryPath = geometryFactoryPath,
                    PathResolver = new RevitPathResolver(userDataFolder, commonDataFolder),
                    Context = GetRevitContext(app),
                    AuthProvider = new RevitOAuth2Provider(SynchronizationContext.Current ?? new SynchronizationContext()),
                    ProcessMode = TaskProcessMode.Synchronous,
                    CLIMode = true,
                    IsHeadless = true,
                    IsServiceMode = true
                });

            LoadMessage = model != null ? "loaded" : "no loaded";

            SetupProfilingHandlers(model);

            var playerHost = new PlayerHostDynamoDefault(model, new DynamoPlayerLogger<PlayerHostDynamoDefault>(logConfig));
            var workflows = new DynamoModelWorkflows(
                playerHost,
                new DynamoPlayerLogger<DynamoModelWorkflows>(logConfig));

            var controller = new DynamoControllerImplementation(playerHost, workflows,
                new DynamoPlayerLogger<DynamoControllerImplementation>(logConfig));

            DynamoPlayerLogger.Initialize(playerHost);

            workflows.LoadDependencies(new GraphTarget() { DependenciesPath = WorkItemFolder });

            var dynHandler = new Handler(playerHost, [new DARunGraphController(controller, model, WorkItemFolder)]);

            var runContent = File.ReadAllText(Path.Combine(WorkItemFolder, "run.json"));
            var testFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var output = string.Empty;
            try
            {
                var res = dynHandler.HandleRoute("POST", "/v1/graph/run", runContent);
                output = res.Result;

                // Force transaction close for now.
                // This is already called on RevitDynamoModel.OnEvaluationCompleted.
                // TODO: figure out if this is required here (I noticed, a couple of times, saving the rvt failed after running a graph)
                // ex. Operation is not permitted when there is any open sub-transaction, transaction, or transaction group.
                // DYN-10310
                RevitServices.Transactions.TransactionManager.Instance?.ForceCloseTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(output);
            File.WriteAllText(Path.Combine(WorkItemFolder, "result.json"), output);

            // Default, save the rvt
            bool saveRvt = setupReq?.GenerateOutputModel ?? true;
            Console.WriteLine($"{nameof(saveRvt)} is set to {saveRvt}");
            if (saveRvt)
            {
                try
                {
                    if (doc.IsModelInCloud)
                    {
                        Console.WriteLine("Document is in cloud.");
                        if (doc.IsWorkshared) // work-shared/C4R model
                        {
                            Console.WriteLine("Document is C4R.");
                            // Syncronize with central
                            //SynchronizeWithCentralOptions swc = new SynchronizeWithCentralOptions();
                            //swc.SetRelinquishOptions(new RelinquishOptions(/*relinquishAll*/true));// Should this be configurable?
                            //doc.SynchronizeWithCentral(new TransactWithCentralOptions(), swc);

                            // Save the project locally (this will detach the model from the cloud, but we will re-upload at a new location)
                            doc.SaveAs(Path.Combine(WorkItemFolder, setupReq.LocalModelFileName));
                        }
                        else 
                        {// Single user cloud model
                            Console.WriteLine("Document is single-user cloud model.");

                            // Save the project locally (this will detach the model from the cloud, but we will re-upload at a new location)
                            doc.SaveAs(Path.Combine(WorkItemFolder, setupReq.LocalModelFileName));

                            /* TODO: figure out if we need to make this work and how.

                            doc.SaveCloudModel();

                            Console.WriteLine($"uploading with token {token}");
                            // TODO: For single-user cloud models, we need to publish the model after saving so that the changes are reflected in the cloud.
                            var cmPath = doc.GetCloudModelPath();
                            var dmClient = new DataManagementClient(null, new StaticAuthenticationProvider(token));
                            var projId = "b." + setupReq.SaveCloudModelLocation.AccountId;

                            var publishTask = dmClient.ExecutePublishModelAsync(projId, new PublishModelPayload()
                            {
                                Type = TypeCommands.Commands,
                                Attributes = new PublishModelPayloadAttributes()
                                {
                                    Extension = new PublishModelPayloadAttributesExtension()
                                    {
                                        Type = TypeCommandtypePublishmodel.CommandsautodeskBim360C4RModelPublish,
                                        VarVersion = "1.0.0"
                                    }
                                },
                                Relationships = new PublishModelPayloadRelationships()
                                {
                                    Resources = new PublishModelPayloadRelationshipsResources()
                                    {
                                        Data = new List<PublishModelPayloadRelationshipsResourcesData>
                                        {
                                            new PublishModelPayloadRelationshipsResourcesData()
                                            {
                                                Id = cmPath.GetModelGUID().ToString(),
                                                Type = TypeItem.Items
                                            }
                                        }
                                    }
                                }
                            });

                            publishTask.Wait();
                            PublishModel respo = publishTask.Result;
                            Console.WriteLine($"Publish result: {JsonConvert.SerializeObject(respo)}");
                            */
                        }
                    }
                    else
                    {
                        /* TODO: Figure ou tif this is useful.
                        var newLoc = setupReq?.SaveCloudModelLocation;
                        if (newLoc != null)
                        {
                            Console.WriteLine($"Saving to new cloud model location with AccountId: {newLoc.AccountId}, ProjectId: {newLoc.ProjectId}, FolderId: {newLoc.FolderId}, ModelName: {newLoc.ModelName}");
                            doc.SaveAsCloudModel(newLoc.AccountId, newLoc.ProjectId, newLoc.FolderId, newLoc.ModelName ?? Path.GetFileName(doc.PathName));
                        }*/

                        // Save locally 
                        doc.SaveAs(setupReq.LocalModelFileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            e.Succeeded = true;
        }

        internal void SetupProfilingHandlers(DynamoModel model)
        {
            model.WorkspaceOpened += ws =>
            {
                if (ws is not HomeWorkspaceModel homeWorkspace) return;

                var nodeHandlers = new Dictionary<NodeModel, Action<NodeModel>[]>();

                EventHandler<EventArgs> evaluationStartedHandler = (sender, args) =>
                {
                    homeWorkspace.EngineController.EnableProfiling(true, homeWorkspace, homeWorkspace.Nodes);
                    Console.WriteLine($"{DateTime.UtcNow:u} : Profiling enabled for {homeWorkspace.Name} dynamo workspace");
                };

                EventHandler<Dynamo.Models.EvaluationCompletedEventArgs> evaluationCompletedHandler = (sender, args) =>
                {
                    if (workspaceHandlers.TryGetValue(homeWorkspace, out var handlers))
                    {
                        CleanupWorkspaceHandlers(homeWorkspace, handlers.evaluationStartedHandler, handlers.evaluationCompletedHandler, handlers.nodeHandlers);
                    }
                };

                foreach (var node in homeWorkspace.Nodes)
                {
                    Action<NodeModel> beginHandler = nm =>
                    {
                        Console.WriteLine($"{DateTime.UtcNow:u} : Node {nm.Name} started execution.");
                    };

                    Action<NodeModel> endHandler = nm =>
                    {
                        var outputSummary = DALogger.SerializeNodeOutputs(nm, homeWorkspace.EngineController);
                        if (!string.IsNullOrEmpty(outputSummary))
                            Console.WriteLine($"{DateTime.UtcNow:u} : Node {nm.Name} outputs: {outputSummary}");

                        var runtimeStatus = homeWorkspace.EngineController.LiveRunnerRuntimeCore.RuntimeStatus;
                        var nodeMessages = DALogger.GetNodeMessages(runtimeStatus, nm.GUID);
                        if (!string.IsNullOrEmpty(nodeMessages))
                            Console.WriteLine($"{DateTime.UtcNow:u} : Node {nm.Name} messages: {nodeMessages}");

                        Console.WriteLine($"{DateTime.UtcNow:u} : Node {nm.Name} finished execution.");
                    };

                    node.NodeExecutionBegin += beginHandler;
                    node.NodeExecutionEnd += endHandler;

                    nodeHandlers[node] = [beginHandler, endHandler];
                }

                homeWorkspace.EvaluationStarted += evaluationStartedHandler;
                homeWorkspace.EvaluationCompleted += evaluationCompletedHandler;

                workspaceHandlers[homeWorkspace] = (evaluationStartedHandler, evaluationCompletedHandler, nodeHandlers);
            };
        }

        private void CleanupWorkspaceHandlers(
            HomeWorkspaceModel workspace,
            EventHandler<EventArgs>? evaluationStartedHandler,
            EventHandler<Dynamo.Models.EvaluationCompletedEventArgs>? evaluationCompletedHandler,
            Dictionary<NodeModel, Action<NodeModel>[]> nodeHandlers)
        {
            if (evaluationStartedHandler != null)
                workspace.EvaluationStarted -= evaluationStartedHandler;

            if (evaluationCompletedHandler != null)
                workspace.EvaluationCompleted -= evaluationCompletedHandler;

            foreach (var kvp in nodeHandlers)
            {
                var node = kvp.Key;
                var nodeHandlerArray = kvp.Value;

                if (nodeHandlerArray.Length >= 2)
                {
                    node.NodeExecutionBegin -= nodeHandlerArray[0];
                    node.NodeExecutionEnd -= nodeHandlerArray[1];
                }
            }

            workspaceHandlers.Remove(workspace);
        }

        private void PreInstallPythonDependencies()
        {
            try
            {
                // Preload all python assemblies at the PythonDllFolder.
                foreach (var pyDll in Directory.EnumerateFiles(Path.Combine(WorkItemFolder, PythonDllFolder), "*.dll"))
                {
                    Assembly.LoadFrom(pyDll);
                }

                var pyIncluded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "Python.Included");
                if (pyIncluded == null)
                {
                    throw new Exception("Could not find Python.Included assembly");
                }
                var type = pyIncluded.GetType("Python.Included.Installer");
                if (type == null)
                {
                    throw new Exception("null Installer type");
                }
                var property = type.GetProperty("INSTALL_PATH", BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    throw new Exception("null INSTALL_PATH property");
                }

                // Set the python install location to the DA workfolder (that is the only place we have wrie access)
                property.SetValue(null, WorkItemFolder);

                // Dynamo's 'VerifyEngineReferences' wants all the PythonEngine's dependencies to be in the Dynamo folder.
                // Temporary until we fix it on the Dynamo side.
                if (PythonEngineManager.Instance.AvailableEngines.Count == 0)
                {
                    PropertyInfo instanceProp = typeof(CPythonEvaluator).GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        PythonEngine engine = (PythonEngine)instanceProp.GetValue(null);
                        if (engine == null)
                        {
                            throw new Exception($"Could not get a valid PythonEngine instance");
                        }

                        PythonEngineManager.Instance.AvailableEngines.Add(engine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not setup python " + ex.Message);
            }
        }
    }
}
