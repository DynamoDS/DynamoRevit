﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Curve By Points
    /// </summary>
    [RegisterForTrace]
    public class CurveByPoints : CurveElement
    {
        #region private mutators

        private void InternalSetReferencePoints(ReferencePointArray pts)
        {
            var cbp = ((Autodesk.Revit.DB.CurveByPoints)InternalCurveElement) as Autodesk.Revit.DB.CurveByPoints;
            var crvPnts = cbp.GetPoints();
            if (!CurveUtils.PointArraysAreSame(crvPnts, pts))
            {
                TransactionManager.Instance.EnsureInTransaction(Document);
                ((Autodesk.Revit.DB.CurveByPoints)InternalCurveElement).SetPoints(pts);
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        private void InternalSetIsReferenceLine(bool isReferenceLine)
        {
            var cbp = ((Autodesk.Revit.DB.CurveByPoints)InternalCurveElement) as Autodesk.Revit.DB.CurveByPoints;
            if (cbp.IsReferenceLine != isReferenceLine)
            {
                TransactionManager.Instance.EnsureInTransaction(Document);
                cbp.IsReferenceLine = isReferenceLine;
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        #endregion

        #region private constructors

        /// <summary>
        /// Construct a model curve from the document.  The result is Dynamo owned
        /// </summary>
        /// <param name="curveByPoints"></param>
        private CurveByPoints(Autodesk.Revit.DB.CurveByPoints curveByPoints)
        {
            SafeInit(() => InitCurveByPoints(curveByPoints), true);
        }

        private CurveByPoints(IEnumerable<Autodesk.Revit.DB.ReferencePoint> refPoints, bool isReferenceLine)
        {
            SafeInit(() => InitCurveByPoints(refPoints, isReferenceLine));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// </summary>
        /// <param name="curveByPoints"></param>
        private void InitCurveByPoints(Autodesk.Revit.DB.CurveByPoints curveByPoints)
        {
            InternalSetCurveElement(curveByPoints);
        }

        private void InitCurveByPoints(IEnumerable<Autodesk.Revit.DB.ReferencePoint> refPoints, bool isReferenceLine)
        {
            //Add all of the elements in the sequence to a ReferencePointArray.
            var refPtArr = new ReferencePointArray();
            foreach (var refPt in refPoints)
            {
                refPtArr.Append(refPt);
            }

            //Phase 1 - Check to see if the object exists and should be rebound
            var cbp =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.CurveByPoints>(Document);

            if (cbp != null)
            {
                InternalSetCurveElement(cbp);
                InternalSetReferencePoints(refPtArr);
                InternalSetIsReferenceLine(isReferenceLine);
                return;
            }

            TransactionManager.Instance.EnsureInTransaction(Document);

            cbp = DocumentManager.Instance.CurrentDBDocument.FamilyCreate.NewCurveByPoints(refPtArr);

            cbp.IsReferenceLine = isReferenceLine;

            InternalSetCurveElement(cbp);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region public constructors

        /// <summary>
        /// Construct a Revit CurveByPoints Element (a CurveElement) from a collection of ReferencePoint's
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isReferenceLine"></param>
        /// <returns></returns>
        public static CurveByPoints ByReferencePoints(ReferencePoint[] points, bool isReferenceLine = false)
        {
            if (points.Count() < 2)
            {
                throw new Exception(Properties.Resources.CurveNeedsTwoPoints);
            }

            return new CurveByPoints(points.Select(x=>x.InternalReferencePoint), isReferenceLine);
        }


        #endregion

        #region Internal constructors

        /// <summary>
        /// Construct a Revit ModelCurve element from an existing element.
        /// </summary>
        /// <param name="curveByPoints"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static CurveByPoints FromExisting(Autodesk.Revit.DB.CurveByPoints curveByPoints, bool isRevitOwned)
        {
            return new CurveByPoints(curveByPoints)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return "CurveByPoints";
        }
    }
}
