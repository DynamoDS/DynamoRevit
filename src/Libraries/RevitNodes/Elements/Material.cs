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
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalMaterial; }
        }

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
            this.InternalMaterial = material;
            this.InternalElementId = material.Id;
            this.InternalUniqueId = material.UniqueId;
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
                throw new ArgumentNullException("name");
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
        public string Name
        {
            get { return this.InternalMaterial.Name; }
        }

        /// <summary>
        /// Get Shininess
        /// </summary>
        public int Shininess
        {
            get { return this.InternalMaterial.Shininess; }
        }

        /// <summary>
        /// Get Smoothness
        /// </summary>
        public int Smoothness
        {
            get { return this.InternalMaterial.Smoothness; }
        }

        /// <summary>
        /// Get Transparency
        /// </summary>
        public int Transparency
        {
            get { return this.InternalMaterial.Transparency; }
        }

        /// <summary>
        /// Get SurfacePatternColor
        /// </summary>
        public DSCore.Color SurfacePatternColor
        {
            get
            {
                return DSCore.Color.ByARGB(255,
                    this.InternalMaterial.SurfacePatternColor.Red,
                    this.InternalMaterial.SurfacePatternColor.Green,
                    this.InternalMaterial.SurfacePatternColor.Blue);
            }
        }

        /// <summary>
        /// Get Material Class
        /// </summary>
        public string MaterialClass
        {
            get { return this.InternalMaterial.MaterialClass; }
        }

        /// <summary>
        /// Get Material category
        /// </summary>
        public string MaterialCategory
        {
            get { return this.InternalMaterial.MaterialCategory; }
        }

        /// <summary>
        /// Get cut pattern color
        /// </summary>
        public DSCore.Color CutPatternColor
        {
            get {
                return DSCore.Color.ByARGB(255, 
                    this.InternalMaterial.CutPatternColor.Red, 
                    this.InternalMaterial.CutPatternColor.Green, 
                    this.InternalMaterial.CutPatternColor.Blue);
            }
        }

        /// <summary>
        /// Get color
        /// </summary>
        public DSCore.Color Color
        {
            get
            {
                return DSCore.Color.ByARGB(255,
                    this.InternalMaterial.Color.Red,
                    this.InternalMaterial.Color.Green,
                    this.InternalMaterial.Color.Blue);
            }
        }

        /// <summary>
        /// Get cut pattern id
        /// </summary>
        public Autodesk.Revit.DB.ElementId CutPatternId
        {
            get
            {
                return this.InternalMaterial.CutPatternId;
            }
        }

        /// <summary>
        /// Get all apperance parameters
        /// </summary>
        public IEnumerable<Parameter> AppearanceParameters
        {
            get
            {
                // Get the active Document
                var document = DocumentManager.Instance.CurrentDBDocument;

                List<Parameter> appearances = new List<Parameter>();
                if (this.InternalMaterial.AppearanceAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.AppearanceAssetElement appearance = (Autodesk.Revit.DB.AppearanceAssetElement)document.GetElement(this.InternalMaterial.AppearanceAssetId);

                    foreach (Autodesk.Revit.DB.Parameter parameter in appearance.Parameters)
                    {
                        Parameter p = new Parameter(parameter);
                        if (!appearances.Contains(p)) appearances.Add(p);
                    }
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
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

                List<Parameter> thermals = new List<Parameter>();
                if (this.InternalMaterial.ThermalAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.PropertySetElement thermal = (Autodesk.Revit.DB.PropertySetElement)document.GetElement(this.InternalMaterial.ThermalAssetId);

                    foreach (Autodesk.Revit.DB.Parameter parameter in thermal.Parameters)
                    {
                        Parameter p = new Parameter(parameter);
                        if (!thermals.Contains(p)) thermals.Add(p);
                    }
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
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

                List<Parameter> structurals = new List<Parameter>();
                if (this.InternalMaterial.StructuralAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.PropertySetElement structural = (Autodesk.Revit.DB.PropertySetElement)document.GetElement(this.InternalMaterial.StructuralAssetId);

                    foreach (Autodesk.Revit.DB.Parameter parameter in structural.Parameters)
                    {
                        Parameter p = new Parameter(parameter);
                        if (!structurals.Contains(p)) structurals.Add(p);
                    }
                }

                return structurals;
            }
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
