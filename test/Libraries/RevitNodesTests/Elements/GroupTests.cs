using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class GroupTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanCreateGroupFromListOfElements()
        {
            // Arrange
            List<int> elementIds = new List<int>() { 184176, 184324 };
            List<Element> elementsToGroup = elementIds.Select(id => ElementSelector.ByElementId(id, true)).ToList();

            // Act
            var group = Group.ByElements(elementsToGroup);
            var memberIds = group.InternalGroup.GetMemberIds().Select(id => id.IntegerValue).ToList();

            // Assert
            Assert.IsNotNull(group);
            CollectionAssert.AreEqual(elementIds, memberIds);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanPlaceGroupInstance()
        {
            // Arrange
            Element groupType = ElementSelector.ByElementId(316699, true).ElementType;
            var expectedLocationForNewGroup = Point.ByCoordinates(13, 17, 19);

            // Act
            var newGroup = Group.PlaceInstance(expectedLocationForNewGroup, groupType);
            var newLocation = newGroup.Location;


            // Assert
            Assert.AreEqual(expectedLocationForNewGroup.X, newLocation.X, Tolerance);
            Assert.AreEqual(expectedLocationForNewGroup.Y, newLocation.Y, Tolerance);
            Assert.AreEqual(expectedLocationForNewGroup.Z, newLocation.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetGroupMembers()
        {
            // Arrange
            Group group = ElementSelector.ByElementId(316699, true) as Group;
            var expectedMembers = new List<int>() { 316137, 316143, 316149, 316155 };

            // Act
            var groupMembers = group.GetMembers();
            var groupMemberIds = groupMembers.Select(x => x.Id);

            // Assert
            CollectionAssert.AreEqual(expectedMembers, groupMemberIds);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanUngroupElementsFromGroup()
        {
            // Arrange
            Group group = ElementSelector.ByElementId(316699, true) as Group;
            var expectedUngroupedElements = new List<int>() { 316137, 316143, 316149, 316155 };

            // Act
            var ungroupedElements = group.UngroupElements();
            var ungroupedElementIds = ungroupedElements.Select(x => x.Id);

            // Assert
            CollectionAssert.AreEqual(expectedUngroupedElements, ungroupedElementIds);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetAttachedDetailGroup()
        {
            // Arrange
            var modelGroup = ElementSelector.ByElementId(316699, true) as Group;
            var attachedGroup = ElementSelector.ByElementId(316701, true) as Group;

            var expectedAttachedDetailGroup = new List<int>() { 316702 };

            // Act
            var attachedDetailGroup = modelGroup.AttachedDetailGroup;
            var attachedDetailGroupId = attachedDetailGroup.Select(x => x.Id).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedAttachedDetailGroup, attachedDetailGroupId);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetGroupType()
        {
            // Arrange
            var modelGroup = ElementSelector.ByElementId(316699, true) as Group;
            var attachedGroup = ElementSelector.ByElementId(316701, true) as Group;

            var expectedModelGroupTypeId = 316700;
            var expectedattachedGroupTypeId = 316702;

            // Act
            var modelGroupTypeId = modelGroup.GroupType.Id;
            var attachedGroupTypeId = attachedGroup.GroupType.Id;

            // Assert
            Assert.AreEqual(expectedModelGroupTypeId, modelGroupTypeId);
            Assert.AreEqual(expectedattachedGroupTypeId, attachedGroupTypeId);


        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanCheckIfGroupIsAttached()
        {
            // Arrange
            var modelGroup = ElementSelector.ByElementId(316699, true) as Group;
            var attachedGroup = ElementSelector.ByElementId(316701, true) as Group;

            // Assert
            Assert.AreEqual(false, modelGroup.IsAttached);
            Assert.AreEqual(true, attachedGroup.IsAttached);
        }

        [Test]
        [TestModel(@".\Group\GroupTest.rvt")]
        public void CanGetGroupLocation()
        {
            // Arrange
            var modelGroup = ElementSelector.ByElementId(316699, true) as Group;
            var attachedGroup = ElementSelector.ByElementId(316701, true) as Group;

            var expectedModelGroupLocation = Point.ByCoordinates(-5838.369, 4122.718, 0.000);
            var expectedAttachedGroupLocationExceptionMessage = Revit.Properties.Resources.AttachedGroupLocation;

            // Act
            var modelGroupLocation = modelGroup.Location;
            var attachedGroupLocation = Assert.Throws<InvalidOperationException>(() => GetLocation(attachedGroup));

            // Assert
            Assert.AreEqual(expectedModelGroupLocation.X, modelGroupLocation.X, Tolerance);
            Assert.AreEqual(expectedModelGroupLocation.Y, modelGroupLocation.Y, Tolerance);
            Assert.AreEqual(expectedModelGroupLocation.Z, modelGroupLocation.Z, Tolerance);
            Assert.AreEqual(expectedAttachedGroupLocationExceptionMessage, attachedGroupLocation.Message);
        }

        private static Point GetLocation(Group modelGroup)
        {
            return modelGroup.Location;
        }

    }
}
