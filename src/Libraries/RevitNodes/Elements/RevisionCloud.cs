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
    /// Revit Revision Cloud
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class RevisionCloud : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        [SupressImportIntoVM]
        internal Autodesk.Revit.DB.RevisionCloud InternalRevitElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRevitElement; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Create revision cloud from revit element
        /// </summary>
        /// <param name="RevisionCloud"></param>
        private RevisionCloud(Autodesk.Revit.DB.RevisionCloud RevisionCloud)
        {
            SafeInit(() => InitElement(RevisionCloud));
        }

        /// <summary>
        /// Create Revision cloud from view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="curves"></param>
        /// <param name="id"></param>
        private RevisionCloud(Autodesk.Revit.DB.View view, List<Autodesk.Revit.DB.Curve> curves, Autodesk.Revit.DB.ElementId id)
        {
            SafeInit(() => Init(view, curves, id));
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="wall"></param>
        private void InternalSetElement(Autodesk.Revit.DB.RevisionCloud element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Helpers for private constructors

        private void InitElement(Autodesk.Revit.DB.RevisionCloud element)
        {
            InternalSetElement(element);
        }

        /// <summary>
        /// Create new RevisionCloud from Curves
        /// </summary>
        /// <param name="view"></param>
        /// <param name="curves"></param>
        /// <param name="id"></param>
        private void Init(Autodesk.Revit.DB.View view,List<Autodesk.Revit.DB.Curve> curves,Autodesk.Revit.DB.ElementId id)
        {
            // get document and open transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // get existing element if possible
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.RevisionCloud>(document);


            if (element == null)
            {
                element = Autodesk.Revit.DB.RevisionCloud.Create(document, view, id, curves);
            }
            else
            {
                // there is no way of updateing the properties.
                // So the cloud is being recreated.
                document.Delete(element.Id);
                element = Autodesk.Revit.DB.RevisionCloud.Create(document, view, id, curves);
            }
            

            InternalSetElement(element);

            // commit transaction and set element for trace
            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);

        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a Revit RevisionCloud element by curves
        /// </summary>
        /// <param name="view">View to place element on</param>
        /// <param name="curves">Cloud outline</param>
        /// <param name="revision">Revit revision</param>
        /// <returns></returns>
        public static RevisionCloud ByCurve(Revit.Elements.Views.View view,  List<Autodesk.DesignScript.Geometry.Curve> curves, Revit.Elements.Element revision)
        {
            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;

            if (!view.IsAnnotationView())
            {
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);
            }

            List<Curve> DSCurves = new List<Curve>();
            foreach (Autodesk.DesignScript.Geometry.Curve curve in curves)
            {
                DSCurves.Add(curve.ToRevitType(true));
            }

            return new RevisionCloud(revitView, DSCurves, revision.InternalElement.Id);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get Revision cloud's revision
        /// </summary>
        public Revision Revision
        {
            get {
                Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
                Autodesk.Revit.DB.Revision revision = (Autodesk.Revit.DB.Revision)document.GetElement(this.InternalRevitElement.RevisionId);
                return Revision.FromExisting(revision, true);
            }
        }



        /// <summary>
        /// Get Revision cloud's curves
        /// </summary>
        public List<Autodesk.DesignScript.Geometry.Curve> Curves
        { 
            get
            {
                List<Autodesk.DesignScript.Geometry.Curve> curves = new List<Autodesk.DesignScript.Geometry.Curve>();
                foreach (Curve curve in this.InternalRevitElement.GetSketchCurves())
                    curves.Add(curve.ToProtoType(true));
                return curves;
            }
        }


        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Rebar from an existing reference
        /// </summary>
        /// <param name="rebar"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static RevisionCloud FromExisting(Autodesk.Revit.DB.RevisionCloud RevisionCloud, bool isRevitOwned)
        {
            return new RevisionCloud(RevisionCloud)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion


    }

}
