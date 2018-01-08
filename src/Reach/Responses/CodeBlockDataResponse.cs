using Reach.Responses.Data;
using System.Runtime.Serialization;


namespace Reach.Responses
{
    /// <summary>
    /// Response with data to redraw a code block node
    /// </summary>
    public class CodeBlockDataResponse : Response
    {
        /// <summary>
        /// Guid of the workspace that contains specified code
        /// block node. Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }
        
        /// <summary>
        /// Guid of the specified code block node
        /// </summary>
        [DataMember]
        public string NodeId { get; private set; }

        /// <summary>
        /// String representing of the data about input,
        /// output ports, text of specified code block node
        /// </summary>
        [DataMember]
        public CodeBlockData Data { get; private set; }

        /// <summary>
        /// The response with the ports and content data for a specified Code block node
        /// </summary>
        /// <param name="wsGuid">Workspace guid which contains code block</param>
        /// <param name="nodeGuid">Code block guid</param>
        /// <param name="data">Information about ports and code. Represented as a string</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"CodeBlockDataResponse",
        ///     "workspaceGuid":"",
        ///     "nodeId":"868f0daf-e037-6565-6d15-b8ebd4fc9f27",
        ///     "data":"{
        ///         "Code":"1..5;\n1..3;",
        ///         "LineIndices": [0, 1],
        ///         "InPorts": [],
        ///         "OutPorts": ["temp_0","temp_1"]
        ///     }",
        ///     "status":0
        /// }
        /// </code>
        public CodeBlockDataResponse(string wsGuid, string nodeGuid, CodeBlockData data)
        {
            WorkspaceGuid = wsGuid;
            NodeId = nodeGuid;
            Data = data;
        }
    }
}
