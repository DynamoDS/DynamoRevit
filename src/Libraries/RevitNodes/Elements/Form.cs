﻿using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    [DynamoServices.RegisterForTrace]
    public class Form : Element
    {

        #region Internal Properties

        internal Autodesk.Revit.DB.Form InternalForm
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalForm; }
        }

        #endregion

        #region Private constructor

        /// <summary>
        /// Construct a Revit Form from an existing form.  
        /// </summary>
        /// <param name="form"></param>
        private Form(Autodesk.Revit.DB.Form form)
        {
            SafeInit(() => InitForm(form), true);
        }

        /// <summary>
        /// Create a Form by lofting
        /// </summary>
        /// <param name="isSolid"></param>
        /// <param name="curves"></param>
        private Form(bool isSolid, ReferenceArrayArray curves)
        {
            SafeInit(() => InitForm(isSolid, curves));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a form element
        /// </summary>
        /// <param name="form"></param>
        private void InitForm(Autodesk.Revit.DB.Form form)
        {
            InternalSetForm(form);
        }

        /// <summary>
        /// Initialize a form element
        /// </summary>
        /// <param name="isSolid"></param>
        /// <param name="curves"></param>
        private void InitForm(bool isSolid, ReferenceArrayArray curves)
        {
            // clean it up
            TransactionManager.Instance.EnsureInTransaction(Document);

            var f = Document.FamilyCreate.NewLoftForm(isSolid, curves);
            InternalSetForm(f);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Private mutator

        private void InternalSetForm(Autodesk.Revit.DB.Form form)
        {
            this.InternalForm = form;
            this.InternalElementId = form.Id;
            this.InternalUniqueId = form.UniqueId;
        }

        #endregion

        #region Hidden public static constructors 

        [IsVisibleInDynamoLibrary(false)]
        public static Form ByLoftCrossSections(ElementCurveReference[][] curves, bool isSolid = true)
        {
            if (curves == null) throw new ArgumentNullException("curves");
            return ByLoftMultiPartCrossSectionsInternal(curves, isSolid);
        }

        [IsVisibleInDynamoLibrary(false)]
        public static Form ByLoftCrossSections(Revit.Elements.Element[][] curves, bool isSolid = true)
        {
            if (curves == null) throw new ArgumentNullException("curves");
            return ByLoftMultiPartCrossSectionsInternal(curves, isSolid);
        }

        #endregion

        #region Public static constructors
        /// <summary>
        ///  Creates a Form by lofting a nested list of curves
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="isSolid"></param>
        /// <returns></returns>
        public static Form ByLoftCrossSections(Autodesk.DesignScript.Geometry.Curve[][] curves, bool isSolid = true)
        {
            if (curves == null) throw new ArgumentNullException("curves");
            return ByLoftMultiPartCrossSectionsInternal(curves, isSolid);
        }

        #endregion

        #region Private static constructors

        private static Form ByLoftMultiPartCrossSectionsInternal(object[][] curves, bool isSolid = true)
        {
            if (curves == null || curves.SelectMany(x => x).Any(x => x == null))
            {
                throw new ArgumentNullException(Properties.Resources.NullInputCurvesError);
            }

            var refArrArr = new ReferenceArrayArray();

            foreach (var curveArr in curves)
            {
                var refArr = new ReferenceArray();
                curveArr.ForEach(x => refArr.Append(ElementCurveReference.TryGetCurveReference(x, "Form").InternalReference));
                refArrArr.Append(refArr);
            }

            return new Form(isSolid, refArrArr);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Construct the Revit element by selection.  
        /// </summary>
        /// <param name="formElement"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Form FromExisting(Autodesk.Revit.DB.Form formElement, bool isRevitOwned)
        {
            return new Form(formElement)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}

