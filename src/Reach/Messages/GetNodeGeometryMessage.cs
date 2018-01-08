using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Requests all geometry primitives of a node with <see cref="NodeId"/> identifier
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"GetNodeGeometryMessage",
    ///     "NodeId":"61e47bad-8d44-6d3e-0b81-557ca672c880"
    /// }
    /// </code>
    class GetNodeGeometryMessage : Message
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string NodeId { get; set; }

        public GetNodeGeometryMessage(string id)
        {
            this.NodeId = id;
        }

        internal override void Execute(MessageHandler handler)
        {
            handler.RetrieveGeometry(this);
        }
    }
}
