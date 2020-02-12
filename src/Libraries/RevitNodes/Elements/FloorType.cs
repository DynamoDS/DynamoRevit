using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FloorType
    /// </summary>
    /// http://revitapisearch.com.s3-website-us-east-1.amazonaws.com/html/b6fd8c08-7eea-1ab4-b7ab-096778b46e8f.htm
    public class FloorType : ElementType
    {
        private const string absorptanceOutputPort = "Absorptance";
        private const string heatTransferCoefficientOutputPort = "HeatTransferCoefficient";
        private const string roughnessOutputPort = "Roughness";
        private const string thermalMassOutputPort = "ThermalMass";
        private const string thermalResistanceOutputPort = "ThermalResistance";

        #region Internal properties

        /// <summary>
        /// An internal reference to the FloorType
        /// </summary>
        internal Autodesk.Revit.DB.FloorType InternalFloorType => InternalElementType as Autodesk.Revit.DB.FloorType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private FloorType(Autodesk.Revit.DB.FloorType type) : base(type)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the FloorType
        /// </summary>
        public new string Name
        {
            get { return InternalFloorType.Name; }
        }

        /// <summary>
        /// Returns whether the element FloorAttributes type is FoundationSlab.
        /// </summary>
        public bool IsFoundationSlab
        {
            get { return InternalFloorType.IsFoundationSlab; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a FloorType from the document given 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new FloorType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var floorType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.FloorType>()
                .FirstOrDefault(x => x.Name == name);

            if (floorType == null)
            {
                throw new Exception(Properties.Resources.FloorTypeNotFound);
            }

            // until there is a way to create a FloorType from Dynamo, 
            // this object should never be cleaned up
            return new FloorType(floorType)
            {
                IsRevitOwned = true
            };

        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a FloorType from a user selected Element.
        /// </summary>
        /// <param name="floorType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FloorType FromExisting(Autodesk.Revit.DB.FloorType floorType, bool isRevitOwned)
        {
            return new FloorType(floorType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the structural material of the FloorType.
        /// </summary>
        /// <returns>Returns the material that defines the element's structural analysis properties.</returns>
        public Material GetStructuralMaterial()
        {
            ElementId materialId = InternalFloorType.StructuralMaterialId;
            if (materialId == null || materialId.IntegerValue < 0)
                throw new InvalidOperationException(Properties.Resources.NoStructuralMaterialAssigned);
            return Document.GetElement(materialId).ToDSType(true) as Material;
        }

        /// <summary>
        /// The calculated and settable thermal properties of the FloorType
        /// </summary>
        /// <returns name = "Absorptance">Value of absorptance.</returns>
        /// <returns name = "HeatTransferCoefficient">The heat transfer coefficient value (U-Value).</returns>
        /// <returns name = "Roughness">Value of roughness.</returns>
        /// <returns name = "ThermalMass">The calculated thermal mass value.</returns>
        /// <returns name = "ThermalResistance">The calculated thermal resistance value (R-Value).</returns>
        [MultiReturn(new[] { absorptanceOutputPort, heatTransferCoefficientOutputPort, roughnessOutputPort, thermalMassOutputPort, thermalResistanceOutputPort })]
        public Dictionary<string, object> GetThermalProperties()
        {
            ThermalProperties thermalProperties = this.InternalFloorType.ThermalProperties;
            if (thermalProperties == null)
                throw new InvalidOperationException(nameof(GetThermalProperties));

            return new Dictionary<string, object>
            {
                { absorptanceOutputPort, thermalProperties.Absorptance },
                { heatTransferCoefficientOutputPort, thermalProperties.HeatTransferCoefficient },
                { roughnessOutputPort, thermalProperties.Roughness },
                { thermalMassOutputPort, thermalProperties.ThermalMass },
                { thermalResistanceOutputPort, thermalProperties.ThermalResistance }
            };
        }

        #endregion

        public override string ToString()
        {
            return InternalFloorType.Name;
        }
    }
}
