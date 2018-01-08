using System.Collections.Generic;
using Reach.Messages.Data;


namespace Reach.Responses
{
    public class ComputationResponse : Response
    {
        /// <summary>
        /// The list with data about nodes which are retrieved after a computation is done
        /// </summary>
        public IEnumerable<ExecutedNode> Nodes { get; private set; }

        /// <summary>
        /// Indicates if geometry is sent along with ComputationResponse
        /// </summary>
        public bool IsGeometrySent { get; private set; }

        /// <summary>
        /// The response with computation results
        /// </summary>
        /// <param name="nodes">List of the node data like status,
        /// items count and geometry data</param>
        /// <param name="isGeomSent">Flag which indicates 
        /// if geometry is sent without an additional request</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"ComputationResponse",
        ///     "nodes":[{
        ///         "$type":"DynamoWebServer.Messages.ExecutedNode",
        ///         "nodeId":"61e47bad-8d44-6d3e-0b81-557ca672c880",
        ///         "state":"Active",
        ///         "stateMessage":"",
        ///         "data":"Array",
        ///         "numItems":5,
        ///         "containsGeometryData":true
        ///     }],
        ///     "isGeometrySent":true,
        ///     "status":0
        /// }
        /// </code>
        public ComputationResponse(IEnumerable<ExecutedNode> nodes, bool isGeomSent)
        {
            this.Nodes = nodes;
            this.IsGeometrySent = isGeomSent;
        }
    }
}
