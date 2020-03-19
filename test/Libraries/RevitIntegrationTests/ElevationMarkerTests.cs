using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using System.Linq;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using RevitServices.Materials;
using System.Collections.Generic;
using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class ElevationMarkerTests : RevitSystemTestBase
    {
        
        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanCreateElevationMarker()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElevationMarker\CanCreateElevationMarker.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedType = "Revit.Elements.ElevationMarker";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newElevationMarkerType = GetPreviewValue("a59b9fc4ba14481f902ce8399ed8c3ca");

            // Assert
            Assert.AreEqual(expectedType, newElevationMarkerType);
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanCreateElevationViewsByMarkerIndex()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElevationMarker\CanCreateElevationViewsByMarkerIndex.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedViewName = "Elevation 8 - a";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var elevationViewName = GetPreviewValue("c0e547cb84b4483cb8b5a2cdd28970c0");

            // Assert
            Assert.AreEqual(expectedViewName, elevationViewName);
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanGetElevationMarkerViewCount()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElevationMarker\CanGetElevationMarkerViewCount.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedViewCount = 3;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var elevationMarkerViewCount = GetPreviewValue("3c4da6eecf544216a574cdf6571aa8de");

            // Assert
            Assert.AreEqual(expectedViewCount, elevationMarkerViewCount);
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanGetElevationMarkerViewByIndex()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElevationMarker\CanGetElevationMarkerViewByIndex.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedViewId = 327396;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var elevationMarkerViewId = GetPreviewValue("dd7da7fe8ead493287da308fe9a7a832");

            // Assert
            Assert.AreEqual(expectedViewId, elevationMarkerViewId);
        }
    }
}
