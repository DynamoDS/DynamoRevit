﻿using System;
using System.Linq;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit ModelTextType
    /// </summary>
    public class ModelTextType : ElementType
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.ModelTextType InternalModelTextType => InternalElementType as Autodesk.Revit.DB.ModelTextType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private ModelTextType(Autodesk.Revit.DB.ModelTextType type):base(type)
        {
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a ModelTextType from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new ModelTextType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.ModelTextType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(String.Format(Properties.Resources.ModelTextTypeNotFound, name));
            }

            return new ModelTextType(type)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from an existing Revit element
        /// </summary>
        /// <param name="modelTextType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ModelTextType FromExisting(Autodesk.Revit.DB.ModelTextType modelTextType, bool isRevitOwned)
        {
            return new ModelTextType(modelTextType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
