using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryReferences;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using Element = Revit.Elements.Element;
using FamilyType = Revit.Elements.FamilyType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class ElementTypeTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetElementTypeProperties()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(184176, true);
            var column = ElementSelector.ByElementId(184324, true);

            var expectedWallTypeName = "Generic - 8\"";
            var expectedWallTypeFamilyName = "Basic Wall";
            var expectedWallTypeCanBeDeleted = true;
            var expectedWallTypeCanBeCopied = true;
            var expectedWallTypeCanBeRenamed = true;

            var expectedColumnTypeName = "24\" x 24\"";
            var expectedColumnTypeFamilyName = "Rectangular Column";
            var expectedColumnTypeCanBeDeleted = true;
            var expectedColumnTypeCanBeCopied = true;
            var expectedColumnTypeCanBeRenamed = true;

            // Act
            var wallElementType = wall.ElementType;
            var columnElementType = column.ElementType;

            var resultWallTypeName = wallElementType.Name;
            var resultWallTypeFamilyName = wallElementType.FamilyName;
            var resultWallTypeCanBeDeleted = wallElementType.CanBeDeleted;
            var resultWallTypeCanBeCopied = wallElementType.CanBeCopied;
            var resultWallTypeCanBeRenamed = wallElementType.CanBeRenamed;

            var resultColumnTypeName = columnElementType.Name;
            var resultColumnTypeFamilyName = columnElementType.FamilyName;
            var resultColumnTypeCanBeDeleted = columnElementType.CanBeDeleted;
            var resultColumnTypeCanBeCopied = columnElementType.CanBeCopied;
            var resultColumnTypeCanBeRenamed = columnElementType.CanBeRenamed;

            // Assert
            Assert.AreEqual(expectedWallTypeName, resultWallTypeName);
            Assert.AreEqual(expectedWallTypeFamilyName, resultWallTypeFamilyName);
            Assert.AreEqual(expectedWallTypeCanBeDeleted, resultWallTypeCanBeDeleted);
            Assert.AreEqual(expectedWallTypeCanBeCopied, resultWallTypeCanBeCopied);
            Assert.AreEqual(expectedWallTypeCanBeRenamed, resultWallTypeCanBeRenamed);

            Assert.AreEqual(expectedColumnTypeName, resultColumnTypeName);
            Assert.AreEqual(expectedColumnTypeFamilyName, resultColumnTypeFamilyName);
            Assert.AreEqual(expectedColumnTypeCanBeDeleted, resultColumnTypeCanBeDeleted);
            Assert.AreEqual(expectedColumnTypeCanBeCopied, resultColumnTypeCanBeCopied);
            Assert.AreEqual(expectedColumnTypeCanBeRenamed, resultColumnTypeCanBeRenamed);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetElementTypeByName()
        {
            // Arrange
            string elementTypeName = "24\" x 24\"";
            string emptyTypeName = "";
            string notFoundTypeName = "wallTypeTest";
            int expectedId = 60411;

            // Act
            var typeElement = Revit.Elements.ElementType.ByName(elementTypeName);
            Assert.Throws<System.ArgumentNullException>(() => Revit.Elements.ElementType.ByName(emptyTypeName));
            Assert.Throws<KeyNotFoundException>(() => Revit.Elements.ElementType.ByName(notFoundTypeName));
            int typeId = typeElement.InternalElement.Id.IntegerValue;

            // Assert
            Assert.AreEqual(expectedId, typeId);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanDuplicateElementType()
        {
            // Arrange
            string elementTypeName = "24\" x 24\"";
            string expectedDuplicatedElementTypeName = elementTypeName + "_Copy";

            // Act
            var typeElement = Revit.Elements.ElementType.ByName(elementTypeName);
            var noElementTypeFound = Assert.Throws<KeyNotFoundException>(() => Revit.Elements.ElementType.ByName(expectedDuplicatedElementTypeName));
            var duplicatedType = typeElement.Duplicate(expectedDuplicatedElementTypeName);
            var duplicatedTypeElementName = duplicatedType.Name;

            // Assert
            Assert.AreEqual(expectedDuplicatedElementTypeName, duplicatedTypeElementName);
            Assert.AreEqual(Revit.Properties.Resources.ElementTypeNameNotFound, noElementTypeFound.Message);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetElementTypePreviewImage()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(184176, true);
            var expectedGetPreviewImageType = typeof(System.Drawing.Bitmap);
            var imageSize = 500;
            var expectedImageSize = new System.Drawing.Size(imageSize, imageSize);

            // Act
            var typeElement = wall.ElementType;
            var previewImage = typeElement.GetPreviewImage(imageSize);

            // Assert
            Assert.AreEqual(expectedGetPreviewImageType, previewImage.GetType());
            Assert.AreEqual(expectedImageSize, previewImage.Size);
        }
    }
}
