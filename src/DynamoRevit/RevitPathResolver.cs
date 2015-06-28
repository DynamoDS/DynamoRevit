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

            var nodesDirectory = Path.Combine(currentAssemblyDir, "nodes");
            var revitNodesDll = Path.Combine(currentAssemblyDir, "RevitNodes.dll");
            var simpleRaaSDll = Path.Combine(currentAssemblyDir, "SimpleRaaS.dll");

            // Just making sure we are looking at the right level of nesting.
            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);
            if (!File.Exists(revitNodesDll))
                throw new FileNotFoundException(revitNodesDll);
            if (!File.Exists(simpleRaaSDll))
                throw new FileNotFoundException(simpleRaaSDll);

            // Add Revit-specific library paths for preloading.
            preloadLibraryPaths = new List<string>
            {
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DSCoreNodes.dll",
                "DSOffice.dll",
                "DSIronPython.dll",
                "FunctionObject.ds",
                "Optimize.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                "Analysis.dll",
                "Display.dll",

                revitNodesDll,
                simpleRaaSDll
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

        public string UserDataRootFolder
        {
            get { return string.Empty; }
        }

        public string CommonDataRootFolder
        {
            get { return string.Empty; }
        }
    }
}
