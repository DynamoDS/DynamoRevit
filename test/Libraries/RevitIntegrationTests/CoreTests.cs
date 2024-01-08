using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CoreNodeModels;
using CoreNodeModels.Input;
using Dynamo.Applications.Models;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Search.SearchElements;
using Dynamo.Selection;
using Dynamo.Tests;
using Dynamo.ViewModels;
using NUnit.Framework;

using RevitServices.Persistence;
using RevitServices.Transactions;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class CoreTests : RevitSystemTestBase
    {

        protected static CoreTests s_initedInstance = null;

        [SetUp]
        public override void Setup()
        {
            if (s_initedInstance == null)
            {
                base.Setup();

                ViewModel.Model.AddZeroTouchNodesToSearch(ViewModel.Model.LibraryServices.GetAllFunctionGroups());

                s_initedInstance = this;
            }
            else
            {
                WrapOf(s_initedInstance);
                CreateTemporaryFolder();

                // This is needed because TearDown will clear the Model, and TearDown
                //  is needed in order to clear the dependencyGraph, otherwise adding
                //  a node will take exponentially long time
                (ViewModel.Model as RevitDynamoModel).InitializeDocumentManager();
            }
        }

        protected override void GetLibrariesToPreload(List<string> libraries)
        {
            // Add multiple libraries to better simulate typical Dynamo application usage.

            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var revitNodesDirectory = Path.Combine(assemblyDirectory, "nodes");
            var revitUINodesDll = Path.Combine(assemblyDirectory, @"nodes\DSRevitNodesUI.dll");
            var revitAANodesDirectory = Path.Combine(assemblyDirectory, @"nodes\analytical-automation-pkg\bin");
            var revitAANodesDll = Path.Combine(assemblyDirectory, @"nodes\analytical-automation-pkg\bin\AnalyticalAutomation.dll");
            var revitAAUINodesDll = Path.Combine(assemblyDirectory, @"nodes\analytical-automation-pkg\bin\AnalyticalAutomationGUI.dll");
            libraries.Add(revitNodesDirectory);
            //libraries.Add(revitUINodesDll); // UI nodes seem to have a problem being loaded in this context
            //libraries.Add(revitAANodesDirectory); // do not load AA nodes, they have their own tests, for now we believe this is not really needed
            //libraries.Add(revitAANodesDll); // UI nodes seem to have a problem being loaded in this context

            base.GetLibrariesToPreload(libraries);
        }

        /// <summary>
        /// Sanity Check graph should always have nodes that error.
        /// </summary>
        [Test]
        [TestModel(@".\empty.rfa")]
        public void SanityCheck()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Core\SanityCheck.dyn");
            string testPath = Path.GetFullPath(samplePath);

            //Assert that there are some errors in the graph
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var errorNodes = AllNodes.Where(x => x.State == ElementState.Warning);
            Assert.Greater(errorNodes.Count(), 0);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CanChangeLacingAndHaveElementsUpdate()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Core\LacingTest.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            var xyzNode = ViewModel.Model.CurrentWorkspace.Nodes.First(x => x.Name == "Point.ByCoordinates");
            Assert.IsNotNull(xyzNode);

            //test the shortest lacing
            xyzNode.UpdateValue(new UpdateValueParams("ArgumentLacing", "Auto"));

            RunCurrentModel();

            var fec = new FilteredElementCollector((Autodesk.Revit.DB.Document)DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(4, fec.ToElements().Count());

            //test the longest lacing
            xyzNode.UpdateValue(new UpdateValueParams("ArgumentLacing", "Longest"));
            RunCurrentModel();

            fec = null;

            fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(5, fec.ToElements().Count());

            //test the cross product lacing
            xyzNode.UpdateValue(new UpdateValueParams("ArgumentLacing", "CrossProduct"));

            RunCurrentModel();

            fec = null;

            fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(20, fec.ToElements().Count());
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void SwitchDocuments()
        {
            //open the workflow and run the expression
            string testPath = Path.Combine(workingDirectory, @".\ReferencePoint\ReferencePoint.dyn");
            ViewModel.OpenCommand.Execute(testPath);
            Assert.AreEqual(3, ViewModel.Model.CurrentWorkspace.Nodes.Count());

            RunCurrentModel();

            //verify we have a reference point
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(1, fec.ToElements().Count());

            //open a new document and activate it
            var initialDoc = (UIDocument)DocumentManager.Instance.CurrentUIDocument;
            string shellPath = Path.Combine(workingDirectory, @".\empty1.rfa");
            TransactionManager.Instance.ForceCloseTransaction();
            DocumentManager.Instance.CurrentUIApplication.OpenAndActivateDocument(shellPath);

            initialDoc.Document.Close(false);

            ////assert that the doc is set on the DocumentManager
            Assert.IsNotNull((Document)DocumentManager.Instance.CurrentDBDocument);

            ////update the double node so the graph reevaluates
            var doubleNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<DoubleInput>();
            doubleNode.Value = doubleNode.Value + .1;

            ////run the expression again
            RunCurrentModel();

            fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(1, fec.ToElements().Count());

        }

        [Test, TestCaseSource(nameof(SetupCopyPastes))]
        [TestModel(@".\empty.rfa")]
        public void CanCopyAndPasteAllNodesOnRevit(NodeSearchElement searchElement)
        {
            var model = ViewModel.Model;

            //searchElement.ProduceNode(); // puts the node into the current workspace
            var nodeModel = searchElement.CreateNode();
            model.CurrentWorkspace.AddAndRegisterNode(nodeModel, false);

            var node = AllNodes.FirstOrDefault();

            DynamoSelection.Instance.ClearSelection();
            DynamoSelection.Instance.Selection.Add(node);
            Assert.AreEqual(1, DynamoSelection.Instance.Selection.Count);

            Assert.DoesNotThrow(() => model.Copy(), string.Format("Could not copy node : {0}", node.GetType()));
            Assert.DoesNotThrow(() => model.Paste(), string.Format("Could not paste node : {0}", node.GetType()));

            model.CurrentWorkspace.RemoveAndDisposeNode(nodeModel);
            model.ClearCurrentWorkspace();
        }

        private static IEnumerable<TestCaseData> SetupCopyPastes()
        {
            var xxx = new TestCaseData();
            var excludes = new List<string>
            {
                "Input",
                "Output"
            };

            var listOfAllNodes = Enumerable.Empty<NodeSearchElement>();
            DynamoViewModel viewModel = s_initedInstance?.ViewModel;
            if (viewModel != null)
                try
                {
                    listOfAllNodes = viewModel.Model.SearchModel.Entries.Where(x => !excludes.Contains(x.Name) && x.FullName.Contains("Revit"));
                }
                catch (System.Exception) { }
            return listOfAllNodes.Select(nse => new TestCaseData(nse).SetName(nse.FullName));
        }
    }
}
