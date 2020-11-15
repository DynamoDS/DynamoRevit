using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class CeilingTypeTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.00001;

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_ValidArgs()
        {
            var ceilingTypeName = "Generic - 12\"";
            var ceilingType = CeilingType.ByName(ceilingTypeName);
            Assert.NotNull(ceilingType);
            Assert.AreEqual(ceilingTypeName, ceilingType.Name);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => CeilingType.ByName(null));
        }

        [Test]
        [TestModel(@".\CeilingType\CeilingType.rvt")]
        public void CanGetCeilingTypeThermalProperties()
        {
            // Arrange
            var ceiling = ElementSelector.ByElementId(357404, true);
            double expectedCeilingTypeAbsorptance = 0.1;
            double expectedCeilingTypeHeatTransferCoefficient = 0.047502;
            int expectedCeilingTypeRoughness = 1;
            double expectedCeilingTypeThermalMass = 0.723;
            double expectedCeilingTypeThermalResistance = 21.051724;

            // Act
            var ceilingType = ceiling.ElementType as CeilingType;
            var thermalProperties = ceilingType.GetThermalProperties();
            double resultCeilingTypeAbsorptance = (double)thermalProperties["Absorptance"];
            double resultCeilingTypeHeatTransferCoefficient = (double)thermalProperties["HeatTransferCoefficient"];
            int resultCeilingTypeRoughness = (int)thermalProperties["Roughness"];
            double resultCeilingTypeThermalMass = (double)thermalProperties["ThermalMass"];
            double resultCeilingTypeThermalResistance = (double)thermalProperties["ThermalResistance"];

            // Assert
            Assert.AreEqual(expectedCeilingTypeAbsorptance, resultCeilingTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedCeilingTypeHeatTransferCoefficient, resultCeilingTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedCeilingTypeRoughness, resultCeilingTypeRoughness);
            Assert.AreEqual(expectedCeilingTypeThermalMass, resultCeilingTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedCeilingTypeThermalResistance, resultCeilingTypeThermalResistance, Tolerance);
        }
    }
}
