using System.IO;
using System.Linq;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using Dynamo.Models;
using RTF.Framework;
using Autodesk.DesignScript.Geometry;
using RevitServices.Persistence;
using System.Collections.Generic;
using RevitServices.Transactions;

namespace RevitSystemTests
{
    [TestFixture]
    public class WorkflowTests : RevitSystemTestBase
    {

        [Test]
        [TestModel(@".\empty.rvt")]
        public void Wall()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Wall.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            var model = ViewModel.Model;
            RunCurrentModel();
            Assert.AreEqual(9, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(9, model.CurrentWorkspace.Connectors.Count());

            // Check for Wall
            var wall = "1367ca93-ad6b-459f-a264-61ebe1eb5edd";
            AssertPreviewCount(wall, 4);

            // get all Walls.
            for (int i = 0; i <= 3; i++)
            {
                var allwalls = GetPreviewValueAtIndex(wall, i) as Wall;
                Assert.IsNotNull(allwalls);
            }
            var wallFromRevit = GetAllWalls();
            Assert.AreEqual(4, wallFromRevit.Count);
        }


        [Test]
        [TestModel(@".\empty.rvt")]
        public void Test_Truss()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoBasic\01 Truss.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(25, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(33, model.CurrentWorkspace.Connectors.Count());

            // Check StructuralFraming.BeamByCurve
            var beam = "bcef7f1f-fff5-48bc-8e78-7302e7344be8";
            AssertPreviewCount(beam, 23);

            // get all StructuralFraming.BeamByCurve.
            for (int i = 0; i < 23; i++)
            {
                var allbeam = GetPreviewValueAtIndex(beam, i) as StructuralFraming;
                Assert.IsNotNull(allbeam);
            }

            // Check ModelCurve.ByCurve
            var modelCurve = "8bf46c38-4123-42b6-90ce-35788bf449ce";
            AssertPreviewCount(modelCurve, 23);

            // get all ModelCurve.ByCurve
            for (int i = 0; i < 23; i++)
            {
                var allstru = GetPreviewValueAtIndex(modelCurve, i) as ModelCurve;
                Assert.IsNotNull(allstru);
            }
        }


        [Test]
        [TestModel(@".\Workflow\curve.rvt")]
        public void Test_StructureFrame()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\01_StructuralFraming.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(12, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(12, model.CurrentWorkspace.Connectors.Count());

            //check structuralFraming.BraceByCurve
            var structuralFraming = "8d0a5991-259c-438d-982a-c9cc2e8b5e79";
            AssertPreviewCount(structuralFraming, 9);

            //check elements in structuralFraming.BraceByCurve ar not null
            for (int i = 0; i < 9; i++)
            {
                var element = GetPreviewValueAtIndex(structuralFraming, i) as StructuralFraming;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Test_AdaptiveComponent1()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\02_Adaptive Component Placement 1.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(11, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(10, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoint
            var AC = "357e7a53-361c-4c1e-81ae-83e16213a39a";
            AssertPreviewCount(AC, 9);

            //check elements in AdaptiveComponent.ByPoint are not null
            for (int i = 0; i < 9; i++)
            {
                var element = GetPreviewValueAtIndex(AC, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Test_AdaptiveComponent2()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\03_Adaptive Component Placement 2.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(17, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(19, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoint
            var AC = "357e7a53-361c-4c1e-81ae-83e16213a39a";
            AssertPreviewCount(AC, 9);

            //check elements in AdaptiveComponent.ByPoint are not null
            for (int i = 0; i < 9; i++)
            {
                var element = GetPreviewValueAtIndex(AC, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Test_ImportSolid()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\04_ImportSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(16, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(16, model.CurrentWorkspace.Connectors.Count());

            //check elements in ImportInstance.ByGeometries are not null
            Assert.IsNotNull(GetPreviewValue("e3fedc00-247a-4971-901c-7fcb063344c6"));
        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Test_PlaceFamiliesByLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\05_PlaceFamiliesByLevel_Set Parameters.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(14, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(17, model.CurrentWorkspace.Connectors.Count());

            //check Element.SetParameterByName
            var ele = "026aadc9-644e-4e6c-b35c-bf1aec67045c";
            AssertPreviewCount(ele, 21);
            for (int i = 0; i < 21; i++)
            {
                var element = GetPreviewValueAtIndex(ele, i) as Element;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\RevitProject\tower.rvt")]
        public void Test_EllipseTower()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\RevitProject\01 Ellipse Tower v1.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(39, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(47, model.CurrentWorkspace.Connectors.Count());

            //check Floor.ByOutLineTypeAndLevel
            var floor = "1bcce36c-7ea3-4c70-9271-544fd378ec41";
            AssertPreviewCount(floor, 14);

            //check Element.OverrideColorInView
            var ele = "d986daac-eae1-4e80-9430-44527fcb133e";
            AssertPreviewCount(ele, 126);
        }
    }
}

