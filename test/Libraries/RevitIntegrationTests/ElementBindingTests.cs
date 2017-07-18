using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using Autodesk.Revit.DB;
using CoreNodeModels.Input;
using Dynamo.Graph.Nodes.ZeroTouch;
using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Tests;

using RevitServices.Persistence;

using Transaction = Autodesk.Revit.DB.Transaction;
using DoubleSlider = CoreNodeModels.Input.DoubleSlider;
using IntegerSlider = CoreNodeModels.Input.IntegerSlider;
using Utils = RevitServices.Elements.ElementUtils;
using Dynamo.Graph.Nodes;

namespace RevitSystemTests
{
    [TestFixture]
    internal class ElementBindingTests : RevitSystemTestBase
    {
        /// <summary>
        /// This function gets all the model curves in the current Revit document
        /// </summary>
        /// <returns>the model curves</returns>
        private static IList<Autodesk.Revit.DB.ModelCurve> GetAllModelCurves()
        {
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "FilteringElements"))
            {
                trans.Start();

                ElementClassFilter ef = new ElementClassFilter(typeof(Autodesk.Revit.DB.CurveElement));
                FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                fec.WherePasses(ef);

                trans.Commit();
                return fec.ToElements().OfType<Autodesk.Revit.DB.ModelCurve>().ToList();
            }
        }

        /// <summary>
        /// This function gets all the reference points in the current Revit document
        /// </summary>
        /// <param name="startNewTransaction">whether do the filtering in a new transaction</param>
        /// <returns>the reference points</returns>
        private static IList<Element> GetAllReferencePointElements(bool startNewTransaction)
        {
            if (startNewTransaction)
            {
                using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "FilteringElements"))
                {
                    trans.Start();

                    ElementClassFilter ef = new ElementClassFilter(typeof(ReferencePoint));
                    FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                    fec.WherePasses(ef);

                    trans.Commit();
                    return fec.ToElements();
                }
            }
            else
            {
                ElementClassFilter ef = new ElementClassFilter(typeof(ReferencePoint));
                FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                fec.WherePasses(ef);
                return fec.ToElements();
            }
        }

        /// <summary>
        /// This function gets all the walls in the current Revit document
        /// </summary>
        /// <param name="startNewTransaction">whether do the filtering in a new transaction</param>
        /// <returns>the walls</returns>
        private static IList<Element> GetAllWallElements(bool startNewTransaction)
        {
            if (startNewTransaction)
            {
                using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "FilteringElements"))
                {
                    trans.Start();

                    ElementClassFilter ef = new ElementClassFilter(typeof(Wall));
                    FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                    fec.WherePasses(ef);

                    trans.Commit();
                    return fec.ToElements();
                }
            }
            else
            {
                ElementClassFilter ef = new ElementClassFilter(typeof(Wall));
                FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                fec.WherePasses(ef);
                return fec.ToElements();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="curve">The curve element which is one arc</param>
        /// <returns></returns>
        private static Autodesk.Revit.DB.Arc GetArcFromArcCurveElement(ModelCurve curve)
        {
            Options gOptions = new Options() { ComputeReferences = true };
            var gElement = curve.get_Geometry(gOptions);

            if (null == gElement)
                return null;

            foreach (GeometryObject geob in gElement)
            {
                var arc = geob as Autodesk.Revit.DB.Arc;
                if (null != arc)
                    return arc;
            }

            return null;
        }

        /// <summary>
        /// Given a node guid, this function will return the ElementId of the binding element.
        /// This function will work if only one element is created by the node.
        /// </summary>
        /// <param name="guid">the node guid</param>
        /// <returns>the element id</returns>
        private ElementId GetBindingElementIdForNode(Guid guid)
        {
            ProtoCore.RuntimeCore runtimeCore = ViewModel.Model.EngineController.LiveRunnerRuntimeCore;
            var guidToCallSites = runtimeCore.RuntimeData.GetCallsitesForNodes(new[] { guid }, runtimeCore.DSExecutable);

            var callSites = guidToCallSites[guid];
            if (!callSites.Any())
                return null;
            var callSite = callSites[0];
            var traceDataList = callSite.TraceData;
            if (!traceDataList.Any())
                return null;
            var data = traceDataList[0].GetLeftMostData();
            var id = data as SerializableId;
            return new ElementId(id.IntID);
        }

        [Test]
        [TestModel(@".\ElementBinding\CreateWallInDynamo.rvt")]
        public void CreateInDynamoModifyInRevitToCauseFailure()
        {
            //Create a wall in Dynamo
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateWallInDynamo.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            //Modify the wall in Revit
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "ModifyInRevit"))
            {
                bool hasError = false;
                trans.Start();

                try
                {
                    IList<Element> rps = GetAllWallElements(false);
                    Assert.AreEqual(1, rps.Count);
                    Wall wall = rps.First() as Wall;
                    List<XYZ> ctrlPnts = new List<XYZ>();
                    ctrlPnts.Add(new XYZ(0.0, 1.0, 0.0));
                    ctrlPnts.Add(new XYZ(1.0, 0.0, 0.0));
                    ctrlPnts.Add(new XYZ(2.0, 0.0, 0.0));
                    ctrlPnts.Add(new XYZ(3.0, 1.0, 0.0));
                    List<double> weights = new List<double>();
                    weights.Add(1.0);
                    weights.Add(1.0);
                    weights.Add(1.0);
                    weights.Add(1.0);
                    var spline = NurbSpline.Create(ctrlPnts, weights);
                    var wallLocation = wall.Location as LocationCurve;
                    wallLocation.Curve = spline;
                }
                catch (Exception e)
                {
                    hasError = true;
                    trans.RollBack();
                }

                if (!hasError)
                    trans.Commit();
            }


            RunCurrentModel();
            
            IList<Element> rps2 = GetAllWallElements(false);
            Assert.AreEqual(1, rps2.Count);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateInDynamoModifyInRevitReRun()
        {
            //Create a reference point at (0.0, 0.0, 0.0);
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateOneReferencePoint.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            //Change the position of the reference point
            var points = GetAllReferencePointElements(true);
            Assert.AreEqual(1, points.Count);
            ReferencePoint pnt = points[0] as ReferencePoint;
            Assert.IsNotNull(pnt);
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "ModifyInRevit"))
            {
                trans.Start();
                pnt.Position = new XYZ(10.0, 0.0, 0.0);
                trans.Commit();
            }

            //Run the graph once again

            RunCurrentModel();
            
            points = GetAllReferencePointElements(true);
            Assert.AreEqual(1, points.Count);
            pnt = points[0] as ReferencePoint;
            Assert.IsTrue(pnt.Position.IsAlmostEqualTo(new XYZ(0.0, 0.0, 0.0)));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateInDynamoCloseGraphReopenGraphRerun()
        {
            //Create a reference point at (0.0, 0.0, 0.0);
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateOneReferencePoint.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //Close the current graph
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            //Open the same graph 
            ViewModel.OpenCommand.Execute(testPath);

            //Run the graph once again
            RunCurrentModel();

            var points = GetAllReferencePointElements(true);
            Assert.AreEqual(2, points.Count);
            var pnt = points[0] as ReferencePoint;
            Assert.IsTrue(pnt.Position.IsAlmostEqualTo(new XYZ(0.0, 0.0, 0.0)));
            pnt = points[1] as ReferencePoint;
            Assert.IsTrue(pnt.Position.IsAlmostEqualTo(new XYZ(0.0, 0.0, 0.0)));
        }

        // TODO: Re-enable the test when open workspace in JSON is enabled.
        [Test, Ignore]
        [TestModel(@".\empty.rfa")]
        public void CreateInDynamoSaveCloseGraphReopenGraphRerun()
        {
            //Create a reference point at (0.0, 0.0, 0.0);
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateOneReferencePoint.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //Save the current graph
            string tempPath = Path.Combine(Path.GetTempPath(), "CreateOneReferencePoint.dyn");
            ViewModel.SaveAsCommand.Execute(tempPath);

            //Close the current graph
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            //Open the saved graph 
            ViewModel.OpenCommand.Execute(tempPath);

            //Run the graph once again
            RunCurrentModel();

            var points = GetAllReferencePointElements(true);
            Assert.AreEqual(1, points.Count);
            var pnt = points[0] as ReferencePoint;
            Assert.IsTrue(pnt.Position.IsAlmostEqualTo(new XYZ(0.0, 0.0, 0.0)));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateInDynamoDeleteInRevit()
        {
            //This test case is to test that elements can be created via Dynamo.
            //After they are deleted in Revit, we can still create them via Dynamo.

            //Create a reference point in Dynamo
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateInDynamo.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);


            RunCurrentModel();
            
            
            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => string.Equals(x.GUID.ToString(), "6a79717b-7438-458a-a725-587be0ba84fd"));
            Assert.IsTrue(selNodes.Any());
            var node = selNodes.First();
            var id1 = GetBindingElementIdForNode(node.GUID);

            //Delete all reference points in Revit
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "DeleteInRevit"))
            {
                trans.Start();

                IList<Element> rps = GetAllReferencePointElements(false);
                var rpIDs = rps.Select(x => x.Id);
                DocumentManager.Instance.CurrentDBDocument.Delete(rpIDs.ToList());

                trans.Commit();
            }

            //Run the graph again

            RunCurrentModel();
            
            var id2 = GetBindingElementIdForNode(node.GUID);

            //Check the number of reference points
            //This also verifies MAGN-2317
            IList<Element> newRps = GetAllReferencePointElements(true);
            Assert.AreEqual(1, newRps.Count());

            //Ensure the binding elements are different
            Assert.IsTrue(!id1.Equals(id2));
        }

        [Test, Ignore]
        [TestModel(@".\empty.rfa")]
        public void CreateInDynamoUndoInRevit()
        {
            //Create a reference point in Dynamo
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateInDynamo.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);


            RunCurrentModel();
            

            //Undo the creation of a reference point in Revit
            Assert.Inconclusive("TO DO");
        }

        [Test, Ignore]
        [TestModel(@".\ElementBinding\BindingCloseReopen.rvt")]
        public void SelectElementCloseReopenDocument()
        {
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\BindingCloseReopen.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            //Select one of the walls
            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => x is ElementSelection<Autodesk.Revit.DB.Element>);
            var selNode = selNodes.First() as ElementSelection<Autodesk.Revit.DB.Element>;

            var elId = new ElementId(184273);
            var el = DocumentManager.Instance.CurrentDBDocument.GetElement(elId);
            //selNode.SelectionResults.Add(el);


            RunCurrentModel();
            

            //Ensure the running result is correct
            Assert.AreEqual(true, GetPreviewValue("6e4abc3b-83fd-44fe-821b-447f1ec0a56c"));

            //Close and reopen the document
            Assert.Inconclusive("TO DO");

            //Run the graph again and ensure the result is correct
            Assert.AreEqual(true, GetPreviewValue("6e4abc3b-83fd-44fe-821b-447f1ec0a56c"));
        }

        [Test, Ignore]
        [TestModel(@".\ElementBinding\BindingCloseReopen.rvt")]
        public void SelectElementFromFamilyDocumentSwitchToProjectDocument()
        {            
            Assert.Inconclusive("TO DO");
        }

        [Test, Ignore]
        [TestModel(@".\empty.rfa")]
        public void CreateInRevitSelectInDynamoUndoInRevit()
        {
            //Create a reference point in Revit
            string rpID;
            ReferencePoint rp;
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "CreateInRevit"))
            {
                trans.Start();

                rp = DocumentManager.Instance.CurrentUIDocument.Document.FamilyCreate.NewReferencePoint(new XYZ());
                rpID = rp.UniqueId;

                trans.Commit();
            }

            //Select the reference point in Dynamo
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\SelectInDynamo.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => x is ElementSelection<Autodesk.Revit.DB.Element>);
            var selNode = selNodes.First() as ElementSelection<Autodesk.Revit.DB.Element>;
            //selNode.SelectionResults.Add(rp);


            RunCurrentModel();
            

            //Undo the creation of a reference point in Revit
            Assert.Inconclusive("TO DO");
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateInRevitSelectInDynamoSelectDifferentElement()
        {
            //This is to test that when a node is binding with a new element, the information related to binding
            //has actually changed.

            //Create two reference points in Revit
            string rpID1, rpID2;
            ReferencePoint rp1, rp2;
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "CreateInRevit"))
            {
                trans.Start();

                rp1 = DocumentManager.Instance.CurrentUIDocument.Document.FamilyCreate.NewReferencePoint(new XYZ());
                rpID1 = rp1.UniqueId;
                rp2 = DocumentManager.Instance.CurrentUIDocument.Document.FamilyCreate.NewReferencePoint(new XYZ(10, 0, 0));
                rpID2 = rp2.UniqueId;

                trans.Commit();
            }

            //Select the first reference point in Dynamo
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\SelectInDynamo.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => x is ElementSelection<Autodesk.Revit.DB.Element>);
            var selNode = selNodes.First() as ElementSelection<Autodesk.Revit.DB.Element>;
            IEnumerable<Element> selection1 = new[] { rp1 };
            selNode.UpdateSelection(selection1);

            RunCurrentModel();
            
            var id1 = selNode.SelectionResults.First();

            //Select the second reference point in Dynamo
            IEnumerable<Element> selection2 = new[] { rp2 };
            selNode.UpdateSelection(selection2);

            RunCurrentModel();
            
            var id2 = selNode.SelectionResults.First();

            //Ensure the element binding is not the same
            Assert.IsTrue(!id1.Equals(id2));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CreateDifferentNumberOfElementsInDynamo()
        {
            //This is to test that the same node can bind with different number of elements correctly

            //Create 4x2 reference points
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateDifferentNumberOfPoints.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            //Check the number of the refrence points
            var points = GetAllReferencePointElements(true);
            Assert.AreEqual(8, points.Count);

            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => string.Equals(x.GUID.ToString(), "a52bee11-4382-4c42-a676-443f9d7eedf2"));
            Assert.IsTrue(selNodes.Any());
            var node = selNodes.First();
            var slider = node as IntegerSlider;

            //Change the slider value from 4 to 3
            slider.Value = 3;

            //Run the graph again

            RunCurrentModel();
            

            //Check the number of the refrence points
            points = GetAllReferencePointElements(true);
            Assert.AreEqual(6, points.Count);
        }

        [Test, Ignore]
        [TestModel(@".\empty.rfa")]
        public void CreateDifferentNumberOfElementsInDynamoWithDifferentLacingStrategies()
        {

            //This is to test that the same node can bind correctly with different number of elements
            //when the lacing strategies for the node change

            Assert.Inconclusive("TO DO");

            /*
            //Create 4x2 reference points
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\CreateDifferentNumberOfPoints.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            //Check the number of the refrence points
            var points = GetAllReferencePointElements(true);
            Assert.AreEqual(8, points.Count);

            var model = ViewModel.Model;
            var selNodes = model.AllNodes.Where(x => string.Equals(x.Name, "ReferencePoint.ByCoordinates"));
            Assert.IsTrue(selNodes.Any());
            var node = selNodes.First() as DSFunction;

            //As the unit test will hang, so make it fail
            Assert.Fail("Reference points will be created at the same location!");

            //Change the slider value from 4 to 3
            node.ArgumentLacing = Dynamo.Models.LacingStrategy.Longest;

            //Run the graph again
          
            RunCurrentModel();
            

            //Check the number of the refrence points
            points = GetAllReferencePointElements(true);
            Assert.AreEqual(4, points.Count);
            */
        }

        [Test]
        [TestModel(@".\ElementBinding\magn-2523.rfa")]
        public void Rebinding_ExceptionIsThrown()
        {
            //This is to test that in the process of rebinding, when an exception is thrown, the related
            //Revit element will be cleaned.

            //Create 8 family instances
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\magn-2523.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //Check the number of the family instances
            var instances = GetAllFamilyInstances(true);
            Assert.AreEqual(8, instances.Count);

            var model = ViewModel.Model;
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => string.Equals(x.GUID.ToString(), "2411be0e-abff-4d32-804c-5e5025a92257"));
            Assert.IsTrue(selNodes.Any());
            var node = selNodes.First();
            var slider = node as DoubleSlider;
            
            //Change the value of the slider from 19.89 to 18.0
            slider.Value = 18.0;
            //Run the graph again
           
            RunCurrentModel();
            
            //Change the value of the slider from 18.0 to 16.0
            slider.Value = 16.0;
            //Run the graph again
           
            RunCurrentModel();

            //Check the number of family instances
            instances = GetAllFamilyInstances(true);
            Assert.AreEqual(8, instances.Count);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void RecoveringNodesFromErrorToNormal()
        {
            string dynFilePath = Path.Combine(workingDirectory, @".\ElementBinding\ErrorToNormal.dyn");
            string testPath = Path.GetFullPath(dynFilePath);

            var model = ViewModel.Model;
            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
            
            var selNodes = model.CurrentWorkspace.Nodes.Where(x => string.Equals(x.GUID.ToString(), "7cc9bd94-7f46-4520-8a47-60baf4419087"));
            Assert.IsTrue(selNodes.Any());
            var node = selNodes.First();
            var slider = node as IntegerSlider;

            //Change the slider value to 10
            slider.Value = 10;
   
            RunCurrentModel();
            
            var curves = GetAllModelCurves();
            Assert.AreEqual(1, curves.Count());

            var curve = curves.ElementAt(0);
            var arc = GetArcFromArcCurveElement(curve);
            Assert.IsNotNull(arc);

            //Change the slider value to 0 to cause a warning/error
            slider.Value = 0;
   
            RunCurrentModel();

            //Check that the ModelCurve node has a warning/error
            Assert.IsTrue(IsNodeInErrorOrWarningState("bebfd220-3c77-4f06-8a8a-143ac07974a3"));

            //Change the slider value to 5
            slider.Value = 5;
   
            RunCurrentModel();

            curves = GetAllModelCurves();
            Assert.AreEqual(1, curves.Count());

            curve = curves.ElementAt(0);
            arc = GetArcFromArcCurveElement(curve);
            Assert.IsNotNull(arc);
            IsFuzzyEqual(5, arc.Radius, 1.0e-6);

            //Change the slider value to 8
            slider.Value = 8;
   
            RunCurrentModel();

            curves = GetAllModelCurves();
            Assert.AreEqual(1, curves.Count());

            curve = curves.ElementAt(0);
            arc = GetArcFromArcCurveElement(curve);
            Assert.IsNotNull(arc);
            IsFuzzyEqual(8, arc.Radius, 1.0e-6);
        }

        [Test, TestModel(@".\ElementBinding\RebindingSingleDimension.rfa")]
        public void Rebinding_SingleDimensionDecreaseOnReopen()
        {
            var model = OpenElementBindingWorkspace("RebindingSingleDimension.dyn");

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var numNode = model.CurrentWorkspace.FirstNodeFromWorkspace<DoubleInput>();

            ChangeNumberValueAndCheckElementCount(numNode, doc, 3, 1);
            ChangeNumberValueAndCheckElementCount(numNode, doc, 8, 1);
        }

        [Test, TestModel(@".\ElementBinding\RebindingSingleDimension.rfa")]
        public void Rebinding_SingleDimensionIncreaseOnReopen()
        {
            var model = OpenElementBindingWorkspace("RebindingSingleDimension.dyn");

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var numNode = model.CurrentWorkspace.FirstNodeFromWorkspace<DoubleInput>();

            ChangeNumberValueAndCheckElementCount(numNode, doc, 8, 1);
            ChangeNumberValueAndCheckElementCount(numNode, doc, 3, 1);
        }

        [Test, TestModel(@".\ElementBinding\RebindingMultiDimension.rfa")]
        public void Rebinding_MultiDimensionDecreaseOnReopen()
        {
            var model = OpenElementBindingWorkspace("RebindingMultiDimension.dyn");

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var numNode = model.CurrentWorkspace.FirstNodeFromWorkspace<DoubleInput>();

            ChangeNumberValueAndCheckElementCount(numNode, doc, 3, 2);
            ChangeNumberValueAndCheckElementCount(numNode, doc, 8, 2);
        }

        [Test, TestModel(@".\ElementBinding\RebindingMultiDimension.rfa")]
        public void Rebinding_MultiDimensionIncreaseOnReopen()
        {
            var model = OpenElementBindingWorkspace("RebindingMultiDimension.dyn");

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var numNode = model.CurrentWorkspace.FirstNodeFromWorkspace<DoubleInput>();

            ChangeNumberValueAndCheckElementCount(numNode, doc, 8, 2);
            ChangeNumberValueAndCheckElementCount(numNode, doc, 3, 2);
        }

        [Test, TestModel(@".\ElementBinding\RebindingSingleDimension.rfa")]
        public void Rebinding_NodeDeletedBeforeRun()
        {
            var model = OpenElementBindingWorkspace("RebindingSingleDimension.dyn");

            var refPtNode = model.CurrentWorkspace.FirstNodeFromWorkspace<DSFunction>();
            var command = new DynamoModel.DeleteModelCommand(refPtNode.GUID);
            ViewModel.ExecuteCommand(command);
            RunCurrentModel();

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var refPts = Utils.AllElementsOfType<ReferencePoint>(doc);
            Assert.AreEqual(refPts.Count(), 0);
        }

        [Test, TestModel(@".\ElementBinding\DynamoSample.rvt")]
        public void Rebinding_ReboundAdaptiveComponentsAreNotDeleted()
        {
            var model = OpenElementBindingWorkspace("RebindingBatchedACs.dyn");
            RunCurrentModel();
            NodeModel adaptiveCompNode = model.CurrentWorkspace.Nodes.Where(x => x.Name == "AdaptiveComponent.ByPoints").First();

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var familyInstances = Utils.AllElementsOfType<FamilyInstance>(doc);
            Assert.AreEqual(9,adaptiveCompNode.GetValue(0, model.EngineController).GetElements().Select(x=>x.Data).ToList().Count);
        }

        [Test]
        [TestModel(@".\ElementBinding\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_UpdateLocation_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\ElementBinding\FamilyInstancePlacementByFace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            var initialNumber = GetAllFamilyInstances(false).Count;

            // The current Revit file already has a family placed at UV param location 0.50
            // We update placement location of family instance to 0.75 param location
            var cbn = GetNode<CodeBlockNodeModel>("a7e21de3-bff4-4d1d-b23d-3fb2db980d57");

            var command = new Dynamo.Models.DynamoModel.UpdateModelValueCommand(
                Guid.Empty, cbn.GUID, "Code", "0.75");
            this.Model.ExecuteCommand(command);

            RunCurrentModel();

            var finalNumber = GetAllFamilyInstances(false).Count;
            var famInst = GetPreviewValue("56cf69ec-d4ca-4add-810d-aee64d003c76") as Revit.Elements.FamilyInstance;
            Assert.IsNotNull(famInst);

            // Assert that there is no change in the total number of family instances in the document
            // as the original should have been updated and no new one should be created
            Assert.AreEqual(initialNumber, finalNumber);
        }

        [Test]
        [TestModel(@".\ElementBinding\MultipleCustomInstance.rvt")]
        public void MultipleCustomNodeInstance()
        {
            string dynPath = Path.Combine(workingDirectory, @".\ElementBinding\PlaceMultipleRevitCustomNodes.dyn");

            ViewModel.OpenCommand.Execute(dynPath);
            RunCurrentModel();

            var walls = GetAllWalls();
            Assert.AreEqual(8, walls.Count());

            var cbn = GetNode<CodeBlockNodeModel>("a4705f1f-1cfb-43eb-ba35-a797d5703d37");
            var command = new Dynamo.Models.DynamoModel.UpdateModelValueCommand(Guid.Empty, cbn.GUID, "Code", "100;100;");
            this.Model.ExecuteCommand(command);

            walls = GetAllWalls();
            Assert.AreEqual(8, walls.Count());
        }


        private DynamoModel OpenElementBindingWorkspace(string name)
        {
            var dynFilePath = Path.Combine(
                workingDirectory, string.Format(
                @".\ElementBinding\{0}", name));
            var testPath = Path.GetFullPath(dynFilePath);

            var model = ViewModel.Model;
            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            return model;
        }

        private void ChangeNumberValueAndCheckElementCount(DoubleInput numNode, Document doc, int value, int power)
        {
            numNode.Value = value.ToString();
            RunCurrentModel();
            var refPts = Utils.AllElementsOfType<ReferencePoint>(doc);
            Assert.AreEqual(refPts.Count(), Math.Pow(value + 1,power));
        }
    }
}
