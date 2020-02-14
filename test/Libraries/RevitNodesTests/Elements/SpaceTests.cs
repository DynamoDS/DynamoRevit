using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class SpaceTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanCreateSpaceFromLevelLocationNameNumber()
        {
            // Arrange
            var level = ElementSelector.ByElementId(311, true) as Level;
            var spaceLocation = Point.ByCoordinates(-5266.333, -2266.975, 0.000);
            string name = "TestSpace";
            string number = "99";

            // Act
            var newSpace = Space.ByLevelLocation(level, spaceLocation, name, number);
            var internalElement = newSpace.InternalRevitElement;

            var newLocation = internalElement.Location as Autodesk.Revit.DB.LocationPoint;

            // Assert
            Assert.NotNull(newSpace);
            Assert.AreEqual(spaceLocation, newLocation.Point.ToPoint());
            Assert.AreEqual(name, newSpace.Name);
            Assert.AreEqual(number, newSpace.Number);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanCheckIfPointIsInsideSpace()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            var point1 = Point.ByCoordinates(-6166.333, 6435.025, 0.000);
            var point2 = Point.ByCoordinates(-6166.333, 383.025, 0.000);

            // Assert
            Assert.AreEqual(false, space.IsPointInsideSpace(point1));
            Assert.AreEqual(true, space.IsPointInsideSpace(point2));
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanSetSpaceName()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;

            string newName = "newSpaceName";

            // Act
            string oldSpaceName = space.Name;
            var renamedSpace = space.SetName(newName);

            // Assert
            Assert.AreNotEqual(oldSpaceName, newName);
            Assert.AreEqual(newName, renamedSpace.Name);
            Assert.AreEqual(space.Id, renamedSpace.Id);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanSetSpaceNumber()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;

            string newNumber = "99";

            // Act
            string oldSpaceNumber = space.Number;
            var renumberdSpace = space.SetNumber(newNumber);

            // Assert
            Assert.AreNotEqual(oldSpaceNumber, newNumber);
            Assert.AreEqual(newNumber, renumberdSpace.Number);
            Assert.AreEqual(space.Id, renumberdSpace.Id);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceArea()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            double expectedArea = 31.302;

            // Act
            double spaceArea = space.Area;

            // Assert
            Assert.AreEqual(expectedArea, spaceArea, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceHeight()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            double expectedHeight = 4000;

            // Act
            double spaceHeight = space.Height;

            // Assert
            Assert.AreEqual(expectedHeight, spaceHeight, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceLocation()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            var expectedLocation = Point.ByCoordinates(-4602.593, 1723.307, 0.000)
;

            // Act
            var spaceLocation = space.Location;

            // Assert
            Assert.AreEqual(expectedLocation.X, spaceLocation.X, Tolerance);
            Assert.AreEqual(expectedLocation.Y, spaceLocation.Y, Tolerance);
            Assert.AreEqual(expectedLocation.Z, spaceLocation.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceName()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            string expectedName = "Space";

            // Act
            string spaceName = space.Name;

            // Assert
            Assert.AreEqual(expectedName, spaceName);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceNumber()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            string expectedNumber = "1";

            // Act
            string spaceNumber = space.Number;

            // Assert
            Assert.AreEqual(expectedNumber, spaceNumber);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceVolume()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            double expectedVolume = 125.2104;

            // Act
            double spaceVolume = space.Volume;

            // Assert
            Assert.AreEqual(expectedVolume, spaceVolume, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCenterBoundary()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            List<List<Curve>> expectedBoundaries = new List<List<Curve>>()
            {
                new List<Curve>()
                {
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-596.333, 3990.025, 0.000),
                        Point.ByCoordinates(-8596.333, 3990.025, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8596.333, 3990.025, 0.000),
                        Point.ByCoordinates(-8596.333, -359.975, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8596.333, -359.975, 0.000),
                        Point.ByCoordinates(-596.333, -359.975, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-596.333, -359.975, 0.000),
                        Point.ByCoordinates(-596.333, 3990.025, 0.000)) as Curve
                }
            };

            // Act
            var boundaryLines = space.CenterBoundary;

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCoreBoundary()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            List<List<Curve>> expectedBoundaries = new List<List<Curve>>()
            {
                new List<Curve>()
                {
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-728.833, 3857.525, 0.000),
                        Point.ByCoordinates(-8463.833, 3857.525, 0.000)) as Curve ,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8463.833, 3857.525, 0.000),
                        Point.ByCoordinates(-8463.833, -392.475, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8463.833, -392.475, 0.000),
                        Point.ByCoordinates (-728.833, -392.475, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-728.833, -392.475, 0.000),
                        Point.ByCoordinates(-728.833, 3857.525, 0.000)) as Curve
                }
            };

            // Act
            var boundaryLines = space.CoreBoundary;

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCoreCenterBoundary()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            List<List<Curve>> expectedBoundaries = new List<List<Curve>>()
            {
                new List<Curve>()
                {
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-678.833, 3907.525, 0.000),
                        Point.ByCoordinates(-8513.833, 3907.525, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8513.833, 3907.525, 0.000),
                        Point.ByCoordinates(-8513.833, -442.475, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8513.833, -442.475, 0.000),
                        Point.ByCoordinates(-678.833, -442.475, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-678.833, -442.475, 0.000),
                        Point.ByCoordinates(-678.833, 3907.525, 0.000)) as Curve
                }
            };

            // Act
            var boundaryLines = space.CoreCenterBoundary;

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceFinishBoundary()
        {
            // Arrange
            var space = ElementSelector.ByElementId(316365, true) as Space;
            List<List<Curve>> expectedBoundaries = new List<List<Curve>>()
            {
                new List<Curve>()
                {
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-741.333, 3845.025, 0.000),
                        Point.ByCoordinates(-8451.333, 3845.025, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8451.333, 3845.025, 0.000),
                        Point.ByCoordinates(-8451.333, -214.975, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-8451.333, -214.975, 0.000),
                        Point.ByCoordinates(-741.333, -214.975, 0.000)) as Curve,
                    Line.ByStartPointEndPoint(
                        Point.ByCoordinates(-741.333, -214.975, 0.000),
                        Point.ByCoordinates(-741.333, 3845.025, 0.000)) as Curve
                }
            };

            // Act
            var boundaryLines = space.FinishBoundary;

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        private static void AssertBoundaryLists(List<List<Curve>> expectedBoundaries, IEnumerable<IEnumerable<Curve>> boundaryLines)
        {
            Assert.AreEqual(expectedBoundaries.Count(), boundaryLines.Count());
            for (int i = 0; i < boundaryLines.Count(); i++)
            {
                var expected = expectedBoundaries.FirstOrDefault()[i];
                var actual = boundaryLines.FirstOrDefault().ToList()[i];

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
