using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.GeometryConversion;

using RevitTestServices;

using RTF.Framework;

using Arc = Autodesk.DesignScript.Geometry.Arc;
using Line = Autodesk.DesignScript.Geometry.Line;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace RevitNodesTests.GeometryConversion
{
    [TestFixture]
    class CurveUtilsTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesLine()
        {
            var line = Line.ByStartPointEndPoint(
                Point.Origin(),
                Point.ByCoordinates(12, 3, 2));

            var revitCurve = line.ToRevitType(false);

            Assert.True(CurveUtils.IsLineLike(revitCurve));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesStraightNurbsCurve()
        {
            var points =
                Enumerable.Range(0, 10)
                    .Select(x => Autodesk.DesignScript.Geometry.Point.ByCoordinates(x, 0));

            var nurbsCurve = NurbsCurve.ByPoints(points, 3);
            var revitCurve = nurbsCurve.ToRevitType(false);

            Assert.True(CurveUtils.IsLineLike(revitCurve));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesNonStraightNurbsCurve()
        {
            var points = new[]
            {
                Point.ByCoordinates(5,5,0),
                Point.ByCoordinates(0,0,0),
                Point.ByCoordinates(-5,5,0),
                Point.ByCoordinates(-10,5,0)
            };

            var nurbsCurve = NurbsCurve.ByPoints(points, 3);

            Assert.False(CurveUtils.IsLineLike(nurbsCurve.ToRevitType(false)));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesStraightHermiteSpline()
        {
            var points =
                Enumerable.Range(0, 10)
                    .Select(x => new XYZ(x, 0, 0));

            var hs = HermiteSpline.Create(
                points.ToList(),
                false,
                new HermiteSplineTangents()
                {
                    StartTangent = new XYZ(1, 0, 0),
                    EndTangent = new XYZ(1, 0, 0)
                });

            Assert.True(CurveUtils.IsLineLike(hs));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesNonStraightHermiteWithStraightControlPoints()
        {
            var points =
                Enumerable.Range(0, 10)
                    .Select(x => new XYZ(x, 0, 0));

            var hs = HermiteSpline.Create(
                points.ToList(),
                false,
                new HermiteSplineTangents()
                {
                    StartTangent = new XYZ(0, 0, 1),
                    EndTangent = new XYZ(1, 0, 0)
                });

            Assert.False(CurveUtils.IsLineLike(hs));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void IsLineLike_Curve_CorrectlyIdentifiesArc()
        {
            var arc = Arc.ByThreePoints(
                Point.Origin(),
                Point.ByCoordinates(1, 1),
                Point.ByCoordinates(0, 1));

            Assert.False(CurveUtils.IsLineLike(arc.ToRevitType(false)));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CurvesAreSimilar_Lines()
        {
            var a = Point.ByCoordinates(0, 0);
            var b = Point.ByCoordinates(1, 1);
            var c = Point.ByCoordinates(2, 2);

            var line1 = Line.ByStartPointEndPoint(a, b);
            var line2 = Line.ByStartPointEndPoint(b, a);
            var line3 = Line.ByStartPointEndPoint(a, c);

            var revitLine1 = line1.ToRevitType();
            var revitLine2 = line2.ToRevitType();
            var revitLine3 = line3.ToRevitType();

            Assert.True(CurveUtils.CurvesAreSimilar(revitLine1, revitLine1));
            Assert.False(CurveUtils.CurvesAreSimilar(revitLine1, revitLine2));
            Assert.False(CurveUtils.CurvesAreSimilar(revitLine1, revitLine3));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CurvesAreSimilar_Arcs()
        {
            var a = Arc.ByCenterPointRadiusAngle(Point.ByCoordinates(0, 0, 0), 1.0, 0, 90, Vector.ZAxis());
            var b = Arc.ByCenterPointRadiusAngle(Point.ByCoordinates(1, 1, 1), 1.0, 0, 90, Vector.ZAxis());
            var c = Arc.ByCenterPointRadiusAngle(Point.ByCoordinates(0, 0, 0), 2.0, 0, 90, Vector.ZAxis());
            var d = Arc.ByCenterPointRadiusAngle(Point.ByCoordinates(0, 0, 0), 2.0, 20, 90, Vector.ZAxis());

            var revitArc1 = a.ToRevitType();
            var revitArc2 = b.ToRevitType();
            var revitArc3 = c.ToRevitType();
            var revitArc4 = d.ToRevitType();

            Assert.True(CurveUtils.CurvesAreSimilar(revitArc1, revitArc1));
            Assert.False(CurveUtils.CurvesAreSimilar(revitArc1, revitArc2));
            Assert.False(CurveUtils.CurvesAreSimilar(revitArc1, revitArc3));
            Assert.False(CurveUtils.CurvesAreSimilar(revitArc1, revitArc4));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CurvesAreSimilar_NurbsCurve()
        {
            var a1 = Point.ByCoordinates(0, 0);
            var a2 = Point.ByCoordinates(1, 1);
            var a3 = Point.ByCoordinates(2, -1);
            var a4 = Point.ByCoordinates(3, 0);

            var b1 = Point.ByCoordinates(0, 0);
            var b2 = Point.ByCoordinates(1, -1);
            var b3 = Point.ByCoordinates(2, 1);
            var b4 = Point.ByCoordinates(3, 0);

            var c1 = Point.ByCoordinates(3, 0);
            var c2 = Point.ByCoordinates(2, 1);
            var c3 = Point.ByCoordinates(1, -1);
            var c4 = Point.ByCoordinates(0, 0);

            var nurbs1 = NurbsCurve.ByPoints(new[] { a1, a2, a3, a4});
            var nurbs2 = NurbsCurve.ByPoints(new[] { b1, b2, b3, b4 });
            var nurbs3 = NurbsCurve.ByPoints(new[] { c1, c2, c3, c4 });

            var revitNurbs1 = nurbs1.ToRevitType();
            var revitNurbs2 = nurbs2.ToRevitType();
            var revitNurbs3 = nurbs3.ToRevitType();

            Assert.True(CurveUtils.CurvesAreSimilar(revitNurbs1, revitNurbs1));
            Assert.False(CurveUtils.CurvesAreSimilar(revitNurbs1, revitNurbs2));
            Assert.False(CurveUtils.CurvesAreSimilar(revitNurbs2, revitNurbs3));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CurvesAreSimilar_Ellipse()
        {
            var a = Autodesk.DesignScript.Geometry.Ellipse.ByOriginRadii(Point.ByCoordinates(0, 0, 0), 1.0, 2.0);
            var b = Autodesk.DesignScript.Geometry.Ellipse.ByOriginRadii(Point.ByCoordinates(0, 0, 0), 2.0, 1.0);

            var revitEllipse1 = a.ToRevitType();
            var revitEllipse2 = b.ToRevitType();

            Assert.True(CurveUtils.CurvesAreSimilar(revitEllipse1, revitEllipse1));
            Assert.False(CurveUtils.CurvesAreSimilar(revitEllipse1, revitEllipse2));
        }

    }
}
