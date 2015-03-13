using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Dynamo.Interfaces;

namespace Dynamo.Applications
{
    class RevitPathResolver : IPathResolver
    {
        private readonly List<string> preloadLibraryPaths;
        private readonly List<string> additionalNodeDirectories;
        private readonly List<string> additionalResolutionPaths;

        internal RevitPathResolver()
        {
            // The executing assembly will be in Revit_20xx folder,
            // so we have to walk up one level.
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            var currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);

            // Just making sure we are looking at the right level of nesting.
            var nodesDirectory = Path.Combine(currentAssemblyDir, "nodes");
            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);

            preloadLibraryPaths = new List<string>
            {
                // Add Revit-specific library paths for preloading.
                Path.Combine(currentAssemblyDir, "RevitNodes.dll"),
                Path.Combine(currentAssemblyDir, "SimpleRaaS.dll")
            };

            // Add an additional node processing folder
            additionalNodeDirectories = new List<string> { nodesDirectory };

            // Add the Revit_20xx folder for assembly resolution
            additionalResolutionPaths = new List<string> { currentAssemblyDir };
        }

        public IEnumerable<string> AdditionalNodeDirectories
        {
            get { return additionalNodeDirectories; }
        }

        public IEnumerable<string> AdditionalResolutionPaths
        {
            get { return additionalResolutionPaths; }
        }

        public IEnumerable<string> PreloadedLibraryPaths
        {
            get { return preloadLibraryPaths; }
        }
    }
}
