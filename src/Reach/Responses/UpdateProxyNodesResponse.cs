using System.Collections.Generic;


namespace Reach.Responses
{
    public class UpdateProxyNodesResponse: Response
    {
        /// <summary>
        /// Guid of the workspace that custom nodes belong to.
        /// If it's home workspace the value is empty
        /// </summary>
        public string WorkspaceId { get; private set; }
        
        /// <summary>
        /// Guid of the custom node workspace that was loaded
        /// </summary>
        public string CustomNodeId { get; private set; }
        
        /// <summary>
        /// Guids of nodes that are not proxy anymore
        /// </summary>
        public IEnumerable<string> NodesIds { get; private set; }

        /// <summary>
        /// This response is being sent from Dynamo Server to the connecting client when
        /// a custom node definition has been uploaded to the server. Responding to this 
        /// response, the client will then update proxy nodes that are found in workspace,
        /// turning them into actual custom node instances.
        /// </summary>
        /// <param name="workspaceId">Workspace id</param>
        /// <param name="customNodeId">Custom node id</param>
        /// <param name="nodesIds">List of proxy nodes</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"UpdateProxyNodesResponse",
        ///     "workspaceId":"00000000-0000-0000-0000-000000000000",
        ///     "customNodeId":"050d4944-8d99-4aab-b0b6-bb9848edd22a",
        ///     "nodesIds":[
        ///         "a2311e4f-b4a2-494e-a067-8b051ef68145"
        ///     ],
        ///     "status":0
        /// }
        /// </code>
        public UpdateProxyNodesResponse(string workspaceId, string customNodeId,
            IEnumerable<string> nodesIds)
        {
            this.WorkspaceId = workspaceId;
            this.CustomNodeId = customNodeId;
            this.NodesIds = nodesIds;
        }
    }
}
