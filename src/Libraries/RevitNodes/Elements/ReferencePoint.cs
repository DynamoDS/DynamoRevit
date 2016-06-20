﻿using System;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Plane = Autodesk.DesignScript.Geometry.Plane;
using Point = Autodesk.DesignScript.Geometry.Point;
using Reference = Autodesk.Revit.DB.Reference;
using UV = Autodesk.Revit.DB.UV;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Reference Point
    /// </summary>
    [RegisterForTrace]
    [PreferredShortName("refPt")]
    public class ReferencePoint : Element, IGraphicItem
    {
        #region Internal properties

        /// <summary>
        /// Internal variable containing the wrapped Revit object
        /// </summary>
        internal Autodesk.Revit.DB.ReferencePoint InternalReferencePoint
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalReferencePoint; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Internal constructor for wrapping a ReferencePoint. 
        /// </summary>
        /// <param name="refPt"></param>
        private ReferencePoint(Autodesk.Revit.DB.ReferencePoint refPt)
        {
            SafeInit(() => InitReferencePoint(refPt));
        }

        /// <summary>
        /// Internal constructor for ReferencePoint Elements that a persistent relationship to a Face
        /// </summary>
        /// <param name="faceReference"></param>
        /// <param name="uv"></param>
        private ReferencePoint(Reference faceReference, UV uv)
        {
            SafeInit(() => InitReferencePoint(faceReference, uv));
        }

        /// <summary>
        /// Internal constructor for ReferencePoint Elements that a persistent relationship to a Curve
        /// </summary>
        /// <param name="curveReference"></param>
        /// <param name="parameter"></param>
        /// <param name="measurementType"></param>
        /// <param name="measureFrom"></param>
        private ReferencePoint(Reference curveReference, double parameter, PointOnCurveMeasurementType measurementType,
            PointOnCurveMeasureFrom measureFrom)
        {
            SafeInit(() => InitReferencePoint(curveReference, parameter, measurementType, measureFrom));
        }

        /// <summary>
        /// Internal constructor for the ReferencePoint
        /// </summary>
        /// <param name="xyz"></param>
        private ReferencePoint(XYZ xyz)
        {
            SafeInit(() => InitReferencePoint(xyz));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a ReferencePoint element
        /// </summary>
        /// <param name="refPt"></param>
        private void InitReferencePoint(Autodesk.Revit.DB.ReferencePoint refPt)
        {
            InternalSetReferencePoint(refPt);
        }

        /// <summary>
        /// Initialize a ReferencePoint element
        /// </summary>
        /// <param name="faceReference"></param>
        /// <param name="uv"></param>
        private void InitReferencePoint(Reference faceReference, UV uv)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldRefPt =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ReferencePoint>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldRefPt != null)
            {
                InternalSetReferencePoint(oldRefPt);
                InternalSetPointOnFace(faceReference, uv);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            var edgePoint = Document.Application.Create.NewPointOnFace(faceReference, uv);
            InternalSetReferencePoint(Document.FamilyCreate.NewReferencePoint(edgePoint));

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);

            // otherwise the point value is invalid for downstream requests
            DocumentManager.Regenerate();
        }

        /// <summary>
        /// Initialize a ReferencePoint element
        /// </summary>
        /// <param name="curveReference"></param>
        /// <param name="parameter"></param>
        /// <param name="measurementType"></param>
        /// <param name="measureFrom"></param>
        private void InitReferencePoint(Reference curveReference, double parameter, PointOnCurveMeasurementType measurementType,
            PointOnCurveMeasureFrom measureFrom)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldRefPt =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ReferencePoint>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldRefPt != null)
            {
                InternalSetReferencePoint(oldRefPt);
                InternalSetPointOnCurve(curveReference, parameter, measurementType, measureFrom);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            var plc = new PointLocationOnCurve(measurementType, parameter, measureFrom);
            var edgePoint = Document.Application.Create.NewPointOnEdge(curveReference, plc);
            InternalSetReferencePoint(Document.FamilyCreate.NewReferencePoint(edgePoint));

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        /// <summary>
        /// Initialize a ReferencePoint element
        /// </summary>
        /// <param name="xyz"></param>
        private void InitReferencePoint(XYZ xyz)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldRefPt =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ReferencePoint>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldRefPt != null)
            {
                InternalSetReferencePoint(oldRefPt);
                InternalSetPosition(xyz);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            InternalSetReferencePoint(Document.FamilyCreate.NewReferencePoint(xyz));

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        #endregion

        #region Private mutators

        private void InternalSetPosition(XYZ xyz)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            if(!InternalReferencePoint.Position.IsAlmostEqualTo(xyz))
                InternalReferencePoint.Position = xyz;

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPointOnFace(Reference faceReference, UV uv)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var edgePoint = Document.Application.Create.NewPointOnFace(faceReference, uv);
            InternalReferencePoint.SetPointElementReference(edgePoint);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPointOnCurve(Reference curveReference, double parameter,
            PointOnCurveMeasurementType measurementType, PointOnCurveMeasureFrom measureFrom)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            
            var plc = new PointLocationOnCurve(measurementType, parameter, measureFrom);
            var edgePoint = Document.Application.Create.NewPointOnEdge(curveReference, plc);
            InternalReferencePoint.SetPointElementReference(edgePoint);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetReferencePoint(Autodesk.Revit.DB.ReferencePoint p)
        {
            InternalReferencePoint = p;
            InternalElementId = InternalReferencePoint.Id;
            InternalUniqueId = InternalReferencePoint.UniqueId;
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets 'X' coordinate of the specified ReferencePoint
        /// </summary>
        public double X
        {
            get
            {
                return Point.X;
            }
            set { InternalSetPosition(new XYZ(value, Y, Z)); }
        }

        /// <summary>
        /// Gets 'Y' coordinate of the specified ReferencePoint
        /// </summary>
        public double Y
        {
            get
            {
                return Point.Y;
            }
            set { InternalSetPosition(new XYZ(X, value, Z)); }
        }

        /// <summary>
        /// Gets 'Z' coordinate of the specified ReferencePoint
        /// </summary>
        public double Z
        {
            get
            {
                return Point.Z;
            }
            set { InternalSetPosition(new XYZ(X, Y, value)); }
        }
        /// <summary>
        /// Gets point geometry from the specified ReferencePoint
        /// </summary>
        public Point Point
        {
            get
            {
                return InternalReferencePoint.Position.ToPoint();
            }
        }
        /// <summary>
        /// Gets XY plane of the specified ReferencePoint
        /// </summary>
        public Plane XYPlane
        {
            get
            {
                var cs = InternalReferencePoint.GetCoordinateSystem();
                var xy = new Autodesk.Revit.DB.Plane(cs.BasisX, cs.BasisY);
                return xy.ToPlane();
            }
        }
        /// <summary>
        /// Gets YZ plane of the specified ReferencePoint
        /// </summary>
        public Plane YZPlane
        {
            get
            {
                var cs = InternalReferencePoint.GetCoordinateSystem();
                var yz = new Autodesk.Revit.DB.Plane(cs.BasisY, cs.BasisZ);
                return yz.ToPlane();
            }
        }
        /// <summary>
        /// Gets XZ plane of the specified ReferencePoint
        /// </summary>
        public Plane XZPlane
        {
            get
            {
                var cs = InternalReferencePoint.GetCoordinateSystem();
                var xz = new Autodesk.Revit.DB.Plane(cs.BasisX, cs.BasisZ);
                return xz.ToPlane();
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Reference Point by x, y, and z coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static ReferencePoint ByCoordinates(double x = 0, double y = 0, double z = 0)
        {
            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }
            return ByPoint(Point.ByCoordinates(x, y, z));
        }

        /// <summary>
        /// Create a Reference Point from a point.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static ReferencePoint ByPoint(Point pt)
        {
            if (pt == null)
            {
                throw new ArgumentNullException("pt");
            }

            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }

            return new ReferencePoint(pt.ToXyz());
        }

        /// <summary>
        /// Create a Reference Point Element offset from a point along a vector
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static ReferencePoint ByPointVectorDistance(Point basePoint, Vector direction, double distance)
        {
            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }

            if (basePoint == null)
            {
                throw new ArgumentNullException("basePoint");
            }

            if (direction == null)
            {
                throw new ArgumentNullException("direction");
            }

            var pt = (Point) basePoint.Translate(direction.Scale(distance));

            return new ReferencePoint(pt.ToXyz());

        }

        /// <summary>
        /// Create a Reference Point at a particular length along a curve
        /// </summary>
        /// <param name="elementCurveReference"></param>
        /// <param name="length">Distance in meters along the curve</param>
        /// <returns></returns>
        public static ReferencePoint ByLengthOnCurveReference(object elementCurveReference, double length)
        {
            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }

            if (elementCurveReference == null)
            {
                throw new ArgumentNullException("elementCurveReference");
            }

            return new ReferencePoint(ElementCurveReference.TryGetCurveReference(elementCurveReference).InternalReference,
                UnitConverter.DynamoToHostFactor(UnitType.UT_Length) * length, PointOnCurveMeasurementType.SegmentLength, PointOnCurveMeasureFrom.Beginning);
        }

        /// <summary>
        /// Create a Reference Point at a parameter on an Curve.  This introduces a persistent relationship between
        /// Elements in the Revit document.
        /// </summary>
        /// <param name="elementCurveReference"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static ReferencePoint ByParameterOnCurveReference(object elementCurveReference, double parameter)
        {
            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }

            if (elementCurveReference == null)
            {
                throw new ArgumentNullException("elementCurveReference");
            }

            return new ReferencePoint(ElementCurveReference.TryGetCurveReference(elementCurveReference).InternalReference, parameter, PointOnCurveMeasurementType.NormalizedCurveParameter, PointOnCurveMeasureFrom.Beginning);
        }

        /// <summary>
        /// Create a Reference Point by UV coordinates on a Face. This introduces a persistent relationship between
        /// Elements in the Revit document.
        /// </summary>
        /// <param name="elementFaceReference"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ReferencePoint ByParametersOnFaceReference(object elementFaceReference, double u, double v)
        {
            if (!Document.IsFamilyDocument)
            {
                throw new Exception(Properties.Resources.ReferencePointCreationFailure);
            }

            if (elementFaceReference == null)
            {
                throw new ArgumentNullException("elementFaceReference");
            }

            return new ReferencePoint(ElementFaceReference.TryGetFaceReference(elementFaceReference).InternalReference, new UV(u, v));
        }

        #endregion

        #region Internal static constructors 

        /// <summary>
        /// Create a Reference Point from a user selected Element.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ReferencePoint FromExisting(Autodesk.Revit.DB.ReferencePoint pt, bool isRevitOwned)
        {
            return new ReferencePoint(pt)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            try
            {
                return string.Format("Reference Point: Location=(X={0}, Y={1}, Z={2})", X, Y, Z);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            try
            {
                return string.Format("Reference Point: Location=(X={0}, Y={1}, Z={2})", X.ToString(format),
                                    Y.ToString(format), Z.ToString(format));
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        #region Tesselation

        /// <summary>
        /// Tessellate Reference Point to render package for visualization.
        /// </summary>
        void IGraphicItem.Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            if (!IsAlive)
                return;

            if (this.InternalElement.IsValidObject)
            {
                package.AddPointVertex(X, Y, Z);
            }
        }

        #endregion

    }
}

