using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Dynamo.Interfaces;

namespace Reach
{
    public class ReachPathResolver : IPathResolver
    {
        public string UserDataRootFolder { get; private set; }
        public string CommonDataRootFolder { get; private set; }

        private readonly List<string> additionalResolutionPaths;
        private readonly List<string> additionalNodeDirectories;
        private readonly List<string> preloadedLibraryPaths;

        public ReachPathResolver(string preloaderLocation, string workingDirectory = null)
        {
            if (workingDirectory == null || !Directory.Exists(workingDirectory))
            {
                workingDirectory = CurrentDirectory();
            }

            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }

            UserDataRootFolder = Path.Combine(workingDirectory, "UserData");
            CommonDataRootFolder = Path.Combine(workingDirectory, "CommonData");

            additionalResolutionPaths = new List<string>();

            if (preloaderLocation != string.Empty && Directory.Exists(preloaderLocation))
            {
                additionalResolutionPaths.Add(preloaderLocation);
            }

            additionalNodeDirectories = new List<string>();
            preloadedLibraryPaths = new List<string>
            {
                "VMDataBridge.dll",
                "ProtoGeometry.dll",
                "DSCoreNodes.dll",
                "FunctionObject.ds",
                "Optimize.ds",
                "DynamoConversions.dll",
                "DynamoUnits.dll",
                "Tessellation.dll",
                //"DSIronPython.dll",
                "Analysis.dll",
                "GeometryColor.dll"
            };

            var libConfig = (Reach.Configuration.ExternalLibrariesSection)ConfigurationManager.GetSection("externalLibraries");
            if (libConfig != null)
            {
               foreach (Reach.Configuration.ExternalLibraryElement lib in libConfig.Libraries)
               {
                  Console.WriteLine("Loading external library " + lib.Path);
                  preloadedLibraryPaths.Add(lib.Path);
               }
            }
        }
        
        private string CurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
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

        /// <summary>
        /// Returns resolved path for the given path if successful else
        /// empty string. Attempts to find the provided file path in the 
        /// Dynamo directory. If this fails, an attempt is made to find 
        /// the file in the executing assembly's directory.
        /// </summary>
        /// <param name="path">Given input path</param>
        /// <returns>Resolved path or empty string</returns>
        public static string ResolveFilePath(string path)
        {
            var fullpath = Path.GetFullPath(path);
            if (File.Exists(fullpath)) return fullpath;

            var file = Path.GetFileName(path);
            var corepath = Path.GetDirectoryName(typeof(IDynamoModel).Assembly.Location);
            fullpath = Path.Combine(corepath, file);
            if (File.Exists(fullpath)) return fullpath;

            var currentpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            fullpath = Path.Combine(currentpath, file);
            if (File.Exists(fullpath)) return fullpath;

            return string.Empty;
        }
    }
}
