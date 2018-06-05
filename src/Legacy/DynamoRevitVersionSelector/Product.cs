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
        /// A way to says "plz keep using the lastest installed version available".
        /// This should be the default value / lazy case (think if the end-user never use this DynamoRevitSelector facility).
        /// We assume, that if the end-user selects explicitly the lastest version, then he'll keep follow tatest version
        /// </summary>
        public static readonly Version LASTESTDYNAMO = null;

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
                var msg = isCurrent ? Resources.DynamoCurrentVersionText : Resources.DynamoVersionText;
                return string.Format(msg, Version.ToString(4));
            }
        }

        /// <summary>
        /// How to describe this product in (Sub Title)
        /// </summary>
        public string SubTitle
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// Generic contructor
        /// </summary>
        public Product(Version version)
        {
            Version = version;
        }

        /// <summary>
        /// Contructor, for a full defined product
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
                return asm.CreateInstance(DYNAMO_TYPENAME);
            }
            catch (Exception)
            {
                throw new System.MethodAccessException($"AsssemblyPath '{AsssemblyPath}', can't be access to '{DYNAMO_TYPENAME}'");
            }
        }
    }

}
