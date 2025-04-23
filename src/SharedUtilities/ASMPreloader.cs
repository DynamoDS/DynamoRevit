using Autodesk.Revit.ApplicationServices;
using DynamoInstallDetective;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dynamo.Applications
{
    internal static class ASMPrealoaderUtils
    {
        /// <summary>
        /// Returns the version of ASM which is installed with Revit at the requested path.
        /// This version number can be used to load the appropriate libG version.
        /// </summary>
        /// <param name="asmLocation">path where asm dlls are located, this is usually the product(Revit) install path</param>
        /// <returns></returns>
        internal static Version findRevitASMVersion(string asmLocation)
        {
            var lookup = new InstalledProductLookUp("Revit", "ASMAHL*.dll");
            var product = lookup.GetProductFromInstallPath(asmLocation);
            var libGversion = new Version(product.VersionInfo.Item1, product.VersionInfo.Item2, product.VersionInfo.Item3);
            return libGversion;
        }

        // [Tech Debt] (Will refactor the code later)
        /// <summary>
        /// Return the preload version of LibG.
        /// </summary>
        /// <param name="preloaderLocation"></param>
        /// <returns></returns>
        internal static Version PreloadLibGVersion(string preloaderLocation)
        {
            preloaderLocation = new DirectoryInfo(preloaderLocation).Name;
            var regExp = new Regex(@"^libg_(\d\d\d)_(\d)_(\d)$", RegexOptions.IgnoreCase);

            var match = regExp.Match(preloaderLocation);
            if (match.Groups.Count == 4)
            {
                return new Version(
                    Convert.ToInt32(match.Groups[1].Value),
                    Convert.ToInt32(match.Groups[2].Value),
                    Convert.ToInt32(match.Groups[3].Value));
            }

            return new Version();
        }

        internal static Version PreloadAsmFromRevit(ControlledApplication controlledApplication, string dynamoCorePath)
        {
            var asmLocation = controlledApplication.SharedComponentsLocation;

            Version libGVersion = findRevitASMVersion(asmLocation);
            // Get the corresponding libG preloader location for the target ASM loading version.
            // If there is exact match preloader version to the target ASM version, use it, 
            // otherwise use the closest below.
            var preloaderLocation = DynamoShapeManager.Utilities.GetLibGPreloaderLocation(libGVersion, dynamoCorePath);

            // [Tech Debt] (Will refactor the code later)
            // The LibG version maybe different in Dynamo and Revit, using the one which is in Dynamo.
            Version preLoadLibGVersion = PreloadLibGVersion(preloaderLocation);
            DynamoShapeManager.Utilities.PreloadAsmFromPath(preloaderLocation, asmLocation);
            return preLoadLibGVersion;
        }


        /// <summary>
        /// DynamoShapeManager.dll is a companion assembly of Dynamo core components,
        /// we do not want a static reference to it (since the Revit add-on can be 
        /// installed anywhere that's outside of Dynamo), we do not want a duplicated 
        /// reference to it. Here we use reflection to obtain GetGeometryFactoryPath
        /// method, and call it to get the geometry factory assembly path.
        /// </summary>
        /// <param name="corePath">The path where DynamoShapeManager.dll can be 
        /// located.</param>
        /// <param name="version">The version of DynamoShapeManager.dll</param>
        /// <returns>Returns the full path to geometry factory assembly.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string GetGeometryFactoryPath(string corePath, Version version)
        {
            var dynamoAsmPath = Path.Combine(corePath, "DynamoShapeManager.dll");
            var assembly = Assembly.LoadFrom(dynamoAsmPath);
            if (assembly == null)
                throw new FileNotFoundException("File not found", dynamoAsmPath);

            var utilities = assembly.GetType("DynamoShapeManager.Utilities");
            var getGeometryFactoryPath = utilities.GetMethod("GetGeometryFactoryPath2");

            return (getGeometryFactoryPath.Invoke(null,
                new object[] { corePath, version }) as string);
        }
    }
}