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
    public class ToposolidTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ToposolidCreationTest_OutlineTypeLevel()
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

            var toposolidType = ToposolidType.ByName("Grassland - 16\"");

            var toposolid = Toposolid.ByOutlineTypeAndLevel(outline, toposolidType, level);
            var allPoints = toposolid.Points;
            Assert.AreEqual(4, allPoints.Count());
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ToposolidCreationTest_PointsTypeLevel()
        {
            var elevation = 100;
            var level = Level.ByElevation(elevation);

            var points = new[]
            {
                Point.ByCoordinates(10, 10, 2), 
                Point.ByCoordinates(90, 0, 3),
                Point.ByCoordinates(90, 90, 4),
                Point.ByCoordinates(10, 90, 5)
            };

            var toposolidType = ToposolidType.ByName("Grassland - 16\"");

            var toposolid = Toposolid.ByPointsTypeAndLevel(points, toposolidType, level);
            var allPoints = toposolid.Points;
            Assert.AreEqual(4, allPoints.Count());
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ToposolidCreationTest_OutlinePointsTypeLevel()
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

            var points = new[]
            {
                Point.ByCoordinates(10, 10, 2),
                Point.ByCoordinates(90, 0, 3),
                Point.ByCoordinates(90, 90, 4),
                Point.ByCoordinates(10, 90, 5)
            };

            var toposolidType = ToposolidType.ByName("Grassland - 16\"");

            var toposolid = Toposolid.ByOutlinePointsTypeAndLevel(outline, points, toposolidType, level);
            var allPoints = toposolid.Points;
            Assert.AreEqual(8, allPoints.Count());
        }
    }
}
