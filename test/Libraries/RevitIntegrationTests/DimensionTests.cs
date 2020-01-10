using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;
using System.Collections.Generic;

namespace RevitSystemTests
{
    [TestFixture]
    class DimensionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Dimension()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Dimension.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim = GetPreviewValue("9ecb0603-0fcd-42da-b9da-ec5c57e1deaf");

            Assert.AreEqual(dim.GetType(), typeof(Revit.Elements.Dimension));


        }

        [Test]
        [TestModel(@".\Dimension\dimensionTests.rvt")]
        public void CanSetAndGetBelowValue()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Dimension\CanSetAndGetBelowValue.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedDimensionValue = new List<string>()
            {
                "Below",
                "Below",
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var valueBelow = GetPreviewCollection("c3c9b916e2624171a8f9819fc22d7519");

            // Assert
            CollectionAssert.AreEqual(expectedDimensionValue, valueBelow);
        }

        [Test]
        [TestModel(@".\Dimension\dimensionTests.rvt")]
        public void CanSetAndGetAboveValue()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Dimension\CanSetAndGetAboveValue.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedDimensionValue = new List<string>()
            {
                "Above",
            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var valueAbove = GetPreviewCollection("612693dede914632a6a517e5292ea1b2");

            // Assert
            CollectionAssert.AreEqual(expectedDimensionValue, valueAbove);
        }
    }
}