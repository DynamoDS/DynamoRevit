using System.IO;
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
    }
}