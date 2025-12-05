using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes.ZeroTouch;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;
using System.Linq;
using Point = Autodesk.DesignScript.Geometry.Point;

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

        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void CreateAreaSchedule()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\CreateAreaSchedule.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var createAreaSchedule = GetPreviewValue("de0f82ea57aa463da89f97f52f7603b9");
            Assert.IsNotNull(createAreaSchedule);

            var schedules = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .ToList();

            //Verify that at least one schedule is named "AreaSchedule"
            Assert.IsTrue(schedules.Any(s => s.Name == "AreaSchedule"), "No AreaSchedule found in the document.");
        }

        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void FilterScheduleView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\FilterScheduleView.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //Verify that the schedule was created
            var schedules = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(ViewSchedule))
                .Cast<ViewSchedule>()
                .ToList();
            Assert.IsTrue(schedules.Any(s => s.Name == "DoorSchedule"), "No DoorSchedule found in the document.");

            //Verify the first part of the script
            var scheduleViewFields = GetFlattenedPreviewValues("065abc4dcee141f68e153d4e465be579");
            Assert.AreEqual(207, scheduleViewFields.Count);

            var addFilters = GetPreviewValue("cdf84e35ad9f4b1b9d0177a8e2adb2db");
            Assert.IsNotNull(addFilters);

            var exportSchedule = GetFlattenedPreviewValues("3dde472b5310413f8820a93a51924fa2");
            Assert.AreEqual(211, exportSchedule.Count);
            Assert.IsNotNull(exportSchedule[150]);

            //Asserts for Filters
            var scheduleFilterFileId = GetPreviewValue("50ad7eff68f84453b99661a6d23a08eb");
            Assert.AreEqual(2, scheduleFilterFileId);

            var scheduleFilterType = GetPreviewValue("6b08ca1903bd4d2bac1194d55a5068a5");
            Assert.AreEqual("Contains", scheduleFilterType);

            var scheduleFilterValue = GetPreviewValue("46460c2f971f42df9bb88a4748b5d069");
            Assert.AreEqual("Doors_ExtDbl_Flush", scheduleFilterValue);

            var scheduleFilters = GetFlattenedPreviewValues("e238cfc088f441e79f5669ee99632824");
            Assert.IsNotNull(scheduleFilters[0]);
            Assert.IsNotEmpty(scheduleFilters[0].ToString());

            //Connect the output of ScheduleView.AddFilters node to the input of ScheduleView.ClearAllFilters node
            var addFiltersNode = GetNode<DSFunction>("cdf84e35-ad9f-4b1b-9d01-77a8e2adb2db");
            var clearFiltersNode = GetNode<DSFunction>("95e2e9a7-0363-4bac-b7c0-a5fdd5769e27");

            MakeConnector(addFiltersNode, clearFiltersNode, 0, 0);
            RunCurrentModel();

            var clearFilters = GetPreviewValue("95e2e9a7-0363-4bac-b7c0-a5fdd5769e27");
            Assert.IsNotNull(clearFilters);
        }
    }
}
