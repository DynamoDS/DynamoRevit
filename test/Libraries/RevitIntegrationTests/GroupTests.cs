using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitSystemTests
{
    [TestFixture]
    public class GroupTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanCreateGroupFromListOfElements()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Group\CanCreateGroupFromListOfElements.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedGroupId = 207894;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newGroupId = GetPreviewValue("e5d988cdaac248f8982268110c84f596");

            // Assert
            Assert.AreEqual(expectedGroupId, newGroupId);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanPlaceGroupInstance()
        {
            // Arange
            string samplePath = Path.Combine(workingDirectory, @".\Group\CanPlaceGroupInstance.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedGroupLocation = Point.ByCoordinates(0, 0, 0);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var newGroupLocationX = GetPreviewValue("f935e846b2fc401c8ff2e4c165b2695b");
            var newGroupLocationY = GetPreviewValue("fe33ae594d3442189c91eaf50d7b010b");
            var newGroupLocationZ = GetPreviewValue("030b3ce5eb5e48d9843dc64080914375");

            // Assert
            Assert.AreEqual(expectedGroupLocation.X, (double)newGroupLocationX, Tolerance);
            Assert.AreEqual(expectedGroupLocation.Y, (double)newGroupLocationY, Tolerance);
            Assert.AreEqual(expectedGroupLocation.Z, (double)newGroupLocationZ, Tolerance);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetGroupMembers()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Group\CanGetGroupMembers.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedGroupMemberIds = new List<int>() { 316137, 316143, 316149, 316155 };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var groupMemberElementIds = GetPreviewCollection("27623eaad92d442986aeb8e19b2a3e75");

            // Assert
            CollectionAssert.AreEqual(expectedGroupMemberIds, groupMemberElementIds);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanUngroupElementsFromGroup()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Group\CanUngroupElementsFromGroup.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedUngroupedElementIds = new List<int>() { 316137, 316143, 316149, 316155 };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var ungroupedElementIds = GetPreviewCollection("64a15121a072410ba83bc3e756123df5");

            // Assert
            CollectionAssert.AreEqual(expectedUngroupedElementIds, ungroupedElementIds);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetGroupProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Group\CanGetGroupProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedModelGroupTypeId = 316700;
            var expectedattachedGroupTypeId = 316702;

            var expectedModelGroupLocation = Point.ByCoordinates(-5838.369, 4122.718, 0.000);

            var expectedAttachedDetailGroup = new List<int>() { 316702 };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var modelGroupIsAttached = GetPreviewValue("8f9c7c5014fc40b0bf32143df26b7cea");
            var attachedGroupIsAttached = GetPreviewValue("6184d48100614e4cbf97148d587ec93d");

            var modelGroupTypeId = GetPreviewValue("86129f4449384f7fb818f1ceace9aa67");
            var attachedGroupTypeId = GetPreviewValue("02045a748aa44c4698de214c7a02202b");

            var modelGroupLocationX = GetPreviewValue("9416e74f55284fdda65953c06dcfee4c");
            var modelGroupLocationY = GetPreviewValue("c2f1bf28f9504bf289ba2d4eb123e208");
            var modelGroupLocationZ = GetPreviewValue("0e84e93aad5f427ea1a7c2ef254f711e");

            var attachedDetailGroupId = GetPreviewCollection("a9e7e1ef060b45ea9f7176adde8bd90f");

            // Assert
            Assert.AreEqual(false, modelGroupIsAttached);
            Assert.AreEqual(true, attachedGroupIsAttached);

            Assert.AreEqual(expectedModelGroupTypeId, modelGroupTypeId);
            Assert.AreEqual(expectedattachedGroupTypeId, attachedGroupTypeId);

            Assert.AreEqual(expectedModelGroupLocation.X, (double)modelGroupLocationX, Tolerance);
            Assert.AreEqual(expectedModelGroupLocation.Y, (double)modelGroupLocationY, Tolerance);
            Assert.AreEqual(expectedModelGroupLocation.Z, (double)modelGroupLocationZ, Tolerance);

            Assert.AreEqual(expectedAttachedDetailGroup, attachedDetailGroupId);
        }
    }
}
