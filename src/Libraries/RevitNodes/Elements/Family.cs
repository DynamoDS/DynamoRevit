﻿using System;
using System.Linq;

using DynamoServices;
using Revit.Application;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Family
    /// </summary>
    [RegisterForTrace]
    public class Family : Element
    {
        #region Internal properties

        internal Autodesk.Revit.DB.Family InternalFamily
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalFamily; }
        }

        #endregion

        #region Private constructors

        private Family(Autodesk.Revit.DB.Family family)
        {
            SafeInit(() => InitFamily(family));
        }

        #endregion

        #region Helper for private constructors

        private void InitFamily(Autodesk.Revit.DB.Family family)
        {
            InternalSetFamily(family);
        }

        #endregion

        #region private mutators

        /// <summary>
        /// Set the internal Revit representation and update the ElementId and UniqueId
        /// </summary>
        /// <param name="family"></param>
        private void InternalSetFamily(Autodesk.Revit.DB.Family family)
        {
            this.InternalFamily = family;
            this.InternalElementId = family.Id;
            this.InternalUniqueId = family.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Obtain the FamilyTypes from this Family
        /// </summary>
        /// <search>
        /// symbols,symbol
        /// </search>
        public FamilyType[] Types
        {
            get
            {
                return InternalFamily.GetFamilySymbolIds().Select(x => Document.GetElement(x)).
                    OfType<Autodesk.Revit.DB.FamilySymbol>().Select(x => FamilyType.FromExisting(x, true)).ToArray();
            }
        }

        /// <summary>
        /// The name of this family
        /// </summary>
        public new string Name
        {
            get
            {
                return InternalFamily.Name;
            }
        }

        #endregion

        #region Static constructors

        /// <summary>
        /// Obtain a Family from the current document given it's name
        /// </summary>
        /// <param name="name">The name of the family in the current document</param>
        /// <returns></returns>
        public static Family ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            TransactionManager.Instance.EnsureInTransaction(Document);

            // look up the loaded family
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.Family));

            // obtain the family type with the provided name
            var families = fec.Cast<Autodesk.Revit.DB.Family>();

            var family = families.FirstOrDefault(x => x.Name == name);

            if (family == null)
            {
                throw new Exception(Properties.Resources.FamilySymbolNotFound1);
            }

            TransactionManager.Instance.TransactionTaskDone();

            return Family.FromExisting(family, true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the document of a loaded family.
        /// </summary>
        /// <returns>Reference of the document of the family.</returns>
        public FamilyDocument FamilyDocument()
        {
            TransactionManager.Instance.ForceCloseTransaction();
            return new FamilyDocument(Document.EditFamily(this.InternalFamily));
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Construct an Element from an existing Element in the Document
        /// </summary>
        /// <param name="family"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Family FromExisting(Autodesk.Revit.DB.Family family, bool isRevitOwned)
        {
            return new Family(family)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region ToString override

        public override string ToString()
        {
            return String.Format("Family: {0}", InternalFamily.Name);
        }

        #endregion

    }
}
