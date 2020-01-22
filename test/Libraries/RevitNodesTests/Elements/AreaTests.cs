using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class AreaTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Area\areaTests.rvt")]
        public void CanGetAreaBoundaries()
        {
            // Arrange
            var area = ElementSelector.ByElementId(316208, true) as Area;

            var expectedCurves = new List<List<Curve>>()
            {
                new List<Curve>()
                {
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(6223.036, 3184.616, 0.000),
                        Point.ByCoordinates(-8076.964, 3184.616, 0.000)),
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8221.964, 3039.616, 0.000),
                        Point.ByCoordinates(-8221.964, -2660.384, 0.000)),
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8076.964, -2805.384, 0.000),
                        Point.ByCoordinates(6223.036, -2805.384, 0.000)),
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(6368.036, -2660.384, 0.000),
                        Point.ByCoordinates(6368.036, 3039.616, 0.000)),
                }
            };

            // Act
            var areaCurves = area.Boundaries;

            // Assert
            AssertListOfCurves(expectedCurves, areaCurves);
        }

        private static void AssertListOfCurves(List<List<Curve>> expectedCurves, List<List<Curve>> areaCurves)
        {
            Assert.AreEqual(expectedCurves.Count(), areaCurves.Count());
            for (int i = 0; i < areaCurves.Count(); i++)
            {
                var expected = expectedCurves.FirstOrDefault()[i];
                var actual = areaCurves.FirstOrDefault().ToList()[i];

                var expectedStartPoint = expected.StartPoint;
                var expectedEndPoint = expected.EndPoint;
                var actualStartPoint = actual.StartPoint;
                var actualEndPoint = actual.EndPoint;

                Assert.AreEqual(expectedStartPoint.X, actualStartPoint.X, Tolerance);
                Assert.AreEqual(expectedStartPoint.Y, actualStartPoint.Y, Tolerance);
                Assert.AreEqual(expectedStartPoint.Z, actualStartPoint.Z, Tolerance);

                Assert.AreEqual(expectedEndPoint.X, actualEndPoint.X, Tolerance);
                Assert.AreEqual(expectedEndPoint.Y, actualEndPoint.Y, Tolerance);
                Assert.AreEqual(expectedEndPoint.Z, actualEndPoint.Z, Tolerance);

            }
        }
    }
}
