using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests.Application
{
    [TestFixture]
    public class FamilyDocumentTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.0001;

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentCategory()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanGetFamilyDocumentCategory.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedCategory = "Generic Models";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var familyDocumentCategory = GetPreviewValue("7f65f95f8e5e4f62a51ceab110298af4") as Revit.Elements.Category;

            // Assert
            Assert.AreEqual(expectedCategory, familyDocumentCategory.Name);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentParameters()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanGetFamilyDocumentParameters.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedOutputType = typeof(Revit.Elements.FamilyParameter);
            var expectedParameterCount = 19;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var familyDocumentParameters = GetPreviewCollection("901b09faa57f4636baf054b2162d907d");

            // Assert
            Assert.AreEqual(expectedParameterCount, familyDocumentParameters.Count);
            familyDocumentParameters.ForEach(param => Assert.AreEqual(expectedOutputType, param.GetType()));
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentCategory()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanSetFamilyDocumentCategory.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedNewCategory = "Furniture";
            var expectedOldCategory = "Generic Models";

            var familyDoc = Revit.Application.FamilyDocument.ByDocument(Revit.Application.Document.Current);
            var oldCategory = familyDoc.Category.Name;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newCategory = familyDoc.Category.Name;


            // Assert
            Assert.AreNotEqual(oldCategory, newCategory);
            Assert.AreEqual(expectedOldCategory, oldCategory);
            Assert.AreEqual(expectedNewCategory, newCategory);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentFormula()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanSetFamilyDocumentFormula.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var paramter = "Width";
            var expectedNewFormula = "Depth * 2";

            var familyDoc = Revit.Application.FamilyDocument.ByDocument(Revit.Application.Document.Current);
            var oldFormula = familyDoc.GetFormula(paramter);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newFormula = familyDoc.GetFormula(paramter);

            // Assert
            Assert.AreNotEqual(oldFormula, newFormula);
            Assert.AreEqual(null, oldFormula);
            Assert.AreEqual(expectedNewFormula, newFormula);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentFormula()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanGetFamilyDocumentFormula.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedFormula = "Height + Leg Height";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var formula = GetPreviewValue("15316717b6554d2bbdc1c71523d95f3f");

            // Assert
            Assert.AreEqual(expectedFormula, formula);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentParameterValue()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanGetFamilyDocumentParameterValue.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedValue = 914.0;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var value = GetPreviewValue("f5ebbe4efbbc44779aaefd40f760899a");

            // Assert
            Assert.AreEqual(expectedValue, (double)value, Tolerance);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentParameterValue()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanSetFamilyDocumentParameterValue.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var paramter = "Width";
            var expectedValue = 2000.0;
            var expectedOldValue = 1000.0;

            var familyDoc = Revit.Application.FamilyDocument.ByDocument(Revit.Application.Document.Current);
            var familyType = "1525x762mm_Student";

            var oldValue = familyDoc.GetParameterValueByName(familyType, paramter);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newValue = familyDoc.GetParameterValueByName(familyType, paramter);

            // Assert
            Assert.AreNotEqual(oldValue, newValue);
            Assert.AreEqual(expectedValue, (double)newValue, Tolerance);
            Assert.AreEqual(expectedOldValue, (double)oldValue, Tolerance);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanAddParameterToFamilyDocument()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanAddParameterToFamilyDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedNewParameterName = "TestParam";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var newParameterName = GetPreviewValue("49f454e7f0794d6190572b4a0532c23f");

            // Assert
            Assert.AreEqual(expectedNewParameterName, newParameterName);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanDeleteParameterFromFamilyDocument()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanDeleteParameterFromFamilyDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedDeletedParameterId = 273388;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var deletedParameterId = GetPreviewValue("ec16522fec4e4b50ab781d56bf39a309");

            // Assert
            Assert.AreEqual(expectedDeletedParameterId, deletedParameterId);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanCreateFamilyDocumentByDocument()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FamilyDocument\CanCreateFamilyDocumentByDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedOutputType = typeof(Revit.Application.FamilyDocument);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var output = GetPreviewValue("2e6e5ff57dfd4abdb56f4d5988c94915");

            // Assert
            Assert.AreEqual(output.GetType(), expectedOutputType);
        }
    }
}
