using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamoServices;
using RevitServices.Transactions;

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

        #region Properties

        /// <summary>
        /// Get RenderingAsset Texture Images
        /// </summary>
        public Dictionary<string, string> GetRenderingAssetTextureImages
        {
            get
            {
                var asset = this.InternalAppearanceAssetElement.GetRenderingAsset();

                List<string> KeyValue = new List<string>();
                Dictionary<string, string> keyValues = new Dictionary<string, string>();
                KeyValue.AddRange(GetAssetProperties(asset, ""));
                foreach(var value in KeyValue)
                {
                    var strs = value.Split(new Char[] { ':' }, 2);
                    if (strs.Length == 2)
                    { 
                        keyValues.Add(strs[0], strs[1]); 
                    }
                }
                return keyValues;
            }
        }

        /// <summary>
        /// Set ImagePath for a Texture Asset.
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public AppearanceAssetElement SetRenderingAssetTextureImage(string propertyPath, string imagePath)
        {
            var properties = propertyPath.Split('.');
            TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            {
                using (Autodesk.Revit.DB.Visual.AppearanceAssetEditScope editScope =
                    new Autodesk.Revit.DB.Visual.AppearanceAssetEditScope(Application.Document.Current.InternalDocument))
                {
                    Autodesk.Revit.DB.Visual.Asset editableAsset = editScope.Start(InternalElementId);
                    Autodesk.Revit.DB.Visual.AssetProperty assetProperty = editableAsset.FindByName(properties[0]);
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
                        assetProperty = connentedAsset.FindByName(properties[i]);
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
            List<string> AssetProps = new List<string>();

            if(asset.Name == "UnifiedBitmapSchema")
            {
                var path = asset.FindByName(Autodesk.Revit.DB.Visual.UnifiedBitmap.UnifiedbitmapBitmap) as Autodesk.Revit.DB.Visual.AssetPropertyString;
                if (path != null) 
                {
                    string newKeyName = keyName + "." + path.Name;

                    AssetProps.Add(newKeyName + ":" + path.Value);
                    return AssetProps;
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
                        string newKeyName = String.IsNullOrEmpty(keyName) ? assetProperty.Name : keyName + "." + assetProperty.Name;
                        AssetProps.AddRange(GetAssetProperties(prop as Autodesk.Revit.DB.Visual.Asset, newKeyName));
                    }
                }
            }

            return AssetProps;
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
