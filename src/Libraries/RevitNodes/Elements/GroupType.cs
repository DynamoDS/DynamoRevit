using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    public class GroupType : ForgeType
    {
        private static Dictionary<string, string> forgeTypeIdToNameDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Internal reference to Revit ForgeTypeId
        /// </summary>
        internal static Dictionary<string, string> ForgeTypeIdToNameDictionary
        {
            get
            {
                if (forgeTypeIdToNameDictionary.Count == 0)
                {
                    var enumerationType = typeof(GroupTypeId);

                    GetForgeTypeIdNamesFromType(enumerationType, forgeTypeIdToNameDictionary);
                }

                return forgeTypeIdToNameDictionary;
            }
        }

        #region Private Construction

        /// <summary>
        /// Init SpecType with typeId of a ForgeTypeId
        /// </summary>
        /// <param name="typeId"></param>
        private GroupType(string typeId) :base(typeId) {}

        /// <summary>
        /// Init SpecType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private GroupType(ForgeTypeId forgeTypeId): base(forgeTypeId) {}

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
            if (ForgeTypeIdToNameDictionary.TryGetValue(InternalForgeTypeId.TypeId, out var name))
            {
                return name;
            }

            return InternalForgeTypeId.TypeId;
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
    }
}
