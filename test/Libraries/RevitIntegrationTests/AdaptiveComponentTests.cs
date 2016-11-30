using System.IO;
using System.Linq;

using Dynamo.Tests;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using DoubleSlider = CoreNodeModels.Input.DoubleSlider;

using Revit.Elements;
using Dynamo.Graph.Nodes;
using RevitServices.Persistence;

namespace RevitSystemTests
{
    [TestFixture]
    class AdaptiveComponentTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentByFace.rfa")]
        public void AdaptiveComponentByFace()
        {
            // Create automation test for AdaptiveComponent
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3983

            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentByFace.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(12, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(13, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            //Check AdaptiveComponent.ByParamentersOnFace
            var adapID = "3e449f19-ffcd-418e-b4f7-1e37f6339150";
            AssertPreviewCount(adapID, 121);
            for (int i = 0; i < 121; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }

        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentByCurve.rfa")]
        public void AdaptiveComponentByCurve()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentByCurve.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(6, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(5, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check for number of Family Instance Creation
            var allElements = "272d86db-a124-48dd-9c41-6a3b17200e10";
            AssertPreviewCount(allElements, 10);

        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByPoints.rfa")]
        public void CreateAdaptiveComponentsByPoints()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByPoints.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "79637f91-d35b-49fc-bc54-4f5a1922633e";
            AssertPreviewCount(adapID, 6);
            for (int i = 0; i < 6; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByPoints.rfa")]
        public void CreateAdaptiveComponentsByPointsFrozen()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByPoints.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "79637f91-d35b-49fc-bc54-4f5a1922633e";
            AssertPreviewCount(adapID, 6);
            for (int i = 0; i < 6; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }

            //delete the adaptive components
            for (int i = 0; i < 6; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                adapValue.Dispose();
            }

            //now freeze adaptive component node
            var adaptnode = model.CurrentWorkspace.NodeFromWorkspace(adapID);
            adaptnode.IsFrozen = true;

            //rereun the graph;
            RunCurrentModel();

            //assert no adaptive components in the model
            Assert.AreEqual(0, GetAllFamilyInstances(true).Count);

        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByPointsOnFace.rfa")]
        public void CreateAdaptiveComponentsByPointsOnFace()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByPointsOnFace.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(11, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(10, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "21fb2ba2-b6bc-4d92-82bb-1d15a07d6929";
            AssertPreviewCount(adapID, 2);
            for (int i = 0; i < 2; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByParametersOnCurve.rfa")]
        public void CreateAdaptiveComponentsByParametersOnCurve()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByParametersOnCurve.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(5, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(4, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "780cbf5b-a3b7-445b-b39b-6e3908abd35b";
            AssertPreviewCount(adapID, 4);
            for (int i = 0; i < 4; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test, Ignore, Category("Failure")]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponent.rfa")]
        public void AdaptiveComponent()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponent.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            const string adaptiveComponentNodeId = "ac5bd8f9-fcf5-46db-b795-3590044edb56";
            AssertPreviewCount(adaptiveComponentNodeId, 5);

            var acNode = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x.GUID.ToString() == adaptiveComponentNodeId);
            Assert.NotNull(acNode);

            var adaptiveComponent = GetPreviewValueAtIndex(adaptiveComponentNodeId, 3) as Revit.Elements.AdaptiveComponent;
            Assert.IsNotNull(adaptiveComponent);

            // change slider value and re-evaluate graph
            var slider = model.CurrentWorkspace.NodeFromWorkspace
                ("91b7e7ef-9db3-4aa2-8762-6a863188e7ec") as DoubleSlider;
            slider.Value = 3;

            RunCurrentModel();
            AssertPreviewCount(adaptiveComponentNodeId, 3);

        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByPoints.rfa")]
        public void CreateAdaptiveComponentsByInvalidPoints()
        {
            // Regresion test case for http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-9099
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByPoints.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var adaptiveComponentCount = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.FamilyInstance>().Count();
            Assert.AreEqual(6, adaptiveComponentCount);

            var codeblocknode = ViewModel.Model.CurrentWorkspace.Nodes.OfType<CodeBlockNodeModel>().FirstOrDefault();
            Assert.IsNotNull(codeblocknode);

            var adaptiveNode = ViewModel.Model.CurrentWorkspace.NodeFromWorkspace("79637f91-d35b-49fc-bc54-4f5a1922633e");
            Assert.NotNull(adaptiveNode);

            // Now provide invalid points to AdaptiveComponet.ByPoints nodes.
            // All previously created components should be deleted.
            MakeConnector(codeblocknode, adaptiveNode, 4, 0);
            RunCurrentModel();

            adaptiveComponentCount = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.FamilyInstance>().Count();
            Assert.AreEqual(0, adaptiveComponentCount);
        }
    }
}