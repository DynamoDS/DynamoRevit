using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{

    [TestFixture]
    public class WallTypeTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_ValidArgs()
        {
            var wallTypeName = "Curtain Wall 1";
            var wallType = WallType.ByName(wallTypeName);
            Assert.NotNull(wallType);
            Assert.AreEqual(wallTypeName, wallType.Name);
        }

        [Test]
        public void ByName_NullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => WallType.ByName(null));
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetWallTypeProperties()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(184176, true);
            var expectedWallTypeName = "Generic - 8\"";
            var expectedWallTypeWidth = 0.666;
            var expectedWallTypeKind = "Basic";
            var expectedWallTypeFunction = "Exterior";

            // Act
            var wallType = wall.ElementType as WallType;
            var resultWallTypeName = wallType.Name;
            var resultWallTypeWidth = wallType.Width;
            var resultWallTypeKind = wallType.Kind;
            var resultWallTypeFunction = wallType.Function;

            // Assert
            Assert.AreEqual(expectedWallTypeName, resultWallTypeName);
            Assert.AreEqual(expectedWallTypeWidth, resultWallTypeWidth, Tolerance);
            Assert.AreEqual(expectedWallTypeKind, resultWallTypeKind);
            Assert.AreEqual(expectedWallTypeFunction, resultWallTypeFunction);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanGetWallTypeThermalProperties()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(184176, true);
            double expectedWallTypeAbsorptance = 0.7;
            double expectedWallTypeHeatTransferCoefficient = 1E+30;
            int expectedWallTypeRoughness = 3;
            double expectedWallTypeThermalMass = 0;
            double expectedWallTypeThermalResistance = 0;

            // Act
            var wallType = wall.ElementType as WallType;
            var thermalProperties = wallType.GetThermalProperties();
            double resultWallTypeAbsorptance = (double)thermalProperties["Absorptance"];
            double resultWallTypeHeatTransferCoefficient = (double)thermalProperties["HeatTransferCoefficient"];
            int resultWallTypeRoughness = (int)thermalProperties["Roughness"];
            double resultWallTypeThermalMass = (double)thermalProperties["ThermalMass"];
            double resultWallTypeThermalResistance = (double)thermalProperties["ThermalResistance"];

            // Assert
            Assert.AreEqual(expectedWallTypeAbsorptance, resultWallTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedWallTypeHeatTransferCoefficient, resultWallTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedWallTypeRoughness, resultWallTypeRoughness);
            Assert.AreEqual(expectedWallTypeThermalMass, resultWallTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedWallTypeThermalResistance, resultWallTypeThermalResistance, Tolerance);
        }
    }

}

