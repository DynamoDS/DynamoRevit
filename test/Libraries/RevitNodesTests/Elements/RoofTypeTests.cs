using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class RoofTypeTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.00001;

        [Test]
        [TestModel(@".\RoofType\RoofType.rvt")]
        public void CanGetRoofTypeThermalProperties()
        {
            // Arrange
            var roof = ElementSelector.ByElementId(316136, true);
            double expectedRoofTypeAbsorptance = 0.7;
            double expectedRoofTypeHeatTransferCoefficient = 0.15892777;
            int expectedRoofTypeRoughness = 3;
            double expectedRoofTypeThermalMass = 1056333.2;
            double expectedRoofTypeThermalResistance = 6.29216624;

            // Act
            var roofType = roof.ElementType as RoofType;
            var thermalProperties = roofType.GetThermalProperties();
            double resultRoofTypeAbsorptance = (double)thermalProperties["Absorptance"];
            double resultRoofTypeHeatTransferCoefficient = (double)thermalProperties["HeatTransferCoefficient"];
            int resultRoofTypeRoughness = (int)thermalProperties["Roughness"];
            double resultRoofTypeThermalMass = (double)thermalProperties["ThermalMass"];
            double resultRoofTypeThermalResistance = (double)thermalProperties["ThermalResistance"];

            // Assert
            Assert.AreEqual(expectedRoofTypeAbsorptance, resultRoofTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedRoofTypeHeatTransferCoefficient, resultRoofTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedRoofTypeRoughness, resultRoofTypeRoughness);
            Assert.AreEqual(expectedRoofTypeThermalMass, resultRoofTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedRoofTypeThermalResistance, resultRoofTypeThermalResistance, Tolerance);
        }
    }
}
