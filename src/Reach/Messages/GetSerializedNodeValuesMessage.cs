using System.Collections.Generic;
using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Requires serialization of nodes in accordance with specified <see cref="Format"/>
    /// </summary>
    class GetSerializedNodeValuesMessage : Message
    {
        /// <summary>
        /// Serialization format
        /// </summary>
        [DataMember]
        public string Format;

        /// <summary>
        /// Nodes GUIDs
        /// </summary>
        [DataMember]
        public IEnumerable<string> NodeIds;

        /// <summary>
        /// Additional params
        /// </summary>
        [DataMember]
        public Dictionary<string, object> Parameters;

        internal override void Execute(MessageHandler handler)
        {
            handler.SendSerializedNodeValue(this);
        }
    }
}
