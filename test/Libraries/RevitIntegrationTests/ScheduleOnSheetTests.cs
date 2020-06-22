using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;
using Dynamo.Tests;

namespace RevitSystemTests
{
    [TestFixture]
    class ScheduleOnSheetTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\ScheduleOnSheet\scheduleonsheet.rvt")]
        public void CanCreateScheduleOnSheet()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ScheduleOnSheet\CanCreateScheduleOnSheet.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedScheduleViewName = "View List";
            string expectedSheetName = "Unnamed";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var scheduleViewName = GetPreviewValue("b0ba0671312c49cfa1c23fb36b396471");
            var sheetName = GetPreviewValue("217fb20b2e1c4ab4a3cf8829d558d831");

            // Assert
            Assert.AreEqual(expectedScheduleViewName, scheduleViewName);
            Assert.AreEqual(expectedSheetName, sheetName);
        }

        [Test]
        [TestModel(@".\ScheduleOnSheet\scheduleonsheet.rvt")]
        public void CanSetGetLocation()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ScheduleOnSheet\CanSetGetLocation.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double x = 250;
            double y = 250;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var location = GetPreviewValue("d3f0f9480674418b8f987290176b2353") as Point;

            // Assert
            Assert.AreEqual(x, location.X, Tolerance);
            Assert.AreEqual(y, location.Y, Tolerance);
        }
    }
}
