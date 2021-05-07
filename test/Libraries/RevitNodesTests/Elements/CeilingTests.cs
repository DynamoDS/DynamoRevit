using System;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class CeilingTests : RevitNodeTestBase
    {
        private double BoundingBoxVolume(BoundingBox bb)
        {
            var val = bb.MaxPoint.Subtract(bb.MinPoint.AsVector());
            return Math.Abs(val.X * val.Y);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_CurveArrayCeilingTypeLevel_ProducesCeilingWithCorrectArea()
        {
            var elevation = 100;
            var level = Level.ByElevation(elevation);

            var outline = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(100, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 0, 0), Point.ByCoordinates(100, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 100, 0), Point.ByCoordinates(0, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 100, 0), Point.ByCoordinates(0, 0, 0))
            };

            var ceilingType = CeilingType.ByName("Generic");

            var ceiling = Ceiling.ByOutlineTypeAndLevel(outline, ceilingType, level);

            BoundingBoxVolume(ceiling.BoundingBox).ShouldBeApproximately(100 * 100, 1e-3);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_PolyCurveCeilingTypeLevel_ProducesCeilingWithCorrectArea()
        {
            var elevation = 100;
            var level = Level.ByElevation(elevation);

            var outline = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(100, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 0, 0), Point.ByCoordinates(100, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 100, 0), Point.ByCoordinates(0, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 100, 0), Point.ByCoordinates(0, 0, 0))
            };

            var polyCurveOutline = PolyCurve.ByJoinedCurves(outline);

            var ceilingType = CeilingType.ByName("Generic");

            var ceiling = Ceiling.ByOutlineTypeAndLevel(polyCurveOutline, ceilingType, level);

            BoundingBoxVolume(ceiling.BoundingBox).ShouldBeApproximately(100 * 100, 1e-3);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_CurveArrayCeilingTypeLevel_ThrowsExceptionWithNullArgument()
        {
            var elevation = 100;
            var level = Level.ByElevation(elevation);

            var outline = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(100, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 0, 0), Point.ByCoordinates(100, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 100, 0), Point.ByCoordinates(0, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 100, 0), Point.ByCoordinates(0, 0, 0))
            };

            var ceilingType = CeilingType.ByName("Generic");

            Assert.Throws(typeof(ArgumentNullException), () => Ceiling.ByOutlineTypeAndLevel(outline, null, level));
            Assert.Throws(typeof(ArgumentNullException), () => Ceiling.ByOutlineTypeAndLevel(outline, ceilingType, null));
        }
    }
}
