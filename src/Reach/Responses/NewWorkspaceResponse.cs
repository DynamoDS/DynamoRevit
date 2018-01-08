using System.Collections.Generic;
using Dynamo.Storage.Data;
using Reach.Messages.Data;


namespace Reach.Responses
{
    public class NewWorkspaceResponse : Response
    {
        /// <summary>
        /// String representation of workspace guid
        /// </summary>
        public string WorkspaceId { get; private set; }

        /// <summary>
        /// Workspace name
        /// </summary>
        public string WorkspaceName { get; private set; }

        /// <summary>
        /// List of all workspaces nodes
        /// </summary>
        public IEnumerable<object> Nodes { get; private set; }

        /// <summary>
        /// List of all workspace connections
        /// </summary>
        public IEnumerable<object> Connections { get; private set; }

        /// <summary>
        /// List of nodes computation results
        /// </summary>
        public IEnumerable<object> NodesResult { get; private set; }

        /// <summary>
        /// Contains all necessary information about camera position
        /// </summary>
        public IEnumerable<CameraData> CameraData { get; private set; }

        /// <summary>
        /// List of all workspaces notes
        /// </summary>
        public IEnumerable<object> Notes { get; private set; }

        /// <summary>
        /// List of all workspaces groups
        /// </summary>
        public IEnumerable<GroupData> Groups { get; private set; }

        /// <summary>
        /// The response with all workspace data which contains in an opened file
        /// </summary>
        /// <param name="workspaceName">Workspace name</param>
        /// <param name="workspaceId">Workspace id</param>
        /// <param name="nodes">List of nodes</param>
        /// <param name="connections">List of connections</param>
        /// <param name="nodesResult">List of computation results</param>
        /// <param name="cameraData">Camera position information</param>
        /// <param name="notes">List of notes</param>
        /// <param name="groups">List of groups</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"NewWorkspaceResponse",
        ///     "workspaceId":"00000000-0000-0000-0000-000000000000",
        ///     "workspaceName":"Home",
        ///     "nodes":[],
        ///     "connections":[],
        ///     "nodesResult":[],
        ///     "status":0
        /// }
        /// </code>
        public NewWorkspaceResponse(string workspaceName,
            string workspaceId,
            IEnumerable<object> nodes, 
            IEnumerable<object> connections,
            IEnumerable<object> nodesResult,
            IEnumerable<CameraData> cameraData,
            IEnumerable<object> notes,
            IEnumerable<GroupData> groups)
        {
            this.WorkspaceName = workspaceName;
            this.WorkspaceId = workspaceId;
            this.Nodes = nodes;
            this.Connections = connections;
            this.NodesResult = nodesResult;
            this.CameraData = cameraData;
            this.Notes = notes;
            this.Groups = groups;
        }
    }
}
