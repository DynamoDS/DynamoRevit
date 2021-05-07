using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit CeilingType
    /// </summary>
    public class CeilingType : ElementType
    {
        private const string absorptanceOutputPort = "Absorptance";
        private const string heatTransferCoefficientOutputPort = "HeatTransferCoefficient";
        private const string roughnessOutputPort = "Roughness";
        private const string thermalMassOutputPort = "ThermalMass";
        private const string thermalResistanceOutputPort = "ThermalResistance";

        #region Internal properties

        /// <summary>
        /// An internal reference to the CeilingType
        /// </summary>
        internal Autodesk.Revit.DB.CeilingType InternalCeilingType => InternalElementType as Autodesk.Revit.DB.CeilingType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private CeilingType(Autodesk.Revit.DB.CeilingType type) : base(type)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the CeilingType
        /// </summary>
        public new string Name
        {
            get { return InternalCeilingType.Name; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a CeilingType from the document given 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new CeilingType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var ceilingType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.CeilingType>()
                .FirstOrDefault(x => x.Name == name);

            if (ceilingType == null)
            {
                throw new Exception(Properties.Resources.CeilingTypeNotFound);
            }

            // until there is a way to create a CeilingType from Dynamo, 
            // this object should never be cleaned up
            return new CeilingType(ceilingType)
            {
                IsRevitOwned = true
            };

        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a CeilingType from a user selected Element.
        /// </summary>
        /// <param name="ceilingType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static CeilingType FromExisting(Autodesk.Revit.DB.CeilingType ceilingType, bool isRevitOwned)
        {
            return new CeilingType(ceilingType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The calculated and settable thermal properties of the CeilingType
        /// </summary>
        /// <returns name = "Absorptance">Value of absorptance.</returns>
        /// <returns name = "HeatTransferCoefficient">The heat transfer coefficient value (U-Value).</returns>
        /// <returns name = "Roughness">Value of roughness.</returns>
        /// <returns name = "ThermalMass">The calculated thermal mass value.</returns>
        /// <returns name = "ThermalResistance">The calculated thermal resistance value (R-Value).</returns>
        [MultiReturn(new[] { absorptanceOutputPort, heatTransferCoefficientOutputPort, roughnessOutputPort, thermalMassOutputPort, thermalResistanceOutputPort })]
        public Dictionary<string, object> GetThermalProperties()
        {
            ThermalProperties thermalProperties = this.InternalCeilingType.ThermalProperties;
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
            return InternalCeilingType.Name;
        }
    }
}
