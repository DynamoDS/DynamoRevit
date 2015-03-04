using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Dynamo.Interfaces;
using DynamoUtilities;

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
            var parentDirectory = Directory.GetParent(currentAssemblyDir);

            // Setup the core paths
            DynamoPathManager.Instance.InitializeCore(parentDirectory.Name);

            preloadLibraryPaths = new List<string>
            {
                // Add Revit-specific library paths for preloading.
                Path.Combine(currentAssemblyDir, "RevitNodes.dll"),
                Path.Combine(currentAssemblyDir, "SimpleRaaS.dll")
            };

            additionalNodeDirectories = new List<string>
            {
                // Add an additional node processing folder
                Path.Combine(currentAssemblyDir, "nodes")
            };

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
