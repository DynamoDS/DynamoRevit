using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Transactions;

namespace Revit.Elements
{
    public class FamilyParameter
    {
        internal Autodesk.Revit.DB.FamilyParameter InternalFamilyParameter { get; set; }

        internal FamilyParameter(Autodesk.Revit.DB.FamilyParameter internalFamilyParameter)
        {
            this.InternalFamilyParameter = internalFamilyParameter;
        }

        #region Properties

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name => InternalFamilyParameter.Definition.Name;

        /// <summary>
        /// Check if the Parameter is read only
        /// </summary>
        public bool IsReadOnly => InternalFamilyParameter.IsReadOnly;

        /// <summary>
        /// Check if the Parameter is shared
        /// </summary>
        public bool IsShared => InternalFamilyParameter.IsShared;

        /// <summary>
        /// Get the parameter's group
        /// </summary>
        public string Group => InternalFamilyParameter.Definition.ParameterGroup.ToString();

        /// <summary>
        /// Get the parameter type
        /// </summary>
        public string ParameterType => InternalFamilyParameter.Definition.ParameterType.ToString();

        /// <summary>
        /// Get the parameter's element Id
        /// </summary>
        public int Id => InternalFamilyParameter.Id.IntegerValue;

        /// <summary>
        /// Get the parameter's unit type
        /// </summary>
        public string UnitType => InternalFamilyParameter.Definition.UnitType.ToString();

        /// <summary>
        /// Get Parameter Storage Type
        /// </summary>
        public string StorageType => InternalFamilyParameter.StorageType.ToString();

        /// <summary>
        /// Indicates if this parameter can be assigned a formula.
        /// </summary>
        public bool CanAssignFormula => InternalFamilyParameter.CanAssignFormula;

        /// <summary>
        /// Indicates if the parameter is a reporting parameter.
        /// </summary>
        public bool IsReporting => InternalFamilyParameter.IsReporting;

        #endregion

    }
}
