using System.IO;

using NUnit.Framework;

using RevitTestServices;

using System.Linq;

namespace RevitSystemTests
{
    [TestFixture]
    class PythonTests : RevitSystemTestBase
    {
        [Test]
        public void CanAddTwoInputsWithPython()
        {
            string graph = Path.Combine(workingDirectory, @".\Python\Python_add.dyn");
            string testPath = Path.GetFullPath(graph);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(5, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(4, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check pythonScript
            var pythonID = "234459e8-97c7-469c-b474-5fa268d9606a";
            Assert.AreEqual(GetPreviewValue(pythonID), 5);

        }
    }
}