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
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count);
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
            Assert.AreEqual(23, model.CurrentWorkspace.Nodes.Count);
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
            Assert.AreEqual(34, model.CurrentWorkspace.Nodes.Count);
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
            Assert.AreEqual(40, model.CurrentWorkspace.Nodes.Count);
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

        [Test, Category("Failure")]
        [TestModel(@".\Workflow\Definitions\Panels.rvt")]
        public void Test_Panels()// run times out
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Definitions\Panels.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(32, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(41, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var color = "4845d25a-c7bd-4e61-8e5d-9dffee11d532";
            AssertPreviewCount(color, 500);
            for (int i = 0; i < 500; i++)
            {
                var element = GetPreviewValueAtIndex(color, i) as Element;
                Assert.IsNotNull(element);
            }
        }

        [Test]
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
            Assert.AreEqual(19, model.CurrentWorkspace.Nodes.Count);
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
            Assert.AreEqual(31, model.CurrentWorkspace.Nodes.Count);
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
    }
}