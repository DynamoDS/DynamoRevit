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
    internal class ProductsManager
    {

        #region Statics
        /// <summary>
        /// Check if a Product is currently loaded in Current Application Domain
        /// </summary>
        /// <param name="version">If a product is loaded, provides its version.</param>
        /// <returns>True if a Dynamo product is currently loaded</returns>
        public static bool IsProductLoaded(ref Version version)
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms)
            {
                if (asm.GetType(Product.DYNAMO_TYPENAME) != null)
                {
                    version = asm.GetName().Version;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Accessor to the list of Dyanmo Product, the list is sorted in a way that .first() is
        /// the most recent one
        /// </summary>
        public List<Product> List
        {
            get {
                var list = mProducts.Values.ToList();
                return list.OrderByDescending(x => x.Version).ToList();
            }
        }

        /// <summary>
        /// Number of Dyanmo Products available
        /// </summary>
        public int Count
        {
            get { return mProducts.Count; }
        }

        /// <summary>
        /// Revit Version
        /// </summary>
        public string RevitVersion {
            get; private set;
        }
        #endregion

        protected UIControlledApplication uiControlledApplication;
        protected Dictionary<Version, Product> mProducts;

        /// <summary>
        /// Contructor
        /// </summary>
        public ProductsManager(UIControlledApplication application)
        {
            uiControlledApplication = application;
            RevitVersion = application.ControlledApplication.VersionNumber;
            var revitFolder = new System.IO.DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var debugPath = revitFolder.Parent.FullName;
            var dynamoProducts = FindDynamoRevitInstallations(debugPath);
            mProducts = new Dictionary<Version, Product>();
            foreach (var p in dynamoProducts)
            {
                if (p.Version < new Version(0, 7))
                    continue; //0.6.3 and older version not supported for Revit2015 onwards
                if (!File.Exists(p.AsssemblyPath))
                    continue; // .DLL do not exist
                mProducts.Add(p.Version, p);
            }
        }

        protected IEnumerable<Product> FindDynamoRevitInstallations(string debugPath)
        {
            // Installation enumeration is based on LocateDynamoInstallations() static method of 'DynamoInstallDetective.dll'
            var assembly = Assembly.LoadFrom(Path.Combine(debugPath, "DynamoInstallDetective.dll"));
            var type = assembly.GetType("DynamoInstallDetective.Utilities");
            var LocateDynamoInstallations = type.GetMethod("LocateDynamoInstallations", BindingFlags.Public | BindingFlags.Static);
            if (LocateDynamoInstallations == null)
            {
                throw new MissingMethodException("Method 'DynamoInstallDetective.Utilities.LocateDynamoInstallations' not found");
            }

            // Where search for "DynamoRevitDS.dll" products which follow this Pattern:
            Func<string, string> fileLocator = p => Path.Combine(p, string.Format("Revit_{0}", RevitVersion), "DynamoRevitDS.dll");

            // Invoke LocateDynamoInstallations
            var methodParams = new object[] { debugPath, fileLocator };
            var installs = LocateDynamoInstallations.Invoke(null, methodParams) as IEnumerable;
            if (null == installs)
                return null;

            // Enumeration return
            return
                installs.Cast<KeyValuePair<string, Tuple<int, int, int, int>>>()
                    .Select(
                        p => new Product(new Version(p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4), fileLocator(p.Key), p.Key)
                );
        }

        /// <summary>
        /// Return true if Product list contains a specific version
        /// </summary>
        public bool Contains(Version version)
        {
            if (version == Product.LASTESTDYNAMO) return mProducts.Count > 0;
            return mProducts.ContainsKey(version);
        }

        /// <summary>
        /// Check if a specific Product is currently loaded
        /// </summary>
        public void Load(Version version)
        {
            // Retreived product
            var product = (version == Product.LASTESTDYNAMO) ? this.List.First() : mProducts[version];

            // Is a dynamo product already loaded ?
            Version currVersion = Product.LASTESTDYNAMO;
            if (IsProductLoaded(ref currVersion))
            {
                var popup = new TaskDialog(Resources.DynamoVersions);
                popup.MainInstruction = Resources.DynamoVersionSelection;
                if (product.Version == currVersion)
                {
                    popup.MainContent = String.Format(Resources.ProductLoaded, product.Version);
                }
                else
                {
                    popup.MainContent = String.Format(Resources.ProductNeedToRestartRevit, product.Version);
                }
                popup.Show();
                return;
            }
            
            // Start Dyanmo product
            var inst = product.CreateInstance();
            var OnStartup = inst.GetType().GetMethod("OnStartup");
            OnStartup.Invoke(inst, new object[] { uiControlledApplication });
        }
    }
}
