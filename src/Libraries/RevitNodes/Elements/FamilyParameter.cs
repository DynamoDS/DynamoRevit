using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;

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
        /// Get the parameter's element Id
        /// </summary>
        public int Id => InternalFamilyParameter.Id.IntegerValue;

        /// <summary>
        /// Get the parameter's spec type
        /// </summary>
        public SpecType SpecType
        {
            get
            {
                var forgeTypeId = InternalFamilyParameter.Definition.GetDataType();

                return string.IsNullOrEmpty(forgeTypeId.TypeId)? null : SpecType.FromExisting(forgeTypeId);
            }
        }

        /// <summary>
        /// Get the parameter's group type
        /// </summary>
        public GroupType GroupType
        {
            get
            {
                var forgeTypeId = InternalFamilyParameter.Definition.GetGroupTypeId();

                return string.IsNullOrEmpty(forgeTypeId.TypeId) ? null : GroupType.FromExisting(forgeTypeId);
            }
        }

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
