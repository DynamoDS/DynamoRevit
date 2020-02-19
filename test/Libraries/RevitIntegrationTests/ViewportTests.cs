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

            int expectedViewportId = 307874;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var viewportId = GetPreviewValue("8e667727f45c4cd398963eca4a22dea5");

            // Assert
            Assert.AreEqual(expectedViewportId, viewportId);
        }
    }
}
