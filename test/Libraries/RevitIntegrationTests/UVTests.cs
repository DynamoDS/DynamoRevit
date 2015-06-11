using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;

using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    internal class UVTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void UVRandom()
        {
            string samplePath = Path.Combine(workingDirectory, @".\UV\UVRandom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            Assert.AreEqual(8, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check UV.ByCoordinates
            var uvID = "436bb218-98ca-40d8-b3d4-53c48238aabd";
            var uv = GetFlattenedPreviewValues(uvID);
            Assert.AreEqual(uv.Count(), 25);
            for (int i = 0; i < 25; i++)
            {
                Assert.IsNotNull(uv[i]);
            }
            
        }
    }
}
