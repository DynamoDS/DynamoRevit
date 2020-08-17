﻿using System.IO;
using System.Linq;
using CoreNodeModels.Input;
using Dynamo.Nodes;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using Revit.Elements;


namespace RevitSystemTests
{
    [TestFixture]
    class ViewTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\View\AxonometricView.rfa")]
        public void AxonometricView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\AxonometricView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\View\OverrideElementColorInView.rvt")]
        public void OverrideElementColorInView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\OverrideElementColorInView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(10, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(16, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var elementID = "99608c4e-c064-4486-a016-7221a5df2e3a";
            AssertPreviewCount(elementID, 100);
            for (int i = 0; i < 100; i++)
            {
                var element = GetPreviewValueAtIndex(elementID, i) as Element;
                Assert.IsNotNull(element);
            }
        }

        [Test]
        [TestModel(@".\View\PerspectiveView.rfa")]
        public void PerspectiveView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\PerspectiveView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test, TestModel(@".\Empty.rvt")]
        public void ExportAsImage()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\ExportImage.dyn");
            string testPath = Path.GetFullPath(samplePath);

            OpenDynamoDefinition(testPath);

            AssertNoDummyNodes();

            // Find the CBN and change it to have two temporary paths.
            var stringNodes = Model.CurrentWorkspace.Nodes.Where(x => x is StringInput).Cast<StringInput>().ToList();
            Assert.AreEqual(stringNodes.Count, 2);

            var tmp1 = Path.GetTempFileName();
            var tmp2 = Path.GetTempFileName();

            tmp1 = Path.ChangeExtension(tmp1, ".png");
            tmp2 = Path.ChangeExtension(tmp1, ".png");

            stringNodes[0].Value = tmp1;
            stringNodes[1].Value = tmp2;


            RunCurrentModel();


            // Ensure that our two temporary files have some data
            var tmp1Info = new FileInfo(tmp1);
            Assert.Greater(tmp1Info.Length, 0);

            var tmp2Info = new FileInfo(tmp2);
            Assert.Greater(tmp2Info.Length, 0);

        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void DuplicateView()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\DuplicateView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string viewName = "TestEast";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var view = GetPreviewValue("615443943b9b492986c7ad29f5bb5358") as Revit.Elements.Views.View;
            Assert.AreEqual(viewName, view.Name);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CropBoxofView()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CropBoxofView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var CropBox = GetPreviewValue("8695703491164e5bb007157631b13638") as Autodesk.DesignScript.Geometry.BoundingBox;
            Assert.IsNotNull(CropBox);

            var IsActive = (bool)GetPreviewValue("9973abd4f58f4467bdcd136b03a46fd4");
            Assert.IsTrue(IsActive);

            var IsVisible = (bool)GetPreviewValue("4a6ebc027a5c401aba16541a3ef668db");
            Assert.IsTrue(IsVisible);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetView3dSpaceProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetView3dSpaceProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            int expectedScale = 100;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var Origin = GetPreviewValue("d6feec0d3c5247af874496eca0a60015") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(Origin);

            var Outline = GetPreviewValue("3346ceebd2ed460abb5a2d5f9891aac1") as Autodesk.DesignScript.Geometry.BoundingBox;
            Assert.IsNotNull(Outline);

            var RightDirection = GetPreviewValue("169e149c912143a0b211a6060745e4df") as Autodesk.DesignScript.Geometry.Vector;
            Assert.IsNotNull(RightDirection);

            var ViewDirection = GetPreviewValue("593868bf4d5b4bd3bbe53cdf25340198") as Autodesk.DesignScript.Geometry.Vector;
            Assert.IsNotNull(ViewDirection);

            var scale = GetPreviewValue("15c8e8f6bffb46bfad5a65900114dc5f");
            Assert.AreEqual(expectedScale, scale);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSetDiscipline()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetSetDiscipline.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var discipline = GetPreviewValue("60c8ce7365214955967367cf21b85762");
            Assert.AreEqual("Structural", discipline);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSetDisplayStyle()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetSetDisplayStyle.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var displayStyle = GetPreviewValue("269cd16b16ee4b80a8fe6195374c884a");
            Assert.AreEqual("Shading", displayStyle);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSetSketchPlane()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetSetSketchPlane.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var sketchPlane = GetPreviewValue("bc12df8c2efc44d19a208a83b16e1709") as SketchPlane;
            var plane = sketchPlane.Plane;
            Assert.AreEqual(0.000, plane.Origin.X, Tolerance);
            Assert.AreEqual(0.000, plane.Origin.Y, Tolerance);
            Assert.AreEqual(2.000, plane.Origin.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void CanGetSetPartsVisibility()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetSetPartsVisibility.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var partsVisibility = GetPreviewValue("22843a82efe942b2b89437e4a115e69a");
            Assert.AreEqual("ShowPartsAndOriginal", partsVisibility);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetSetCategoryOverridesAndHidden()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\CanGetSetCategoryOverridesAndHidden.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var categoryOverrides = GetPreviewValue("66dc72c413124c57a8416d0f97bca1de") as Revit.Filter.OverrideGraphicSettings;
            Assert.IsNotNull(categoryOverrides);

            Assert.IsTrue((bool)GetPreviewValue("c07c8e0e57ab42769fa86b8df7b6056c"));
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void HideElementTemporary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\HideElementTemporary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var view = GetPreviewValue("1fe4f90c6cc044bd91bee7a61258f83b") as Revit.Elements.Views.View;
            Assert.IsNotNull(view);
            
            Assert.IsFalse((bool)GetPreviewValue("0c1c543858f343b89d659f2b537b66dc"));
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void HideCategoryTemporary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\HideCategoryTemporary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var view = GetPreviewValue("b1da33f02c9649bc958299b723222a2c") as Revit.Elements.Views.View;
            Assert.IsNotNull(view);

            Assert.IsFalse((bool)GetPreviewValue("3bd57778cfb14469bbacd88a81b59341"));
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void IsolateCategoriesTemporary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\IsolateCategoriesTemporary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var view = GetPreviewValue("4cf1acfe08fa406b9fa6977185ef07ac") as Revit.Elements.Views.View;
            Assert.IsNotNull(view);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void IsolateElementsTemporary()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\View\IsolateElementsTemporary.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // Assert
            var view = GetPreviewValue("f516cb1302f549e3b5eb62b80a7688a7") as Revit.Elements.Views.View;
            Assert.IsNotNull(view);
        }
    }
}