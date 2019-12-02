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
        public void CanGetElementSubComponents()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetElementSubComponents.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedBeamSystemSubElementCount = 5;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var resultBeamSystemSubElementCount = GetPreviewValue("939d773907c94323b2abe1f1df69761c");
            var resultWallSubElement = GetPreviewValue("ed85fef0a6e649dcb1384f8ec231b051");

            // Assert
            Assert.AreEqual(expectedBeamSystemSubElementCount, resultBeamSystemSubElementCount);
            Assert.IsNull(resultWallSubElement);
        }


    }
}
