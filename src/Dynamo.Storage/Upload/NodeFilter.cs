using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Models;
using Dynamo.Storage.Conversion;

namespace Dynamo.Storage.Upload
{
    public static class NodeFilter
    {
        public static bool IsValidNode(string creationName, DynamoModel dynamoModel)
        {
            var searchEntries = dynamoModel.SearchModel.SearchEntries;
            var searchEntry = searchEntries
                .FirstOrDefault(se => se.CreationName == creationName);

            if (searchEntry == null)
            {
                Guid functionId;
                // it might be custom node creationName
                return Guid.TryParse(creationName, out functionId) && dynamoModel.CustomNodeManager.Contains(functionId);
            }

            return IsValidNode(creationName, searchEntry.Assembly);
        }

        internal static bool IsValidNode(NodeModel node)
        {
            var creationName = ReachNode.GetCreationName(node);
            var assemblyName = node is DSFunctionBase
                ? (node as DSFunctionBase).Controller.Definition.Assembly
                : node.GetType().Assembly.GetName().Name;

            return IsValidNode(creationName, assemblyName);
        }

        public static IEnumerable<NodeModel> GetDisabledNodes(IEnumerable<NodeModel> nodes)
        {
            var query = from node in nodes where !IsValidNode(node) select node;

            return query;
        }

        public static bool IsValidNode(string creationName, string assemblyName)
        {
            // If the assembly has been enabled in the allowed assemblies array
            // and is loaded in the externalLibraries
            if (IsAssemblyEnabled(assemblyName))
            {
                // return whether the specified creation name
                // is allowed.
                return IsCreationNameEnabled(creationName);
            }

            // If the first two conditions aren't met,
            // check to see if the assembly is part of a white-listed package
            try
            {
                var existingAsmName = AssemblyName.GetAssemblyName(assemblyName);

                // If the whitelist hasn't been loaded yet, load it now.
                if(!Greg.Whitelist.WhiteListedAssemblyNames.Any()) {
                    Greg.Whitelist.UpdateWhitelist(GetPackageManagerUrl());
                }

                var isWhitelisted = Greg.Whitelist.IsAssemblyInWhitelistedPackage(existingAsmName, Greg.Whitelist.WhiteListedAssemblyNames);

                return isWhitelisted;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// A helper method which reads the "DynamoPackages config file to get the
        /// url for the package manager. If that config can't be found,
        /// the production package manager url will be used.
        /// </summary>
        /// <returns></returns>
        public static string GetPackageManagerUrl()
        {
            var packageManagerUrl = "http://dynamopackages.com";

            var packagesDll = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DynamoPackages.dll");

            if (string.IsNullOrEmpty(packagesDll)) return packageManagerUrl;

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(packagesDll);
                var key = config.AppSettings.Settings["packageManagerAddress"];

                if (key != null)
                {
                    packageManagerUrl = key.Value;
                }

            }
            catch (ConfigurationErrorsException ex)
            {
                Console.WriteLine("The DynamoPackages.dll.config file could not be found. Using the production package manager url");
                Console.WriteLine(ex.Message);
            }

            return packageManagerUrl;
        }

        #region Private static helper methods

        private static bool IsCreationNameEnabled(string creationName)
        {
            return !IsSatNode(creationName) && !IsIoNode(creationName) && !IsPythonNode(creationName);
        }

        private static bool IsAssemblyEnabled(string assemblyName)
        {
            var depathedName = Path.GetFileNameWithoutExtension(assemblyName);

            return AllowedAssemblies.Contains(depathedName);
        }

        private static bool IsSatNode(string name)
        {
            return name.Contains("ExportToSAT");
        }

        private static bool IsIoNode(string name)
        {
            return name.StartsWith("DSCore.IO") || IoNodes.Contains(name);
        }

        private static bool IsPythonNode(string name)
        {
            return name.Contains("Python");
        }

        private static readonly string[] IoNodes = new[]
        {
            "Directory Path",
            "File.FromPath",
            "Directory.FromPath",
            "Watch Image",
            "Web Request"
        };

        private static readonly string[] AllowedAssemblies = new[]
        {
            "DynamoCore",
            "CoreNodeModels",
            "Watch3DNodeModels",
            "UnitsUI",
            "BuiltIn",
            "Operators",
            "ProtoGeometry",
            "DSCoreNodes",
            "DynamoUnits",
            "Tessellation",
            "Analysis",
            "GeometryColor"
        };

#endregion
    }
}
