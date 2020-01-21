using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Transactions;

namespace Revit.Elements
{
    public class FamilyParameter
    {
        internal Autodesk.Revit.DB.FamilyParameter InternalFamilyParameter
        {
            get;
            set;
        }

        internal FamilyParameter(Autodesk.Revit.DB.FamilyParameter internalFamilyParameter)
        {
            this.InternalFamilyParameter = internalFamilyParameter;
        }

        #region Properties

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name
        {
            get
            {
                return InternalFamilyParameter.Definition.Name;
            }
        }

        /// <summary>
        /// Check if the Parameter is read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return InternalFamilyParameter.IsReadOnly; }
        }

        /// <summary>
        /// Check if the Parameter is shared
        /// </summary>
        public bool IsShared
        {
            get { return InternalFamilyParameter.IsShared; }
        }

        /// <summary>
        /// Get the parameter's group
        /// </summary>
        public string Group
        {
            get { return InternalFamilyParameter.Definition.ParameterGroup.ToString(); }
        }

        /// <summary>
        /// Get the parameter type
        /// </summary>
        public string ParameterType
        {
            get { return InternalFamilyParameter.Definition.ParameterType.ToString(); }
        }

        /// <summary>
        /// Get the parameter's element Id
        /// </summary>
        public int Id
        {
            get { return InternalFamilyParameter.Id.IntegerValue; }
        }

        /// <summary>
        /// Get the parameter's unit type
        /// </summary>
        public string UnitType
        {
            get { return InternalFamilyParameter.Definition.UnitType.ToString(); }
        }


        /// <summary>
        /// Get Parameter Storage Type
        /// </summary>
        public string StorageType
        {
            get
            {
                return InternalFamilyParameter.StorageType.ToString();
            }
        }

        /// <summary>
        /// Indicates if this parameter can be assigned a formula.
        /// </summary>
        public bool CanAssignFormula
        {
            get { return InternalFamilyParameter.CanAssignFormula; }
        }

        public bool IsReporting
        {
            get { return InternalFamilyParameter.IsReporting; }
        }

        #endregion

    }
}
