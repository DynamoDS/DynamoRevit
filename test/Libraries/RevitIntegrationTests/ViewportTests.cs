using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;

namespace RevitSystemTests
{
    [TestFixture]
    public class ViewportTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanCreateViewportOnSheet()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanCreateViewportOnSheet.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedViewportName = "Title w Line";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var viewportName = GetPreviewValue("e01a83242a3d4a2cb02789c3c90bb1a1");

            // Assert
            Assert.AreEqual(expectedViewportName, viewportName);
        }
    }
}
