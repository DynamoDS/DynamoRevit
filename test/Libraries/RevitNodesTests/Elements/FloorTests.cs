using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FloorTests : RevitNodeTestBase
    {

        private double BoundingBoxVolume(BoundingBox bb)
        {
            var val = bb.MaxPoint.Subtract(bb.MinPoint.AsVector());
            return Math.Abs( val.X * val.Y * val.Z );
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_CurveArrayFloorTypeLevel_ProducesFloorWithCorrectArea()
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

            var floorType = FloorType.ByName("Generic - 12\"");

            var floor = Floor.ByOutlineTypeAndLevel(outline, floorType, level);

            BoundingBoxVolume(floor.BoundingBox).ShouldBeApproximately(100 * 100, 1e-3);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_PolyCurveFloorTypeLevel_ProducesFloorWithCorrectArea()
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

            var polyCurveOutline = PolyCurve.ByJoinedCurves(outline, 0.001, false);

            var floorType = FloorType.ByName("Generic - 12\"");

            var floor = Floor.ByOutlineTypeAndLevel(polyCurveOutline, floorType, level);

            BoundingBoxVolume(floor.BoundingBox).ShouldBeApproximately(100 * 100, 1e-3);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_CurveArrayFloorTypeLevel_ThrowsExceptionWithNullArgument()
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

            var floorType = FloorType.ByName("Generic - 12\"");

            Assert.Throws(typeof(ArgumentNullException), () => Floor.ByOutlineTypeAndLevel(outline, null, level));
            Assert.Throws(typeof(ArgumentNullException), () => Floor.ByOutlineTypeAndLevel(outline, floorType, null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void CreateFoundationSlab_ByOutlineTypeAndLevel_CurveArrayFloorTypeLevel()
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

            var floorType = FloorType.ByName("6\" Foundation Slab");

            var floor = Floor.ByOutlineTypeAndLevel(outline, floorType, level);

            Assert.NotNull(floor);
            Assert.IsTrue(floor.InternalFloor.FloorType.IsFoundationSlab);
            Assert.AreEqual(floor.InternalFloor.FloorType.Name, "6\" Foundation Slab");
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void FloorSlabShapePoints_Edit()
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

            var floorType = FloorType.ByName("Generic - 12\"");

            var floor = Floor.ByOutlineTypeAndLevel(outline, floorType, level);

            Assert.NotNull(floor);
            

            floor.AddPoint(Point.ByCoordinates(50, 50, 0));
            Assert.IsTrue(floor.Points.ToList().Count == 5);

            double elev = floor.Points.First().Z;
            floor.MovePoint(floor.Points.First(), 10);

            floor.Points.First().Z.ShouldBeApproximately(elev + 10);

        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_PolyCurveFloorTypeLevel_ProducesFloorWithCorrectOffset()
        {
            var elevation = 100;
            var level = Level.ByElevation(elevation);

            var outline = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 50), Point.ByCoordinates(100, 0, 50)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 0, 50), Point.ByCoordinates(100, 100, 50)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 100, 50), Point.ByCoordinates(0, 100, 50)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 100, 50), Point.ByCoordinates(0, 0, 50))
            };

            var polyCurveOutline = PolyCurve.ByJoinedCurves(outline, 0.001, false);

            var floorType = FloorType.ByName("Generic - 12\"");

            var floor = Floor.ByOutlineTypeAndLevel(polyCurveOutline, floorType, level);

            var param = floor.GetParameterValueByName("Height Offset From Level");
            ((double)param).ShouldBeApproximately(-50);
        }
    }
}

