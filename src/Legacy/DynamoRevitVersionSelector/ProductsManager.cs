using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace Dynamo.Applications
{
    internal class ProductsManager
    {
        /// <summary>
        /// Check if a Product is currently loaded in Current Application Domain and returns its version.
        /// </summary>
        /// <returns>Dynamo product which is currently loaded, Product.NODYNAMO otherwise</returns>
        public static Version LoadedProduct()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms)
            {
                if (asm.GetType(Product.DYNAMO_TYPENAME) != null)
                {
                    return asm.GetName().Version;
                }
            }
            return Product.NODYNAMO;
        }

        /// <summary>
        /// Revit Version
        /// </summary>
        public string RevitVersion
        {
            get; private set;
        }

        #region Product List
        /// <summary>
        /// Accessor to the list of Dyanmo Product (the list is sorted).
        /// </summary>
        /// <seealso cref="Contains"/>
        public List<Product> List
        {
            get {
                var list = mProducts.Values.ToList();
                return list.OrderByDescending(x => x.Version).ToList(); // -- list.first() = most recent version
            }
        }

        /// <summary>
        /// Number of Dyanmo Products available
        /// </summary>
        /// <seealso cref="Contains"/>
        public int Count
        {
            get { return mProducts.Count; }
        }

        /// <summary>
        /// Return true if Product list contains a specific version (product should exist and be a real)
        /// </summary>
        public bool Contains(Version version)
        {
            if (version == Product.LASTESTDYNAMO) return false;
            if (version == Product.NODYNAMO) return false;
            return mProducts.ContainsKey(version);
        }

        /// <summary>
        /// Return true if Product list contains at least one product with dynamo player in it
        /// </summary>
        public bool ContainsDynamoPlayer()
        {
            //Minimum version requirements needed by Playlist feature for Dynamo and Revit.
            Version MinDynamoVersionForPlaylist = new Version(1, 2);
            foreach (var p in mProducts.Values)
            {
                if (p.Version >= MinDynamoVersionForPlaylist)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns one Product
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Product Get(Version version)
        {
            return mProducts[version];
        }
        #endregion

        protected UIControlledApplication uiControlledApplication;
        protected Dictionary<Version, Product> mProducts;

        public delegate void BeforeLoadEventHandler();
        public event BeforeLoadEventHandler OnBeforeLoad;

        /// <summary>
        /// ProductsManager Contructor, Manage available Dyanmo Products (DLLs assemblies)
        /// </summary>
        public ProductsManager(UIControlledApplication application)
        {
            uiControlledApplication = application;
            RevitVersion = application.ControlledApplication.VersionNumber;
            var revitFolder = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
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
                throw new MissingMethodException("Method 'DynamoInstallDetective.Utilities.LocateDynamoInstallations' not found");

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

        internal class LoadResult
        {
            /// <summary>
            /// Version before Load() call
            /// </summary>
            public Version fromVersion;

            /// <summary>
            /// Version after Load() call
            /// </summary>
            public Version afterVersion;

            public enum ResultType {
                FAILED,
                DONE,
                PENDING
            };

            /// <summary>
            /// Load Result
            /// </summary>
            public ResultType Result;

            public bool IsSucceed {
                get { return Result != ResultType.FAILED; }
            }
        }

        /// <summary>
        /// Load a specific product version
        /// </summary>
        /// <param name="version">Version expected</param>
        /// <returns>Succeed if no error occurs</returns>
        public LoadResult Load(Version version)
        {
            // Retreived product
            Product product = null;
            do
            {
                if (version == Product.NODYNAMO)
                {
                    product = new Product(Product.NODYNAMO);
                    break;
                }
                if (version == Product.LASTESTDYNAMO)
                {
                    product = this.List.First();
                    break;
                }

                product = mProducts[version];
            } while (false);

            LoadResult ret = new LoadResult();
            ret.fromVersion = LoadedProduct();
            ret.afterVersion = product.Version;

            // Trivial - nothing to do
            if (ret.fromVersion == ret.afterVersion)
            {
                ret.Result = LoadResult.ResultType.DONE;
                return ret;
            }

            // Can we Load dll right now ?
            if (ret.fromVersion != Product.NODYNAMO)
            {
                ret.Result = LoadResult.ResultType.PENDING;
                return ret;
            }

            // Actual Load
            try
            {
                OnBeforeLoad?.Invoke();
                product.CreateInstance();
                if (Result.Succeeded != product.OnStartup(uiControlledApplication))
                    throw new Exception("OnStartup() Error");
                ret.Result = LoadResult.ResultType.DONE;
                return ret;
            }
            catch
            {
                ret.Result = LoadResult.ResultType.FAILED;
                return ret;
            }
        }
    }
}
