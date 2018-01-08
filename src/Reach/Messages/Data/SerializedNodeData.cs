using System;
using System.Runtime.Serialization;

namespace Reach.Messages.Data
{
    public class SerializedNodeData
    {
        /// <summary>
        /// Guid of the specified node.
        /// </summary>
        [DataMember]
        public Guid NodeId;

        /// <summary>
        /// Custom data object to be serialized as JSON.
        /// </summary>
        [DataMember]
        public object Data;
    };
}
