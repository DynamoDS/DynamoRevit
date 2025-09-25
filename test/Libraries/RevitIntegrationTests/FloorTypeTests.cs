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
using Dynamo.Graph.Nodes;
using System.Collections.Generic;
using Dynamo.Tests;

namespace RevitSystemTests
{
    [TestFixture]
    public class FloorTypeTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanGetFloorTypeThermalProperties()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FloorType\canGetFloorTypeThermalProperties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            double expectedFloorTypeAbsorptance = 0.7;
            double expectedFloorTypeHeatTransferCoefficient = 0.117437;
            int expectedFloorTypeRoughness = 3;
            double expectedFloorTypeThermalMass = 365549.43;
            double expectedFloorTypeThermalResistance = 8.515159;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var resultFloorTypeAbsorptance = GetPreviewValue("56cb939de2cc4ca4a21731ba78f4299b");
            var resultFloorTypeHeatTransferCoefficient = GetPreviewValue("6d63251353044032b5167428b7749f62");
            var resultFloorTypeRoughness = GetPreviewValue("9cd0f885729641dc928f93250bae096a");
            var resultFloorTypeThermalMass = GetPreviewValue("e60b5201ae23449196beb090d0d825d1");
            var resultFloorTypeThermalResistance = GetPreviewValue("1ca68872f565419b838cdbb8306057d3");

            // Assert
            Assert.AreEqual(expectedFloorTypeAbsorptance, (double)resultFloorTypeAbsorptance, Tolerance);
            Assert.AreEqual(expectedFloorTypeHeatTransferCoefficient, (double)resultFloorTypeHeatTransferCoefficient, Tolerance);
            Assert.AreEqual(expectedFloorTypeRoughness, resultFloorTypeRoughness);
            Assert.AreEqual(expectedFloorTypeThermalMass, (double)resultFloorTypeThermalMass, Tolerance);
            Assert.AreEqual(expectedFloorTypeThermalResistance, (double)resultFloorTypeThermalResistance, Tolerance);
        }

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanGetFloorTypeStructuralMaterial()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FloorType\canGetFloorTypeStructuralMaterial.dyn");
            string testPath = Path.GetFullPath(samplePath);

            int expectedMaterialIdOnFloor = 45453;

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var structuralMaterailId = GetPreviewValue("170186573b144adc88acbeba69b47f9c");

            // Assert
            Assert.AreEqual(expectedMaterialIdOnFloor, structuralMaterailId);
        }

        [Test]
        [TestModel(@".\FloorType\FloorType.rvt")]
        public void CanCheckIfFloorTypeIsFoundationSlab()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\FloorType\CanCheckIfFloorTypeIsFoundationSlab.dyn");
            string testPath = Path.GetFullPath(samplePath);

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var isFoundationSlab = GetPreviewValue("4b5c27789bab4c43a6864e33786a2847");

            // Assert
            Assert.AreEqual(false, isFoundationSlab);

            //Change the element selected in the node
            var selectElementNodeID = "5576b80a36a74d7d8c318465d38b0f7b";
            int newElementSelected = 316157;

            // Change the selected element in the SelectModelElement node
            var selectElementNode = ViewModel.Model.CurrentWorkspace.Nodes
                .FirstOrDefault(n => n.GUID.ToString("N") == selectElementNodeID) as DSModelElementSelection;
            if (selectElementNode != null)
            {
                var document = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument;
                selectElementNode.UpdateSelection(new List<Autodesk.Revit.DB.Element> { document.GetElement(new ElementId(newElementSelected)) });
            }

            // Re-run the model to update outputs
            RunCurrentModel();

            // Verify the new selection is a slab
            var isFoundationSlabAfterChange = GetPreviewValue("4b5c27789bab4c43a6864e33786a2847");
            Assert.AreEqual(true, isFoundationSlabAfterChange);
        }


        [Test]
        [TestModel(@".\DifferentTypeRooms.rvt")]
        public void FloorNodes()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\FloorNodes.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var floorTypeName = GetPreviewValue("810634a9a60f41a2ab24833354eb6f54");
            Assert.AreEqual("Standard", floorTypeName);

            var floorTypeByName = GetPreviewValue("a450f0ac27ea4802902459aa1e2b374b");
            Assert.AreEqual("FloorTest", floorTypeByName.ToString());

            var categoryName = GetPreviewValue("de31ecaf910a407bb9d94fd8fe9c67a2");
            Assert.AreEqual("Floors", categoryName);

            var floorPlanView = GetPreviewValue("8fbbb68682a2473e9289ee30c476cfe6");
            var expectedValue = "FloorPlanView(Name = Level 0(1) )";
            Assert.AreEqual(expectedValue, floorPlanView.ToString());
        }


    }
}
