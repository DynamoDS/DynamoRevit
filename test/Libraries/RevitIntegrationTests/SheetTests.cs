using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.IO;

using System.Linq;
using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    public class SheetTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlock()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Sheet\ByNameNumberTitleBlock.dyn");
            string testPath = Path.GetFullPath(samplePath);


            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var sheet = GetPreviewValue("54254be3a86d4ba89931d5c0a3428624");

            // Assert
            Assert.NotNull(sheet);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSheetProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Sheet\CanGetSheetProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string titleBlockName = "A1 metric";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var titleBlock = GetPreviewValueAtIndex("79ba6b1087a147d591e5e9c7e650ef8d", 0);
            Assert.AreEqual(titleBlockName, (titleBlock as Revit.Elements.Element).Name);

            var viewportsID = "6d45f04305a845fea459400737406301";
            AssertPreviewCount(viewportsID, 5);
            for (int i = 0; i < 5; i++)
            {
                var viewport = GetPreviewValueAtIndex(viewportsID, i) as Revit.Elements.Viewport;
                Assert.IsNotNull(viewport);
            }

            var schedulesID = "f94fa1de83564759a08ea6f0ac6246f6";
            AssertPreviewCount(schedulesID, 2);
            for (int i = 0; i < 2; i++)
            {
                var scheduleOnSheet = GetPreviewValueAtIndex(schedulesID, i) as Revit.Elements.ScheduleOnSheet;
                Assert.IsNotNull(scheduleOnSheet);
            }
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void GetSetSheetNameNumber()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Sheet\GetSetSheetNameNumber.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string newSheetName = "Test";
            string newSheetNumber = "A002";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var sheetName = GetPreviewValue("cdfa7ee24c9c4387af9ade23f7ea28f3");
            Assert.AreEqual(newSheetName, sheetName);
            var sheetNumber = GetPreviewValue("723ce0df917a443aa410891e01bdc0aa");
            Assert.AreEqual(newSheetNumber, sheetNumber);
        }
    }
}
