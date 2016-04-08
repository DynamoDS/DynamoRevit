using System.IO;
using System.Linq;

using Autodesk.DesignScript.Geometry;

using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Tests;

using NUnit.Framework;

using Revit.Elements;

using RevitNodesTests;

using RevitTestServices;

using RTF.Framework;
using RevitServices.Persistence;
using System.Collections.Generic;
using RevitServices.Transactions;
using Revit.GeometryConversion;

using DoubleSlider = CoreNodeModels.Input.DoubleSlider;
using IntegerSlider = CoreNodeModels.Input.IntegerSlider;
using Dynamo.Applications.Models;
using System;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using Revit.Elements.InternalUtilities;

namespace RevitSystemTests
{
    [TestFixture]
    class BugTests : RevitSystemTestBase
    {
        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_66.rfa")]
        public void MAGN_66()
        {
            // Details are available in defect http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-66

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN_66.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("Failure")]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_102()
        {
            // Details are available in defect http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-102

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN_102_projectPointsToFace_selfContained.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(14, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(15, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN-122_wallsAndFloorsAndLevels.rvt")]
        public void MAGN_122()
        {
            // Details are available in defect http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-122
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN_122_wallsAndFloorsAndLevels.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(20, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(23, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check for Walls Creation
            var walls = "b7392d1d-6333-4bed-b10d-7b83520d2c3e";
            AssertPreviewCount(walls, 6);

            // I will un-comment this code once Ian fix the issue with Document.IsFamilyDocument 
            //var wallinst = GetPreviewValueAtIndex(walls, 3) as Floor;
            //Assert.IsNotNull(wallinst);
            //Assert.IsNotNullOrEmpty(wallinst.Name);

            // Check for Floor Creation
            var floors = "25392912-b625-4020-8dd1-81923c5e4823";
            AssertPreviewCount(floors, 6);

            // I will un-comment this code once Ian fix the issue with Document.IsFamilyDocument 
            //var floorInst = GetPreviewValueAtIndex(floors, 3) as Floor;
            //Assert.IsNotNull(floorInst);
            //Assert.IsNotNullOrEmpty(floorInst.Name);

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN-438_structuralFraming_simple.rvt")]
        public void MAGN_438()
        {
            // Details are available in defect http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-438
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN-438_structuralFraming_simple.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(17, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(17, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_2576_DataImport.rvt")]
        public void MAGN_2576()
        {
            var samplePath = Path.Combine(workingDirectory, @".\\Bugs\Defect_MAGN_2576.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Details are available in defect http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-2576
            var model = ViewModel.Model;
            var workspace = ViewModel.Model.CurrentWorkspace;

            // check all the nodes and connectors are loaded
            Assert.AreEqual(12, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(14, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // there should not be any crash on running this graph.
            // below node should have an error because there is no selection for Floor Type.
            var nodeModel = workspace.NodeFromWorkspace("cc38d11d-cda2-4294-81dc-119776af7338");
            Assert.AreEqual(ElementState.Warning, nodeModel.State);

        }
        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN-3620_topo.rvt")]
        public void MAGN_3620()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3620

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN-3620_Elementgeometry.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(2, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(1, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check for all Elements Creation
            var allElements = "c3d4e57e-2292-4d18-a603-30467df92d3f";
            AssertPreviewCount(allElements, 15);

            // Verification of Curve creation.
            var polyCurve = GetPreviewValueAtIndex(allElements, 1) as PolyCurve;
            Assert.IsNotNull(polyCurve);
            Assert.IsTrue(polyCurve.IsClosed);

            // Verify last geometry is Mesh
            var mesh = GetPreviewValueAtIndex(allElements, 14) as Mesh;
            Assert.IsNotNull(mesh);


        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_3784()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3784

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\Bugs\MAGN_3784.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(5, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            // evaluate graph
            var refPtNodeId = "92774673-e265-4378-b8ba-aef86c1a616e";
            var refPt = GetPreviewValue(refPtNodeId) as ReferencePoint;
            Assert.IsNotNull(refPt);
            Assert.AreEqual(0, refPt.X);

            // change slider value and re-evaluate graph
            IntegerSlider slider = model.CurrentWorkspace.NodeFromWorkspace
                ("55a992c9-8f16-4c07-a049-b0627d78c93c") as IntegerSlider;
            slider.Value = 10;

            RunCurrentModel();

            refPt = GetPreviewValue(refPtNodeId) as ReferencePoint;
            Assert.IsNotNull(refPt);
            (10.0).ShouldBeApproximately(refPt.X);

            RunCurrentModel();

            // Cross check from Revit side.
            var selectElementType = "4a99826a-eb73-4831-857c-909579c7eb12";
            var refPt1 = GetPreviewValueAtIndex(selectElementType, 0) as ReferencePoint;
            AssertPreviewCount(selectElementType, 1);

            Assert.IsNotNull(refPt1);
            (10.0).ShouldBeApproximately(refPt1.X, 1.0e-6);

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_4511()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-34511

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, 
                                    @".\Bugs\MAGN_4511_NullInputToForm.ByLoftCrossSections.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            // If this test reaches here, it means there is no hang in system.
            Assert.Pass();

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_2589.rvt")]
        public void MAGN_2589()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-2589

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_2589.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(20, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(29, model.CurrentWorkspace.Connectors.Count());

            // Validation for Geometry Instance import.
            var geometryInstance = "d017525b-2b02-44c4-88cf-2ed887c14a17";
            var solid = GetPreviewValue(geometryInstance) as ImportInstance;
            Assert.IsNotNull(solid);

            // change slider (Resolution) value and re-evaluate graph
            DoubleSlider slider = model.CurrentWorkspace.NodeFromWorkspace
                ("f6c91ebf-7eac-426c-81a9-97a0d5121fa5") as DoubleSlider;
            slider.Value = 35;

            RunCurrentModel();

            var solid1 = GetPreviewValue(geometryInstance) as ImportInstance;
            Assert.IsNotNull(solid1);

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_3408.rvt")]
        public void MAGN_3408()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3408

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_3408.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(14, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(12, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check for Wall
            var wall = "8f649c1c-d2c2-42dd-8b87-8dbe49afff4f";
            AssertPreviewCount(wall, 6);

            // get all Walls.
            for (int i = 0; i <= 5; i++)
            {
                var allwalls = GetPreviewValueAtIndex(wall, i) as Wall;
                Assert.IsNotNull(allwalls);
            }

            // Verification for first point from List
            var firsPoint = GetPreviewValue("f6d11b16-8f5e-45a1-9dce-cb8e8528baf2") as Point;
            Assert.IsNotNull(firsPoint);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_529_GetFamilyInstanceLocation.rfa")]
        public void MAGN_529()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-529

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory,
                                            @".\Bugs\MAGN_529_GetFamilyInstanceLocation.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(7, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(6, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Verification for final Surface created using Points captured from Family Location.
            var surface = GetPreviewValue("b28838eb-5f65-4f29-ae33-913becb3036e") as Surface;
            Assert.IsNotNull(surface);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\SurfaceNew.rfa")]
        public void SurfaceCreatedUsingCurvesFromRevit()
        {
            /* This test is not dependent on any defect, this is for validating Curves from
               Revit can be used to Create Surface and Solids.*/

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Bugs\SurfaceNew.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(28, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(32, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check for Surface
            var surface = "e7c8c8ba-2150-4e78-9628-04cd46f59400";
            AssertPreviewCount(surface, 20);

            // get all Surfaces.
            for (int i = 0; i <= 19; i++)
            {
                var allSurface = GetPreviewValueAtIndex(surface, i) as Surface;
                Assert.IsNotNull(allSurface);
            }

            // Check for Solid
            var solid = "cbd9b0db-c803-4319-b059-1d2c149be8a4";
            AssertPreviewCount(solid, 20);

            // get all Solids.
            for (int i = 0; i <= 19; i++)
            {
                var allSolids = GetPreviewValueAtIndex(solid, i) as Autodesk.DesignScript.Geometry.Solid;
                Assert.IsNotNull(allSolids);
            }
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_4566()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-4566
            // Passing empty list was resulting in Crash.

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_4566.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(10, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(9, model.CurrentWorkspace.Connectors.Count());

            /* As Nodes output is Null because of Empty List, this doesn’t need any validation on 
             any node. Test reaches here means there is no Crash on running the graph. */
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_4737()
        {
            // Details are available in defect 
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-4566
            // Passing ReferencePlane to watch node was crashng Dynamo.

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory,
                                        @".\Bugs\MAGN_4737_ReferencePlaneInWatchNode.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            var refPlane = GetPreviewValue("85c1f8c5-00da-4a7e-94c7-655140e39f6a") as Plane;
            Assert.IsNotNull(refPlane);
        }
        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void WorkflowDefect_4797()
        {
            //Dynamo throws exception on top of Revit but works in standalone mode.
            //http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-4797
            //Open attached dyn file and run, it will create Polygons and points in standalone mode
            
            string samplePath = Path.Combine(workingDirectory, @".\Bugs\MarkerData.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            AssertNoDummyNodes();
            
            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
        }
        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void RunAutomatic_5066()
        {
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-5066
            // FailedToObtain this object "DesignScriptEntity.Dispose " if run automatically on while moving from to next file

            string samplePath = Path.Combine(workingDirectory, @".\Bugs\mobius.dyn");
            string samplePath2 = Path.Combine(workingDirectory, @".\Bugs\mobius2.dyn");
            string testPath = Path.GetFullPath(samplePath);
            string testPath2 = Path.GetFullPath(samplePath2);

            AssertNoDummyNodes();

            ViewModel.OpenCommand.Execute(testPath);
            ViewModel.HomeSpace.RunSettings.RunType = RunType.Automatic;
            ViewModel.OpenCommand.Execute(testPath2);

            RunCurrentModel();
        }
        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void RevitNodes_ThroughCBN_2451()
        {

            // Access the Revit related nodes from code block nodes.Due to namespace class the error was thrown.
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-2451 - 
            // Description in the bug - Improve error reporting to user when there is a namespace collision

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory,
                                        @".\Bugs\Revitnodes_ThroughCBN_2451.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            var refPoint = GetPreviewValue("eb2244c2-e1d9-40c9-adea-4e94ed87795d") as ReferencePoint;
            Assert.IsNotNull(refPoint);

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_5160()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_5160.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            RunCurrentModel();

            var ModelCurveByCurveNode = GetNode<DSFunction>("f4ef10a1-1aed-49d5-8474-39cdbdf5fea6");
            var NurbsCurveByPointsNode = GetNode<DSFunction>("d200379e-5c8c-4f8b-968d-2f0887223d68");
            var NurbsCurveByControlPointsNode = GetNode<DSFunction>("8ba3309a-6ded-4059-a074-fa0c2b291919");
            var LineByStartPointEndPointNode = GetNode<DSFunction>("7979f6ce-63b6-4cfb-9872-9d05812a111c");

            //Connect the output of NurbsCurve.ByPoints node to the input of ModelCurve.ByCurve node
            MakeConnector(NurbsCurveByPointsNode, ModelCurveByCurveNode, 0, 0);
            RunCurrentModel();
            var curves = GetAllCurveElements();
            Assert.AreEqual(1, curves.Count);

            //Connect the output of NurbsCurve.ByControlPoints node to the input of ModelCurve.ByCurve node
            MakeConnector(NurbsCurveByControlPointsNode, ModelCurveByCurveNode, 0, 0);
            RunCurrentModel();
            //There will be an error and there should be no curves in the document
            curves = GetAllCurveElements();
            Assert.AreEqual(0, curves.Count);

            //Connect the output of Line.ByStartPointEndPoint node to the input of ModelCurve.ByCurve node
            MakeConnector(LineByStartPointEndPointNode, ModelCurveByCurveNode, 0, 0);
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            RunCurrentModel();
            //There should be only one curve in the document
            curves = GetAllCurveElements();
            Assert.AreEqual(1, curves.Count);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\StructuralFoundationTest.rvt")]
        public void MAGN_4679()
        {
           string samplePath = Path.Combine(workingDirectory, @".\Bugs\StructuralFoundationTest.dyn");
           string testPath = Path.GetFullPath(samplePath);

           //open the test file
           ViewModel.OpenCommand.Execute(testPath);
           AssertNoDummyNodes();

           RunCurrentModel();

           var watchNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<Watch>();
           Assert.NotNull(watchNode.CachedValue);
           Assert.IsInstanceOf<Autodesk.DesignScript.Geometry.Point>(watchNode.CachedValue);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void MAGN_6710()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_6710.dyn");
            string testPath = Path.GetFullPath(filePath);

            //open the test file
            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            RunCurrentModel();

            var curves = GetAllCurveElements();
            Assert.AreEqual(1, curves.Count);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rvt")]
        public void MAGN_7075()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7075.dyn");
            string testPath = Path.GetFullPath(filePath);

            //open the test file
            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            //Create 4 curve elements in Revit
            List<Autodesk.Revit.DB.Element> curves = new List<Autodesk.Revit.DB.Element>();
            var document = DocumentManager.Instance.CurrentUIDocument.Document;
            Autodesk.Revit.DB.Plane plane;
            Autodesk.Revit.DB.SketchPlane sp;
            using (var trans = new Autodesk.Revit.DB.Transaction(document, "CreateModelCurvesInRevit"))
            {
                trans.Start();

                Point[] points = { Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(200, 0, 0), Point.ByCoordinates(200, 100, 0), Point.ByCoordinates(0, 100, 0) };
                var line1 = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(points[0], points[1]);
                var line2 = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(points[1], points[2]);
                var line3 = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(points[2], points[3]);
                var line4 = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(points[3], points[0]);

                var revitLine1 = line1.ToRevitType(false);
                var revitLine2 = line2.ToRevitType(false);
                var revitLine3 = line3.ToRevitType(false);
                var revitLine4 = line4.ToRevitType(false);

                plane = new Autodesk.Revit.DB.Plane(new Autodesk.Revit.DB.XYZ(0, 0, 1), new Autodesk.Revit.DB.XYZ(0, 0, 0));
                sp = Autodesk.Revit.DB.SketchPlane.Create(document, plane);

                var modelCurv1 = document.Create.NewModelCurve(revitLine1, sp);
                var modelCurv2 = document.Create.NewModelCurve(revitLine2, sp);
                var modelCurv3 = document.Create.NewModelCurve(revitLine3, sp);
                var modelCurv4 = document.Create.NewModelCurve(revitLine4, sp);

                trans.Commit();

                curves.Add(modelCurv1);
                curves.Add(modelCurv2);
                curves.Add(modelCurv3);
                curves.Add(modelCurv4);
            }

            var node = ViewModel.Model.CurrentWorkspace.Nodes.OfType<DSModelElementsSelection>().First();
            node.UpdateSelection(curves);

            RunCurrentModel();

            //Create a line in Revit
            Autodesk.Revit.DB.ElementId lineID;
            using (var trans = new Autodesk.Revit.DB.Transaction(document, "CreateModelLine"))
            {
                trans.Start();
                var line = document.Create.NewModelCurve(Autodesk.Revit.DB.Line.CreateBound(new Autodesk.Revit.DB.XYZ(-100, 0, 0),
                    new Autodesk.Revit.DB.XYZ(-50, 0, 0)), sp);
                lineID = line.Id;
                trans.Commit();
            }

            RunCurrentModel();

            //Delete the created line in Revit
            using (var trans = new Autodesk.Revit.DB.Transaction(document, "DeleteReferencePoint"))
            {
                trans.Start();
                document.Delete(lineID);
                trans.Commit();
            }

            RunCurrentModel();
            //There should be no infinite loop, otherwise, there will be an error with this test case.
        }

        [Test, Category("Failure")]// Failure being tracked as MAGN-7656
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void SelectionButtonShouldBeDisabledAfterOpeningNewDocument()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7251.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            var node = AllNodes.OfType<DSModelElementSelection>().ElementAt(0);
            node.RevitDynamoModel = Model as RevitDynamoModel;
            Assert.IsTrue(node.CanSelect);

            string newRfaFilePath = Path.Combine(workingDirectory, "modelLines.rfa");
            DocumentManager.Instance.CurrentUIApplication.OpenAndActivateDocument(newRfaFilePath);
            node = AllNodes.OfType<DSModelElementSelection>().ElementAt(0);
            Assert.IsFalse(node.CanSelect);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Samples\DynamoSample.rvt")]
        public void GetParameterValueByNameWorksForSheetNumber()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_5870.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            Action<string> checkFunc = delegate(string guid)
            {
                List<object> objects = GetPreviewCollection(guid);

                Assert.AreEqual(objects.Count, 5);
                Assert.IsTrue(string.CompareOrdinal(objects[0] as string, "A102") == 0);
                Assert.IsTrue(string.CompareOrdinal(objects[1] as string, "A104") == 0);
                Assert.IsTrue(string.CompareOrdinal(objects[2] as string, "A103") == 0);
                Assert.IsTrue(string.CompareOrdinal(objects[3] as string, "A105") == 0);
                Assert.IsTrue(string.CompareOrdinal(objects[4] as string, "A101") == 0);
            };

            checkFunc("a45ab78b-b7cb-4317-a763-ac8c9a55753f");
            checkFunc("8fad0166-bb08-48e0-a62c-dcfb6b11a9fe");
            checkFunc("d4fd1c38-e3cb-484b-8187-66061476cd6a");
            checkFunc("6defe7da-caf9-489e-991d-4665eb26e786");
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void CanOpenAndRunOtherFilesAfterOpeningFileWithSelectNode()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7679.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            filePath = Path.Combine(workingDirectory, @".\Samples\Revit_Color.dyn");
            testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void AllElementsInActiveViewReturnsViewableElements()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7641_simplified.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();
            RunCurrentModel();

            var elements = GetPreviewCollection("55a73e51-1021-44e4-aacd-a4222ca2ba25");
            Assert.AreEqual(elements.Count(), 3);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void OpenNewFileNotCleanupOldElements()
        {
            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7229_1.dyn");
            string testPath = Path.GetFullPath(filePath);
            ViewModel.OpenCommand.Execute(testPath);

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.ReferencePoint));
            Assert.AreEqual(4, fec.ToElements().Count());

            filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7229_2.dyn");
            testPath = Path.GetFullPath(filePath);
            ViewModel.OpenCommand.Execute(testPath);
            fec.OfClass(typeof(Autodesk.Revit.DB.ReferencePoint));
            Assert.AreEqual(6, fec.ToElements().Count());

            Model.ClearCurrentWorkspace();
            fec.OfClass(typeof(Autodesk.Revit.DB.ReferencePoint));
            Assert.AreEqual(6, fec.ToElements().Count());
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\EleBindingTest_MAGN-7937.rfa")]
        public void EleBindingTest_MAGN_7937()
        {

            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\EleBindingTest_MAGN-7937.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(2, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(1, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            string refPtNodeId = "23e2f77c-bd3f-4376-83aa-45dedde795b8";
            var refPt = GetPreviewValue(refPtNodeId) as ReferencePoint;
            Assert.IsNotNull(refPt);
            refPt.Z.ShouldBeApproximately(7);

            // Count all Reference points in Revit.
            var refPoints = GetAllReferencePoints();
            Assert.AreEqual(1, refPoints.Count);

            // change slider value and re-evaluate graph
            IntegerSlider slider = model.CurrentWorkspace.NodeFromWorkspace
                ("bdcd9b06-989f-4bac-a94d-b84a432d33ea") as IntegerSlider;
            slider.Value = 10;

            RunCurrentModel();

            var modifiedRefPt = GetPreviewValue(refPtNodeId) as ReferencePoint;
            Assert.IsNotNull(modifiedRefPt);
            modifiedRefPt.Z.ShouldBeApproximately(10);

            // This is to validate there is no dulicate point in revit. 
            // After slider update there should be only one ref point in revit.
            var modifiedRefPoints1 = GetAllReferencePoints();
            Assert.AreEqual(1, modifiedRefPoints1.Count);

        }

        [Test]
        [TestModel(@".\Bugs\MAGN_8407.rvt")]
        public void GetParameterValueByName_MAGN_8407()
        {
            var model = ViewModel.Model;
            string dynfilePath = Path.Combine(workingDirectory,
                                                @".\Bugs\MAGN_8407.dyn");
            string testPath = Path.GetFullPath(dynfilePath);

            ViewModel.OpenCommand.Execute(testPath);

            // check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            AssertNoDummyNodes();

            // run
            RunCurrentModel();

            // Check that it has returned a category
            string nodeID = "fa304227-cea5-4490-8081-6e6b25fadf82";
            var cat = GetPreviewValue(nodeID) as Category;
            Assert.IsTrue(string.CompareOrdinal(cat.Name, "Walls") == 0);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_7977.rfa")]
        public void SelectedFaceIsTransformedCorrectly_MAGN_7977()
        {
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory,
                                                @".\Bugs\MAGN_7977.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            // check all the nodes and connectors are loaded
            Assert.AreEqual(5, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(4, model.CurrentWorkspace.Connectors.Count());

            AssertNoDummyNodes();

            // evaluate  graph
            RunCurrentModel();

            // Check that it has returned the Surface from Import Instance node.
            string nodeID = "4f522c79-76e5-40af-a137-1cd535d3061d";
            var maxZ = (double)GetPreviewValue(nodeID);
            maxZ.ShouldBeApproximately(10.0);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\emptyS.rvt")]
        public void Rotation_MAGN_8056()
        {
            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_8056.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            RunCurrentModel();

            string locationNodeId = "8c9dbf40-73d7-4788-84b2-ccf015fa47de";
            var locations = GetPreviewCollection(locationNodeId);
            var point = locations[0] as Point;
            point.X.ShouldBeApproximately(2.0);

            string numberSliderNodeId = "971b7986-59e2-4ad1-b087-0e9e4158506a";
            DoubleSlider slider = GetNode<DoubleSlider>(numberSliderNodeId) as DoubleSlider;
            slider.Value = 10.0; // Set the new rotation

            RunCurrentModel();
            locations = GetPreviewCollection(locationNodeId);
            point = locations[0] as Point;
            point.X.ShouldBeApproximately(2.0);// The x coordination of the column should not change
            point.Y.ShouldBeApproximately(0.0);// The x coordination of the column should not change
            point.Z.ShouldBeApproximately(0.0);// The x coordination of the column should not change

            //After updating the rotation angle, check that all family instances are rotated for 10.0 degrees
            var objects = GetPreviewCollection("2af2362b-38a7-4db7-976d-39f4e74c8e07");
            foreach(var obj in objects)
            {
                var instance = obj as Revit.Elements.FamilyInstance;
                Assert.IsNotNull(instance);

                double[] rotationAngles;
                var familyInstance = instance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
                var transform = familyInstance.GetTransform();
                TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
                (10.0).ShouldBeApproximately(rotationAngles[0] * 180 / Math.PI);
            }
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rfa")]
        public void ElementGeometryIssue_MAGN_7978()
        {

            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\ElementGeometryIssue_MAGN_7978.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(6, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(4, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check that it has returned the Surface from Import Instance node.
            string nodeID = "7d28ba60-e656-4626-a61f-bcbf8c763b52";
            var surface = GetPreviewValue(nodeID) as Surface;
            Assert.IsNotNull(surface);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_9132.rfa")]
        public void TraceDataForBatchProcessing_MAGN_9132()
        {
            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_9132.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();
            AssertPreviewCount("79637f91-d35b-49fc-bc54-4f5a1922633e", 11);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rvt")]
        public void OverrideColorInView_MAGN_7741()
        {
            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_7741.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(5, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(7, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            string nodeID = "fd42a0e6-e256-48b6-b65b-7b2fac46af1f";
            var curve = GetPreviewValue(nodeID) as ModelCurve;
            Assert.IsNotNull(curve);

            var settings = DocumentManager.Instance.CurrentUIDocument.ActiveView.
                GetElementOverrides(curve.InternalElement.Id);
            Assert.AreEqual(255, settings.ProjectionLineColor.Red);
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\Bugs\MAGN_8367.rvt")]
        public void UpdatedGraphIsNotRunAgain_MAGN_8367()
        {
            var model = ViewModel.Model;

            string filePath = Path.Combine(workingDirectory, @".\Bugs\MAGN_8367.dyn");
            string testPath = Path.GetFullPath(filePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(6, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(7, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            string selectNodeID = "982d28d8-a6ea-45d0-ac23-9f7c0f88c312";
            var selectNode = GetNode<DSModelElementSelection>(selectNodeID) as DSModelElementSelection;
            string codeBlockNodeID = "d36ae6dc-ae6e-4ad8-b616-af73464e5f57";
            var codeBlockNode = GetNode<CodeBlockNodeModel>(codeBlockNodeID) as CodeBlockNodeModel;
            string colorNodeID = "11192cef-552f-43bb-b5ab-42f809e285e0";
            var colorNode = GetNode<DSFunction>(colorNodeID) as DSFunction;

            // Change the selected element
            selectNode.UpdateSelection(new[] { ElementSelector.
                ByUniqueId("2049bcda-4652-4dce-8114-728cd33b120b-00000f4f").InternalElement });
            // Change the color to (0, 255, 255)
            colorNode.InPorts[1].Disconnect(colorNode.InPorts[1].Connectors[0]);
            RunCurrentModel();

            // Change the selected element back to the first element
            selectNode.UpdateSelection(new[] { ElementSelector.
                ByUniqueId("350f68bb-624c-405f-b93f-f7e6ff82778b-00000910").InternalElement });
            // Change the color again to (0, 0, 255)
            colorNode.InPorts[2].Disconnect(colorNode.InPorts[2].Connectors[0]);
            RunCurrentModel();

            // Now check the overriden color of element with ID of 2320 to be (0, 0, 255)
            var settings = DocumentManager.Instance.CurrentUIDocument.ActiveView.
                GetElementOverrides(ElementSelector.
                ByUniqueId("350f68bb-624c-405f-b93f-f7e6ff82778b-00000910").InternalElement.Id);
            Assert.AreEqual(0, settings.ProjectionLineColor.Red);
            Assert.AreEqual(0, settings.ProjectionLineColor.Green);
            Assert.AreEqual(255, settings.ProjectionLineColor.Blue);
        }

        protected static IList<Autodesk.Revit.DB.CurveElement> GetAllCurveElements()
        {
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.CurveElement));
            return fec.ToElements().Cast<Autodesk.Revit.DB.CurveElement>().ToList();
        }

        protected static IList<Autodesk.Revit.DB.ReferencePoint> GetAllReferencePoints()
        {
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.ReferencePoint));
            return fec.ToElements().Cast<Autodesk.Revit.DB.ReferencePoint>().ToList();
        }
    }
}
