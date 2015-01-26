﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DSCoreNodesUI;
using Dynamo.Annotations;
using Dynamo.Models;
using Dynamo.Nodes;
using Dynamo.Search.SearchElements;
using Dynamo.Selection;

using NUnit.Framework;

using RevitServices.Persistence;
using RevitServices.Transactions;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class CoreTests : SystemTest
    {
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

            var xyzNode = ViewModel.Model.CurrentWorkspace.Nodes.First(x => x.NickName == "Point.ByCoordinates");
            Assert.IsNotNull(xyzNode);
            
            //test the shortest lacing
            xyzNode.ArgumentLacing = LacingStrategy.Shortest;

            RunCurrentModel();

            var fec = new FilteredElementCollector((Autodesk.Revit.DB.Document)DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(4, fec.ToElements().Count());

            //test the longest lacing
            xyzNode.ArgumentLacing = LacingStrategy.Longest;
            RunCurrentModel();

            fec = null;

            fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(5, fec.ToElements().Count());

            //test the cross product lacing
            xyzNode.ArgumentLacing = LacingStrategy.CrossProduct;

            RunCurrentModel();

            fec = null;

            fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ReferencePoint));
            Assert.AreEqual(20, fec.ToElements().Count());
        }

        [Test, Category("Failure")]
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
            var doubleNodes = ViewModel.Model.CurrentWorkspace.Nodes.Where(x => x is BasicInteractive<double>);
            BasicInteractive<double> node = doubleNodes.First() as BasicInteractive<double>;
            node.Value = node.Value + .1;

            ////run the expression again
            RunCurrentModel();

            //fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            //fec.OfClass(typeof(ReferencePoint));
            //Assert.AreEqual(1, fec.ToElements().Count());

            //finish out by restoring the original
            //initialDoc = DocumentManager.GetInstance().CurrentUIApplication.ActiveUIDocument;
            //shellPath = Path.Combine(workingDirectory, @"empty.rfa");
            //DocumentManager.GetInstance().CurrentUIApplication.OpenAndActivateDocument(shellPath);
            //initialDoc.Document.Close(false);

        }

        [Test, TestCaseSource("SetupCopyPastes")]
        [TestModel(@".\empty.rfa")]
        public void CanCopyAndPasteAllNodesOnRevit(NodeModelSearchElement searchElement)
        {
            var model = ViewModel.Model;

            searchElement.ProduceNode(); // puts the node into the current workspace

            var node = AllNodes.FirstOrDefault();

            DynamoSelection.Instance.ClearSelection();
            DynamoSelection.Instance.Selection.Add(node);
            Assert.AreEqual(1, DynamoSelection.Instance.Selection.Count);

            Assert.DoesNotThrow(() => model.Copy(), string.Format("Could not copy node : {0}", node.GetType()));
            Assert.DoesNotThrow(() => model.Paste(), string.Format("Could not paste node : {0}", node.GetType()));

            model.ClearCurrentWorkspace();
        }

        private IEnumerable<NodeModelSearchElement> SetupCopyPastes()
        {
            var excludes = new List<string>();
            excludes.Add("Input");
            excludes.Add("Output");

            return
                ViewModel.Model.SearchModel.SearchEntries.OfType<NodeModelSearchElement>()
                    .Where(x => !excludes.Contains(x.Name));
        }
    }
}
