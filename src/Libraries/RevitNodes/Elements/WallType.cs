﻿using System;
using System.Linq;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit WallType
    /// </summary>
    public class WallType : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.WallType InternalWallType
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalWallType; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Construct from an existing Revit Element
        /// </summary>
        /// <param name="type"></param>
        private WallType(Autodesk.Revit.DB.WallType type)
        {
            SafeInit(() => InitWallType(type));
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a WallType element
        /// </summary>
        /// <param name="type"></param>
        private void InitWallType(Autodesk.Revit.DB.WallType type)
        {
            InternalSetWallType(type);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="wallType"></param>
        private void InternalSetWallType(Autodesk.Revit.DB.WallType wallType)
        {
            this.InternalWallType = wallType;
            this.InternalElementId = wallType.Id;
            this.InternalUniqueId = wallType.UniqueId;
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets the name of the specified wall type
        /// </summary>
        public new string Name 
        {
            get
            {
                return InternalWallType.Name;
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a walltype from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static WallType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.WallType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(Properties.Resources.WallTypeNotFound);
            }

            return new WallType(type)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from an existign Revit element
        /// </summary>
        /// <param name="wallType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static WallType FromExisting(Autodesk.Revit.DB.WallType wallType, bool isRevitOwned)
        {
            return new WallType(wallType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return InternalWallType.Name;
        }
    }
}
