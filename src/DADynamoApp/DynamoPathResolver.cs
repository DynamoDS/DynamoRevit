using System.Reflection;
using System.Runtime.Loader;

namespace DADynamoApp
{
  internal static class DynamoPathResolver
  {
    public static string DynamoCorePath
    {
      get
      {
        if (string.IsNullOrEmpty(dynamopath))
        {
          dynamopath = GetDynamoCorePath();
        }
        return dynamopath;
      }
    }

    /// <summary>
    /// Finds the Dynamo Core path by looking into registery or potentially a config file.
    /// </summary>
    /// <returns>The root folder path of Dynamo Core.</returns>
    private static string GetDynamoCorePath()
    {
      var dynamoRevitRootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
      var dynamoRoot = GetDynamoRoot(dynamoRevitRootDirectory);
      return dynamoRoot;
    }

    /// <summary>
    /// Gets Dynamo Root folder from the given DynamoRevit root.
    /// </summary>
    /// <param name="dynamoRevitRoot">The root folder of DynamoRevit binaries</param>
    /// <returns>The root folder path of Dynamo Core</returns>
    private static string GetDynamoRoot(string dynamoRevitRoot)
    {
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

    private static string dynamopath;

    public static Assembly? Default_Resolving(AssemblyLoadContext arg1, AssemblyName args)
    {
      var assemblyPath = string.Empty;
      var assemblyName = new AssemblyName(args.Name).Name + ".dll";

      try
      {
        assemblyPath = Path.Combine(DynamoPathResolver.DynamoCorePath, assemblyName);
        if (File.Exists(assemblyPath))
        {
          return arg1.LoadFromAssemblyPath(assemblyPath);
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

        return (File.Exists(assemblyPath) ? arg1.LoadFromAssemblyPath(assemblyPath) : null);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
      }
    }

    public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
      var assemblyPath = string.Empty;
      var assemblyName = new AssemblyName(args.Name).Name + ".dll";

      try
      {
        assemblyPath = Path.Combine(DynamoPathResolver.DynamoCorePath, assemblyName);
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
