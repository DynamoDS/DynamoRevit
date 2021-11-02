﻿using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit ViewDrafting
    /// </summary>
    [RegisterForTrace]
    public class DraftingView : View
    {

        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.ViewDrafting InternalViewDrafting
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewDrafting; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private DraftingView(Autodesk.Revit.DB.ViewDrafting view)
        {
            SafeInit(() => InitDraftingView(view), true);
        }
      
        /// <summary>
        /// Private constructor
        /// </summary>
        private DraftingView(string name)
        {
            SafeInit(() => InitDraftingView(name));
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Initialize a DraftingView element
        /// </summary>
        private void InitDraftingView(Autodesk.Revit.DB.ViewDrafting view)
        {
            InternalSetDraftingView(view);
        }

        /// <summary>
        /// Initialize a DraftingView element
        /// </summary>
        private void InitDraftingView(string name)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            FilteredElementCollector collector = new FilteredElementCollector(Document);
            var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().
                First(vft => vft.ViewFamily == ViewFamily.Drafting);

            var vd = ViewDrafting.Create(Document, viewFamilyType.Id);

            //rename the view
            if (!vd.Name.Equals(name))
                vd.Name = View3D.CreateUniqueViewName(name);

            InternalSetDraftingView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Private mutators


        /// <summary>
        /// Set the InternalViewDrafting property and the associated element id and unique id
        /// </summary>
        /// <param name="floor"></param>
        private void InternalSetDraftingView(Autodesk.Revit.DB.ViewDrafting floor)
        {
            this.InternalViewDrafting = floor;
            this.InternalElementId = floor.Id;
            this.InternalUniqueId = floor.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit DraftingView given it's name
        /// </summary>
        /// <param name="name">Name of the view</param>
        /// <returns>The view</returns>
        public static DraftingView ByName( string name )
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new DraftingView( name );
        }
        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a View from a user selected Element.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static DraftingView FromExisting(Autodesk.Revit.DB.ViewDrafting view, bool isRevitOwned)
        {
            return new DraftingView(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
