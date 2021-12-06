using System;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    public class SpecType : ForgeType
    {
        #region Private Construction

        /// <summary>
        /// Init SpecType with typeId of a ForgeTypeId
        /// </summary>
        /// <param name="typeId"></param>
        private SpecType(string typeId) : base(typeId)
        {
            CheckForValidForgeIdType();
        }

        /// <summary>
        /// Init SpecType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private SpecType(ForgeTypeId forgeTypeId) : base(forgeTypeId)
        {
            CheckForValidForgeIdType();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Get a SpecType by Forge schema identifier.
        /// </summary>
        /// <param name="typeId">a schema identifier</param>
        /// <returns></returns>
        public static SpecType ByTypeId(string typeId)
        {
            return new SpecType(typeId);
        }

        #endregion

        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            var specName = LabelUtils.GetLabelForSpec(InternalForgeTypeId);

            return "SpecType(Name = " + specName + ")";
        }

        #region Internal static constructor

        /// <summary>
        /// Wrap an existing ForgeTypeId to SpecType
        /// </summary>
        /// <param name="forgeTypeId"></param>
        /// <returns></returns>
        internal static SpecType FromExisting(ForgeTypeId forgeTypeId)
        {
            return new SpecType(forgeTypeId);
        }

        #endregion

        private void CheckForValidForgeIdType()
        {
            if (!SpecUtils.IsSpec(InternalForgeTypeId))
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.InvalidForgeTypeId, nameof(SpecType)));
            }
        }
    }
}
