using System.Reflection;

namespace DADynamoApp
{
    internal static class DynamoPathResolver
    {
        /// <summary>
        /// Finds the Dynamo Core path by looking into registery or potentially a config file.
        /// </summary>
        /// <returns>The root folder path of Dynamo Core.</returns>
        private static string GetDynamoCorePath()
        {
            var dynamoRevitRoot = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //TODO: use config file to setup Dynamo Path for debug builds.

            //When there is no config file, just replace DynamoRevit by Dynamo 
            //from the 'dynamoRevitRoot' folder.
            var parent = new DirectoryInfo(dynamoRevitRoot);
            var path = string.Empty;
            while (null != parent && parent.Name != @"DynamoRevit")
            {
                path = Path.Combine(parent.Name, path);
                parent = Directory.GetParent(parent.FullName);
            }

            return parent != null ? Path.Combine(Path.GetDirectoryName(parent.FullName), @"Dynamo", path) : dynamoRevitRoot;
        }

        private static string dynamopath = null;

       
        public static Assembly ResolveAssembly(object? sender, ResolveEventArgs args)
        {
            dynamopath ??= GetDynamoCorePath();

            var assemblyName = new AssemblyName(args.Name).Name + ".dll";
            var assemblyPath = Path.Combine(dynamopath, assemblyName);
            try
            {
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
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
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
            }
        }
    }
}
