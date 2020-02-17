using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{

    [TestFixture]
    public class FloorTypeTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.00001;

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_ValidArgs()
        {
            var floorTypeName = "Generic - 12\"";
            var floorType = FloorType.ByName(floorTypeName);
            Assert.NotNull(floorType);
            Assert.AreEqual(floorTypeName, floorType.Name);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => FloorType.ByName(null));
        }

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanGetFloorTypeThermalProperties()
        {
            // Arrange
            var floor = ElementSelector.ByElementId(316138, true);
            double expectedFloorTypeAbsorptance = 0.7;
            double expectedFloorTypeHeatTransferCoefficient = 0.117437;
            int expectedFloorTypeRoughness = 3;
            double expectedFloorTypeThermalMass = 365549.43;
            double expectedFloorTypeThermalResistance = 8.515159;

            // Act
            var floorType = floor.ElementType as FloorType;
            var thermalProperties = floorType.GetThermalProperties();
            double resultFloorTypeAbsorptance = (double)thermalProperties["Absorptance"];
            double resultFloorTypeHeatTransferCoefficient = (double)thermalProperties["HeatTransferCoefficient"];
            int resultFloorTypeRoughness = (int)thermalProperties["Roughness"];
            double resultFloorTypeThermalMass = (double)thermalProperties["ThermalMass"];
            double resultFloorTypeThermalResistance = (double)thermalProperties["ThermalResistance"];

            // Assert
            Assert.AreEqual(expectedFloorTypeAbsorptance, resultFloorTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedFloorTypeHeatTransferCoefficient, resultFloorTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedFloorTypeRoughness, resultFloorTypeRoughness);
            Assert.AreEqual(expectedFloorTypeThermalMass, resultFloorTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedFloorTypeThermalResistance, resultFloorTypeThermalResistance, Tolerance);
        }

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanGetFloorTypeStructuralMaterial()
        {
            // Arrange
            var floor = ElementSelector.ByElementId(316138, true);
            var foundationSlab = ElementSelector.ByElementId(316157, true);

            int expectedMaterialIdOnFloor = 45453;
            string expectedErrorMessageOnFoundationSlab = Revit.Properties.Resources.NoStructuralMaterialAssigned;

            // Act
            var floorType = floor.ElementType as FloorType;
            var foundationType = foundationSlab.ElementType as FloorType;
            var structuralMaterial = floorType.GetStructuralMaterial();
            int structuralMaterailId = structuralMaterial.Id;
            var foundationSlabException = Assert.Throws<System.InvalidOperationException>(() => foundationType.GetStructuralMaterial());

            // Assert
            Assert.AreEqual(expectedMaterialIdOnFloor, structuralMaterailId);
            Assert.AreEqual(expectedErrorMessageOnFoundationSlab, foundationSlabException.Message);
        }

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanCheckIfFloorTypeIsFoundationSlab()
        {
            // Arrange
            var floor = ElementSelector.ByElementId(316138, true);
            var foundationSlab = ElementSelector.ByElementId(316157, true);

            // Act
            var floorType = floor.ElementType as FloorType;
            var foundationType = foundationSlab.ElementType as FloorType;
 

            // Assert
            Assert.AreEqual(false, floorType.IsFoundationSlab);
            Assert.AreEqual(true, foundationType.IsFoundationSlab);
        }
    }
}

