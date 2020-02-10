using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class WallTypeTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\element.rvt")]
        public void canGetWallTypeProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Wall\canGetWallTypeProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedWallTypeName = "Generic - 8\"";
            var expectedWallTypeWidth = 0.666;
            var expectedWallTypeKind = "Basic";
            var expectedWallTypeFunction = "Exterior";

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var resultWallTypeName = GetPreviewValue("531d75472d3d462fa93724bbe74058fe");
            var resultWallTypeWidth = GetPreviewValue("6af41d911db04e69a679e8b643e03420");
            var resultWallTypeKind = GetPreviewValue("a46593138725448bb5908c632781f159");
            var resultWallTypeFunction = GetPreviewValue("fcd8ba943add4f318de76c88200c8b7a");

            // Assert
            Assert.AreEqual(expectedWallTypeName, resultWallTypeName);
            Assert.AreEqual(expectedWallTypeWidth, (double)resultWallTypeWidth, Tolerance);
            Assert.AreEqual(expectedWallTypeKind, resultWallTypeKind);
            Assert.AreEqual(expectedWallTypeFunction, resultWallTypeFunction);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetWallTypeThermalProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Wall\canGetWallTypeThermalProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double expectedWallTypeAbsorptance = 0.7;
            double expectedWallTypeHeatTransferCoefficient = 1E+30;
            int expectedWallTypeRoughness = 3;
            double expectedWallTypeThermalMass = 0;
            double expectedWallTypeThermalResistance = 0;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var resultWallTypeAbsorptance = GetPreviewValue("56cb939de2cc4ca4a21731ba78f4299b");
            var resultWallTypeHeatTransferCoefficient = GetPreviewValue("6d63251353044032b5167428b7749f62");
            var resultWallTypeRoughness = GetPreviewValue("9cd0f885729641dc928f93250bae096a");
            var resultWallTypeThermalMass = GetPreviewValue("e60b5201ae23449196beb090d0d825d1");
            var resultWallTypeThermalResistance = GetPreviewValue("1ca68872f565419b838cdbb8306057d3");

            // Assert
            Assert.AreEqual(expectedWallTypeAbsorptance, (double)resultWallTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedWallTypeHeatTransferCoefficient, (double)resultWallTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedWallTypeRoughness, resultWallTypeRoughness);
            Assert.AreEqual(expectedWallTypeThermalMass, (double)resultWallTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedWallTypeThermalResistance, (double)resultWallTypeThermalResistance, Tolerance);
        }
    }
}
