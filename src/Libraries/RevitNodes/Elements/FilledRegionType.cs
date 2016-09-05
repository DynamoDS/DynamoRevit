using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace Revit.Elements
{
    /// <summary>
    /// Revit filled Region Type
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FilledRegionType : Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FilledRegionType InternalRevitElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRevitElement; }
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        private void InternalSetElement(Autodesk.Revit.DB.FilledRegionType element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// FilledRegionType
        /// </summary>
        /// <param name="FilledRegion"></param>
        private FilledRegionType(Autodesk.Revit.DB.FilledRegionType FilledRegion)
        {
            SafeInit(() => InitElement(FilledRegion));
        }


        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a FilledRegionType from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FilledRegionType ByName(string name)
        {
            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.FilledRegionType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(String.Format(Properties.Resources.TypeNotFound, name));
            }

            return new FilledRegionType(type)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get Name
        /// </summary>
        public string Name
        {
            get { return this.InternalRevitElement.Name; }
        }

        /// <summary>
        /// Get Color
        /// </summary>
        public DSCore.Color Color
        {
            get { return DSCore.Color.ByARGB(255, this.InternalRevitElement.Color.Red,this.InternalRevitElement.Color.Green, this.InternalRevitElement.Color.Blue); }
        }

        /// <summary>
        /// Get FillPatternId
        /// </summary>
        public ElementId FillPatternId
        {
            get { return this.InternalRevitElement.FillPatternId; }
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init Element
        /// </summary>
        private void InitElement(Autodesk.Revit.DB.FilledRegionType element)
        {
            InternalSetElement(element);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from existing
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FilledRegionType FromExisting(Autodesk.Revit.DB.FilledRegionType instance, bool isRevitOwned)
        {
            return new FilledRegionType(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion


    }

}
