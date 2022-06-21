using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamoServices;
using RevitServices.Transactions;
using Autodesk.DesignScript.Runtime;
using System.Reflection;

namespace Revit.Elements
{
    [RegisterForTrace]
    public class AppearanceAssetElement : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Element
        /// </summary>
        internal Autodesk.Revit.DB.AppearanceAssetElement InternalAppearanceAssetElement
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalAppearanceAssetElement; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for DSAppearanceAssetElement
        /// </summary>
        /// <param name="appearanceAssetElement"></param>
        private AppearanceAssetElement(Autodesk.Revit.DB.AppearanceAssetElement appearanceAssetElement)
        {
            SafeInit(() => InitAppearanceAssetElement(appearanceAssetElement), true);
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a AppearanceAssetElement element
        /// </summary>
        /// <param name="appearanceAssetElement"></param>
        private void InitAppearanceAssetElement(Autodesk.Revit.DB.AppearanceAssetElement appearanceAssetElement)
        {
            InternalSetAppearanceAssetElement(appearanceAssetElement);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ELementId, and UniqueId
        /// </summary>
        /// <param name="appearanceAssetElement"></param>
        private void InternalSetAppearanceAssetElement(Autodesk.Revit.DB.AppearanceAssetElement appearanceAssetElement)
        {
            this.InternalAppearanceAssetElement = appearanceAssetElement;
            this.InternalElementId = appearanceAssetElement.Id;
            this.InternalUniqueId = appearanceAssetElement.UniqueId;
        }

      #endregion

      #region public methods

      /// <summary>
      /// Get texture image path of the appearance asset
      /// </summary>
      /// <returns name = "imageProperties">The list of all the properties that have images connected. A property's meaning can be queried in Revit API documentation, e.g., for "GenericDiffuse.UnifiedbitmapBitmap", it means property "GenericDiffuse" connects a texture asset which has a "UnifiedbitmapBitmap" property</returns>
      /// <returns name = "imagePaths">The list of all the image path</returns>
      [MultiReturn(new[] { "imageProperties", "imagePaths" })]
        public Dictionary<string, object> GetRenderingAssetTextureImages()
        {
            var asset = this.InternalAppearanceAssetElement.GetRenderingAsset();
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            List<string> KeyValue = new List<string>();
            KeyValue.AddRange(GetAssetProperties(asset, ""));
            foreach(var value in KeyValue)
            {
               var strs = value.Split(new Char[] { ':' }, 2);
               if (strs.Length == 2)
               {
                  keys.Add(strs[0]);
                  values.Add(strs[1]); 
               }
            }
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            keyValues.Add("imageProperties", keys);
            keyValues.Add("imagePaths", values);
            return keyValues;
        }

      /// <summary>
      /// Set texture image path of the appearance asset
      /// </summary>
      /// <param name="imageProperty">A property that has a image connected. This can be queried by calling GetRenderingAssetTextureImages first</param>
      /// <param name="imagePath">File path of the image</param>
      /// <returns>AppearanceAssetElement</returns>
      public AppearanceAssetElement SetRenderingAssetTextureImage(string imageProperty, string imagePath)
        {
            var properties = imageProperty.Split('.');
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            {
                using (Autodesk.Revit.DB.Visual.AppearanceAssetEditScope editScope =
                    new Autodesk.Revit.DB.Visual.AppearanceAssetEditScope(Application.Document.Current.InternalDocument))
                {
                    Autodesk.Revit.DB.Visual.Asset editableAsset = editScope.Start(InternalElementId);
                    Type assetClass = GetAssetClass(editableAsset);
                    PropertyInfo pinfo = assetClass.GetProperty(properties[0]);
                    object internalProp = pinfo.GetValue(null);
                    Autodesk.Revit.DB.Visual.AssetProperty assetProperty = editableAsset.FindByName(internalProp.ToString());
                    if(assetProperty == null)
                    {
                        throw new ArgumentException(Properties.Resources.AppearanceAssetElementPropertyPathInvalid);
                    }
                    Autodesk.Revit.DB.Visual.Asset connentedAsset = assetProperty.GetSingleConnectedAsset();
                    for (int i = 1; i < properties.Length; i++)
                    {
                        if(connentedAsset == null)
                        {
                            throw new ArgumentException(Properties.Resources.AppearanceAssetElementPropertyPathInvalid);
                        }
                        Type connentedAssetClass = GetAssetClass(connentedAsset);
                        PropertyInfo connentedPinfo = connentedAssetClass.GetProperty(properties[i]);
                        object connentedInternalProp = connentedPinfo.GetValue(null);
                        assetProperty = connentedAsset.FindByName(connentedInternalProp.ToString());
                        if (assetProperty == null)
                        {
                            throw new ArgumentException(Properties.Resources.AppearanceAssetElementPropertyPathInvalid);
                        }
                        connentedAsset = assetProperty.GetSingleConnectedAsset();
                    }
                    if (assetProperty != null && assetProperty.Name == Autodesk.Revit.DB.Visual.UnifiedBitmap.UnifiedbitmapBitmap)
                    {
                        (assetProperty as Autodesk.Revit.DB.Visual.AssetPropertyString).Value = imagePath;
                    }
                    else
                    {
                        throw new ArgumentException(Properties.Resources.AppearanceAssetElementPropertyPathInvalid);
                    }
                    editScope.Commit(true);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Private Helpers

        private List<string> GetAssetProperties(Autodesk.Revit.DB.Visual.Asset asset, string keyName)
        {
            Type assetClass = GetAssetClass(asset);
            Dictionary<string, string> InternalPropertiesMap = new Dictionary<string, string>();
            foreach (var p in assetClass.GetProperties())
            {
               var v = p.GetValue(null);
               InternalPropertiesMap.Add(v.ToString(), p.Name);
            }
            List<string> AssetProps = new List<string>();
            Autodesk.Revit.DB.Visual.AssetProperty nameProperty = asset.FindByName("BaseSchema");
            string assetSchema = (nameProperty as Autodesk.Revit.DB.Visual.AssetPropertyString).Value;
            if (assetSchema == "UnifiedBitmapSchema")
            {
                var path = asset.FindByName(Autodesk.Revit.DB.Visual.UnifiedBitmap.UnifiedbitmapBitmap) as Autodesk.Revit.DB.Visual.AssetPropertyString;
                if (path != null) 
                {
                    string APIProperty;
                    if (InternalPropertiesMap.TryGetValue(path.Name, out APIProperty))
                    {
                        string newKeyName = keyName + "." + APIProperty;
                        AssetProps.Add(newKeyName + ":" + path.Value);
                        return AssetProps;
                    }
                }
            }

            for (int i = 0; i < asset.Size; i++)
            {
                Autodesk.Revit.DB.Visual.AssetProperty assetProperty = asset.Get(i);
                if (assetProperty.NumberOfConnectedProperties > 0)
                {
                    var connProps = assetProperty.GetAllConnectedProperties();
                    foreach(var prop in connProps)
                    {
                        string APIProperty;
                        if (InternalPropertiesMap.TryGetValue(assetProperty.Name, out APIProperty))
                        {
                           string newKeyName = String.IsNullOrEmpty(keyName) ? APIProperty : keyName + "." + APIProperty;
                           AssetProps.AddRange(GetAssetProperties(prop as Autodesk.Revit.DB.Visual.Asset, newKeyName));
                        }
                    }
                }
            }

            return AssetProps;
        }

        private Type GetAssetClass(Autodesk.Revit.DB.Visual.Asset asset)
        {
            Autodesk.Revit.DB.Visual.AssetProperty nameProperty = asset.FindByName("BaseSchema");
            string assetSchema = (nameProperty as Autodesk.Revit.DB.Visual.AssetPropertyString).Value;
            string assetType = "Autodesk.Revit.DB.Visual." + assetSchema.Substring(0, assetSchema.Length - 6);
            // Get Revit Assembly
            var revitAssembly = System.Reflection.Assembly.GetAssembly(typeof(Autodesk.Revit.DB.Visual.Asset));
            // Get Type from Assembly by string
            Type assetClass = revitAssembly.GetType(assetType);
            return assetClass;
        }
        #endregion

        #region Internal static constructors

        /// <summary>
        /// Wrap an element in the associated DS type
        /// </summary>
        /// <param name="appearanceAssetElement">The appearanceAssetElement</param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static AppearanceAssetElement FromExisting(Autodesk.Revit.DB.AppearanceAssetElement appearanceAssetElement, bool isRevitOwned)
        {
            return new AppearanceAssetElement(appearanceAssetElement)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
