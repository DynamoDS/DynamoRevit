﻿using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

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
        /// Init a new detail curve from curve
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
                    element = document.FamilyCreate.NewDetailCurve(view, curve);
                }
                else
                {
                    element = document.Create.NewDetailCurve(view, curve);
                }                
            }
            else
            {
                element.SetGeometryCurve(curve, true);                
            }

            InternalSetCurveElement(element);

            // Set transaction task done & element for trace
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

            if (!curve.IsPlanar)
            {
                throw new Exception(Properties.Resources.CurveIsNotPlanar);
            }

            if (curve is Autodesk.DesignScript.Geometry.PolyCurve)
            {
                throw new Exception(Properties.Resources.PolyCurvesConversionError);
            }

            // Pull Curve onto the XY plane to place it correctly on the view.
            Autodesk.DesignScript.Geometry.Plane XYplane = Autodesk.DesignScript.Geometry.Plane.XY();
            Autodesk.DesignScript.Geometry.Curve flattenedCurve = curve.PullOntoPlane(XYplane);

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;

            return new DetailCurve(revitView, flattenedCurve.ToRevitType());
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get Geometry Curve
        /// </summary>
        public Autodesk.DesignScript.Geometry.Curve Curve
        {
            get
            { 
                return this.InternalCurveElement.GeometryCurve.ToProtoType();
            }
        }

        /// <summary>
        /// Set Geometry Curve
        /// </summary>
        public void SetCurve(Autodesk.DesignScript.Geometry.Curve curve)
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            this.InternalCurveElement.SetGeometryCurve(curve.ToRevitType(),true);
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
