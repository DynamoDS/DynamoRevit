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
            int expectedElementCount = 2;
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Act - get output of Element.GetJoinedElements in Dynamo script
            var joinedElementsCount = GetPreviewValue("a3e87f4fcb0f4326a239cedf26e2748c");

            // Assert - check if outcome element ids are the same as the expected element ids
            Assert.AreEqual(expectedElementCount, joinedElementsCount);
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
            var hostedElements = GetPreviewValue("52001294a6284c1185f9564d8924f781");
            int expectedValues = 4;

            // Assert
            // check if outcome is the same as the expected collection
            Assert.AreEqual(expectedValues, hostedElements);
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
            Assert.AreEqual(false, GetPreviewValue("d9811fadc1964cd0b58c440e227ce9ba"));
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

            //check Select Model Element
            var elem = ElementSelector.ByElementId(184176, true);
            Assert.IsNotNull(elem);

            bool originalPinnedStatus = elem.InternalElement.Pinned;
            Assert.AreEqual(false, originalPinnedStatus);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            
            //now flip the switch for setting the pinned status to true
            var boolNode = ViewModel.Model.CurrentWorkspace.Nodes.Where(x => x is CoreNodeModels.Input.BoolSelector).First();
            bool boolNodeValue = true;
            ((CoreNodeModels.Input.BasicInteractive<bool>)boolNode).Value = boolNodeValue;

            RunCurrentModel();
            bool newPinnedStatus = elem.InternalElement.Pinned;
            Assert.AreNotEqual(originalPinnedStatus, newPinnedStatus);
            Assert.AreEqual(boolNodeValue, newPinnedStatus);
            
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanCheckIfTwoElementsAreJoined()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canCheckIfTwoElementsAreJoined.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
           
            // Act - Get values of the two IsJoined nodes
            // first one should return false as it is checking two elements that are not joined
            // second one should return true as it test two joined elements
            var isJoinedFalse = GetPreviewValue("b29dc14336bc4c9382f9acc5036d632f");
            var isJoinedTrue = GetPreviewValue("b49a72f29d504acdb524ab0953623ea5");

            // Assert
            Assert.AreEqual(true, isJoinedTrue);
            Assert.AreEqual(false, isJoinedFalse);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanUnjoinListOfElements()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canUnjoinListOfElements.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var modifiedElements = GetPreviewValue("2bf6ae19361e4c8e841071411eb02fc8");

            // Assert
            Assert.AreEqual(5, modifiedElements);
        }
    }
}