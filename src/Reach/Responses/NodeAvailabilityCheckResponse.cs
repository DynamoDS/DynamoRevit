using System.Collections.Generic;

namespace Reach.Responses
{
    public class NodeAvailabilityCheckResponse: Response
    {
        /// <summary>
        /// A collection of node creation names representing
        /// nodes that have been requested of the server,
        /// but are not available.
        /// </summary>
        public IEnumerable<string> UnavailableNodes { get; private set; }

        public NodeAvailabilityCheckResponse(IEnumerable<string> unavailableNodes)
        {
            UnavailableNodes = unavailableNodes;
        }
    }
}
