using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class CeilingTypeTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.00001;
        [Test]
        [TestModel(@".\CeilingType\CeilingType.rvt")]
        public void CanGetCeilingTypeThermalProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\CeilingType\CanGetCeilingTypeThermal.dyn");
            string testPath = Path.GetFullPath(samplePath);
            double expectedCeilingTypeAbsorptance = 0.1;
            double expectedCeilingTypeHeatTransferCoefficient = 0.269729;
            int expectedCeilingTypeRoughness = 1;
            double expectedCeilingTypeThermalMass = 14779.376715;
            double expectedCeilingTypeThermalResistance = 3.707423;

            // Act
            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //var thermalProperties = GetPreviewCollection("c6fe2793b6b440738e49bcffd21d8913");
            double resultCeilingTypeAbsorptance = (double)GetPreviewValue("1c4a77ba20dc48f689f4935a93d0550e");
            double resultCeilingTypeHeatTransferCoefficient = (double)GetPreviewValue("ba2eef6c8d4c4b99ae7669acf29a22b4");
            var resultCeilingTypeRoughness = GetPreviewValue("8b2f11cc1c9c469d9f1ccfd6e732ceb0");
            double resultCeilingTypeThermalMass = (double)GetPreviewValue("106b74b022304826b587879ce515f547");
            double resultCeilingTypeThermalResistance = (double)GetPreviewValue("7eeea37764ad40bc94a9fdda0ceca0eb");

            // Assert
            Assert.AreEqual(expectedCeilingTypeAbsorptance, resultCeilingTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedCeilingTypeHeatTransferCoefficient, resultCeilingTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedCeilingTypeRoughness, resultCeilingTypeRoughness);
            Assert.AreEqual(expectedCeilingTypeThermalMass, resultCeilingTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedCeilingTypeThermalResistance, resultCeilingTypeThermalResistance, Tolerance);
        }
    }
}
