using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var nodesDirectory = Path.Combine(assemblyDirectory, "nodes");
            var revitNodesDll = Path.Combine(assemblyDirectory, "RevitNodes.dll");
            var simpleRaaSDll = Path.Combine(assemblyDirectory, "SimpleRaaS.dll");

            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);
            if (!File.Exists(revitNodesDll))
                throw new FileNotFoundException(revitNodesDll);
            if (!File.Exists(simpleRaaSDll))
                throw new FileNotFoundException(simpleRaaSDll);

            additionalResolutionPaths = new HashSet<string> { assemblyDirectory };
            additionalNodeDirectories = new HashSet<string> { nodesDirectory };
            preloadedLibraryPaths = new HashSet<string> { revitNodesDll, simpleRaaSDll };
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
