using System.IO;
using System.Linq;

using Dynamo.Nodes;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using Revit.Elements;


namespace RevitSystemTests
{
    [TestFixture]
    class ViewTests : RevitSystemTestBase
    {

        [Test]
        [TestModel(@".\View\AxonometricView.rfa")]
        public void AxonometricView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\AxonometricView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\View\OverrideElementColorInView.rvt")]
        public void OverrideElementColorInView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\OverrideElementColorInView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(10, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(16, model.CurrentWorkspace.Connectors.Count());

            //check Element.OverrideColorInView
            var elementID = "99608c4e-c064-4486-a016-7221a5df2e3a";
            AssertPreviewCount(elementID, 100);
            for (int i = 0; i < 100; i++)
            {
                var element = GetPreviewValueAtIndex(elementID, i) as Element;
                Assert.IsNotNull(element);
            }
        }

        [Test]
        [TestModel(@".\View\PerspectiveView.rfa")]
        public void PerspectiveView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\PerspectiveView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test, TestModel(@".\Empty.rvt")]
        public void ExportAsImage()
        {
            string samplePath = Path.Combine(workingDirectory, @".\View\ExportImage.dyn");
            string testPath = Path.GetFullPath(samplePath);

            OpenDynamoDefinition(testPath);

            AssertNoDummyNodes();

            // Find the CBN and change it to have two temporary paths.
            var stringNodes = Model.CurrentWorkspace.Nodes.Where(x => x is StringInput).Cast<StringInput>().ToList();
            Assert.AreEqual(stringNodes.Count, 2);

            var tmp1 = Path.GetTempFileName();
            var tmp2 = Path.GetTempFileName();

            tmp1 = Path.ChangeExtension(tmp1, ".png");
            tmp2 = Path.ChangeExtension(tmp1, ".png");

            stringNodes[0].Value = tmp1;
            stringNodes[1].Value = tmp2;


            RunCurrentModel();


            // Ensure that our two temporary files have some data
            var tmp1Info = new FileInfo(tmp1);
            Assert.Greater(tmp1Info.Length, 0);

            var tmp2Info = new FileInfo(tmp2);
            Assert.Greater(tmp2Info.Length, 0);

        }
    }
}