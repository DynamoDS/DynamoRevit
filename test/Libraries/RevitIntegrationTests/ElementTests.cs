﻿using System.IO;

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
        public void CanGetElementParentElement()
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
        [TestModel(@".\Element\elementIntersection.rvt")]
        public void CanGetIntersectingElementsOfSpecificCategory()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canGetIntersectingElementsOfSpecificCategory.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedIntersectingElementId = 316246;

            // Act - Get the intersecting element id of wall category
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var actualIntersectingElementId = GetPreviewValue("438ec88918b94167887c7f4b2813ebfe");
            
            // Assert
            Assert.AreEqual(expectedIntersectingElementId, actualIntersectingElementId);
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
            var areJoinedBeforeRun = GetPreviewValue("e31b7404cd6243578b0e50b5a525baa4");
            var areJoinedAfterRun = GetPreviewValue("4779f361975a4a5fa5f8b0f6b94159ad");

            // Assert - The preview value of areJoinedAfterRun comes from an AllFalse node
            // that checks if every output of AreJoined is false,  returning
            // true if all elements have been unjoined.
            Assert.AreEqual(areJoinedBeforeRun, areJoinedAfterRun);

        }

        [Test]
        [TestModel(@".\Element\elementTransform.rvt")]
        public void CanTransformElement()
        {
            // Arrange
            var delta = 0.001;
            string samplePath = Path.Combine(workingDirectory, @".\Element\canTransformElement.dyn");
            string testPath = Path.GetFullPath(samplePath);
            var expectedLocationX = 4665.007;
            var expectedLocationY = -2577.392;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var transformedLocationX = GetPreviewValue("3280c1e7e17e4caabed24d5819904bcb") as double?;
            var transformedLocationY = GetPreviewValue("c8939e3477f844d5963a1fc98420fc08") as double?;
            var originalLocationX = GetPreviewValue("ab73733213ab4f7a95d7c0dd86dd7011") as double?;
            var originalLocationY = GetPreviewValue("8924547b83f24e718e9c9589ae288489") as double?;

            // Assert
            Assert.IsNotNull(transformedLocationX);
            Assert.IsNotNull(transformedLocationY);
            Assert.IsNotNull(originalLocationX);
            Assert.IsNotNull(originalLocationY);

            Assert.AreEqual(expectedLocationX, transformedLocationX, delta);
            Assert.AreEqual(expectedLocationY, transformedLocationY, delta);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanSwitchJoinOrderOfTwoJoinedElements()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canSwitchJoinOrderOfTwoJoinedElements.dyn");
            string testPath = Path.GetFullPath(samplePath);

            int originalCuttingElementId = 208422;
            int newOrderCuttingElementId = 208572;

            // Act - get the Id of the first element from SwitchGeometryJoinOrder
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var cuttingElementIdNewOrder = GetPreviewValue("8b96e9f628314bcab833ea4f830bc2a7");
        
            // Assert
            Assert.AreEqual(newOrderCuttingElementId, cuttingElementIdNewOrder);
            Assert.AreNotEqual(originalCuttingElementId, cuttingElementIdNewOrder);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanUnjoinTwoElements()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canUnjoinTwoElements.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var areJoinedBeforeRun = GetPreviewValue("c753a1ccf59f49dd8c01549b51dd2961");
            Assert.AreEqual(true, areJoinedBeforeRun);
            var areJoinedAfterRun = GetPreviewValue("5132f787bcdc44608b2cff2b7540c3e5");
            var unjoinElementsException = GetPreviewValue("8fc0495f5b1047599609404407446c79");

            // Assert
            Assert.AreEqual(true, areJoinedBeforeRun);
            Assert.AreNotEqual(areJoinedBeforeRun, areJoinedAfterRun);
            Assert.IsNull(unjoinElementsException);
        }
        
        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanJoinTwoIntersectingElements()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Element\canJoinTwoIntersectingElements.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedElementId = 208259;

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Act 
            var firstJoinedElementId = GetPreviewValue("0ee537c473c04470b5041d16d9b5ab12");
            var nonIntersectingElementsResult = GetPreviewValue("a9e31eafd9d5488ab843ea434c9243ed");

            // Assert
            Assert.AreEqual(expectedElementId, firstJoinedElementId);
            Assert.IsNull(nonIntersectingElementsResult);
        }
    }
}
