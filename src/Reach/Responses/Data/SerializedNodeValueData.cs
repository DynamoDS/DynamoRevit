using System.Runtime.Serialization;

namespace Reach.Responses.Data
{
    /// <summary>
    ///     This class represents serialized node's value data
    /// </summary>
    class SerializedNodeValueData
    {
        /// <summary>
        /// Node's GUID
        /// </summary>
        [DataMember]
        public string NodeId { get; set; }

        /// <summary>
        /// The serialization of the node's value
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }
}
