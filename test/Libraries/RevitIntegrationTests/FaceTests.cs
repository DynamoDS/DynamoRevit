using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;

namespace RevitSystemTests
{
    [TestFixture]
    internal class FaceTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Face\GetSurfaceDomain.rvt")]
        public void GetSurfaceDomain()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Face\GetSurfaceDomain.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(7 ,model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(7, model.CurrentWorkspace.Connectors.Count());

            //check UV.Bycoordinate
            var uvID = "87702b56-9085-4c8e-89c0-3b2d7052c2dc";
            AssertPreviewCount(uvID, 9);
            var uvList = GetFlattenedPreviewValues(uvID);
            Assert.AreEqual(uvList.Count(), 9);
            foreach (var element in uvList)
            {
                Assert.IsNotNull(element);
                Assert.AreEqual(element.GetType().ToString(), "Autodesk.DesignScript.Geometry.UV");
            }

            
        }
    }
}
