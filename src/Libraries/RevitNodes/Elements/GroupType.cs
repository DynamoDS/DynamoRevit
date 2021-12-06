using System;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    public class GroupType : ForgeType
    {
        #region Private Construction

        /// <summary>
        /// Init GroupType with typeId of a ForgeTypeId
        /// </summary>
        /// <param name="typeId"></param>
        private GroupType(string typeId) : base(typeId)
        {
            CheckForValidForgeIdType();
        }

        /// <summary>
        /// Init GroupType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private GroupType(ForgeTypeId forgeTypeId) : base(forgeTypeId)
        {
            CheckForValidForgeIdType();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Get a GroupType by forge schema identifier.
        /// </summary>
        /// <param name="typeId">a schema identifier</param>
        /// <returns></returns>
        public static GroupType ByTypeId(string typeId)
        {
            return new GroupType(typeId);
        }

        #endregion

        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            var groupName = LabelUtils.GetLabelForGroup(InternalForgeTypeId);

            return "GroupType(Name = " + groupName + ")";
        }

        #region Internal static constructor

        /// <summary>
        /// Wrap an existing ForgeTypeId to GroupType
        /// </summary>
        /// <param name="forgeTypeId"></param>
        /// <returns></returns>
        internal static GroupType FromExisting(ForgeTypeId forgeTypeId)
        {
            return new GroupType(forgeTypeId);
        }

        #endregion

        private void CheckForValidForgeIdType()
        {
            if (!ParameterUtils.IsBuiltInGroup(InternalForgeTypeId))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.InvalidForgeTypeId, nameof(GroupType)));
            }
        }
    }
}
