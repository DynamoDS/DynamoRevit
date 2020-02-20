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
    class ElementTypeTests : RevitSystemTestBase
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
        public void CanGetElementTypeByName()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElementType\canGetElementTypeByName.dyn");
            string testPath = Path.GetFullPath(samplePath);
            int expectedId = 60411;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            Assert.AreEqual(expectedId, GetPreviewValue("880295ffe9b24d21b20e1ac1a63c2bb8"));
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetElementTypeProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElementType\CanGetElementTypeProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);
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
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var resultWallTypeName = GetPreviewValue("d899410efc5a403e88b549ece538225d");
            var resultWallTypeFamilyName = GetPreviewValue("519683950ccd48d6a492b3ec12e986f2");
            var resultWallTypeCanBeDeleted = GetPreviewValue("a8ca2b4b0d5f4174b462acce7bb10251");
            var resultWallTypeCanBeCopied = GetPreviewValue("99bf8171a172484ab72ce945284f6df4");
            var resultWallTypeCanBeRenamed = GetPreviewValue("eedc15bd5821488c9dd2692b2284d549");

            var resultColumnTypeName = GetPreviewValue("42dd5303a1954ccdbeddcd28bfae485e");
            var resultColumnTypeFamilyName = GetPreviewValue("e5a65c09716e496b8173a0d1730eddc4");
            var resultColumnTypeCanBeDeleted = GetPreviewValue("b0b2f2c796604f1f909225140c8aa8a9");
            var resultColumnTypeCanBeCopied = GetPreviewValue("47df316000ab4d7890931beff0062398");
            var resultColumnTypeCanBeRenamed = GetPreviewValue("40a3de5ece89444f96fa95c988e067ee");

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
        public void CanDuplicateElementType()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElementType\CanDuplicateElementType.dyn");
            string testPath = Path.GetFullPath(samplePath);
            var expectedWallTypeName = "Generic - 8\"_Copy";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var resultWallTypeName = GetPreviewValue("4fd1de6c83ba46e1afb671d363e1a30e");

            // Assert
            Assert.AreEqual(expectedWallTypeName, resultWallTypeName);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetElementTypePreviewImage()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\ElementType\CanGetElementTypePreviewImage.dyn");
            string testPath = Path.GetFullPath(samplePath);
            var expectedImageWidth = 500;
            var expectedImageHeight = 500;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var resultImageWidth = GetPreviewValue("a9b4d5a3d4bc4293b141f8f93446b1d4");
            var resultImageHeight = GetPreviewValue("522c3a4d6670498e995a9e593710d08e");

            // Assert
            Assert.AreEqual(expectedImageWidth, resultImageWidth);
            Assert.AreEqual(expectedImageHeight, resultImageHeight);
        }
    }
}
