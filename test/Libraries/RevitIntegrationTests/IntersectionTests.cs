using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;

using Autodesk.DesignScript.Geometry;

using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class IntersectionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Intersect\CurveCurveIntersection.rfa")]
        public void CurveCurveIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\CurveCurveIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(17, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(26,model.CurrentWorkspace.Connectors.Count());

            //check Geometry.Intersect
            var pointID = "6781ed8d-1a93-493e-ad61-a390b09ea0c2";
            AssertPreviewCount(pointID, 4);
            for (int i = 0; i < 4; i++)
            {
                var point = GetPreviewValueAtIndex(pointID, i) as Point;
                Assert.IsNotNull(point);
            }      
            //check Geometry.Intersect
            var emptyID = "d04b07f3-948a-4229-975c-70cdd8743b08";
            AssertPreviewCount(emptyID, 0);           
        }

     /* 
      * [Test]
        [TestModel(@".\Intersect\CurveFaceIntersection.rfa")]
        public void CurveFaceIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\CurveFaceIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            Assert.AreEqual(16, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(28, ViewModel.Model.CurrentWorkspace.Connectors.Count());
            
        }
       */

        [Test]
        [TestModel(@".\Intersect\FaceFaceIntersection.rfa")]
        public void FaceFaceIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\FaceFaceIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(9, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Geometry.Intersect
            var nurbsCurveID = "4d491f10-5390-4bd0-92c2-9e00c088e4f7";
            AssertPreviewCount(nurbsCurveID, 1);
            var nurbsCurve = GetFlattenedPreviewValues(nurbsCurveID);
            Assert.IsNotNull(nurbsCurve[0] as NurbsCurve);
      
            //check Geometry.Intersect
            var nurbsCurve2ID = "d4357e6a-3ed9-4559-af7c-5624ed71b142";
            var nurbsCurve2 = GetFlattenedPreviewValues(nurbsCurve2ID);
            Assert.IsNotNull(nurbsCurve2[0] as NurbsCurve);

            //check Geometry.Intersect
            var nurbsCurve3ID = "e59a06db-dfc8-46cb-9113-657b820bc598";
            Assert.AreEqual(GetPreviewValue(nurbsCurve3ID), null);                      
        }

        [Test]
        [TestModel(@".\Intersect\EdgePlaneIntersection.rfa")]
        public void EdgePlaneIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\EdgePlaneIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            Assert.AreEqual(20, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(19, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Geometry.Intersect
            var lineID = "dd081ef7-9645-4ee0-a1b3-5ed834be25c2";
            var line = GetFlattenedPreviewValues(lineID);
            Assert.AreEqual(line.Count(), 1);
            Assert.AreEqual(line[0].GetType().ToString(), "Autodesk.DesignScript.Geometry.Line");
            Assert.IsNotNull(line[0]);
        }
    }
}
