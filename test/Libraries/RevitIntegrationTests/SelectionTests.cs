using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using CoreNodeModels;
using DSRevitNodesUI;
using Dynamo.Graph.Nodes;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Tests;

using NUnit.Framework;
using RevitServices.Persistence;
using RevitServices.Transactions;

using RevitTestServices;

using RTF.Framework;
using ReferencePoint = Revit.Elements.ReferencePoint;
using Surface = Autodesk.DesignScript.Geometry.Surface;

namespace RevitSystemTests
{
    [TestFixture]
    class SelectionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void FamilyTypeSelectorNode()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Selection\SelectFamily.dyn");
            string testPath = Path.GetFullPath(samplePath);

            //open the test file
            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            //first assert that we have only one node
            var nodeCount = ViewModel.Model.CurrentWorkspace.Nodes.Count();
            Assert.AreEqual(1, nodeCount);

            //assert that we have the right number of family symbols
            //in the node's items source
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Family));
            int count = fec.ToElements().Cast<Family>().Sum(f => f.GetFamilySymbolIds().Count());

            var typeSelNode = (FamilyTypes)ViewModel.Model.CurrentWorkspace.Nodes.First();
            Assert.AreEqual(typeSelNode.Items.Count, count);

            //assert that the selected index is correct
            Assert.AreEqual(typeSelNode.SelectedIndex, 3);

            //now try and set the selected index to something
            //greater than what is possible
            typeSelNode.SelectedIndex = count + 5;
            Assert.AreEqual(typeSelNode.SelectedIndex, -1);
        }
        
        [Test, Category("SmokeTests"), TestModel(@".\empty.rfa")]
        public void SelectionDocModificationSync()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Selection\SelectAndUpdate.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            var selectNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<DSModelElementSelection>();
            var watchNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<Watch>();

            // Create a reference point in Revit
            Autodesk.Revit.DB.ReferencePoint p1;
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document))
            {
                trans.Start("Create reference point for testing.");

                p1 = DocumentManager.Instance.CurrentUIDocument.Document.FamilyCreate.NewReferencePoint(new XYZ(0, 0, 0));


                trans.Commit();
            }
            // select the reference point using the selection node
            selectNode.UpdateSelection(new[] { p1 });
            
            RunCurrentModel();

            Assert.AreEqual(0, watchNode.CachedValue);

            // Update the reference point position in Revit
            using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document))
            {
                trans.Start("Updating reference point for testing.");

                p1.Position = new XYZ(10, 0, 0);

                trans.Commit();
            }

            // Verify that the selection node updates
            Assert.AreEqual(true, selectNode.NeedsForceExecution);

            RunCurrentModel();

            Assert.AreEqual(10, watchNode.CachedValue); //Actual value depends on units
        }

        [Test, Category("SmokeTests"), TestModel(@".\empty.rfa")]
        public void NodeModificationSelectionNoSync()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Selection\SelectAndUpdate.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            var selectNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<DSModelElementSelection>();
            var watchNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<Watch>();

            // Create a reference point in Dynamo
            var refPt = ReferencePoint.ByCoordinates(0, 0, 0);
            selectNode.UpdateSelection(new[] { refPt.InternalElement });

            RunCurrentModel();

            Assert.AreEqual(0, watchNode.CachedValue);

            // Update the reference point position in Dynamo
            refPt.X = 10;

            TransactionManager.Instance.ForceCloseTransaction();

            // Verify that the selection node does not update
            Assert.AreEqual(false, selectNode.NeedsForceExecution);

            RunCurrentModel();

            Assert.AreEqual(0, watchNode.CachedValue); //Actual value depends on units
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\Selection.rfa")]
        public void EmptySingleSelectionReturnsNull()
        {
            // Verify that an empty single-selection returns null
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectAndMultiSelect.dyn"));

            var guid = "938e1543-c1d5-4c92-83a7-3abcae2b8264";
            var selectionNode = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(n => n.GUID.ToString() == guid) as ElementSelection<Element>;
            Assert.NotNull(selectionNode, "The requested node could not be found");
            
            selectionNode.ClearSelections();


            RunCurrentModel();
            
            var element = GetPreviewCollection(guid);
            Assert.Null(element);
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\Selection.rfa")]
        public void EmptyMultiSelectionReturnsNull()
        {
            // Verify that an empty multi-selection returns null
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectAndMultiSelect.dyn"));

            var guid = "34f4f2cc-63c3-41ec-91fa-68db7820cee5";
            var selectionNode = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(n => n.GUID.ToString() == guid) as ElementSelection<Element>;
            Assert.NotNull(selectionNode, "The requested node could not be found");

            selectionNode.ClearSelections();


            RunCurrentModel();
            
            var element = GetPreviewCollection(guid);
            Assert.Null(element);
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\Selection.rfa")]
        public void SingleSelectionReturnsSingleObject()
        {
            // Verify that the selection of a single element
            // returns only one object NOT a list of objects
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectAndMultiSelect.dyn"));

            RunCurrentModel();
           // Assert.DoesNotThrow(()=>ViewModel.Model.RunExpression());

            var guid = "938e1543-c1d5-4c92-83a7-3abcae2b8264";
            var element = GetPreviewValue(guid);
            Assert.IsInstanceOf<Revit.Elements.ReferencePoint>(element);
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\Selection.rfa")]
        public void MultiSelectionReturnsMultipleObjects()
        {
            // Verify that the selection of many elements
            // returns a list of objects
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectAndMultiSelect.dyn"));


            RunCurrentModel();
            

            var guid = "34f4f2cc-63c3-41ec-91fa-68db7820cee5";
            var element = GetPreviewCollection(guid);
            Assert.IsInstanceOf<List<object>>(element);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectModelElement()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory,@".\Selection\SelectModelElement.dyn"));
            TestSelection<Element, Element>(SelectionType.One);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectModelElements()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectModelElements.dyn"));
            TestSelection<Element, Element>(SelectionType.Many);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectFace()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectFace.dyn"));

            RunCurrentModel();

            // Get the selection node
            var selectNode = (ReferenceSelection)(ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is ReferenceSelection));
            Assert.NotNull(selectNode);

            // The select faces node returns a list of lists
            var list = GetFlattenedPreviewValues(selectNode.GUID.ToString());
            Assert.AreEqual(1, list.Count());
            Assert.IsInstanceOf<Surface>(list[0]);

            // Clear the selection
            selectNode.ClearSelections();

            RunCurrentModel();

            list = GetFlattenedPreviewValues(selectNode.GUID.ToString());
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectEdge()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectEdge.dyn"));

            RunCurrentModel();
            

            var selectionNode = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(n => n is ReferenceSelection) as ReferenceSelection;
            Assert.NotNull(selectionNode);
            var element = GetPreviewValue(selectionNode.GUID.ToString());
            Assert.IsInstanceOf<NurbsCurve>(element);

            selectionNode.ClearSelections();

            RunCurrentModel();
            
            element = GetPreviewValue(selectionNode.GUID.ToString());
            Assert.Null(element);
        }

        //[Test]
        //[TestModel(@".\Selection\Selection.rfa")]
        //public void SelectPointOnFace()
        //{
        //    OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectPointOnFace.dyn"));
        //    TestSelection<Reference,Reference>(SelectionType.One);
        //}

        //[Test]
        //[TestModel(@".\Selection\Selection.rfa")]
        //public void SelectUVOnFace()
        //{
        //    OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectUVOnFace.dyn"));
        //    TestSelection<Reference,Reference>(SelectionType.One);
        //}

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectDividedSurfaceFamilies()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectDividedSurfaceFamilies.dyn"));

            // Get the selection node
            var selectNode = (ElementSelection<DividedSurface>)(ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is ElementSelection<DividedSurface>));
            Assert.NotNull(selectNode);

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(DividedSurface));

            var ds = fec.ToElements().FirstOrDefault() as DividedSurface;
            Assert.NotNull(ds);

            RunCurrentModel();

            var elements = GetPreviewCollection(selectNode.GUID.ToString());
            Assert.AreEqual(25, elements.Count);

            // Reset the selection
            selectNode.UpdateSelection(new[] { ds });

            using (var trans = new Transaction(DocumentManager.Instance.CurrentDBDocument))
            {
                try
                {
                    trans.Start("SelectDividedSurfaceFamilies_Test");

                    // Flex the divided surface division and ensure the 
                    // SelectionResults is updated.
                    ds.USpacingRule.Number = 3;
                    ds.VSpacingRule.Number = 3;

                    trans.Commit();
                }
                catch(Exception ex)
                {
                    if (trans.HasStarted())
                    {
                        trans.RollBack();
                    }

                    Assert.Fail(ex.Message);
                }
            }

            RunCurrentModel();

            elements = GetPreviewCollection(selectNode.GUID.ToString());
            Assert.AreEqual(9, elements.Count);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectFaces()
        {
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectFaces.dyn"));

            RunCurrentModel();

            // Get the selection node
            var selectNode = (ReferenceSelection)(ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is ReferenceSelection));
            Assert.NotNull(selectNode);

            // The select faces node returns a list of lists
            var list = GetFlattenedPreviewValues(selectNode.GUID.ToString());
            Assert.AreEqual(3, list.Count);
            Assert.IsInstanceOf<Surface>(list[0]);

            // Clear the selection
            selectNode.ClearSelections();

            RunCurrentModel();

            list = GetFlattenedPreviewValues(selectNode.GUID.ToString());
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        [Category("SmokeTests")]
        [TestModel(@".\Selection\Selection.rfa")]
        public void SelectionVerifyElementID()
        {
            // Open Xml graph
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectModelElement.dyn"));
            RunCurrentModel();

            // Expected identification values
            const string expectedSelectionId = "d854cdd2-7ea0-4cc0-bd7b-18891f94b3ee-000082e3";
            const string expectedUUID = "bce7e393-aba2-4136-80ad-5aa136e5c5bf";

            // Select model element node
            var modelElementNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<DSModelElementSelection>();
            var elementSelectionId = modelElementNode.SelectionIdentifier.First();
            var elementUUID = modelElementNode.GUID.ToString();

            // Assert node exists and returns expected identifiers
            Assert.NotNull(modelElementNode);
            Assert.AreEqual(expectedSelectionId, elementSelectionId);
            Assert.AreEqual(expectedUUID, elementUUID);
    
            // Save model in temp location
            ViewModel.CurrentSpace.Save(Path.Combine(workingDirectory, @".\Selection\SelectModelElement_temp.dyn"));

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Open Json temp file
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectModelElement_temp.dyn"));
            RunCurrentModel();

            // Repeat verifications
            modelElementNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<DSModelElementSelection>();
            elementSelectionId = modelElementNode.SelectionIdentifier.First();
            elementUUID = modelElementNode.GUID.ToString();
            Assert.NotNull(modelElementNode);
            Assert.AreEqual(expectedSelectionId, elementSelectionId);
            Assert.AreEqual(expectedUUID, elementUUID);

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Delete temp file
            File.Delete(Path.Combine(workingDirectory, @".\Selection\SelectionElementId_temp.dyn"));
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\SelectionSync.rvt")]
        public void SelectionInSyncWithDocumentOperations_Elements()
        {
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(Wall));

            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectionSyncElements.dyn"));

            const string selectNodeGuid = "3dbe16b8-e855-4229-a1cf-4643e69ba7b4";

            var walls = fec.ToElements();
            int remainingWallCount = walls.Count;
            while (remainingWallCount > 1)
            {
                remainingWallCount = DeleteWallAndRun<Revit.Elements.Wall>(selectNodeGuid);
            }
        }

        [Test, Category("SmokeTests"), TestModel(@".\Selection\SelectionSync.rvt")]
        public void SelectionInSyncWithDocumentOperations_References()
        {
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(Wall));

            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\Selection\SelectionSyncReferences.dyn"));

            const string selectNodeGuid = "91fd4f06-dde2-449f-aff5-f6203e4777ed";
            var walls = fec.ToElements();
            int remainingWallCount = walls.Count;
            while (remainingWallCount > 1)
            {
                remainingWallCount = DeleteWallAndRun<Surface>(selectNodeGuid);
            }

        }

        [Test]
        [TestModel(@".\Selection\DynamoSample.rvt")]
        public void DynamoAllSelectionNodeTests_WithPreSelectedEntities()
        {
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory,
                                                @".\Selection\DynamoAllSelectionNodeTests.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            ViewModel.OpenCommand.Execute(testPath);

            // check all the nodes and connectors are loaded
            Assert.AreEqual(38, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(20, model.CurrentWorkspace.Connectors.Count());
            
            AssertNoDummyNodes();

            // evaluate  graph
            RunCurrentModel();

            // ElementsAtLevel
            // See http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-9069
            // The number of elements at level is different here in 2016 and 2017, due
            // to a fault in the 2017 API.
            var allElementAtLevelNodeID = "da009f85-80e6-4541-b8fd-165ea7f23449";
            AssertPreviewCount(allElementAtLevelNodeID, 73);

            var wall = GetPreviewValueAtIndex(allElementAtLevelNodeID, 0) as Revit.Elements.Wall;
            Assert.IsNotNull(wall);

            var floor = GetPreviewValueAtIndex(allElementAtLevelNodeID, 2) as Revit.Elements.Floor;
            Assert.IsNotNull(floor);

            // ElementsOfCategories & Adaptive Points, as output of Categories 
            // passed to ElementsOFCategory
            var elementsOfCategoryNodeID = "24f225e1-8883-48c3-a8ba-773b2734336c";
            AssertPreviewCount(elementsOfCategoryNodeID, 22);
            for (int i = 0; i <= 21; i++)
            {
                var refPt = GetPreviewValueAtIndex(elementsOfCategoryNodeID, i) as ReferencePoint;
                Assert.IsNotNull(refPt);
            }

            // ElementsOfCategories & Views, as output of Categories 
            // passed to ElementsOFCategory
            var elementsOfViewNodeID = "2569434a-b34f-4512-a4fe-f065c7a10175";
            AssertPreviewCount(elementsOfViewNodeID, 33);

            // ElementsOfFamilyType & Family Type, as FamilyType output passed to 
            // ElementsOfFamilyTypes node.
            var elementsOfFamilyTypeNodeID = "657017f1-4775-4daa-b00e-8d85392be6b5";
            AssertPreviewCount(elementsOfFamilyTypeNodeID, 11);
            for (int i = 0; i <= 10; i++)
            {
                var family = GetPreviewValueAtIndex(elementsOfFamilyTypeNodeID, i) 
                                                        as Revit.Elements.AbstractFamilyInstance;
                Assert.IsNotNull(family);
            }

            // ElementsOfType & ElementTypes, as ElementType output passed to ElementsOfTypes node.
            var elementsOfTypeNodeID = "186142a6-eb54-4d39-8292-5b016a3a1c3b";
            AssertPreviewCount(elementsOfTypeNodeID, 27);
            for (int i = 0; i <= 26; i++)
            {
                var curtainGridLine = GetPreviewValueAtIndex(elementsOfTypeNodeID, i) 
                                                            as Revit.Elements.Element;
                Assert.IsNotNull(curtainGridLine);
            }

            // Select Model Elements
            var selectModelElementsNodeID = "e18b4c42-71b7-4f32-b0a5-6aa89ac32aeb";
            AssertPreviewCount(selectModelElementsNodeID, 8);
            for (int i = 0; i <= 7; i++)
            {
                var wall1 = GetPreviewValueAtIndex(selectModelElementsNodeID, i) as Revit.Elements.Wall ;
                Assert.IsNotNull(wall1);
            }

            // Select Model Element
            var selectModelElementNode = GetPreviewValue
                                ("137a10ad-a34f-4524-aaed-1c608e7ce8b6") as Revit.Elements.Element;
            Assert.IsNotNull(selectModelElementNode);

            // Select Face
            var selectFaceNodeID = GetPreviewValueAtIndex
                                        ("26942776-c0f5-4a23-9be7-95ef80809951", 0) as Surface;
            Assert.IsNotNull(selectFaceNodeID);

            // Select Faces (Below Node ID taken from Flatter to make it 1D array)
            var selectFacesNodeID = "00a2189b-88f4-4e15-b9f0-efefa6357b37";
            AssertPreviewCount(selectFacesNodeID, 5);
            for (int i = 0; i <= 4; i++)
            {
                var surface = GetPreviewValueAtIndex(selectFacesNodeID, i) as Surface;
                Assert.IsNotNull(surface);
            }

            // StructuralFramingTypes
            var structuralFramingTypesNodeID = GetPreviewValue("b786fdf5-b6fa-4aab-a2fd-b40ac12b8b87");
            Assert.IsNotNull(structuralFramingTypesNodeID);

            // StructuralColumnTypes
            var structuralColumnTypesNodeID = GetPreviewValue("6daf7b70-8693-46ff-8e9c-23798c8d31f8");
            Assert.IsNotNull(structuralColumnTypesNodeID);

            // Floor Type
            var floorTypesNodeID = GetPreviewValue("e718e402-13d1-458f-8b16-705aba49e3ac");
            Assert.IsNotNull(floorTypesNodeID);

            // Wall Type
            var wallTypesNodeID = GetPreviewValue("4ef3c2ed-4131-432d-9f1f-d48ae863cc23");
            Assert.IsNotNull(wallTypesNodeID);

            // Select Edge
            var selectEdgeNodeID = GetPreviewValue("90b176d6-a31e-4a06-b82e-c2991050eb96") 
                                                        as Autodesk.DesignScript.Geometry.Line;
            Assert.IsNotNull(selectEdgeNodeID);

            // Views
            var viewNodeID = GetPreviewValue
                ("a4070b7f-dddb-406e-bbe8-7c1dbe6b1d8f") as Revit.Elements.Views.FloorPlanView;
            Assert.IsNotNull(viewNodeID);
            
        }


        # region Private Methods

        private int DeleteWallAndRun<T>(string testGuid)
        {
            var walls = GetWalls();
   
            var wall = walls.FirstOrDefault();
            if (walls.Count == 0)
            {
                Assert.Fail("No more walls could be found in the model.");
            }
            using (var t = new Transaction(DocumentManager.Instance.CurrentDBDocument))
            {
                t.Start("Delete wall test.");
                DocumentManager.Instance.CurrentDBDocument.Delete(wall.Id);
                t.Commit();
            }

            walls = GetWalls();

            RunCurrentModel();

            var values = GetFlattenedPreviewValues(testGuid);
            Assert.AreEqual(values.Count, walls.Count);
            Assert.IsInstanceOf<T>(values.First());

            return walls.Count;
        }

        private List<Wall> GetWalls()
        {
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(Wall));

            return fec.ToElements().Cast<Wall>().ToList();
        }



        /// <summary>
        /// Find the first selection node in a graph, run the graph
        /// and assert that the returned object is valid. Then
        /// clear the selection and re-run, ensuring that the result
        /// is null.
        /// </summary>
        /// <typeparam name="T1">The type parameter for the selector method.</typeparam>
        /// <typeparam name="T2">The expected return type for elements in the selection.</typeparam>
        /// <param name="selectionType"></param>
        private void TestSelection<T1,T2>(SelectionType selectionType)
        {
            RunCurrentModel();

            // Find the first node of the specified selection type
            
            //LC Modularization repair: I don't think we can be as simplistic as choosing
            //the current workspace
            var selectNode =
                ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<RevitSelection<T1, T2>>();
            Assert.NotNull(selectNode);

            switch (selectionType)
            {
                case SelectionType.One:
                    TestSingleSelection(selectNode);
                    break;
                case SelectionType.Many:
                    TestMultipleSelection(selectNode);
                    break;
            }
        }

        /// <summary>
        /// Test running the node, then clearing the 
        /// selection and running again.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="selectNode"></param>
        private void TestSingleSelection<T1, T2>(SelectionBase<T1, T2> selectNode)
        {
            var element = GetPreviewValue(selectNode.GUID.ToString());
            Assert.NotNull(element);
            Assert.IsTrue(selectNode.State != ElementState.Warning);
            selectNode.ClearSelections();
            RunCurrentModel();
            element = GetPreviewValue(selectNode.GUID.ToString());
            Assert.Null(element);
            Assert.IsTrue(selectNode.State == ElementState.Warning);
        }

        /// <summary>
        /// Test running the node, then clearing the 
        /// selection and running again.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="selectNode"></param>
        private void TestMultipleSelection<T1, T2>(SelectionBase<T1, T2> selectNode)
        {
            var elements = GetPreviewCollection(selectNode.GUID.ToString());
            Assert.NotNull(elements);
            Assert.IsTrue(selectNode.State != ElementState.Warning);
            Assert.Greater(elements.Count(), 0);
            selectNode.ClearSelections();
            RunCurrentModel();
            elements = GetPreviewCollection(selectNode.GUID.ToString());
            Assert.Null(elements);
            Assert.IsTrue(selectNode.State == ElementState.Warning);
        }

        private void OpenAndAssertNoDummyNodes(string samplePath)
        {
            var testPath = Path.GetFullPath(samplePath);

            //open the test file
            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();
        }

        #endregion 
    }
}
