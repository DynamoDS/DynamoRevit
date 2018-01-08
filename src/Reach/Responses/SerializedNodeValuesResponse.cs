using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Reach.Responses
{
    /// <summary>
    /// The serialized form of the node values
    /// </summary>
    class SerializedNodeValuesResponse : Response
    {
        /// <summary>
        /// Nodes values
        /// </summary>
        [DataMember]
        public IEnumerable<Data.SerializedNodeValueData> Values { get; private set; }

        /// <summary>
        /// Serialization format
        /// </summary>
        [DataMember]
        public string Format { get; private set; }

        public SerializedNodeValuesResponse(IEnumerable<Data.SerializedNodeValueData> values, string format)
        {
            this.Values = values;
            this.Format = format;
        }
    }
}
