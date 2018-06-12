using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using DynamoRevitVersionSelector.Properties;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace Dynamo.Applications
{
    /// <summary>
    /// A Dyanmo product definition
    /// </summary>
    internal class Product
    {
        #region Statics
        /// <summary>
        /// A way to says "do not loat load Dyanmo during starup".
        /// </summary>
        /// <remarks>
        /// Legacy use case - This should be the default case
        /// This might be usefull when people need to change Dyanmo version on each Revit Session
        /// </remarks>
        public static readonly Version NODYNAMO = null;

        /// <summary>
        /// A way to says "please keep using the lastest installed version available".
        /// </summary>
        public static readonly Version LASTESTDYNAMO = new Version(0,0,0,0);

        /// <summary>
        /// Dynamo product's assembly should always define the same type name
        /// </summary>
        public static readonly String DYNAMO_TYPENAME = "Dynamo.Applications.DynamoRevitApp";
        #endregion

        #region Accessors
        /// <summary>
        /// Version
        /// </summary>
        public Version Version
        {
            get; private set;
        }

        /// <summary>
        /// Asssembly Path (i.e. "DynamoRevitDS.dll" location), maybe null
        /// </summary>
        public string AsssemblyPath
        {
            get; private set;
        }

        /// <summary>
        /// Flag to say that this product is currently used
        /// </summary>
        public bool isCurrent
        {
            get; set;
        }

        /// <summary>
        /// How to describe this product in UI (Title)
        /// </summary>
        public string Title
        {
            get
            {
                if (Version == NODYNAMO)
                    return Resources.DynamoVersionKeepClear;
                var msg = isCurrent ? Resources.DynamoCurrentVersionText : Resources.DynamoVersionText;
                return string.Format(msg, Version.ToString(4));
            }
        }

        /// <summary>
        /// How to describe this product (Sub Title)
        /// </summary>
        public string SubTitle
        {
            get; set;
        }

        /// <summary>
        /// Generic String conversion
        /// </summary>
        public override string ToString()
        {
            return Title;
        }

        /// <summary>
        /// Get product instance (if it has been created)
        /// </summary>
        /// <seealso cref="CreateInstance"/>
        public Object Instance
        {
            get; private set;
        }
        #endregion

        /// <summary>
        /// Lazy constructor
        /// </summary>
        public Product(Version version)
        {
            Version = version;
        }

        /// <summary>
        /// Constructor, for a full defined/existing dynamo product
        /// </summary>
        public Product(Version version, string assemblyPath, string installFolder)
        {
            Version = version;
            AsssemblyPath = assemblyPath;
            SubTitle = installFolder;
        }

        /// <summary>
        /// Create a class instance from Assembly, exception needs to be properly handled
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when Asssembly from AsssemblyPath can't be Loaded</exception>
        /// <exception cref="System.MethodAccessException">Thrown when method can't be Access</exception>
        public Object CreateInstance()
        {
            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(AsssemblyPath);
            }
            catch (Exception)
            {
                throw new System.ArgumentException($"AsssemblyPath '{AsssemblyPath}', can't be loaded");
            }
            try
            {
                Instance = asm.CreateInstance(DYNAMO_TYPENAME);
                return Instance;
            }
            catch (Exception)
            {
                throw new System.MethodAccessException($"AsssemblyPath '{AsssemblyPath}', can't be access to '{DYNAMO_TYPENAME}'");
            }
        }

        /// <summary>
        /// Execute OnStartup() by reflection
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            var method = Instance.GetType().GetMethod("OnStartup");
            return (Result)method.Invoke(Instance, new object[] { application });
        }

        /// <summary>
        /// Execute ExecuteDynamoCommand() by reflection
        /// </summary>
        /// <param name="journalData"></param>
        /// <param name="appUI"></param>
        /// <returns></returns>
        public Result ExecuteDynamoCommand(IDictionary<string, string> journalData, UIApplication appUI)
        {
            var method = Instance.GetType().GetMethod("ExecuteDynamoCommand");
            return (Result)method.Invoke(Instance, new object[] { journalData, appUI });
        }
    }

}
