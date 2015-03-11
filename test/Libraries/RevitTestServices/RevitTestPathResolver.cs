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
            AddResolutionPath(assemblyDirectory);
            AddNodeDirectory(Path.Combine(assemblyDirectory, "nodes"));
            AddPreloadLibraryPath(Path.Combine(assemblyDirectory, "RevitNodes.dll"));
            AddPreloadLibraryPath(Path.Combine(assemblyDirectory, "SimpleRaaS.dll"));
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
