using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Exceptions
{
    public class InvalidNodesException: Exception
    {
        private string message;
        public override string Message { get { return message; } }

        public IEnumerable<string> InvalidNodeNames { get; private set; }

        public InvalidNodesException(IEnumerable<NodeModel> invalidNodes)
        {
            InvalidNodeNames = invalidNodes.Select(n => n.Name).Distinct();
            message = "Your workspace contains nodes that are currently not allowed online:\n" +
                      String.Join("\n", InvalidNodeNames);
        }
    }
}
