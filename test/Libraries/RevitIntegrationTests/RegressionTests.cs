using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dynamo.Applications.Models;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Search.SearchElements;
using NUnit.Framework;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using static Dynamo.ViewModels.SearchViewModel;

namespace RevitSystemTests
{
    [TestFixture]
    public class RegressionTest : RevitSystemTestBase
    {
        //protected static RegressionTest s_initedInstance = null;

        //[SetUp]
        //public override void Setup()
        //{
        //    if (s_initedInstance == null)
        //    {
        //        base.Setup();

        //        ViewModel.Model.AddZeroTouchNodesToSearch(ViewModel.Model.LibraryServices.GetAllFunctionGroups());

        //        s_initedInstance = this;
        //    }
        //    else
        //    {
        //        WrapOf(s_initedInstance);
        //        CreateTemporaryFolder();

        //        // This is needed because TearDown will clear the Model, and TearDown
        //        //  is needed in order to clear the dependencyGraph, otherwise adding
        //        //  a node will take exponentially long time
        //        (ViewModel.Model as RevitDynamoModel).InitializeDocumentManager();
        //    }
        //}

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
        /// Automated creation of regression test cases. Opens each workflow
        /// runs it, and checks for errors or warnings. Regression test cases should
        /// be structured such that they do not yield warnings or errors.
        /// </summary>
        /// <param name="dynamoFilePath">The path of the dynamo workspace.</param>
        /// <param name="revitFilePath">The path of the Revit rfa or rvt file.</param>
        [Test]
        [TestCaseSource(nameof(SetupRevitRegressionTests))]
        public void Regressions(RegressionTestData testData)
        {
            Exception exception = null;

            try
            {
                var dynamoFilePath = testData.Arguments[0].ToString();
                var revitFilePath = testData.Arguments[1].ToString();

                //ensure that the incoming arguments are not empty or null
                //if a dyn file is found in the regression tests directory
                //and there is no corresponding rfa or rvt, then an empty string
                //or a null will be passed into here.
                Assert.IsNotNull(dynamoFilePath, "Dynamo file path is invalid or missing.");
                Assert.IsNotEmpty(dynamoFilePath, "Dynamo file path is invalid or missing.");
                Assert.IsNotNull(revitFilePath, "Revit file path is invalid or missing.");
                Assert.IsNotEmpty(revitFilePath, "Revit file path is invalid or missing.");

                //open the revit model
                SwapCurrentModel(revitFilePath);

                //Ensure SystemTestBase picks up the right directory.
                pathResolver = new RevitTestPathResolver();
                (pathResolver as RevitTestPathResolver).InitializePreloadedLibraries();

                //Setup should be called after swapping document, so that RevitDynamoModel 
                //is now associated with swapped model.
                Setup();

                //open the dyn file
                ViewModel.OpenCommand.Execute(dynamoFilePath);
                Assert.IsTrue(ViewModel.Model.CurrentWorkspace.Nodes.Any());
                AssertNoDummyNodes();

                //run the expression and assert that it does not
                //throw an error

                RunCurrentModel();

                var errorNodes =
                    ViewModel.Model.CurrentWorkspace.Nodes.Where(
                        x => x.State == ElementState.Error || x.State == ElementState.Warning);
                Assert.AreEqual(0, errorNodes.Count());
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            // this is not needed anymore, NUnit ensures this happens between each test case
            //finally
            //{
            //    TearDown();
            //}

            if (exception != null)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void NoUINodesLeftBehind()
        {
            // This verifies Revit UI Nodes are not lost in translation by comparing
            // the overall node count on the active ws pre and post save

            // Assertion variables
            int preSaveNodeCount = 0;
            int postSaveNodeCount = 0;

            // Setup home ws
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            // Iterate through all loaded nodes in library & add Revit UI nodes to ws
            //var nodeList = Model.SearchModel.Search(string.Empty, Model.LuceneUtility);
            var nodeList = Model.SearchModel.Entries;
            foreach (var node in nodeList)
            {
                var assembly = Path.GetFileName(node.Assembly);

                var searchElement = node as NodeSearchElement;
                if (assembly == "DSRevitNodesUI.dll")
                {
                    var currentNode = searchElement.CreateNode();
                    Model.AddNodeToCurrentWorkspace(currentNode, true);
                }
            }

            // Number of nodes sucessfully added to ws before saving
            preSaveNodeCount = Model.CurrentWorkspace.Nodes.Count();

            // Save workspace
            ViewModel.CurrentSpace.Save(Path.Combine(workingDirectory, @".\AllNodes.dyn"));

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Open Json temp file
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\AllNodes.dyn"));
            RunCurrentModel();

            // Number of nodes sucessfully restored to ws after saving/opening
            postSaveNodeCount = Model.CurrentWorkspace.Nodes.Count();

            // Active node count is the same after reopening saved file
            Assert.IsTrue(preSaveNodeCount == postSaveNodeCount);

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Delete temp file
            File.Delete(Path.Combine(workingDirectory, @".\AllNodes.dyn"));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void NoDuplicatePortsOnUINodes()
        {
            // This test verifies Revit UI Nodes are not producing duplicate
            // outports upon deserialization due to a lack of a JSON constructor

            // Assertion variables
            List<int> preSaveOutPortCount = new List<int>();
            List<int> postSaveOutPortCount = new List<int>();
            List<string> nodeCreationNames = new List<string>();
            string outputData = "";

            // Setup home ws
            var homespace = Model.CurrentWorkspace as HomeWorkspaceModel;
            Assert.NotNull(homespace, "The current workspace is not a HomeWorkspaceModel");

            // Iterate through all loaded nodes in library & add Revit UI nodes to ws
            var nodeList = Model.SearchModel.Entries;
            foreach (var node in nodeList)
            {
                var searchElement = node as NodeSearchElement;
                var assembly = Path.GetFileName(node.Assembly);

                if (assembly == "DSRevitNodesUI.dll")
                {
                    var currentNode = searchElement.CreateNode();
                    Model.AddNodeToCurrentWorkspace(currentNode, true);
                    nodeCreationNames.Add(searchElement.CreationName);
                }
            }

            // Build list of outport counts for each active nodes before saving
            foreach (var node in ViewModel.CurrentSpace.Nodes)
            {
                preSaveOutPortCount.Add(node.OutPorts.Count);
            }

            // Save workspace
            ViewModel.CurrentSpace.Save(Path.Combine(workingDirectory, @".\DupePorts.dyn"));

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Open Json temp file
            OpenAndAssertNoDummyNodes(Path.Combine(workingDirectory, @".\DupePorts.dyn"));
            RunCurrentModel();

            // Build list of outport counts for each sucessfully restored nodes after saving/opening
            foreach (var node in ViewModel.CurrentSpace.Nodes)
            {
                postSaveOutPortCount.Add(node.OutPorts.Count);
            }

            // PortCount lists have matching lengths pre/post save
            Assert.IsTrue(preSaveOutPortCount.Count == postSaveOutPortCount.Count);
            // PortCount values for each node match pre/post save
            for (int i = 0; i < preSaveOutPortCount.Count; i++)
            {
                // Write node name to the xml results if assertion is false
                if (preSaveOutPortCount[i] != postSaveOutPortCount[i])
                {
                    outputData += nodeCreationNames[i] + ", ";
                }
            }

            if (outputData.Length > 0)
            {
                Assert.Fail("OutPort Count Inconsistency: " + outputData.Remove(outputData.Length - 2));
            }

            // Close workspace
            Assert.IsTrue(ViewModel.CloseHomeWorkspaceCommand.CanExecute(null));
            ViewModel.CloseHomeWorkspaceCommand.Execute(null);

            // Delete temp file
            File.Delete(Path.Combine(workingDirectory, @".\DupePorts.dyn"));
        }

        // Helper Functions

        private void OpenAndAssertNoDummyNodes(string samplePath)
        {
            var testPath = Path.GetFullPath(samplePath);

            // Open the test file
            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();
        }

        /// <summary>
        /// Method referenced by the automated regression testing setup method.
        /// Populates the test cases based on file pairings in the regression tests folder.
        /// </summary>
        /// <returns></returns>
        private static List<RegressionTestData> SetupRevitRegressionTests()
        {
            var testParameters = new List<RegressionTestData>();

            var config = RevitTestConfiguration.LoadConfiguration();
            string testsLoc = Path.Combine(config.WorkingDirectory, "Regression");
            var regTestPath = Path.GetFullPath(testsLoc);

            var di = new DirectoryInfo(regTestPath);
            foreach (var folder in di.GetDirectories())
            {
                var dyns = folder.GetFiles("*.dyn");
                foreach (var fileInfo in dyns)
                {
                    var data = new object[2];
                    data[0] = fileInfo.FullName;

                    //find the corresponding rfa or rvt file
                    var nameBase = fileInfo.FullName.Remove(fileInfo.FullName.Length - 4);
                    var rvt = nameBase + ".rvt";
                    var rfa = nameBase + ".rfa";

                    //add test parameters for rvt, rfa, or both
                    if (File.Exists(rvt))
                    {
                        data[1] = rvt;
                    }

                    if (File.Exists(rfa))
                    {
                        data[1] = rfa;
                    }

                    testParameters.Add(new RegressionTestData { Arguments = data, TestName = folder.Name });
                }
            }


            return testParameters;
        }
    }

    public class RegressionTestData
    {
        public object[] Arguments { get; set; }
        public string TestName { get; set; }
        public override string ToString()
        {
            return TestName ?? "RegressionTest";
        }
    }
}
