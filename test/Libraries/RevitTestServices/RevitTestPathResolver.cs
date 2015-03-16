using System.Collections.Generic;
using System.IO;
using Dynamo.Interfaces;

namespace RevitTestServices
{
    public class RevitTestPathResolver : IPathResolver
    {
        private readonly HashSet<string> additionalResolutionPaths;
        private readonly HashSet<string> additionalNodeDirectories;
        private readonly HashSet<string> preloadedLibraryPaths;

        public RevitTestPathResolver()
        {
            additionalResolutionPaths = new HashSet<string>();
            additionalNodeDirectories = new HashSet<string>();
            preloadedLibraryPaths = new HashSet<string>();
        }

        public RevitTestPathResolver(string assemblyDirectory) : this()
        {
            var nodesDirectory = Path.Combine(assemblyDirectory, "nodes");
            var revitNodesDll = Path.Combine(assemblyDirectory, "RevitNodes.dll");
            var simpleRaaSDll = Path.Combine(assemblyDirectory, "SimpleRaaS.dll");

            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);
            if (!File.Exists(revitNodesDll))
                throw new FileNotFoundException(revitNodesDll);
            if (!File.Exists(simpleRaaSDll))
                throw new FileNotFoundException(simpleRaaSDll);

            AddResolutionPath(assemblyDirectory);
            AddNodeDirectory(nodesDirectory);
            AddPreloadLibraryPath(revitNodesDll);
            AddPreloadLibraryPath(simpleRaaSDll);
        }

        public IEnumerable<string> AdditionalResolutionPaths
        {
            get { return additionalResolutionPaths; }
        }

        public IEnumerable<string> AdditionalNodeDirectories
        {
            get { return additionalNodeDirectories; }
        }

        public IEnumerable<string> PreloadedLibraryPaths
        {
            get { return preloadedLibraryPaths; }
        }

        public void AddNodeDirectory(string nodeDirectory)
        {
            if (!additionalNodeDirectories.Contains(nodeDirectory))
                additionalNodeDirectories.Add(nodeDirectory);
        }

        public void AddResolutionPath(string resolutionPath)
        {
            if (!additionalResolutionPaths.Contains(resolutionPath))
                additionalResolutionPaths.Add(resolutionPath);
        }

        public void AddPreloadLibraryPath(string preloadLibraryPath)
        {
            if (!preloadedLibraryPaths.Contains(preloadLibraryPath))
                preloadedLibraryPaths.Add(preloadLibraryPath);
        }
    }
}
