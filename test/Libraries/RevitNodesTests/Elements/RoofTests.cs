using System;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using NUnit.Framework;
using System.Linq;
using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class RoofTests : RevitNodeTestBase
    {

        private double BoundingBoxVolume(BoundingBox bb)
        {
            var val = bb.MaxPoint.Subtract(bb.MinPoint.AsVector());
            return Math.Abs( val.X * val.Y * val.Z );
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel_CurveArrayRoofTypeLevel_ProducesRoofWithCorrectArea()
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

            var roofType = RoofType.ByName("Generic - 9\"");

            var roof = Roof.ByOutlineTypeAndLevel(outline, roofType, level);

            BoundingBoxVolume(roof.BoundingBox).ShouldBeApproximately(7500, 1e-3);
        }


        [Test]
        [TestModel(@".\Empty.rvt")]
        public void RoofSlabShapePoints_Edit()
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

            var roofType = RoofType.ByName("Generic - 9\"");

            var roof = Roof.ByOutlineTypeAndLevel(outline, roofType, level);

            Assert.NotNull(roof);

            roof.AddPoint(Point.ByCoordinates(50, 50, 0));
            Assert.IsTrue(roof.Points.ToList().Count == 5);
            
            double elev = roof.Points.First().Z;
            roof.MovePoint(roof.Points.First(), 10);

            roof.Points.First().Z.ShouldBeApproximately(10 + elev);

        }
    }
}

