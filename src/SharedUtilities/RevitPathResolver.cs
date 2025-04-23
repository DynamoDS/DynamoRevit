using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Dynamo.Interfaces;

namespace Dynamo.Applications
{
    class DynamoRevitAssemblyResolver
    {
        /// <summary>
        /// Handler to the ApplicationDomain's AssemblyResolve event.
        /// If an assembly's location cannot be resolved, an exception is
        /// thrown. Failure to resolve an assembly will leave Dynamo in 
        /// a bad state, so we should throw an exception here which gets caught 
        /// by our unhandled exception handler and presents the crash dialogue.
        /// </summary>
        /// <param name="dynamoCorePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static Assembly ResolveDynamoAssembly(string dynamoCorePath, List<string> additionalPaths, ResolveEventArgs args)
        {
            var assemblyPath = string.Empty;
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {
                assemblyPath = Path.Combine(dynamoCorePath, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }

                if (additionalPaths != null)
                {
                    foreach (var additionalPath in additionalPaths)
                    {
                        assemblyPath = Path.Combine(additionalPath, assemblyName);
                        if (File.Exists(assemblyPath))
                        {
                            return Assembly.LoadFrom(assemblyPath);
                        }
                    }
                }

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                // Try "Dynamo 0.x\Revit_20xx" folder first...
                assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                if (!File.Exists(assemblyPath))
                {
                    // If assembly cannot be found, try in "Dynamo 0.x" folder.
                    var parentDirectory = Directory.GetParent(assemblyDirectory);
                    assemblyPath = Path.Combine(parentDirectory.FullName, assemblyName);
                }

                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
            }
        }
    }

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
                "DSCPython.dll",
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

            // Add other internal nodes to preload list
            var internalNodes = DynamoRevitInternalNodes.GetNodesToPreload();
            foreach (var assemblyPath in internalNodes)
            {
                if (File.Exists(assemblyPath))
                {
                    preloadLibraryPaths.Add(assemblyPath);
                    string assemblyDir = Path.GetDirectoryName(assemblyPath);
                    additionalNodeDirectories.Add(assemblyDir);
                    additionalResolutionPaths.Add(assemblyDir);
                }
            }

            // Add assembly load paths from internal packages - for dependencies between other Revit addins
            var internalNodesAdditionalAssemblyLoadPaths = DynamoRevitInternalNodes.GetAdditionalAssemblyLoadPaths();
            foreach (var assemblyPath in internalNodesAdditionalAssemblyLoadPaths)
            {
                if (Directory.Exists(assemblyPath))
                {
                    additionalResolutionPaths.Add(assemblyPath);
                }
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

        public IEnumerable<string> GetDynamoUserDataLocations()
        {
            var appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var paths = new List<string>();
            //Pre 1.0 Dynamo Studio user data was stored at %appdata%\Dynamo\
            var dynamoFolder = Path.Combine(appDatafolder, "Dynamo");
            if (Directory.Exists(dynamoFolder))
            {
                paths.AddRange(Directory.EnumerateDirectories(dynamoFolder));
            }

            //From 1.0 onwards Dynamo Studio user data is stored at %appdata%\Dynamo\Dynamo Revit\
            var revitFolder = Path.Combine(dynamoFolder, "Dynamo Revit");
            if (Directory.Exists(revitFolder))
            {
                paths.AddRange(Directory.EnumerateDirectories(revitFolder));
            }

            return paths;
        }
    }
}