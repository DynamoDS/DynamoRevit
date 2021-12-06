using System;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    public class GroupType : ForgeType
    {
        #region Private Construction

        /// <summary>
        /// Init SpecType with typeId of a ForgeTypeId
        /// </summary>
        /// <param name="typeId"></param>
        private GroupType(string typeId) : base(typeId)
        {
            CheckForValidForgeIdType();
        }

        /// <summary>
        /// Init SpecType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private GroupType(ForgeTypeId forgeTypeId) : base(forgeTypeId)
        {
            CheckForValidForgeIdType();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Get a ForgeTypeId by schema identifier.
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
        /// Wrap an exsiting ForgeTypeId to ForgeType
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
                throw new Exception("This id string is not valid for a " + nameof(SpecType));
            }
        }
    }
}
