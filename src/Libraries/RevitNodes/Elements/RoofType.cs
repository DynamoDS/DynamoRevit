using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit RoofType
    /// </summary>
    public class RoofType : ElementType
    {
        private const string absorptanceOutputPort = "Absorptance";
        private const string heatTransferCoefficientOutputPort = "HeatTransferCoefficient";
        private const string roughnessOutputPort = "Roughness";
        private const string thermalMassOutputPort = "ThermalMass";
        private const string thermalResistanceOutputPort = "ThermalResistance";

        #region Internal properties

        /// <summary>
        /// An internal reference to the RoofType
        /// </summary>
        internal Autodesk.Revit.DB.RoofType InternalRoofType => InternalElementType as Autodesk.Revit.DB.RoofType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="roofType"></param>
        private RoofType(Autodesk.Revit.DB.RoofType roofType) : base(roofType)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the RoofType
        /// </summary>
        public new string Name
        {
            get { return InternalRoofType.Name; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a RoofType from the document given 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new RoofType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var RoofType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.RoofType>()
                .FirstOrDefault(x => x.Name == name);

            if (RoofType == null)
            {
                throw new Exception(Properties.Resources.RoofTypeNotFound);
            }

            // until there is a way to create a RoofType from Dynamo, 
            // this object should never be cleaned up
            return new RoofType(RoofType)
            {
                IsRevitOwned = true
            };

        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a RoofType from a user selected Element.
        /// </summary>
        /// <param name="RoofType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static RoofType FromExisting(Autodesk.Revit.DB.RoofType RoofType, bool isRevitOwned)
        {
            return new RoofType(RoofType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The calculated and settable thermal properties of the RoofType.
        /// </summary>
        /// <returns name = "Absorptance">Value of absorptance.</returns>
        /// <returns name = "HeatTransferCoefficient">The heat transfer coefficient value (U-Value).</returns>
        /// <returns name = "Roughness">Value of roughness.</returns>
        /// <returns name = "ThermalMass">The calculated thermal mass value.</returns>
        /// <returns name = "ThermalResistance">The calculated thermal resistance value (R-Value).</returns>
        [MultiReturn(new[] { absorptanceOutputPort, heatTransferCoefficientOutputPort, roughnessOutputPort, thermalMassOutputPort, thermalResistanceOutputPort })]
        public Dictionary<string, object> GetThermalProperties()
        {
            ThermalProperties thermalProperties = this.InternalRoofType.ThermalProperties;
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
            return InternalRoofType.Name;
        }
    }
}
