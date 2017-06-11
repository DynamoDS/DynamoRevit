using System;
using System.Linq;
using System.Collections.Generic;
using DynamoServices;
using RevitServices.Persistence;

namespace Revit.Elements
{
    [RegisterForTrace]
    public class Material : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Element
        /// </summary>
        internal Autodesk.Revit.DB.Material InternalMaterial
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement => InternalMaterial;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for DSMaterial
        /// </summary>
        /// <param name="material"></param>
        private Material(Autodesk.Revit.DB.Material material)
        {
            SafeInit(() => InitMaterial(material));
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a Material element
        /// </summary>
        /// <param name="material"></param>
        private void InitMaterial(Autodesk.Revit.DB.Material material)
        {
            InternalSetMaterial(material);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ELementId, and UniqueId
        /// </summary>
        /// <param name="material"></param>
        private void InternalSetMaterial(Autodesk.Revit.DB.Material material)
        {
            InternalMaterial = material;
            InternalElementId = material.Id;
            InternalUniqueId = material.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a material from the current document by the name
        /// </summary>
        /// <param name="name">The name of the material</param>
        /// <returns></returns>
        public static Material ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>()
                .FirstOrDefault(x => x.Name == name);

            if (mat == null)
            {
                throw new Exception(Properties.Resources.MaterialNotFound);
            }

            return new Material(mat)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get Material Name
        /// </summary>
        /// <returns></returns>
        public new string Name => InternalMaterial.Name;

        /// <summary>
        /// Get Shininess
        /// </summary>
        public int Shininess => InternalMaterial.Shininess;

        /// <summary>
        /// Get Smoothness
        /// </summary>
        public int Smoothness => InternalMaterial.Smoothness;

        /// <summary>
        /// Get Transparency
        /// </summary>
        public int Transparency => InternalMaterial.Transparency;

        /// <summary>
        /// Get SurfacePatternColor
        /// </summary>
        public DSCore.Color SurfacePatternColor => ToDSColor(InternalMaterial.SurfacePatternColor);

        /// <summary>
        /// Get Material Class
        /// </summary>
        public string MaterialClass => InternalMaterial.MaterialClass;

        /// <summary>
        /// Get Material category
        /// </summary>
        public string MaterialCategory => InternalMaterial.MaterialCategory;

        /// <summary>
        /// Get cut pattern color
        /// </summary>
        public DSCore.Color CutPatternColor => ToDSColor(InternalMaterial.CutPatternColor);

        /// <summary>
        /// Get color
        /// </summary>
        public DSCore.Color Color => ToDSColor(InternalMaterial.Color);

        /// <summary>
        /// Get cut pattern id
        /// </summary>
        public int CutPatternId => InternalMaterial.CutPatternId.IntegerValue;

        /// <summary>
        /// Get all apperance parameters
        /// </summary>
        public IEnumerable<Parameter> AppearanceParameters
        {
            get
            {
                // Get the active Document
                var document = DocumentManager.Instance.CurrentDBDocument;

                var appearances = new List<Parameter>();
                if (InternalMaterial.AppearanceAssetId == Autodesk.Revit.DB.ElementId.InvalidElementId) return appearances;

                var appearance = document.GetElement(InternalMaterial.AppearanceAssetId) as Autodesk.Revit.DB.AppearanceAssetElement;
                if (appearance == null) return appearances;

                foreach (var parameter in appearance.Parameters)
                {
                    var p = new Parameter(parameter as Autodesk.Revit.DB.Parameter);
                    if (!appearances.Contains(p)) appearances.Add(p);
                }
                return appearances;
            }
        }

        /// <summary>
        /// Get all thermal parameters
        /// </summary>
        public IEnumerable<Parameter> ThermalParameters
        {
            get
            {
                var document = DocumentManager.Instance.CurrentDBDocument;

                var thermals = new List<Parameter>();
                if (InternalMaterial.ThermalAssetId == Autodesk.Revit.DB.ElementId.InvalidElementId) return thermals;

                var thermal = document.GetElement(InternalMaterial.ThermalAssetId) as Autodesk.Revit.DB.PropertySetElement;
                if (thermal == null) return thermals;

                foreach (var parameter in thermal.Parameters)
                {
                    var p = new Parameter(parameter as Autodesk.Revit.DB.Parameter);
                    if (!thermals.Contains(p)) thermals.Add(p);
                }

                return thermals;
            }
        }

        /// <summary>
        /// Get all structural parameters
        /// </summary>
        public IEnumerable<Parameter> StructuralParameters
        {
            get
            {
                var document = DocumentManager.Instance.CurrentDBDocument;

                var structurals = new List<Parameter>();
                if (InternalMaterial.StructuralAssetId == Autodesk.Revit.DB.ElementId.InvalidElementId) return structurals;

                var structural = document.GetElement(InternalMaterial.StructuralAssetId) as Autodesk.Revit.DB.PropertySetElement;
                if (structural == null) return structurals;

                foreach (var parameter in structural.Parameters)
                {
                    var p = new Parameter(parameter as Autodesk.Revit.DB.Parameter);
                    if (!structurals.Contains(p)) structurals.Add(p);
                }

                return structurals;
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Convert Revit to DS Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private DSCore.Color ToDSColor(Autodesk.Revit.DB.Color color)
        {
            return DSCore.Color.ByARGB(255, color.Red, color.Green, color.Blue);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Wrap an element in the associated DS type
        /// </summary>
        /// <param name="material">The material</param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Material FromExisting(Autodesk.Revit.DB.Material material, bool isRevitOwned)
        {
            return new Material(material)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
