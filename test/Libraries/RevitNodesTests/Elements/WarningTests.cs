using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Revit.Application;
using Revit.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class WarningTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetWarningsFromCurrentDocument()
        {
            // Arrange
            var expectedWarningCount = 15;
            var expectedType = typeof(Revit.Application.Warning);

            // Act
            var warnings = Warning.GetWarnings(Document.Current);
            var distinctWarningTypes = warnings.Distinct().ToList().First().GetType();

            // Assert
            Assert.AreEqual(expectedWarningCount, warnings.Count);
            Assert.AreEqual(expectedType, distinctWarningTypes);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSeverityOfWarning()
        {
            // Arrange
            var expectedWarningSeverity = "Warning";

            // Act
            var warnings = Warning.GetWarnings(Document.Current);
            var distinctWarningSeverity = warnings.Select(warn => warn.Severity).Distinct().ToList().First();

            // Assert
            Assert.AreEqual(expectedWarningSeverity, distinctWarningSeverity);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetWarningDescription()
        {
            // Arrange
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
            var warnings = Warning.GetWarnings(Document.Current);
            var distinctWarningDescription = warnings.Select(warn => warn.Description).Distinct().ToList();

            // Assert
            CollectionAssert.AreEqual(expectedWarningDescriptions, distinctWarningDescription);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetFailingElementsFromWarning()
        {
            // Arrange
            var expectedFailingElements = new List<int>()
            {
                427092,
                627729,
            };

            // Act
            var warnings = Warning.GetWarnings(Document.Current);
            var failingElements = warnings.First().GetFailingElements();
            var failingElementIds = failingElements.Select(x => x.Id);

            // Assert
            CollectionAssert.AreEqual(expectedFailingElements, failingElementIds);
        }
    }
}
