using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Graph.Workspaces;

namespace Dynamo.Storage.Upload
{
    //this classs reproduced from DynamoPublish extension
    /// <summary>
    /// A Workspace and its dependencies
    /// </summary>
    public sealed class WorkspaceDependencies
    {
        /// <summary>
        /// The Workspace for which the dependencies are to be collected.
        /// </summary>
        public readonly WorkspaceModel Workspace;

        /// <summary>
        /// The full collection of workspaces representing the dependencies
        /// </summary>
        public readonly IEnumerable<CustomNodeWorkspaceModel> CustomNodeWorkspaces;

        private WorkspaceDependencies(WorkspaceModel Workspace, IEnumerable<ICustomNodeWorkspaceModel> customNodeWorkspaces)
        {
            this.Workspace = Workspace;
            this.CustomNodeWorkspaces = customNodeWorkspaces.OfType<CustomNodeWorkspaceModel>();
        }
       
        /// <summary>
        /// Get all of the dependencies from a workspace
        /// </summary>
        /// <param name="workspace">The workspace to read the dependencies from</param>
        /// <param name="customNodeManager">A custom node manager to look up dependencies</param>
        /// <returns>A WorkspaceDependencies object containing the workspace and its CustomNodeWorkspaceModel dependencies</returns>
        public static WorkspaceDependencies Collect(WorkspaceModel workspace, ICustomNodeManager customNodeManager)
        {
            if (workspace == null) throw new ArgumentNullException("workspace");
            if (customNodeManager == null) throw new ArgumentNullException("customNodeManager");

            // collect all dependencies
            var dependencies = new HashSet<CustomNodeDefinition>();
            //if the workspace is a main workspace then find all functions and their dependencies
            if (workspace is HomeWorkspaceModel)
            {
                foreach (var node in workspace.Nodes.OfType<Function>())
                {
                    dependencies.Add(node.Definition);
                    foreach (var dep in node.Definition.Dependencies)
                    {
                        dependencies.Add(dep);
                    }
                }
            }
             //else the workspace is a customnode - and we can add the dependencies directly
            else
            {
                foreach (var def in (workspace as CustomNodeWorkspaceModel).CustomNodeDefinition.Dependencies)
                {
                    dependencies.Add(def);
                }
            }
            var customNodeWorkspaces = new List<ICustomNodeWorkspaceModel>();
            foreach (var dependency in dependencies)
            {
                ICustomNodeWorkspaceModel customNodeWs;
                var workspaceExists = customNodeManager.TryGetFunctionWorkspace(dependency.FunctionId, false, out customNodeWs);

                if (!workspaceExists)
                {
                    throw new InvalidOperationException(String.Format("Custom node workspace {0} could not be found in the customNodeManager ", dependency.FunctionName));
                }

                if (!customNodeWorkspaces.Contains(customNodeWs))
                {
                    customNodeWorkspaces.Add(customNodeWs);
                }
            }

            return new WorkspaceDependencies(workspace, customNodeWorkspaces);
        }
    }
}
