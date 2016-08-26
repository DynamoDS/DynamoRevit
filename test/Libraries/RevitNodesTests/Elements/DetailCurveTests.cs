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



    }
}
