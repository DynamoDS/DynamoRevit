using System;
using System.Linq;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit RoofType
    /// </summary>
    public class RoofType : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal reference to the RoofType
        /// </summary>
        internal Autodesk.Revit.DB.RoofType InternalRoofType
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRoofType; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="roofType"></param>
        private RoofType(Autodesk.Revit.DB.RoofType roofType)
        {
            SafeInit(() => InitRoofType(roofType));
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Initialize a RoofType element
        /// </summary>
        /// <param name="roofType"></param>
        private void InitRoofType(Autodesk.Revit.DB.RoofType roofType)
        {
            InternalSetRoofType(roofType);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the RoofType property, element id, and unique id
        /// </summary>
        /// <param name="RoofType"></param>
        private void InternalSetRoofType(Autodesk.Revit.DB.RoofType roofType)
        {
            this.InternalRoofType = roofType;
            this.InternalElementId = roofType.Id;
            this.InternalUniqueId = roofType.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the RoofType
        /// </summary>
        public new string Name
        {
            get { return InternalRoofType.Name; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a RoofType from the document given 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static RoofType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var RoofType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.RoofType>()
                .FirstOrDefault(x => x.Name == name);

            if (RoofType == null)
            {
                throw new Exception(Properties.Resources.RoofTypeNotFound);
            }

            // until there is a way to create a RoofType from Dynamo, 
            // this object should never be cleaned up
            return new RoofType(RoofType)
            {
                IsRevitOwned = true
            };

        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a RoofType from a user selected Element.
        /// </summary>
        /// <param name="RoofType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static RoofType FromExisting(Autodesk.Revit.DB.RoofType RoofType, bool isRevitOwned)
        {
            return new RoofType(RoofType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return InternalRoofType.Name;
        }
    }
}
