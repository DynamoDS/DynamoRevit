using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit WallType
    /// </summary>
    public class WallType : ElementType
    {
        private const string absorptanceOutputPort = "Absorptance";
        private const string heatTransferCoefficientOutputPort = "HeatTransferCoefficient";
        private const string roughnessOutputPort = "Roughness";
        private const string thermalMassOutputPort = "ThermalMass";
        private const string thermalResistanceOutputPort = "ThermalResistance";

        #region Internal properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.WallType InternalWallType => InternalElementType as Autodesk.Revit.DB.WallType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private WallType(Autodesk.Revit.DB.WallType type) : base(type)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the WallType
        /// </summary>
        public new string Name
        {
            get { return InternalWallType.Name; }
        }

        public double Width
        {
            get { return InternalWallType.Width; }
        }

        public string Kind
        {
            get { return InternalWallType.Kind.ToString(); }
        }

        public string Function
        {
            get { return InternalWallType.Function.ToString(); }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a walltype from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new WallType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.WallType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(Properties.Resources.WallTypeNotFound);
            }

            return new WallType(type)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from an existign Revit element
        /// </summary>
        /// <param name="wallType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static WallType FromExisting(Autodesk.Revit.DB.WallType wallType, bool isRevitOwned)
        {
            return new WallType(wallType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// The calculated and settable thermal properties of the WallType
        /// Returns null if the wall has no thermal properties. Curtain walls and stacked
        /// walls do not store thermal properties.
        /// </summary>
        /// <returns name = "Absorptance">Value of absorptance.</returns>
        /// <returns name = "HeatTransferCoefficient">The heat transfer coefficient value (U-Value).</returns>
        /// <returns name = "Roughness">Value of roughness.</returns>
        /// <returns name = "ThermalMass">The calculated thermal mass value.</returns>
        /// <returns name = "ThermalResistance">The calculated thermal resistance value (R-Value).</returns>
        [MultiReturn(new[] { absorptanceOutputPort, heatTransferCoefficientOutputPort, roughnessOutputPort, thermalMassOutputPort, thermalResistanceOutputPort})]
        public Dictionary<string, object> GetThermalProperties()
        {
            ThermalProperties thermalProperties = this.InternalWallType.ThermalProperties;
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
    }
}
