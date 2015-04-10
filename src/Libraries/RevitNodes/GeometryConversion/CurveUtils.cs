using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using DynamoUnits;

namespace Revit.GeometryConversion
{
    [SupressImportIntoVM]
    public static class CurveUtils
    {
        public static readonly double Tolerance = 1e-6;

        public static bool ReferencePointsAreSame(ReferencePoint pnt1, ReferencePoint pnt2)
        {
            if (pnt1.Position.IsAlmostEqualTo(pnt2.Position, Tolerance))
                return true;

            return false;
        }

        public static bool PointArraysAreSame(ReferencePointArray pnts1, ReferencePointArray pnts2)
        {
            int size1 = pnts1.Size;
            int size2 = pnts2.Size;
            if (size1 != size2)
                return false;

            for (int i = 0; i < size1; i++)
            {
                var pnt1 = pnts1.get_Item(i);
                var pnt2 = pnts2.get_Item(i);
                if (!ReferencePointsAreSame(pnt1, pnt2))
                    return false;
            }

            return true;
        }

        public static Plane GetPlaneFromCurve(Curve c, bool planarOnly)
        {
            //find the plane of the curve and generate a sketch plane
            double period = c.IsBound ? 0.0 : (c.IsCyclic ? c.Period : 1.0);

            var p0 = c.IsBound ? c.Evaluate(0.0, true) : c.Evaluate(0.0, false);
            var p1 = c.IsBound ? c.Evaluate(0.5, true) : c.Evaluate(0.25 * period, false);
            var p2 = c.IsBound ? c.Evaluate(1.0, true) : c.Evaluate(0.5 * period, false);

            if (IsLineLike(c))
            {
                XYZ norm = null;

                //keep old plane computations
                if (Math.Abs(p0.Z - p2.Z) < Tolerance)
                {
                    norm = XYZ.BasisZ;
                }
                else
                {
                    var v1 = p1 - p0;
                    var v2 = p2 - p0;

                    var p3 = new XYZ(p2.X, p2.Y, p0.Z);
                    var v3 = p3 - p0;
                    norm = v1.CrossProduct(v3);
                    if (norm.IsZeroLength())
                    {
                        norm = v2.CrossProduct(XYZ.BasisY);
                    }
                    norm = norm.Normalize();
                }

                return new Plane(norm, p0);

            }

            var cLoop = new CurveLoop();
            cLoop.Append(c.Clone());
            if (cLoop.HasPlane())
            {
                return cLoop.GetPlane();
            }
            if (planarOnly)
                return null;

            // Get best fit plane using tesselation
            var points = c.Tessellate().Select(x => x.ToPoint(false));

            var bestFitPlane =
                Autodesk.DesignScript.Geometry.Plane.ByBestFitThroughPoints(points);

            return bestFitPlane.ToPlane(false);
        }

        public static bool IsLineLike(Curve crv)
        {
            if (crv is Line) return true;
            if (crv is HermiteSpline) return IsLineLikeInternal(crv as HermiteSpline);
            if (crv is NurbSpline) return IsLineLikeInternal(crv as NurbSpline);
            
            // This assumes no infinite radius arcs/ellipses
            return false;
        }

        /// <summary>
        /// This method uses basic checks to compare curves for similarity.
        /// It starts by comparing the curves' end points. Curves which have similar
        /// end points but different directions will not be regarded as similar,
        /// because directionality is important in Revit for other purposes. 
        /// Depending on the curve type, other comparisons are then performed. 
        /// </summary>
        /// <param name="a">The first curve.</param>
        /// <param name="b">The second curve.</param>
        /// <returns>Returns true if the curves are similar within Tolerance, and 
        /// false if they are not.</returns>
        public static bool CurvesAreSimilar(Curve a, Curve b)
        {
            if (a.GetType() != b.GetType())
                return false;

            if (!CurvesHaveSameEndpoints(a, b, Tolerance))
                return false;

            dynamic ad = a;
            dynamic bd = b;

            return AreSimilar(ad, bd, Tolerance);
        }

        #region IsLineLike Helpers

        private static bool IsLineLikeInternal(NurbSpline crv)
        {
            return IsLineLikeInternal(crv.CtrlPoints);
        }

        private static bool IsLineLikeInternal(HermiteSpline crv)
        {
            var controlPoints = crv.ControlPoints;
            var controlPointsAligned = IsLineLikeInternal(controlPoints);

            if (!controlPointsAligned) return false;

            // It's possible for the control points to be straight, but the tangents
            // might not be aligned with the curve.  Let's check the tangents are aligned.
            var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                controlPoints.First().ToPoint(false),
                controlPoints.Last().ToPoint(false));

            var lineDir = line.Direction.Normalized().ToXyz(false);
            line.Dispose();

            var tangents = crv.Tangents;
            var startTan = tangents.First().Normalize();
            var endTan = tangents.Last().Normalize();

            return Math.Abs(startTan.DotProduct(endTan) - 1) < Tolerance &&
                Math.Abs(lineDir.DotProduct(endTan) - 1) < Tolerance;
        }

        private static bool IsLineLikeInternal(IList<XYZ> points)
        {
            if (points.Count == 2) return true;

            using (var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                points.First().ToPoint(false),
                points.Last().ToPoint(false)))
            {
                // Are any of the points distant from the line created by connecting the two
                // end points?
                return !points.Any(x => x.ToPoint(false).DistanceTo(line) > Tolerance);
            }
        }

        // This test checks end points with directionality. Curves that share end points
        /// but are reversed are not considered similar.
        private static bool CurvesHaveSameEndpoints(Curve a, Curve b, double tolerance)
        {
            return a.GetEndPoint(0).IsAlmostEqualTo(b.GetEndPoint(0), tolerance) &&
                a.GetEndPoint(1).IsAlmostEqualTo(b.GetEndPoint(1));
        }

        private static bool AreSimilar(Line a, Line b, double tolerance)
        {
            return CurvesHaveSameEndpoints(a,b, tolerance);
        }

        private static bool AreSimilar(Arc a, Arc b, double tolerance)
        {
            return a.Center.IsAlmostEqualTo(b.Center) && 
                a.Radius.AlmostEquals(b.Radius, Tolerance) && 
                a.IsCyclic == b.IsCyclic &&
                a.Normal.IsAlmostEqualTo(b.Normal);
        }

        private static bool AreSimilar(HermiteSpline a, HermiteSpline b, double tolerance)
        {
            return CompareRandomParameterLocationDistances(a, b, tolerance);
        }

        private static bool AreSimilar(NurbSpline a, NurbSpline b, double tolerance)
        {
            return CompareRandomParameterLocationDistances(a, b, tolerance);
        }

        private static bool AreSimilar(Ellipse a, Ellipse b, double tolerance)
        {
            return a.Center.IsAlmostEqualTo(b.Center, tolerance) &&
                a.RadiusX.AlmostEquals(b.RadiusX, tolerance) &&
                a.RadiusY.AlmostEquals(b.RadiusY, tolerance) && 
                a.Normal.IsAlmostEqualTo(b.Normal);
        }

        private static bool AreSimilar(CylindricalHelix a, CylindricalHelix b, double tolerance)
        {
            return a.Height.AlmostEquals(b.Height, tolerance) &&
                a.BasePoint.IsAlmostEqualTo(b.BasePoint, tolerance) &&
                a.Pitch.AlmostEquals(b.Pitch, tolerance) &&
                a.Radius.AlmostEquals(b.Radius, tolerance) &&
                a.XVector.IsAlmostEqualTo(b.XVector) &&
                a.YVector.IsAlmostEqualTo(b.YVector) &&
                a.ZVector.IsAlmostEqualTo(b.ZVector);
        }

        private static bool CompareRandomParameterLocationDistances(
            Curve a, Curve b, double tolerance)
        {
            for (var i = 0; i < 20; i++)
            {
                var rand = new Random();
                var t = rand.NextDouble();

                var ptA = a.Evaluate(t, true);
                var ptB = b.Evaluate(t, true);

                if (ptA.DistanceTo(ptB) > tolerance)
                    return false;
            }

            return true;
        }

        #endregion

    }
}
