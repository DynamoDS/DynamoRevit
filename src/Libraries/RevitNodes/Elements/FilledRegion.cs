﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// Revit filled Region
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FilledRegion : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FilledRegion InternalRevitElement
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
        private void InternalSetElement(Autodesk.Revit.DB.FilledRegion element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FilledRegion"></param>
        private FilledRegion(Autodesk.Revit.DB.FilledRegion FilledRegion)
        {
            SafeInit(() => InitElement(FilledRegion), true);
        }

        /// <summary>
        /// Create Filled Region by Curves
        /// </summary>
        /// <param name="view"></param>
        /// <param name="typeId"></param>
        /// <param name="boundary"></param>
        private FilledRegion(View view, ElementId typeId, List<CurveLoop> boundary)
        {
            SafeInit(() => Init(view, typeId, boundary));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init element
        /// </summary>
        /// <param name="element"></param>
        private void InitElement(Autodesk.Revit.DB.FilledRegion element)
        {
            InternalSetElement(element);
        }

        /// <summary>
        /// Init Filled Region by Curves
        /// </summary>
        /// <param name="view"></param>
        /// <param name="typeId"></param>
        /// <param name="boundary"></param>
        private void Init(View view, ElementId typeId, IEnumerable<CurveLoop> boundary)
        {
            // Get document and start transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // Get existing element
            var region = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FilledRegion>(document);

            if (null != region)
                DocumentManager.Instance.DeleteElement(new ElementUUID(region.UniqueId));

            region = Autodesk.Revit.DB.FilledRegion.Create(document, typeId, view.Id, boundary.ToList()); 

            InternalSetElement(region);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a Revit FilledRegion element by Curves
        /// </summary>
        /// <param name="view">View to place filled region on</param>
        /// <param name="boundary">Boundary curves</param>
        /// <param name="regionType">Region Type</param>
        /// <returns></returns>
        public static FilledRegion ByCurves(Revit.Elements.Views.View view, IEnumerable<Autodesk.DesignScript.Geometry.Curve> boundary, FilledRegionType regionType)
        {
            Autodesk.Revit.DB.FilledRegionType type = regionType.InternalRevitElement;

            CurveLoop loop = new CurveLoop();
            foreach (Autodesk.DesignScript.Geometry.Curve curve in boundary)
            {
                loop.Append(curve.ToRevitType());
            }

            if (loop.IsOpen())
            {
                throw new Exception(Properties.Resources.CurveLoopNotClosed);
            }

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;

            if (!view.IsAnnotationView())
            {
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);
            }

            return new FilledRegion(revitView, type.Id, new List<CurveLoop>() { loop });
        }

        #endregion

        #region Internal static constructors

        internal static FilledRegion FromExisting(Autodesk.Revit.DB.FilledRegion instance, bool isRevitOwned)
        {
            return new FilledRegion(instance) { IsRevitOwned = isRevitOwned };
        }

        #endregion

    }

}
