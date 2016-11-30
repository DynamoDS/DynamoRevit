using System;
using System.Linq;
using Autodesk.Revit.DB;

using DynamoServices;

using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyType, the Revit API refers to this as a FamilySymbol
    /// </summary>
    [RegisterForTrace]
    public class FamilyType: Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal wrapper property
        /// </summary>
        internal Autodesk.Revit.DB.FamilySymbol InternalFamilySymbol
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalFamilySymbol; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for building a DSFamilySymbol
        /// </summary>
        /// <param name="symbol"></param>
        private FamilyType(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            SafeInit(() => InitFamilySymbol(symbol));
        }

        #endregion

        #region Helper for private constructors

        /// <summary>
        /// Initialize a FamilySymbol element
        /// </summary>
        /// <param name="symbol"></param>
        private void InitFamilySymbol(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            InternalSetFamilySymbol(symbol);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal model of the family symbol along with its ElementId and UniqueId
        /// </summary>
        /// <param name="symbol"></param>
        private void InternalSetFamilySymbol(Autodesk.Revit.DB.FamilySymbol symbol)
        {
            this.InternalFamilySymbol = symbol;
            this.InternalElementId = symbol.Id;
            this.InternalUniqueId = symbol.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get the name of this Family Type
        /// </summary>
        public new string Name
        {
            get
            {
                return InternalFamilySymbol.Name;
            }
        }

        /// <summary>
        /// Get the parent family of this FamilyType
        /// </summary>
        public Family Family
        {
            get
            {
                return Family.FromExisting(this.InternalFamilySymbol.Family, true);
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a FamilyType given its parent Family and the FamilyType's name.
        /// </summary>
        /// <param name="family">The FamilyTypes's parent Family</param>
        /// <param name="name">The name of the FamilyType</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static FamilyType ByFamilyAndName(Family family, string name)
        {
            if (family == null)
            {
                throw new ArgumentNullException("family");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            // obtain the family symbol with the provided name
            var symbol =
                family.InternalFamily.GetFamilySymbolIds().Select(x=>Document.GetElement(x)).
                Cast<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == name);

            if (symbol == null)
            {
               throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound2, name));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Select a FamilyType give it's family name and type name.
        /// </summary>
        /// <param name="familyName">The FamilyType's parent Family name.</param>
        /// <param name="typeName">The name of the FamilyType.</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static FamilyType ByFamilyNameAndTypeName(string familyName, string typeName)
        {
            if (familyName == null)
            {
                throw new ArgumentNullException("familyName");
            }

            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            //find the family
            var collector = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            collector.OfClass(typeof (Autodesk.Revit.DB.Family));
            var family = (Autodesk.Revit.DB.Family)collector.ToElements().FirstOrDefault(x => x.Name == familyName);

            if (family == null)
            {
                throw new Exception(string.Format(Properties.Resources.FamilyNotFound, familyName));
            }

            // obtain the family symbol with the provided name
            var symbol =
                family.GetFamilySymbolIds().Select(x => DocumentManager.Instance.CurrentDBDocument.GetElement(x)).
                OfType<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == typeName);

            if (symbol == null)
            {
                throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound2, typeName));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        /// <summary>
        /// Select a FamilyType given it's name.  This method will return the first FamilyType it finds if there are
        /// two or more FamilyTypes with the same name.
        /// </summary>
        /// <param name="name">The name of the FamilyType</param>
        /// <returns></returns>
        /// <search>
        /// symbol
        /// </search>
        public static FamilyType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            // look up the loaded family
            var symbol = DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.FamilySymbol>()
                .FirstOrDefault(x => x.Name == name);

            if (symbol == null)
            {
                throw new Exception(String.Format(Properties.Resources.FamilySymbolNotFound3, name));
            }

            return new FamilyType(symbol)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Obtain a FamilyType by selection. 
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FamilyType FromExisting(Autodesk.Revit.DB.FamilySymbol familyType, bool isRevitOwned)
        {
            return new FamilyType(familyType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region ToString override

        public override string ToString()
        {
            return String.Format("Family Type: {0}, Family: {1}", InternalFamilySymbol.Name,
                InternalFamilySymbol.Family.Name);
        }

        #endregion

    }
}
