using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Interfaces;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using System.ComponentModel;
using Dynamo.Visualization;

namespace Reach.Rendering
{
    public class WorkspaceRenderPackageCache : IRenderPackageCache {
    
        private Dictionary<Guid, IEnumerable<IRenderPackage>> cache = new Dictionary<Guid, IEnumerable<IRenderPackage>>();
        private readonly IWorkspaceModel workspace;
        private bool disposed;

        public event Action<NodeModel> RequestNodeVisualUpdate;

        private void OnRequestNodeVisualUpdate(NodeModel node)
        {
            if (RequestNodeVisualUpdate != null)
            {
                RequestNodeVisualUpdate(node);
            }
        }

        public event Action RenderPackagesUpdated;

        private void OnRenderPackagesUpdated()
        {
            if (RenderPackagesUpdated != null)
            {
                RenderPackagesUpdated();
            }
        }

        public WorkspaceRenderPackageCache(HomeWorkspaceModel workspace)
        {
            this.workspace = workspace;

            // register all existing nodes
            foreach (var node in workspace.Nodes)
            {
                NodeAdded(node);
            }

            workspace.NodeAdded += NodeAdded;
            workspace.NodeRemoved += NodeRemoved;
        }

        public void Dispose()
        {
            if (disposed) return;
            workspace.NodeAdded -= NodeAdded;
            workspace.NodeRemoved -= NodeRemoved;

            // deregister all nodes
            foreach (var node in workspace.Nodes)
            {
                NodeRemoved(node);
            }

            cache = null;
            disposed = true;
        }

        public IEnumerable<IRenderPackage> GetRenderPackages(Guid nodeModelGuid)
        {
            if (HasRenderPackages(nodeModelGuid))
            {
                return this.cache[nodeModelGuid];
            }

            return null;
        }

        public bool HasRenderPackages(Guid nodeModelGuid)
        {
            return cache.ContainsKey(nodeModelGuid);
        }

        private void NodeRenderPackagesUpdated(NodeModel nodeModel, RenderPackageCache renderPackages)
        {
            if (cache == null) return;
            
            cache[nodeModel.GUID] = renderPackages.Packages;
            OnRenderPackagesUpdated();
        }

        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var node = sender as NodeModel;
            if (node == null || e.PropertyName != "IsVisible") return;

            OnRequestNodeVisualUpdate(node);
        }

        private void NodeAdded(NodeModel nodeModel)
        {
            nodeModel.RenderPackagesUpdated += NodeRenderPackagesUpdated;
            nodeModel.PropertyChanged += OnNodePropertyChanged;
        }

        private void NodeRemoved(NodeModel nodeModel)
        {
            nodeModel.RenderPackagesUpdated -= NodeRenderPackagesUpdated;
            nodeModel.PropertyChanged -= OnNodePropertyChanged;
        }
    }
}
