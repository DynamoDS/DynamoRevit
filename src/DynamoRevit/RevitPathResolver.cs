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
        private readonly string userDataRootFolder;
        private readonly string commonDataRootFolder;

        internal RevitPathResolver(string userDataFolder, string commonDataFolder)
        {
            // The executing assembly will be in Revit_20xx folder,
            // so we have to walk up one level.
            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            var currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);

            var nodesDirectory = Path.Combine(currentAssemblyDir, "nodes");
            var revitNodesDll = Path.Combine(currentAssemblyDir, "RevitNodes.dll");

            // Just making sure we are looking at the right level of nesting.
            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);
            if (!File.Exists(revitNodesDll))
                throw new FileNotFoundException(revitNodesDll);

            // Add Revit-specific library paths for preloading.
            preloadLibraryPaths = new List<string>
            {
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DesignScriptBuiltin.dll",
                "DSCoreNodes.dll",
                "DSOffice.dll",
                "DSIronPython.dll",
                "FunctionObject.ds",
                "BuiltIn.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                "Analysis.dll",
                "GeometryColor.dll",

                revitNodesDll
            };

            // Add an additional node processing folder
            additionalNodeDirectories = new List<string> { nodesDirectory };

            // Add the Revit_20xx folder for assembly resolution
            additionalResolutionPaths = new List<string> { currentAssemblyDir };

			// Add Steel nodes to preload
			var steelNodesDirectory = Path.Combine(currentAssemblyDir, "nodesSteelConnections", "steel-pkg", "bin");
            var steelNodesDll = Path.Combine(steelNodesDirectory, "AdvanceSteelConnAutoNodes.dll");

            if (File.Exists(steelNodesDll) == true)
            {
              preloadLibraryPaths.Add(steelNodesDll);

              var steelCustomNodesDirectory = Path.Combine(currentAssemblyDir, "nodesSteelConnections", "steel-pkg", "dyf");
              additionalNodeDirectories.Add(steelNodesDirectory);
              additionalNodeDirectories.Add(steelCustomNodesDirectory);
            }
			
            this.userDataRootFolder = userDataFolder;
            this.commonDataRootFolder = commonDataFolder;
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
            get { return userDataRootFolder; }
        }

        public string CommonDataRootFolder
        {
            get { return commonDataRootFolder; }
        }
    }
}
