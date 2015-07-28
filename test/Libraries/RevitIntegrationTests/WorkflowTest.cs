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
    public class WorkflowTest : RevitSystemTestBase
    {

        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-02.rvt")]
        public void Test_Vignette_02_Adaptive_Family()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-02-Adaptive-Family.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(15, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoints
            var adaptiveComp = "700fd421-636c-4cd9-8604-36f027f045ee";
            var adaptiveCompVlaue = GetPreviewValue(adaptiveComp) as AdaptiveComponent;
            Assert.IsNotNull(adaptiveComp);

            //check Flatten
            var flattenId = "14719205-f9d8-40a0-9a04-2760c5c816be";
            AssertPreviewCount(flattenId, 8);
            for (int i = 0; i < 8; i++)
            {
                var element = GetPreviewValueAtIndex(flattenId, i) as Point;
                Assert.AreEqual(System.Math.Abs(element.X), 50);
                Assert.AreEqual(System.Math.Abs(element.Y), 100);
                if (i >= 4)
                {
                    Assert.AreEqual(System.Math.Abs(element.Z), 80);
                }
                else
                {
                    Assert.AreEqual(System.Math.Abs(element.Z), 0);
                }
            }
        }


        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-02.rvt")]
        public void Test_Vignette_02_Adaptive_Family_Mult()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-02-Adaptive-Families-Mult.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(23, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(27, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoints
            var adaptiveComp = "700fd421-636c-4cd9-8604-36f027f045ee";
            AssertPreviewCount(adaptiveComp, 9);
            for (int i = 0; i < 9; i++)
            {
                var element = GetPreviewValueAtIndex(adaptiveComp, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\Lists and Structural Framing\Structure.rvt")]
        public void Test_Curvy_surface_structure()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Lists and Structural Framing\06 Curvy surface structure.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(34, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(41, model.CurrentWorkspace.Connectors.Count());

            //check List.Map
            var mapId = "d4c58803-4ac4-4994-acfc-a6b38381fb81";
            AssertPreviewCount(mapId, 4);
            var mapList = GetFlattenedPreviewValues(mapId);
            foreach (var element in mapList)
            {
                Assert.IsNotNull(element);
            }

            //check StructuralFraming.BeamByCurve
            var strucID = "a40220ce-325f-4eeb-92c4-7a36fb224999";
            AssertPreviewCount(strucID, 100);
            for (int i = 0; i < 100; i++)
            {
                var element = GetPreviewValueAtIndex(strucID, i) as StructuralFraming;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\Lists and Structural Framing\RevitProject.rvt")]
        public void Test_Curvy_surface_structure_From_Revit()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Lists and Structural Framing\05 Curvy surface structure From Revit.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            AssertNoDummyNodes();

            var model = ViewModel.Model;
            Assert.AreEqual(40, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(48, model.CurrentWorkspace.Connectors.Count());
          
            //check StructuralFraming.BeamByCurve
            var strucID = "a40220ce-325f-4eeb-92c4-7a36fb224999";
            var struc = GetPreviewValue(strucID) as StructuralFraming;
            Assert.IsNotNull(struc);
          
            //check List.Map
            var mapID = "d4c58803-4ac4-4994-acfc-a6b38381fb81";
            AssertPreviewCount(mapID, 4);
            var map = GetFlattenedPreviewValues(mapID);
            Assert.AreEqual(map.Count, 100);       
        }

        [Test, Category("Failure")]// should add lunchbox package before this test can pass
        [TestModel(@".\Workflow\Definitions\Panels.rvt")]
        public void Test_Panels()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Definitions\Panels.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(32, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(41, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var color = "4845d25a-c7bd-4e61-8e5d-9dffee11d532";
            AssertPreviewCount(color, 6);
            for (int i = 0; i < 6; i++)
            {
                var element = GetPreviewValueAtIndex(color, i) as Element;
                Assert.IsNotNull(element);
            }
        }

        [Test, Category("Failure")]// should add lunchbox package before this test can pass
        [TestModel(@".\Workflow\PerforatedScreenByImage\PanelWall.rvt")]
        public void Test_PanelWall()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\PerforatedScreenByImage\PanelWall.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();

            var model = ViewModel.Model;
            Assert.AreEqual(19, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(21, model.CurrentWorkspace.Connectors.Count());

            //check Element.SetParamterByName
            var elementsID = "4ad86c1b-2e41-4374-b72b-467b3551c401";
            AssertPreviewCount(elementsID, 60);
            for (int i = 0; i < 60; i++)
            {
                var element = GetPreviewValueAtIndex(elementsID, i) as Element;
                Assert.IsNotNull(element);
            }

            //check AdaptiveComponent.ByPoint
            var acID = "85e957b8-90b7-46a5-823c-18aaa4155c98";
            AssertPreviewCount(acID, 60);
            for (int i = 0; i < 60; i++)
            {
                var element = GetPreviewValueAtIndex(acID, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }
        }

        [Test]
        [TestModel(@".\Workflow\PerforatedScreenByImage\PanelWall.rvt")]
        public void Test_PerforationsByImage()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\PerforatedScreenByImage\Perforations by image.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();

            var model = ViewModel.Model;
            Assert.AreEqual(31, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(41, model.CurrentWorkspace.Connectors.Count());

            //check ImportInstance.ByGeometries
            var importInstanceID = "88f8982b-c29e-44c5-8f01-c18560ac9eb9";
            var importInstance = GetPreviewValue(importInstanceID) as ImportInstance;
            Assert.IsNotNull(importInstance);   
      
            //check Curve.ExtrudeAsSolid
            var solidID = "15cdd045-e5dc-4217-85eb-0c7aac2c7901";
            AssertPreviewCount(solidID, 20);
            var solid = GetFlattenedPreviewValues(solidID);
            foreach (var element in solid)
            {
                Assert.IsNotNull(solid);
            }

            //check Solid.ByUnion
            var unionID = "0892604a-39a6-40f4-a12b-4043959de522";
            var union = GetPreviewValue(unionID) as Solid;
            Assert.IsNotNull(union);

            //check Surface.SubtractFrom
            var geometryID = "d15326de-522b-441f-b85b-90ae2dbb8207";
            AssertPreviewCount(geometryID, 1);
            var geometry = GetPreviewValueAtIndex(geometryID, 0) as Surface;
            Assert.IsNotNull(geometry);
        }


        [Test]
        [TestModel(@".\empty.rvt")]
        public void Test_Truss()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoBasic\01 Truss.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(25, model.CurrentWorkspace.Nodes.Count());
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
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\01_StructuralFraming.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(12, model.CurrentWorkspace.Nodes.Count());
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
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\02_Adaptive Component Placement 1.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(11, model.CurrentWorkspace.Nodes.Count());
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
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\03_Adaptive Component Placement 2.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(17, model.CurrentWorkspace.Nodes.Count());
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
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\04_ImportSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(16, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(16, model.CurrentWorkspace.Connectors.Count());

            //check elements in ImportInstance.ByGeometries are not null
            Assert.IsNotNull(GetPreviewValue("e3fedc00-247a-4971-901c-7fcb063344c6") as ImportInstance);
        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Test_PlaceFamiliesByLevel()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\05_PlaceFamiliesByLevel_Set Parameters.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(14, model.CurrentWorkspace.Nodes.Count());
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


        [Test, Category("Failure")]
        [TestModel(@".\Workflow\RevitProject\tower.rvt")]
        public void Test_EllipseTower01()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\RevitProject\01 Ellipse Tower v1.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(39, model.CurrentWorkspace.Nodes.Count());
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
                Assert.IsNotNull(element); //This node get error with the latest Dynamo
            }

        }


        [Test, Category("Failure")]
        [TestModel(@".\Workflow\RevitProject\tower.rvt")]
        public void Test_EllipseTower03()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\RevitProject\03 Ellipse Tower v3.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(45, model.CurrentWorkspace.Nodes.Count());
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
                Assert.IsNotNull(element); //This node get error with the latest Dynamo
            }
        }


        [Test]
        [TestModel(@".\Workflow\Python\HostedObjectStuff_Sample.rvt")]
        public void Test_Python()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Python\Revit API via Python.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(11, model.CurrentWorkspace.Connectors.Count());

            //check Python Script
            var pScript1 = "4caa3a16-50d9-4416-ae45-b5ad06d74c94";
            AssertPreviewCount(pScript1, 2);
            var flatvalue1 = GetFlattenedPreviewValues(pScript1);
            foreach (var ele in flatvalue1)
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
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7214
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\CodeBlocksReference\Panels_Nodes.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(32, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(42, model.CurrentWorkspace.Connectors.Count());


            //check AdaptiveComponent.ByPoints
            var adaptiveComp = "954276c8-5452-49bd-92a8-46fa14421d09";
            AssertPreviewCount(adaptiveComp, 456);
            for (int i = 0; i < 456; i++)
            {
                var element = GetPreviewValueAtIndex(adaptiveComp, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }
            //check Element.OverrideColorInView
            var color = "4845d25a-c7bd-4e61-8e5d-9dffee11d532";
            AssertPreviewCount(color, 456);
            for (int i = 0; i < 456; i++)
            {
                var element = GetPreviewValueAtIndex(color, i) as Element;
                Assert.IsNotNull(element);
            }
        }


        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-04-BaseFile.rvt")]
        public void Test_Vignette_04_Attractors()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            //http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7971
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-04-Attractors.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(15, model.CurrentWorkspace.Connectors.Count());

            //check Element.SetParameterByName
            var elementId = "ba0bd430-9f2d-4e1b-af13-c535c6ff6388";
            AssertPreviewCount(elementId, 55);
            for (int i = 0; i < 55; i++)
            {
                var elementValue = GetPreviewValueAtIndex(elementId, i) as Element;
                Assert.IsNotNull(elementValue);
            }                     
        }



        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-04-BaseFile.rvt")]
        public void Test_Vignette_04_Deviation()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7971
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-04-Deviation.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(19, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(28, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var elementId = "ae04fa3b-c494-403d-92d3-1f728dde45c9";
            AssertPreviewCount(elementId, 880);
            for (int i = 0; i < 880; i++)
            {
                var elementValue = GetPreviewValueAtIndex(elementId, i) as Element;
                Assert.IsNotNull(elementValue);
            }
        }



        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-04-BaseFile.rvt")]
        public void Test_Vignette_04_Solar()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7971
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-04-Solar.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(28, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(35, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var elementId = "ae04fa3b-c494-403d-92d3-1f728dde45c9";
            AssertPreviewCount(elementId, 2);
            for (int i = 0; i < 2; i++)
            {
                var elementValue = GetPreviewValueAtIndex(elementId, i) as Element;
                Assert.IsNotNull(elementValue);
            }

            //check Polygon.PlaneDeviation
            var listId = "f80ed841-3ccb-4fe5-9c13-594ffd53ab88";
            var listValue = GetFlattenedPreviewValues(listId);
            foreach (var ele in listValue)
            {
                Assert.IsNotNull(ele);
            }
        }

    }
}