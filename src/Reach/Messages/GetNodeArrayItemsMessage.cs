using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Retrieves a Json text representation of the specified array range contained in a node with id <see cref="NodeId"/>
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"GetNodeArrayItemsMessage",
    ///     "NodeId": "61e47bad-8d44-6d3e-0b81-557ca672c880",
    ///     "IndexFrom": 3,
    ///     "Length": 5
    /// }
    /// </code>
    class GetNodeArrayItemsMessage : Message
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string NodeId { get; set; }

        /// <summary>
        /// Index from which Flood requests array items
        /// </summary>
        [DataMember]
        public int IndexFrom { get; set; }

        /// <summary>
        /// Number of requested array items 
        /// </summary>
        [DataMember]
        public int Length { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.RetrieveArrayItems(this);
        }
    }
}
