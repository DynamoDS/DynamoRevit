using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    class SpaceTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanCreateSpaceFromLevelLocationNameNumber()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanCreateSpaceFromLevelLocationNameNumber.dyn");
            string testPath = Path.GetFullPath(samplePath);

            int expectedElementId = 317174;
            string expectedElementType = "Revit.Elements.Space";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var elementId = GetPreviewValue("d58d61787cb54c81b6c73cb210d1ebc8");
            var elementType = GetPreviewValue("27b8ddd75db840a1b75a830a32ea673b");
            
            // Assert
            Assert.AreEqual(expectedElementId, elementId);
            Assert.AreEqual(expectedElementType, elementType);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanCheckIfPointIsInsideSpace()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanCheckIfPointIsInsideSpace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var isPointInsideSpace = GetPreviewValue("9622164b8dcb4886a29c821c5bdd5737");

            // Assert
            Assert.AreEqual(true, isPointInsideSpace);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanSetSpaceName()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanSetSpaceName.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedSpaceName = "newSpaceName";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newSpaceName = GetPreviewValue("4aa69bb9c86b454a8d76b3d0e83cbc0c");

            // Assert
            Assert.AreEqual(expectedSpaceName, newSpaceName);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanSetSpaceNumber()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanSetSpaceNumber.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedSpaceNumber = "99";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newSpaceNumber = GetPreviewValue("06f60350f8ca490bbf39c9d63441cbc1");

            // Assert
            Assert.AreEqual(expectedSpaceNumber, newSpaceNumber);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceArea()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceArea.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double expectedArea = 31.302;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var spaceArea = GetPreviewValue("b97fe9f0024a42118e78f6a3941a345e");

            // Assert
            Assert.AreEqual(expectedArea, (double)spaceArea, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceHeight()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceHeight.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double expectedHeight = 4000;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var spaceHeight = GetPreviewValue("1d23c0bf90ae442dacee35ffd9909373");

            // Assert
            Assert.AreEqual(expectedHeight, (double)spaceHeight, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceLocation()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceLocation.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedLocationX = -4602.592;
            var expectedLocationY = 1723.307;
            var expectedLocationZ = 0;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var locationX = GetPreviewValue("1821c9e428a74b0cb3e7e6d55752879e");
            var locationY = GetPreviewValue("57415ae7839f4b8c9204052fc65a5b24");
            var locationZ = GetPreviewValue("c8d89da660004dc1a4336b5636ec85f0");

            // Assert
            Assert.AreEqual(expectedLocationX, (double)locationX, Tolerance);
            Assert.AreEqual(expectedLocationY, (double)locationY, Tolerance);
            Assert.AreEqual(expectedLocationZ, (double)locationZ, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceName()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceName.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedName = "Space";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var spaceName = GetPreviewValue("13e7ef587489467296974073017d5531");

            // Assert
            Assert.AreEqual(expectedName, spaceName);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceNumber()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceNumber.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedNumber = "1";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var spaceNumber = GetPreviewValue("08900fd842604bb7816090b214396fd1");

            // Assert
            Assert.AreEqual(expectedNumber, spaceNumber);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceVolume()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceVolume.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double expectedVolume = 125.2104;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var spaceVolume = GetPreviewValue("b3525dc82f674c33abd466efe5077dd1");

            // Assert
            Assert.AreEqual(expectedVolume, (double)spaceVolume, Tolerance);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCenterBoundary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceCenterBoundary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            List<Curve> expectedBoundaries = new List<Curve>()
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
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var boundaryLines = GetFlattenedPreviewValues("43a8ecf7e1dd4b1495c2118a42c1b314");

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCoreBoundary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceCoreBoundary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            List<Curve> expectedBoundaries = new List<Curve>()
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
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var boundaryLines = GetFlattenedPreviewValues("846fe26bba7f4246b93181691fb02f34");

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceCoreCenterBoundary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceCoreCenterBoundary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            List<Curve> expectedBoundaries = new List<Curve>()
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
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var boundaryLines = GetFlattenedPreviewValues("ee07e78a0ad24f6784c93d12780f1584");

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        [Test]
        [TestModel(@".\Space\spaceTest.rvt")]
        public void CanGetSpaceFinishBoundary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Space\CanGetSpaceFinishBoundary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            List<Curve> expectedBoundaries = new List<Curve>()
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
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var boundaryLines = GetFlattenedPreviewValues("455e735b1e9949c384e113361f14d42a");

            // Assert
            AssertBoundaryLists(expectedBoundaries, boundaryLines);
        }

        private static void AssertBoundaryLists(List<Curve> expectedBoundaries, List<object> boundaryLines)
        {
            Assert.AreEqual(expectedBoundaries.Count(), boundaryLines.Count());
            for (int i = 0; i < boundaryLines.Count(); i++)
            {
                var expected = expectedBoundaries[i];
                var actual = (Curve)boundaryLines[i];

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