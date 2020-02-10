using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// Revit filled Region Type
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FilledRegionType : ElementType
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FilledRegionType InternalRevitElement => InternalElementType as Autodesk.Revit.DB.FilledRegionType;

        #endregion

        #region Private constructors

        /// <summary>
        /// FilledRegionType
        /// </summary>
        /// <param name="FilledRegion"></param>
        private FilledRegionType(Autodesk.Revit.DB.FilledRegionType FilledRegion):base(FilledRegion)
        {
        }


        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a FilledRegionType from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new FilledRegionType ByName(string name)
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
        public new string Name
        {
            get { return this.InternalRevitElement.Name; }
        }

        /// <summary>
        /// Get Color
        /// </summary>
        public DSCore.Color Color
        {
            get { return DSCore.Color.ByARGB(255, this.InternalRevitElement.ForegroundPatternColor.Red,this.InternalRevitElement.ForegroundPatternColor.Green, this.InternalRevitElement.ForegroundPatternColor.Blue); }
        }

        /// <summary>
        /// Get FillPatternId
        /// </summary>
        public ElementId FillPatternId
        {
            get { return this.InternalRevitElement.ForegroundPatternId; }
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
