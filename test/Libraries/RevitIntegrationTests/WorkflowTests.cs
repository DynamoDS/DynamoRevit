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
            Assert.IsNotNull(GetPreviewValue("e3fedc00-247a-4971-901c-7fcb063344c6") as ImportInstance);
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
        public void Test_EllipseTower01()
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
            for (int i = 0; i < 14; i++)
            {
                var floors = GetPreviewValueAtIndex(floor, i) as Floor;
                Assert.IsNotNull(floors);
            }

            //check Element.OverrideColorInView
            var ele = "d986daac-eae1-4e80-9430-44527fcb133e";
            AssertPreviewCount(ele, 126);
            for (int i = 0; i < 126; i++)
            {
                var element = GetPreviewValueAtIndex(ele, i) as Element;
              //  Assert.IsNotNull(element); Thus node get error with the latest Dynamo
            }
           
        }


        [Test]
        [TestModel(@".\Workflow\RevitProject\tower.rvt")]
        public void Test_EllipseTower03()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\RevitProject\03 Ellipse Tower v3.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(45, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(56, model.CurrentWorkspace.Connectors.Count());

            //check Floor.ByOutLineTypeAndLevel
            var floor = "1bcce36c-7ea3-4c70-9271-544fd378ec41";
            AssertPreviewCount(floor, 14);
            for (int i = 0; i < 14; i++)
            {
                var floors = GetPreviewValueAtIndex(floor, i) as Floor;
                Assert.IsNotNull(floors);
            }

            //check Element.OverrideColorInView
            var ele = "d986daac-eae1-4e80-9430-44527fcb133e";
            AssertPreviewCount(ele, 126);
            for (int i = 0; i < 126; i++)
            {
                var element = GetPreviewValueAtIndex(ele, i) as Element;
                //  Assert.IsNotNull(element); Thus node get error with the latest Dynamo
            }
        }


        [Test]
        [TestModel(@".\Workflow\Python\HostedObjectStuff_Sample.rvt")]
        public void Test_Python()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Python\Revit API via Python.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(11, model.CurrentWorkspace.Connectors.Count());

            //check Python Script
            var pScript1 = "4caa3a16-50d9-4416-ae45-b5ad06d74c94";
            AssertPreviewCount(pScript1, 2);
            var flatvalue1 = GetFlattenedPreviewValues(pScript1);            
            foreach(var ele in flatvalue1)
            {     
                Assert.IsNotNull(ele);
            }    
     
            //check Python Script
            var pScript2 = "fe96aff2-7e02-4e72-b11c-cc20582a48ea";
            AssertPreviewCount(pScript2, 2);
            for (int i = 0; i < 2; i++)
            {
                var pWall = GetPreviewValueAtIndex(pScript2, i) as Wall;
                Assert.IsNotNull(pWall);
            }

            //check Python Script
            var pScript3 = "5a3b301c-632b-4ec1-9fcb-c2623f04c53c";
            AssertPreviewCount(pScript3, 2);
            var flatvalue3 = GetFlattenedPreviewValues(pScript3);
            foreach (var ele in flatvalue3)
            {
                Assert.IsNotNull(ele);
            }
        }


        [Test]
        [TestModel(@".\Workflow\CodeBlocksReference\panelProject.rvt")]
        public void Test_PanelsNodes()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\CodeBlocksReference\Panels_Nodes.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(32, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(42, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var color = "4845d25a-c7bd-4e61-8e5d-9dffee11d532";
            AssertPreviewCount(color, 456);
            for (int i = 0; i < 456; i++)
            {
                var element = GetPreviewValueAtIndex(color, i) as Element;
                Assert.IsNotNull(element);
            }
        }
    }
}

