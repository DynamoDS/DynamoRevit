using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;

using Revit.Elements;

using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    class ToposolidTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void ToposolidFromPointsTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Toposolid\ToposolidFromPointsTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(14, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            var toposolidID = "4ee4adcf329b41b39bbbea243f77aba3";
            var toposolid = GetPreviewValue(toposolidID) as Toposolid;
            Assert.IsNotNull(toposolidID);
        }


        [Test]
        [TestModel(@".\Toposolid\toposolid.rvt")]
        public void PointsFromToposolid()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Toposolid\PointsFromToposolid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(2, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(1, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            var pointsID = "bccd461dc5f7460385efb965192d8f56";
            AssertPreviewCount(pointsID, 1156);
            for (int i = 0; i < 1156; i++)
            {
                var point = GetPreviewValueAtIndex(pointsID, i) as Point;
                Assert.IsNotNull(point);
            }
        }
    }
}