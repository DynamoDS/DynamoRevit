using System.Collections.Generic;
using System.Runtime.Serialization;
using Reach.Messages.Data;

namespace Reach.Responses
{
    [DataContract]
    public class RestResponse
    {
        [DataMember]
        public IEnumerable<ExecutedNode> Nodes { get; set; }
        [DataMember]
        public IEnumerable<GeometryData> Geometry { get; set; }
        [DataMember]
        public IEnumerable<SerializedNodeData> SerializedNodes { get; set; }
    }
}
