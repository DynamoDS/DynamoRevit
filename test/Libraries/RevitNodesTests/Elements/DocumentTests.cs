﻿using NUnit.Framework;
using Revit.Application;
using Revit.Elements;
using RevitServices.Transactions;
using RevitTestServices;
using RTF.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class DocumentTests : RevitNodeTestBase
    {

        [Test]
        public void Current()
        {
            var doc = Document.Current;
            Assert.NotNull(doc);
            Assert.NotNull(doc.ActiveView);
            Assert.IsTrue(doc.IsFamilyDocument);
        }

        [Test]
        [TestModel(@".\Document\LocalModel\Project1_LocalFile.rvt")]
        public void CanGetWorksharingModelPath()
        {
            // Arrange
            var doc = Document.Current;
            string expectedWorksharingFilePath = @"DynamoRevit\test\System\Document\CentralModel";
            bool expectedIsCloudPathResult = false;

            // Act
            string resultWorksharingPath = doc.WorksharingPath;
            bool resultIsCloudPath = doc.IsCloudPath;

            // Assert
            Assert.IsTrue(resultWorksharingPath.Contains(expectedWorksharingFilePath));
            Assert.AreEqual(expectedIsCloudPathResult, resultIsCloudPath);
        }

        [Test]
        [TestModel(@".\Document\BIM360\4481adfb-0f03-4e58-9f49-8bd37dde9e0e.rvt")]
        public void CanGetWorksharingModelPathOnCloudModel()
        {
            // Arrange
            var doc = Document.Current;
            string expectedWorksharingFilePath = @"BIM 360://Node test/BIM360_model.rvt";
            bool expectedIsCloudPathResult = true;

            // Act
            string resultWorksharingPath = doc.WorksharingPath;
            bool resultIsCloudPath = doc.IsCloudPath;

            // Assert
            Assert.IsTrue(resultWorksharingPath.Contains(expectedWorksharingFilePath));
            Assert.AreEqual(expectedIsCloudPathResult, resultIsCloudPath);
        }

        private void GetWorksharingPathThrowsNullReferenceInNonSharedDocument()
        {
            string worksharePath = Document.Current.WorksharingPath;
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSaveFamilyInCurrentDocument()
        {
            // Arrange
            var saveableFamily = ElementSelector.ByElementId(110049, true);
            var noneditableFamily = ElementSelector.ByElementId(20915, true);
            string savedFamilyName = saveableFamily.Name + ".rfa";

            string nonExistingTempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string existingTempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(existingTempFolder);

            string expectedSavedFilePathNonExistingFolder = Path.GetFullPath(Path.Combine(nonExistingTempFolder, savedFamilyName));
            string expectedSavedFilePathExistingFolder = Path.GetFullPath(Path.Combine(existingTempFolder, savedFamilyName));

            // Act
            var doc = Document.Current;
            var resultSavedFamilyInNonExistingFolder = doc.SaveFamilyToFolder((Family)saveableFamily, nonExistingTempFolder);
            var resultSavedFamilyInExistingFolder = doc.SaveFamilyToFolder((Family)saveableFamily, existingTempFolder);
            var resultnoneditableFamily = Assert.Throws<Autodesk.Revit.Exceptions.ArgumentException>(() => doc.SaveFamilyToFolder((Family)noneditableFamily, existingTempFolder));
            var fileExistInNonExistingFolder = File.Exists(Path.Combine(nonExistingTempFolder, savedFamilyName));
            var fileExistInExistingFolder = File.Exists(Path.Combine(existingTempFolder, savedFamilyName));

            // Assert
            Assert.AreEqual(expectedSavedFilePathNonExistingFolder, resultSavedFamilyInNonExistingFolder);
            Assert.AreEqual(expectedSavedFilePathExistingFolder, resultSavedFamilyInExistingFolder);
            Assert.IsTrue(fileExistInNonExistingFolder);
            Assert.IsTrue(fileExistInExistingFolder);

            // Clean up
            Directory.Delete(nonExistingTempFolder, true);
            Directory.Delete(existingTempFolder, true);
        }

        [Test]
        [TestModel(@".\Document\DocumentPurgeUnusedTest.rvt")]
        public void CanRecursivelyPurgeUnusedElementsFromDocument()
        {
            // Arrange
            var expectedPurgedElementIds = new List<int>() { 217063, 221347, 216753, 416, 208080, 210695 };
            string expectedPurgeMessageSecondRun = Revit.Properties.Resources.NoElementsToPurge;
            
            // Act - second purge should throw exception as there is nothing left to purge after doing PurgeUnused(true).
            var document = Document.Current;
            var resultFirstRun = document.PurgeUnused(true);
            var resultSecondRun = Assert.Throws<System.InvalidOperationException>(() => document.PurgeUnused(true));

            // Assert
            Assert.AreEqual(expectedPurgedElementIds, resultFirstRun);
            Assert.AreEqual(expectedPurgeMessageSecondRun, resultSecondRun.Message);
        }

        [Test]
        [TestModel(@".\Document\DocumentPurgeUnusedTest.rvt")]
        public void CanPurgeUnusedElementsFromDocument()
        {
            // Arrange
            var expectedPurgedElementIdsFirstRun = new List<int>() { 217063, 221347 };
            var expectedPurgedElementIdsSecondRun = new List<int>() { 216753, 416, 208080, 210695 };

            // Act - as we are not running recursivly, second run should return element ids
            var document = Document.Current;
            var resultFirstRun = document.PurgeUnused();
            var resultSecondRun = document.PurgeUnused();

            // Assert
            Assert.AreEqual(expectedPurgedElementIdsFirstRun, resultFirstRun);
            Assert.AreEqual(expectedPurgedElementIdsSecondRun, resultSecondRun);
        }

    }
}
