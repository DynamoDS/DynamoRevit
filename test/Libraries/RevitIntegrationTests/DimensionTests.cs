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

        [Test]
        [TestModel(@".\Dimension\DimensionCreation.rvt")]
        public void ByFaces()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Dimension\ByFaces.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim1 = GetPreviewValue("0f6d28f0d11045d39ebaec1f6b75951c");
            var dim2 = GetPreviewValue("242a8771b20b48f397b0f89fb075f05d");

            Assert.AreEqual(dim1.GetType(), typeof(Revit.Elements.Dimension));
            Assert.AreEqual(dim2.GetType(), typeof(Revit.Elements.Dimension));
        }

        [Test]
        [TestModel(@".\Dimension\DimensionCreation.rvt")]
        public void ByEdges()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Dimension\ByEdges.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim = GetPreviewValue("68633628abac49948b493965b3654c84");

            Assert.AreEqual(dim.GetType(), typeof(Revit.Elements.Dimension));
        }


        [Test]
        [TestModel(@".\Dimension\DimensionCreation.rvt")]
        public void ByReferences()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Dimension\ByReferences.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim = GetPreviewValue("a0f302a9a4be4335903dafd93f9939db");

            Assert.AreEqual(dim.GetType(), typeof(Revit.Elements.Dimension));
        }
    }
}