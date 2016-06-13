using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;
using RVT = Autodesk.Revit.DB;

namespace Revit.Elements
{
    /// <summary>
    /// Revit Detail Curve
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class DetailCurve : CurveElement
    {
        #region Private constructors

        /// <summary>
        /// Construct a detail curve from an existing element
        /// </summary>
        /// <param name="element">Revit element</param>
        private DetailCurve(Autodesk.Revit.DB.DetailCurve element)
        {
            SafeInit(() => InitElement(element));
        }

        /// <summary>
        /// Construct a detail curve by curve
        /// </summary>
        /// <param name="view">View</param>
        /// <param name="curve">Curve</param>
        private DetailCurve(Autodesk.Revit.DB.View view, Autodesk.Revit.DB.Curve curve)
        {
            SafeInit(() => Init(view, curve));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize Detail Curve element
        /// </summary>
        /// <param name="curve"></param>
        private void InitElement(Autodesk.Revit.DB.DetailCurve curve)
        {
            InternalSetCurveElement(curve);
        }

        /// <summary>
        /// Create a new detail curve from curve
        /// </summary>
        /// <param name="view"></param>
        /// <param name="curve"></param>
        private void Init(Autodesk.Revit.DB.View view, Autodesk.Revit.DB.Curve curve)
        {
            // Open Transaction and get document
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            
            // Get exsiting element
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.DetailCurve>(document);

            if (element == null)
            {
                if (document.IsFamilyDocument)
                {
                    document.FamilyCreate.NewDetailCurve(view, curve);
                }
                else
                {
                    document.Create.NewDetailCurve(view, curve);
                }                
            }
            else
            {
                element.SetGeometryCurve(curve, true);                
            }

            InternalSetCurveElement(element);

            // Commit transaction
            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a Revit DetailCurve element from a curve
        /// </summary>
        /// <param name="view">View to place the detail curve on</param>
        /// <param name="curve">Curve to create detailcurve from</param>
        /// <returns></returns>
        public static DetailCurve ByCurve(Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Curve curve)
        {
            if (!view.IsAnnotationView())
            {
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);
            }

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;

            return new DetailCurve(revitView, curve.ToRevitType(true));
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get Geometry Curve
        /// </summary>
        public Autodesk.DesignScript.Geometry.Curve GetCurve
        {
            get
            { 
                return this.InternalCurveElement.GeometryCurve.ToProtoType(true);
            }
        }

        /// <summary>
        /// Set Geometry Curve
        /// </summary>
        public void SetCurve(Autodesk.DesignScript.Geometry.Curve curve)
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            this.InternalCurveElement.SetGeometryCurve(curve.ToRevitType(true),true);
            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Private static constructors

        internal static DetailCurve FromExisting(Autodesk.Revit.DB.DetailCurve instance, bool isRevitOwned)
        {
            return new DetailCurve(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }


        #endregion

        public override string ToString()
        {
            return "DetailCurve";
        }

    }



}
