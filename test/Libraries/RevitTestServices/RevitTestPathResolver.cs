using System.IO;
using System.Reflection;
using TestServices;

namespace RevitTestServices
{
    public class RevitTestPathResolver : TestPathResolver
    {
        public RevitTestPathResolver() { }

        internal RevitTestPathResolver(TestPathResolverParams resolverParams) : base(resolverParams)
        {
            //Set UserDataBootFolder & CommonDataRootFolder to PathResolver.
        }
        public void InitializePreloadedLibraries()
        {
            AddPreloadLibraryPath("VMDataBridge.dll");
            AddPreloadLibraryPath("ProtoGeometry.dll");
            AddPreloadLibraryPath("DesignScriptBuiltin.dll");
            AddPreloadLibraryPath("DSCoreNodes.dll");
            AddPreloadLibraryPath("DSOffice.dll");
            AddPreloadLibraryPath("DSIronPython.dll");
            AddPreloadLibraryPath("FunctionObject.ds");
            AddPreloadLibraryPath("BuiltIn.ds");
            AddPreloadLibraryPath("DynamoConversions.dll");
            AddPreloadLibraryPath("DynamoUnits.dll");
            AddPreloadLibraryPath("Tessellation.dll");
            AddPreloadLibraryPath("Analysis.dll");
            AddPreloadLibraryPath("SimplexNoise.dll");

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var nodesDirectory = Path.Combine(assemblyDirectory, "nodes");
            var revitNodesDll = Path.Combine(assemblyDirectory, "RevitNodes.dll");

            if (!Directory.Exists(nodesDirectory))
                throw new DirectoryNotFoundException(nodesDirectory);
            if (!File.Exists(revitNodesDll))
                throw new FileNotFoundException(revitNodesDll);

            AddNodeDirectory(nodesDirectory);
            AddResolutionPath(assemblyDirectory);
            AddPreloadLibraryPath(revitNodesDll);
        }
    }
}
