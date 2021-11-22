using System;
using System.Collections.Generic;
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
        private protected ForgeType(string typeId)
        {
            ForgeTypeId forgeTypeId = new ForgeTypeId(typeId);
            InternalSetForgeType(forgeTypeId);
        }

        /// <summary>
        /// Init ForgeType with an existing ForgeTypeId
        /// </summary>
        /// <param name="forgeTypeId"></param>
        private protected ForgeType(ForgeTypeId forgeTypeId)
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

        [IsVisibleInDynamoLibrary(false)]
        public override string ToString()
        {
            return InternalForgeTypeId.TypeId;
        }

        protected static void GetForgeTypeIdNamesFromType(Type enumerationType, Dictionary<string,string> dictionary)
        {
            var properties = enumerationType.GetProperties();

            foreach (var property in properties)
            {
                string name = property.Name;
                ForgeTypeId forgeTypeID = property.GetValue(null, null) as ForgeTypeId;
                dictionary[forgeTypeID.TypeId] = name;
            }

            var types = enumerationType.GetNestedTypes();
            foreach (var type in types)
            {
                var nestProperties = type.GetProperties();
                foreach (var nestProperty in nestProperties)
                {
                    string name = nestProperty.Name;
                    ForgeTypeId forgeTypeID = nestProperty.GetValue(null, null) as ForgeTypeId;
                    dictionary[forgeTypeID.TypeId] = name;
                }
            }
        }
    }
}
