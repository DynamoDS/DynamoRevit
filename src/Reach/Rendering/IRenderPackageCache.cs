using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Interfaces;
using Dynamo.Graph.Nodes;

namespace Reach.Rendering
{
    public interface IRenderPackageCache : IDisposable
    {
        bool HasRenderPackages(Guid guid);
        IEnumerable<IRenderPackage> GetRenderPackages(Guid guid);
        event Action<NodeModel> RequestNodeVisualUpdate;
        event Action RenderPackagesUpdated;
    }
}