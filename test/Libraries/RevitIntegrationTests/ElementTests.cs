using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using System.Linq;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using RevitServices.Materials;
namespace RevitSystemTests
{
    [TestFixture]
    class ElementTests : RevitSystemTestBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            //setup the material manager just for tests
            var mgr = MaterialsManager.Instance;
            mgr.InitializeForActiveDocumentOnIdle();
        }
        
        [Test]
        [TestModel(@".\element.rvt")]
        public void CanDeleteElement()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Element\deleteWallFromDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            // query count node to verify 1 item deleted as a result of the wall deletion. 
            Assert.AreEqual(1, GetPreviewValue("ccd8a5ba37fd4b1297def564392ccf54"));
         }

        [Test]
        [TestModel(@".\Element\elementComponents.rvt")]
        public void CanGetElementChildComponents()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetElementChildElements.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedBeamSystemChildElementCount = 5;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var resultBeamSystemChildElementCount = GetPreviewValue("939d773907c94323b2abe1f1df69761c");
            var resultWallChildElement = GetPreviewValue("ed85fef0a6e649dcb1384f8ec231b051");

            // Assert
            Assert.AreEqual(expectedBeamSystemChildElementCount, resultBeamSystemChildElementCount);
            Assert.IsNull(resultWallChildElement);
        }

        [Test]
        [TestModel(@".\Element\elementComponents.rvt")]
        public void CanGetElementSuperComponent()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetElementParentElement.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedWindowParentElementId = 319481;
            int expectedBeamParentElementId = 319537;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var resultWindowParentElement = GetPreviewValue("54c8d93d86494469b9e0cd06c78d248b");
            var resultBeamParentElement = GetPreviewValue("85a73f659194410db9d9cf27355b0fd6");

            // Assert
            Assert.AreEqual(expectedBeamParentElementId, resultBeamParentElement);
            Assert.AreEqual(expectedWindowParentElementId, resultWindowParentElement);
        }

    }
}
