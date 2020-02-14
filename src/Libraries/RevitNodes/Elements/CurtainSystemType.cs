using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit CurtainSystemType
    /// </summary>
    public class CurtainSystemType : ElementType
    {
        #region Internal properties

        /// <summary>
        /// An internal reference to the CurtainSystemType
        /// </summary>
        internal Autodesk.Revit.DB.CurtainSystemType InternalCurtainSystemType => InternalElementType as Autodesk.Revit.DB.CurtainSystemType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for the Element
        /// </summary>
        /// <param name="curtainSystemType"></param>
        private CurtainSystemType(Autodesk.Revit.DB.CurtainSystemType curtainSystemType):base(curtainSystemType)
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The name of the CurtainSystemType
        /// </summary>
        public new string Name
        {
            get { return InternalCurtainSystemType.Name; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a CurtainSystemType from the document given 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new CurtainSystemType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var curtainSystemType = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.CurtainSystemType>()
                .FirstOrDefault(x => x.Name == name);

            if (curtainSystemType == null)
            {
                throw new Exception(Properties.Resources.CurtainSystemTypeNotFound);
            }

            // until there is a way to create a CurtainSystemType from Dynamo, 
            // this object should never be cleaned up
            return new CurtainSystemType(curtainSystemType)
            {
                IsRevitOwned = true
            };

        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a CurtainSystemType from a user selected Element.
        /// </summary>
        /// <param name="curtainSystemType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static CurtainSystemType FromExisting(Autodesk.Revit.DB.CurtainSystemType curtainSystemType, bool isRevitOwned)
        {
            return new CurtainSystemType(curtainSystemType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return InternalCurtainSystemType.Name;
        }
    }
}
