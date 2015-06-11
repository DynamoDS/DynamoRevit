using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class LengthTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void Length()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Length\Length.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(5, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check length of String

            Assert.AreEqual(GetPreviewValue("9311b424-7887-4332-be51-82795d3e3ce6"), -27.341145833333318d);


        }
    }
}