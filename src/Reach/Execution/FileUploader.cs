using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dynamo.Models;
using Reach.Messages;
using Reach.Messages.Data;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Storage.Upload;

namespace Reach.Execution
{
    class FileUploader
    {
        private List<NodeToCreate> nodesToCreate;
        private List<ConnectorToCreate> connectorsToCreate;

        public bool IsCustomNode { get; private set; }
        public IEnumerable<NodeToCreate> NodesToCreate { get { return nodesToCreate; } }
        public IEnumerable<ConnectorToCreate> ConnectorsToCreate { get { return connectorsToCreate; } }
        public IEnumerable<string> InvalidNodeNames { get; private set; }

        public FileUploader()
        {
            nodesToCreate = new List<NodeToCreate>();
            connectorsToCreate = new List<ConnectorToCreate>();
        }

        internal ProcessResult ProcessFileData(UploadFileMessage uploadFileMessage, DynamoModel dynamoModel)
        {
            try
            {
                var result = ProcessResult.Succeeded;
                InvalidNodeNames = null;
                var filePath = string.Empty;

                // if path was specified it means NWK is used
                if (!string.IsNullOrEmpty(uploadFileMessage.Path))
                {
                    var fileName = Path.GetFileName(uploadFileMessage.Path);
                    filePath = Path.GetTempPath() + "\\" + fileName;
                    result = ProcessResult.RespondWithPath;
                    File.Copy(uploadFileMessage.Path, filePath, true);
                }
                else
                {
                    var content = uploadFileMessage.FileContent;
                    filePath = Path.GetTempPath() + "\\" + uploadFileMessage.FileName;
                    File.WriteAllBytes(filePath, content);
                }

                var previousHomeWs = dynamoModel.Workspaces.OfType<HomeWorkspaceModel>().First();
                var previousHomeWsPath = Path.GetTempFileName();
                var runtimeCore = dynamoModel.EngineController.LiveRunnerRuntimeCore;

                try
                {
                    previousHomeWs.Save(previousHomeWsPath);
                }
                catch
                {
                    return ProcessResult.Failed;
                }

                dynamoModel.ExecuteCommand(new DynamoModel.OpenFileCommand(filePath, true));
                var uploadedWs = dynamoModel.CurrentWorkspace;
                IsCustomNode = uploadedWs is CustomNodeWorkspaceModel;

                // delete temporary file
                File.Delete(filePath);

                nodesToCreate.Clear();
                connectorsToCreate.Clear();

                var invalidNodes = NodeFilter.GetDisabledNodes(uploadedWs.Nodes);
                if (invalidNodes.Any())
                {
                    // back to the previous Home workspace
                    if (!dynamoModel.Workspaces.Contains(previousHomeWs))
                    {
                        dynamoModel.ExecuteCommand(new DynamoModel.OpenFileCommand(previousHomeWsPath, true));
                    }

                    // remove the workspace with not allowed nodes
                    dynamoModel.RemoveWorkspace(uploadedWs);
                    dynamoModel.ResetEngine();

                    InvalidNodeNames = invalidNodes.Select(n => n.Name).Distinct();
                    return ProcessResult.Failed;
                }

                return result;
            }
            catch
            {
                return ProcessResult.Failed;
            }
        }

        internal void AddCreationData(NodeModel node, string data)
        {
            var nodeToCreate = new NodeToCreate(node, data);
            nodesToCreate.Add(nodeToCreate);
            connectorsToCreate.AddRange(ConnectorToCreate.GetOutgoingConnectors(node));
        }
    }

    enum ProcessResult
    {
        Failed,
        Succeeded,
        RespondWithPath
    }
}
