using System;
using System.Collections.Generic;
using System.Linq;
using DynamoServices;
using RevitServices.Persistence;
using System.Diagnostics;

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
            get { return ToDSColor(this.InternalMaterial.SurfaceForegroundPatternColor); }
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
            get { return ToDSColor(this.InternalMaterial.CutForegroundPatternColor); }
        }

        /// <summary>
        /// Get color
        /// </summary>
        public DSCore.Color Color
        {
            get { return ToDSColor(this.InternalMaterial.Color); }
        }

        /// <summary>
        /// Get cut pattern id
        /// </summary>
        public int CutPatternId
        {
            get
            {
                return this.InternalMaterial.CutForegroundPatternId.IntegerValue;
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
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

                List<Parameter> appearances = new List<Parameter>();
                List<Parameter> invalidAppearances = new List<Parameter>();
                if (this.InternalMaterial.AppearanceAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.AppearanceAssetElement appearance = document.GetElement(this.InternalMaterial.AppearanceAssetId) as Autodesk.Revit.DB.AppearanceAssetElement;
                    if (appearance != null)
                    {
                        FilterParameters(appearance.Parameters, ref appearances, ref invalidAppearances);
                    }
                }
#if DEBUG
                if (invalidAppearances.Any())
                {
                    Debug.WriteLine("There are {0} invalid structural parameters", invalidAppearances.Count);
                    foreach (var p in invalidAppearances)
                    {
                        Debug.WriteLine(string.Format("\t{0}", p.Id));
                    }
                }
#endif
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
                List<Parameter> invalidThermals = new List<Parameter>();
                if (this.InternalMaterial.ThermalAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.PropertySetElement thermal = document.GetElement(this.InternalMaterial.ThermalAssetId) as Autodesk.Revit.DB.PropertySetElement;
                    if (thermal != null)
                    {
                        FilterParameters(thermal.Parameters, ref thermals, ref invalidThermals);
                    }
                }

#if DEBUG
                if (invalidThermals.Any())
                {
                    Debug.WriteLine("There are {0} invalid structural parameters", invalidThermals.Count);
                    foreach (var p in invalidThermals)
                    {
                        Debug.WriteLine(string.Format("\t{0}", p.Id));
                    }
                }
#endif

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
                List<Parameter> invalidStructurals = new List<Parameter>();
                if (this.InternalMaterial.StructuralAssetId != Autodesk.Revit.DB.ElementId.InvalidElementId)
                {
                    Autodesk.Revit.DB.PropertySetElement structural = document.GetElement(this.InternalMaterial.StructuralAssetId) as Autodesk.Revit.DB.PropertySetElement;
                    if (structural != null)
                    {
                        FilterParameters(structural.Parameters, ref structurals, ref invalidStructurals);
                    }
                }
#if DEBUG
                if(invalidStructurals.Any())
                {
                    Debug.WriteLine("There are {0} invalid structural parameters", invalidStructurals.Count);
                    foreach(var p in invalidStructurals)
                    {
                        Debug.WriteLine(string.Format("\t{0}", p.Id));
                    }
                }
#endif
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

        private void FilterParameters(Autodesk.Revit.DB.ParameterSet parameters, ref List<Parameter> ValidParameters, ref List<Parameter> InvalidParameters)
        {
            foreach (var parameter in parameters)
            {
                Parameter p = new Parameter(parameter as Autodesk.Revit.DB.Parameter);
                if (p.InternalParameter.Definition == null)
                {
                    if (!InvalidParameters.Contains(p))
                        InvalidParameters.Add(p);
                    continue;
                }
                if (!ValidParameters.Contains(p))
                    ValidParameters.Add(p);
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
