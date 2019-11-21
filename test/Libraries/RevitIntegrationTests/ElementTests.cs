using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using System.Linq;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using RevitServices.Materials;
using System.Collections.Generic;
using Revit.Elements;

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
   
        /// <summary>
        /// Checks if Elements hosted elements can be retrived from Dynamo
        /// </summary>
        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanGetJoinedElementsFromElement()
        {
            // Arrange - setup to run Dynamo script
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetJoinedElementsFromElement.dyn");
            string testPath = Path.GetFullPath(samplePath);
            List<int> expectedElementIds = new List<int>() { 184176, 208422 };
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            List<Element> joinedElements = GetPreviewValue("bae6e489b6d34519996aa4f9c9ad8e67") as List<Element>;
            Assert.IsNotNull(joinedElements);

            // Act - get output of Element.GetJoinedElements in Dynamo script
            var joinedElementIds = joinedElements
                .Select(x => x.Id)
                .ToArray();
            Assert.IsNotNull(joinedElementIds);

            // Assert - check if outcome element ids are the same as the expected element ids
            CollectionAssert.AreEqual(expectedElementIds, joinedElementIds);
        }

        /// <summary>
        /// Checks if Elements hosted elements can be retrived from Dynamo
        /// </summary>
        [Test]
        [TestModel(@".\Element\hostedElements.rvt")]
        public void CanGetHostedElements()
        {
            // Arrange
            // set-up to run dynamo script
            string samplePath = Path.Combine(workingDirectory, @".\Element\canSuccessfullyGetHostedElements.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Act
            // get output of Element.Names in dynamo script
            List<string> hostedElementNames = GetPreviewValue("d9a3fb06d30a4c088c582ae81ca4245f") as List<string>;
            List<string> expectedValues = new List<string>() { "600 x 3100", "600 x 3100", "600 x 3100", "Rectangular Straight Wall Opening" };

            // Assert
            // check if outcome is the same as the expected collection
            CollectionAssert.AreEqual(expectedValues, hostedElementNames);
        }

        [Test]
        [TestModel(@".\Element\elementPinned.rvt")]
        public void CanGetPinnedStatus()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetPinnedStatus.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
            // query AllFalse node to check that the output of IsPinned is false for both elements in test model
            Assert.AreEqual(true, GetPreviewValue("d9811fadc1964cd0b58c440e227ce9ba"));
        }

        /// <summary>
        /// Checks that an Element's pinned status can be set from Dynamo.
        /// </summary>
        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSetPinnedStatus()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Element\setElementsPinStatus.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            //check Select Model Element
            var selectElement = "f49d6941-4497-43c3-9a52-fe4e5424e4e7-0002cf70;";
            var selectElementValue = GetPreviewValue(selectElement) as Element;
            Assert.IsNotNull(selectElementValue);

            bool originalPinnedStatus = selectElementValue.IsPinned;
            Assert.IsNotNull(originalPinnedStatus);
            Assert.AreEqual(false, originalPinnedStatus);

            //now flip the switch for setting the pinned status to true
            var boolNode = ViewModel.Model.CurrentWorkspace.Nodes.Where(x => x is CoreNodeModels.Input.BoolSelector).First();
            Assert.IsNotNull(boolNode);
            bool boolNodeValue = true;
            ((CoreNodeModels.Input.BasicInteractive<bool>)boolNode).Value = boolNodeValue;

            RunCurrentModel();
            bool newPinnedStatus = selectElementValue.IsPinned;
            Assert.AreNotEqual(originalPinnedStatus, newPinnedStatus);
            Assert.IsNotNull(newPinnedStatus);
            Assert.AreEqual(boolNodeValue, newPinnedStatus);
            
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanCheckIfTwoElementsAreJoined()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canCheckIfTwoElementsAreJoined");
            string testPath = Path.GetFullPath(samplePath);
            
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
           
            // Act - Get values of the two IsJoined nodes
            // first one should return false as it is checking two elements that are not joined
            // second one should return true as it test two joined elements
            var isJoinedFalse = GetPreviewValue("d18424a424aa476588f6f466675b7123");
            var isJoinedTrue = GetPreviewValue("f93b0fb9baca4a6fa4d9818b4dffd713");

            // Assert
            Assert.AreEqual(true, isJoinedTrue);
            Assert.AreEqual(false, isJoinedFalse);
        }
    }
}
