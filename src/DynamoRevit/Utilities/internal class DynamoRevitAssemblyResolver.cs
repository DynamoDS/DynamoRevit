using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

internal class DynamoRevitAssemblyResolver
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
