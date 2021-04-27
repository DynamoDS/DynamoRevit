using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;
using Dynamo.Tests;

namespace RevitSystemTests
{
    [TestFixture]
    public class ViewportTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanCreateViewportOnSheet()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanCreateViewportOnSheet.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedViewportName = "Title w Line";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var viewportName = GetPreviewValue("e01a83242a3d4a2cb02789c3c90bb1a1");

            // Assert
            Assert.AreEqual(expectedViewportName, viewportName);
        }


        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanGetSetViewportBoxCenter()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanGetSetViewportBoxCenter.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            double x = 250.00;
            double y = 500.00;

            var BoxCenterPoint = GetPreviewValue("0f460e212a8b40f2b7edcfee66a8db6e") as Point;

            Assert.AreEqual(x, BoxCenterPoint.X, Tolerance);
            Assert.AreEqual(y, BoxCenterPoint.Y, Tolerance);
        }

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanResetLocationWhenCreate()
        {
            var model = ViewModel.Model;
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanResetLocationWhenCreate.dyn");
            string testPath = Path.GetFullPath(samplePath);

            string expectedViewportName = "Title w Line";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            IntegerSlider64Bit slider = model.CurrentWorkspace.NodeFromWorkspace
                ("d443e82fd58143f5a30fd35f41ca0426") as IntegerSlider64Bit;
            slider.Value = 100;

            RunCurrentModel();

            var viewportName = GetPreviewValue("e01a83242a3d4a2cb02789c3c90bb1a1");

            // Assert
            Assert.AreEqual(expectedViewportName, viewportName);
        }

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanGetViewportProperties()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanGetViewportProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            double x = 250.00;
            double y = 250.00;

            var BoxCenterPoint = GetPreviewValue("07fd4f5267904e51a2041a34d9406398") as Point;

            Assert.AreEqual(x, BoxCenterPoint.X, Tolerance);
            Assert.AreEqual(y, BoxCenterPoint.Y, Tolerance);


            var BoxOutline = GetPreviewValue("fadd22b09d164db19547b18dc955b25e") as BoundingBox;
            var LabelOutline = GetPreviewValue("d6bac18f3a7e4f249f07722a259b4105") as BoundingBox;

            Assert.IsNotNull(BoxOutline);
            Assert.IsNotNull(LabelOutline);

            string sheetName = "Unnamed";
            string viewName = "{3D}";

            var sheet = GetPreviewValue("c5d386d2b2ca4fddaf0e4de3eca8e98e") as Revit.Elements.Views.Sheet;
            var view = GetPreviewValue("493993d2bd5a419ca32e0e2722662e85") as Revit.Elements.Views.View;

            Assert.AreEqual(sheetName, sheet.Name);
            Assert.AreEqual(viewName, view.Name);
        }

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanGetSetViewportLabelOffset()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanGetSetViewportLabelOffset.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            double x = 23.00;
            double y = -10.00;

            var LabelOffsetPoint = GetPreviewValue("c508619ef62747e6a7eb764ffe8c74a7") as Point;

            Assert.AreEqual(x, LabelOffsetPoint.X, Tolerance);
            Assert.AreEqual(y, LabelOffsetPoint.Y, Tolerance);
        }

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanGetSetViewportLabelLineLength()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Viewport\CanGetSetViewportLabelLineLength.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            double length = 1.00;

            var lineLength = double.Parse(GetPreviewValue("f939542380fd4b9d8bb1091c76ce3004").ToString());

            Assert.AreEqual(length, lineLength, Tolerance);
        }
    }
}
