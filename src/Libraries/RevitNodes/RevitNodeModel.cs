using Dynamo.Graph.Nodes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dynamo.Applications.Models
{
    public abstract class RevitNodeModel : NodeModel
    {
        public RevitNodeModel() { }

        [JsonConstructor]
        public RevitNodeModel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }
    }
}