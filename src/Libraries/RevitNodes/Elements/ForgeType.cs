using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    public class ForgeType
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to Revit ForgeTypeId
        /// </summary>
        internal ForgeTypeId InternalForgeTypeId
        {
            get; private set;
        }

        #endregion

        #region Private Construction
        
        /// <summary>
        /// Init ForgeType with typeId of a ForgeTypeId
        /// </summary>
        /// <param name="typeId"></param>
        private ForgeType(string typeId)
        {
            ForgeTypeId forgeTypeId = new ForgeTypeId(typeId);
            InternalSetForgeType(forgeTypeId);
        }

        /// <summary>
        /// Init ForgeType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private ForgeType(ForgeTypeId forgeTypeId)
        {
            InternalSetForgeType(forgeTypeId);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private void InternalSetForgeType(ForgeTypeId forgeTypeId)
        {
            this.InternalForgeTypeId = forgeTypeId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The schema identifier. 
        /// </summary>
        public string TypeId
        {
            get
            {
                return InternalForgeTypeId.TypeId;
            }
        }

        /// <summary>
        /// Specifies whether the .NET object represents a valid Revit entity. 
        /// </summary>
        public bool IsValidObject
        {
            get
            {
                return InternalForgeTypeId.IsValidObject;
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Get a ForgeTypeId by schema identifier.
        /// </summary>
        /// <param name="typeId">a schema identifier</param>
        /// <returns></returns>
        public static ForgeType ByTypeId(string typeId)
        {
            return new ForgeType(typeId);
        }

        #endregion

        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            return InternalForgeTypeId.TypeId;
        }

        #region Internal static constructor

        /// <summary>
        /// Wrap an exsiting ForgeTypeId to ForgeType
        /// </summary>
        /// <param name="forgeTypeId"></param>
        /// <returns></returns>
        internal static ForgeType FromExisting(ForgeTypeId forgeTypeId)
        {
            return new ForgeType(forgeTypeId);
        }

        #endregion
    }
}
