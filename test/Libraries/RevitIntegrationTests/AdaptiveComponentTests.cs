using System.IO;
using System.Linq;

using Dynamo.Tests;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using DoubleSlider = DSCoreNodesUI.Input.DoubleSlider;

using Revit.Elements;

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
            Assert.AreEqual(11, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(12, model.CurrentWorkspace.Connectors.Count());

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
            var allElements = "e83c14bb-864f-4730-900f-0905dac6dcad";
            AssertPreviewCount(allElements, 10);

        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByPointsInBatch.rfa")]
        public void CreateAdaptiveComponentsByPointsInBatch()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByPointsInBatch.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(4, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "17138c91-24be-4ad7-96ef-7dbb9a6793ad";
            AssertPreviewCount(adapID, 6);
            for (int i = 0; i < 6; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByFaceInBatch.rfa")]
        public void CreateAdaptiveComponentsByFaceInBatch()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByFaceInBatch.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(11, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(10, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "1745f4d3-df8d-4b53-95d8-59b4762cfd1d";
            AssertPreviewCount(adapID, 2);
            for (int i = 0; i < 2; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test]
        [TestModel(@".\AdaptiveComponent\AdaptiveComponentsByCurveInBatch.rfa")]
        public void CreateAdaptiveComponentsByCurveInBatch()
        {
            var model = ViewModel.Model;

            string testFilePath = Path.Combine(workingDirectory, @".\AdaptiveComponent\AdaptiveComponentsByCurveInBatch.dyn");
            string testPath = Path.GetFullPath(testFilePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // Check all the nodes and connectors are loaded
            Assert.AreEqual(5, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(4, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Check the number of the created adaptive components is correct
            var adapID = "0a9173c2-3101-4619-a959-0f1f99470c99";
            AssertPreviewCount(adapID, 4);
            for (int i = 0; i < 4; i++)
            {
                var adapValue = GetPreviewValueAtIndex(adapID, i) as AdaptiveComponent;
                Assert.IsNotNull(adapValue);
            }
        }

        [Test]
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
    }
}