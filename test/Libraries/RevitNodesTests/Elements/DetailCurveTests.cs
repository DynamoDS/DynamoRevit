using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class DetailCurveTests : RevitNodeTestBase
    {
        /// <summary>
        /// Revit Detail Curve Test
        /// </summary>
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByCurve_ValidArgs()
        {
            var line = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(100, 0, 0));
            Assert.NotNull(line);

            var detailCurve = DetailCurve.ByCurve(Revit.Application.Document.Current.ActiveView,line);
            Assert.NotNull(detailCurve);

            var curveRef = detailCurve.ElementCurveReference;
            Assert.NotNull(curveRef);

            var curve = detailCurve.Curve;
            curve.Length.ShouldBeApproximately(100);
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByCurve_Curve_AcceptsStraightDegree3NurbsCurve()
        {
            var points =
                Enumerable.Range(0, 10)
                    .Select(x => Autodesk.DesignScript.Geometry.Point.ByCoordinates(x, 0));

            var nurbsCurve = NurbsCurve.ByPoints(points, 3);
            Assert.NotNull(nurbsCurve);

            var detailCurve = DetailCurve.ByCurve(Revit.Application.Document.Current.ActiveView, nurbsCurve);
            Assert.NotNull(detailCurve);

            detailCurve.Curve.Length.ShouldBeApproximately(9);
            detailCurve.Curve.StartPoint.ShouldBeApproximately(Point.Origin());
            detailCurve.Curve.EndPoint.ShouldBeApproximately(Point.ByCoordinates(9, 0, 0));

        }

    }
}
