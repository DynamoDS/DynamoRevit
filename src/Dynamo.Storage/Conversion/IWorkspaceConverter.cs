using System.Collections.Generic;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Storage.Conversion
{
    public interface IWorkspaceConverter
    {
        /// <summary>
        /// Transforms workspace model into object with fields which required in node.js server.
        /// </summary>
        /// <param name="workspace">Workspace for transform</param>
        /// <returns>Suitable object for node.js</returns>
        object CreateWorkspace(WorkspaceModel workspace);

        /// <summary>
        /// Transforms nodes list into object suitable for node.js server.
        /// </summary>
        /// <param name="nodes">Nodes for transform</param>
        /// <returns>Transformed list of nodes</returns>
        IEnumerable<object> CreateNodes(IEnumerable<NodeModel> nodes);

        /// <summary>
        /// Transforms connections list into object suitable for node.js server.
        /// </summary>
        /// <param name="connectors">Connections for transform</param>
        /// <returns>Transformed list of connections</returns>
        IEnumerable<object> CreateConnections(IEnumerable<ConnectorModel> connectors);
    }
}
