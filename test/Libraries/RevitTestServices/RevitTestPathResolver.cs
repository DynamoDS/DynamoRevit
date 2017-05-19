using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TestServices;

namespace RevitTestServices
{
    public class RevitTestPathResolver : TestPathResolver
    {
        public void InitializePreloadedLibraries()
        {
            AddPreloadLibraryPath("VMDataBridge.dll");
            AddPreloadLibraryPath("ProtoGeometry.dll");
            AddPreloadLibraryPath("DSCoreNodes.dll");
            AddPreloadLibraryPath("DSOffice.dll");
            AddPreloadLibraryPath("DSIronPython.dll");
            AddPreloadLibraryPath("FunctionObject.ds");
            AddPreloadLibraryPath("BuiltIn.ds");
            AddPreloadLibraryPath("DynamoConversions.dll");
            AddPreloadLibraryPath("DynamoUnits.dll");
            AddPreloadLibraryPath("Tessellation.dll");
            AddPreloadLibraryPath("Analysis.dll");

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

            AddNodeDirectory(nodesDirectory);
            AddResolutionPath(assemblyDirectory);
            AddPreloadLibraryPath(revitNodesDll);
            AddPreloadLibraryPath(simpleRaaSDll);
        }
    }
}
