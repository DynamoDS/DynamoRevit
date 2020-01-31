using NUnit.Framework;
using Revit.Application;
using Revit.Elements;
using RevitServices.Transactions;
using RevitTestServices;
using RTF.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RevitNodesTests.Application
{
    [TestFixture]
    public class FamilyDocumentTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.0001;

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentCategory()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var expectedCategory = "Generic Models";

            // Act
            var famCategory = famDoc.Category.Name;

            // Assert
            Assert.AreEqual(expectedCategory, famCategory);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentParameters()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var expectedParameterType = typeof(FamilyParameter);

            // Act
            List<FamilyParameter> famParameters = famDoc.Parameters;

            // Assert
            Assert.True(famParameters.All(param => param.GetType() == expectedParameterType));
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentCategory()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var expectedNewCategory = "Furniture";

            // Act
            var currentCategory = famDoc.InternalFamilyDocument.OwnerFamily.FamilyCategory.Name;
            famDoc.SetCategory(Category.ByName(expectedNewCategory));
            var newCategory = famDoc.InternalFamilyDocument.OwnerFamily.FamilyCategory.Name;

            // Assert
            Assert.AreNotEqual(currentCategory, newCategory);
            Assert.AreEqual(expectedNewCategory, newCategory);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentFormula_ValidArgs()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "Width";
            var expectedNewFormula = "Height * 2";

            // Act
            var currentFormula = famDoc.GetFormula(paramName);
            famDoc.SetFormula(paramName, expectedNewFormula);
            var newFormula = famDoc.GetFormula(paramName);

            // Assert
            Assert.IsNull(currentFormula);
            Assert.AreEqual(expectedNewFormula, newFormula);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentFormula_BadArgs()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "badParamName";
            var newFormula = "Height * 2";
            var expectedExceptionMsg = Revit.Properties.Resources.ParameterNotFound;

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => famDoc.SetFormula(paramName, newFormula));

            // Assert
            Assert.AreEqual(expectedExceptionMsg, ex.Message);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentFormula_ValidArgs()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "Depth";
            var expectedFormula = "Height + Leg Height";

            // Act
            var currentFormula = famDoc.GetFormula(paramName);

            // Assert
            Assert.AreEqual(expectedFormula, currentFormula);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentFormula_BadArgs()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "badParamName";
            var expectedExceptionMsg = Revit.Properties.Resources.ParameterNotFound;

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => famDoc.GetFormula(paramName));

            // Assert
            Assert.AreEqual(expectedExceptionMsg, ex.Message);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanGetFamilyDocumentParameterValue()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var doubleParam = "Depth";
            var elementParam = "Top Material";
            var stringParam = "Keynote";

            var expectedDoubleValue = 914;
            var expectedElementIdValue = 10863;
            var expectedStringValue = "N";

            // Act
            var doubleValue = famDoc.GetParameterValueByName(doubleParam);
            var elementValue = famDoc.GetParameterValueByName(elementParam) as Element;
            var stringValue = famDoc.GetParameterValueByName(stringParam);

            // Assert
            Assert.AreEqual(expectedDoubleValue, doubleValue);
            Assert.AreEqual(expectedElementIdValue, elementValue.Id);
            Assert.AreEqual(expectedStringValue, stringValue);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentParameterValue()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var doubleParam = "Height";
            var elementParam = "Top Material";
            var stringParam = "Keynote";

            var expectedDoubleValue = 1000.0;
            var expectedSetDoubleWithIntValue = 2000;
            var expectedElementValue = ElementSelector.ByElementId(10862);
            var expectedStringValue = "test";
            

            // Act
            var oldDoubleValue = famDoc.GetParameterValueByName(doubleParam);
            var oldElementValue = famDoc.GetParameterValueByName(elementParam) as Element;
            var oldStringValue = famDoc.GetParameterValueByName(stringParam);

            famDoc.SetParameterValueByName(doubleParam, expectedDoubleValue);
            famDoc.SetParameterValueByName(elementParam, expectedElementValue);
            famDoc.SetParameterValueByName(stringParam, expectedStringValue);

            var newDoubleValue = famDoc.GetParameterValueByName(doubleParam);
            var newElementValue = famDoc.GetParameterValueByName(elementParam) as Element;
            var newStringValue = famDoc.GetParameterValueByName(stringParam);

            // Assert
            Assert.AreNotEqual(oldDoubleValue, newDoubleValue);
            Assert.AreNotEqual(oldElementValue, newElementValue);
            Assert.AreNotEqual(oldStringValue, newStringValue);

            Assert.AreEqual(expectedDoubleValue, (double)newDoubleValue, Tolerance);
            Assert.AreEqual(expectedElementValue.Id, newElementValue.Id);
            Assert.AreEqual(expectedStringValue, newStringValue);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanSetFamilyDocumentParameterDoubleValueWithInt()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var doubleParam = "Height";

            var expectedIntValue = 2000;

            // Act
            var oldDoubleValue = famDoc.GetParameterValueByName(doubleParam);

            famDoc.SetParameterValueByName(doubleParam, expectedIntValue);

            var newDoubleValue = famDoc.GetParameterValueByName(doubleParam);

            // Assert
            Assert.AreNotEqual(oldDoubleValue, newDoubleValue);
            Assert.AreEqual(expectedIntValue, (double)newDoubleValue, Tolerance);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanAddParameterToFamilyDocument()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "TestParam";
            var paramGroup = "PG_DATA";
            var paramType = "Text";
            var instance = false;

            // Act - Check that the new parameter dosent exist by trying to get the value of it
            Assert.Throws<InvalidOperationException>(() => famDoc.GetParameterValueByName(paramName));
            // add the new parameter and get the value to verify that it has been created.
            famDoc.AddParameter(paramName, paramGroup, paramType, instance);
            var newParamValue = famDoc.GetParameterValueByName(paramName);

            // Assert - the expected value of the new parameter is an empty string as we havent set it to anything
            Assert.AreEqual(string.Empty, newParamValue);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanDeleteParameterFromFamilyDocument()
        {
            // Arrange
            var famDoc = new FamilyDocument(Document.Current.InternalDocument);
            var paramName = "Top Material";

            // Act
            var oldParamValue = famDoc.GetParameterValueByName(paramName);
            famDoc.DeleteParameter(paramName);
            // Should throw as parameter no longer should exist
            Assert.Throws<InvalidOperationException>(() => famDoc.GetParameterValueByName(paramName));

            // Assert
            Assert.IsNotNull(oldParamValue);
        }

        [Test]
        [TestModel(@".\FamilyDocument\familyDocumentTests.rfa")]
        public void CanCreateFamilyDocumentByDocument()
        {
            // Arrange
            var document = Document.Current;

            // Act
            var familyDocument = FamilyDocument.ByDocument(document);

            // Assert
            Assert.IsNotNull(familyDocument);
            Assert.AreEqual(typeof(FamilyDocument), familyDocument.GetType());
        }
    }
}
