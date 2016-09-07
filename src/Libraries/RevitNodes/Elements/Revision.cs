using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;


namespace Revit.Elements
{
    /// <summary>
    /// Revit Revision
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Revision : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        [SupressImportIntoVM]
        internal Autodesk.Revit.DB.Revision InternalRevitElement
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

        #region Private constructors

        /// <summary>
        /// Create from an existing Revit Element
        /// </summary>
        private Revision(Autodesk.Revit.DB.Revision Revision)
        {
            SafeInit(() => InitElement(Revision));
        }

        /// <summary>
        /// Create Revision
        /// </summary>
        /// <param name="name"></param>
        /// <param name="visibility"></param>
        /// <param name="revDate"></param>
        /// <param name="description"></param>
        /// <param name="issued"></param>
        /// <param name="issuedBy"></param>
        /// <param name="issuedTo"></param>
        /// <param name="numberType"></param>
        private Revision(string name, RevisionVisibility visibility, string revDate, string description, bool issued, string issuedBy, string issuedTo, RevisionNumberType numberType)
        {
            SafeInit(() => Init(name, visibility, revDate, description, issued, issuedBy, issuedTo, numberType));
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        private void InternalSetElement(Autodesk.Revit.DB.Revision element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Helpers for private constructors

        private void InitElement(Autodesk.Revit.DB.Revision element)
        {
            InternalSetElement(element);
        }

        /// <summary>
        /// Create new Revision
        /// </summary>
        /// <param name="name"></param>
        /// <param name="visibility"></param>
        /// <param name="revDate"></param>
        /// <param name="description"></param>
        /// <param name="issued"></param>
        /// <param name="issuedBy"></param>
        /// <param name="issuedTo"></param>
        /// <param name="numberType"></param>
        private void Init(string name, RevisionVisibility visibility, string revDate, string description, bool issued, string issuedBy, string issuedTo, RevisionNumberType numberType)
        {
            // Get document and open transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // Get existing element if any
            var RevisionElem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Revision>(document);

            if (RevisionElem == null)
            { 
                RevisionElem = Autodesk.Revit.DB.Revision.Create(document); 
            }
            
            // Apply properties
            if (RevisionElem.Visibility != visibility)
            {
                RevisionElem.Visibility = visibility;
            }

            if (RevisionElem.Description != description)
            {
                RevisionElem.Description = description;
            }

            if (RevisionElem.RevisionDate != revDate)
            {
                RevisionElem.RevisionDate = revDate;
            }

            if (RevisionElem.Issued != issued)
            {
                RevisionElem.Issued = issued;
            }

            if (RevisionElem.IssuedBy != issuedBy)
            {
                RevisionElem.IssuedBy = issuedBy;
            }

            if (RevisionElem.IssuedTo != issuedTo)
            {
                RevisionElem.IssuedTo = issuedTo;
            }

            if (RevisionElem.NumberType != numberType)
            {
                RevisionElem.NumberType = numberType;
            }

            InternalSetElement(RevisionElem);

            // commit transaction and set element for trace
            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a new Revit Revision by Name
        /// </summary>
        /// <param name="name">Revision Name</param>
        /// <param name="visibility">Visibility settings</param>
        /// <param name="revDate">Revision Date</param>
        /// <param name="description">Description</param>
        /// <param name="issued">Issuing status</param>
        /// <param name="issuedBy">Issued by</param>
        /// <param name="issuedTo">Issued to</param>
        /// <param name="numberType">Number type</param>
        /// <returns></returns>
        public static Revision ByName(string name, string revDate, string description, bool issued, string issuedBy, string issuedTo, string visibility = "",string numberType = "")
        {
            RevisionVisibility revVisibility = RevisionVisibility.CloudAndTagVisible;
            if (!Enum.TryParse<RevisionVisibility>(visibility, out revVisibility)) revVisibility = RevisionVisibility.CloudAndTagVisible;
            
            RevisionNumberType revNumberType = RevisionNumberType.Alphanumeric;
            if (!Enum.TryParse<RevisionNumberType>(numberType, out revNumberType)) revNumberType = RevisionNumberType.Alphanumeric;

            return new Revision(name, revVisibility, revDate, description, issued, issuedBy, issuedTo, revNumberType);
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Get Revision Date
        /// </summary>
        public string RevisionDate
        {
            get { return this.InternalRevitElement.RevisionDate; }
        }

        /// <summary>
        /// Set Revision Date
        /// </summary>
        /// <param name="value">Revision Date</param>
        public void SetRevisionDate(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.RevisionDate = value;
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get IssuedTo
        /// </summary>
        public string IssuedTo
        {
            get { return this.InternalRevitElement.IssuedTo; }
        }

        /// <summary>
        /// Set IssuedTo
        /// </summary>
        /// <param name="value">IssuedTo</param>
        public void SetIssuedTo(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.IssuedTo = value;
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get IssuedBy
        /// </summary>
        public string IssuedBy
        {
            get { return this.InternalRevitElement.IssuedBy; }
        }

        /// <summary>
        /// Set IssuedBy
        /// </summary>
        /// <param name="value">IssuedBy</param>
        public void SetIssuedBy(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.IssuedBy = value;
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get Issued
        /// </summary>
        public bool Issued
        {
            get { return this.InternalRevitElement.Issued; }
        }

        /// <summary>
        /// Set Issued
        /// </summary>
        /// <param name="value">Issued</param>
        public void SetIssued(bool value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.Issued = value;
            TransactionManager.Instance.TransactionTaskDone();
        }


        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create Revision from existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Revision FromExisting(Autodesk.Revit.DB.Revision instance, bool isRevitOwned)
        {
            return new Revision(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }

}
