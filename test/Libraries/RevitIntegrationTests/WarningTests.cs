using NUnit.Framework;
using Revit.Application;
using RevitTestServices;
using RTF.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitSystemTests
{
    [TestFixture]
    public class WarningTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetWarningsFromCurrentDocument()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Warning\CanGetWarningsFromCurrentDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedWarningCount = 15;
            var expectedType = typeof(Revit.Application.Warning);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var warnings = GetPreviewCollection("08422b2f0a374986861861090bff5c3c");
            var distinctWarningTypes = warnings.Distinct().First();


            // Assert
            Assert.AreEqual(expectedWarningCount, warnings.Count);
            Assert.AreEqual(expectedType, distinctWarningTypes.GetType());
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSeverityOfWarning()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Warning\CanGetSeverityOfWarning.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedWarningSeverity = "Warning";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var warningSeverity = GetPreviewCollection("cdfe30eacb6043b3bc9bfa5e97dcceb3");
            var distinctWarningSeverity = warningSeverity.Distinct().First();

            // Assert
            Assert.AreEqual(expectedWarningSeverity, distinctWarningSeverity);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetWarningDescription()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Warning\CanGetWarningDescription.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedWarningDescriptions = new List<string>()
            {
                "Highlighted walls overlap. One of them may be ignored when Revit finds room boundaries. Use Cut Geometry to embed one wall within the other.",
                "Room is not in a properly enclosed region",
                "Multiple Rooms are in the same enclosed region.  The correct area and perimeter will be assigned to one Room and the others will display \"Redundant Room.\"  You should separate the regions, delete the extra Rooms, or move them into different regions.",
                "Area is not in a properly enclosed region",
                "Multiple Areas are in the same enclosed region.  The correct area and perimeter will be assigned to one Area and the others will display \"Redundant Area.\"  You should separate the regions, delete the extra Areas, or move them into different regions.",
                "Stair top end exceeds or cannot reach the top elevation of the stair. Add/remove risers at the top end by control or change the stair run's \"Relative Top Height\" parameter in the properties palette."
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var distinctWarningDescription = GetPreviewCollection("41aa7c05f7f14e08812a2cabfdedce19").Distinct().ToList();

            // Assert
            CollectionAssert.AreEqual(expectedWarningDescriptions, distinctWarningDescription);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetFailingElementsFromWarning()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Warning\CanGetFailingElementsFromWarning.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedFailingElements = new List<int>()
            {
                427092,
                627729,
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var failingElementIds = GetPreviewCollection("2737364bb8c44178a817b1aca121305a");

            // Assert
            CollectionAssert.AreEqual(expectedFailingElements, failingElementIds);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetWarningsFromWarningType()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Warning\CanGetWarningsFromWarningType.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedWarningCount = 2;
            var expectedWarningType = typeof(Revit.Application.Warning);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var node = GetNode<DSRevitNodesUI.AllWarningsOfType>("9dcb3039c7534bf494e5b16df19c33b2");
            var warnings = GetPreviewCollection("9dcb3039c7534bf494e5b16df19c33b2");

            // Assert
            Assert.AreEqual(expectedWarningCount, warnings.Count);
        }
    }
}
