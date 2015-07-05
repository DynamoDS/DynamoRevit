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
    class TopographyTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void TopographyFromPoints()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Topography\TopographyFromPoints.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(13, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Topography.ByPoint
            var topographyID = "a0b02f6c-a144-4267-b62c-31983661aefa";
            var topography = GetPreviewValue(topographyID) as Topography;
            Assert.IsNotNull(topography);
        }

        [Test]
        [TestModel(@".\Topography\topography.rvt")]
        public void PointsFromTopography()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Topography\PointsFromTopography.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(2, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(1, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Topography.Points
            var pointsID = "c366f888-3b98-4101-9bd7-a1a84e538c61";
            AssertPreviewCount(pointsID, 1156);
            for (int i = 0; i < 1156; i++)
            {
                var point = GetPreviewValueAtIndex(pointsID, i) as Point;
                Assert.IsNotNull(point);
            }
            
        }
    }
}
