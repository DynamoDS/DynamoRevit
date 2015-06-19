using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.IO;

using System.Linq;
using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    class SolidTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void BlendSolid()
        {

            string samplePath = Path.Combine(workingDirectory, @".\Solid\BlendSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(26, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(53, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Create List
            var listID = "238f2bff-076c-482f-9420-7c1111ac4b95";
            AssertPreviewCount(listID, 2);
            var list = GetFlattenedPreviewValues(listID);
            Assert.AreEqual(list[0].ToString(), list[1].ToString());

            //check Solid.Byloft
            var solidID = "b8d5c31d-e23d-4e18-bb17-ffce4da2f53e";
            var solid = GetPreviewValue(solidID) as Solid;
            Assert.IsNotNull(solid);
        }


      /*  [Test]
        [TestModel(@".\empty.rfa")]
        public void Loft()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\Loft.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(30, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(53, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            
        }

*/
        [Test]
        [TestModel(@".\empty.rfa")]
        public void RevolveSolid()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\RevolveSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(32, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(41, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Solid.ByRevolve
            var solidID = "22bcf6c3-d181-4809-b31c-fabb81497e56";
            var solid = GetPreviewValue(solidID) as Solid;
            Assert.IsNotNull(solid);

            //check Polycurve.ByThickeningCurve
            var polyCurveID = "17552e36-97a1-4187-a8b5-69085465a7dc";
            var polyCurve = GetPreviewValue(polyCurveID) as PolyCurve;
            Assert.AreEqual(polyCurve.ToString(), "PolyCurve(NumberOfCurves = 4)");

        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void SolidBySkeleton()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\SolidBySkeleton.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(28, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(57, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Create List, which contain a list of Line
            var listID = "623a22b5-34df-4542-9c79-1ae68f5bf706";
            AssertPreviewCount(listID, 3);
            for (int i = 0; i < 3; i++)
            {
                var line = GetPreviewValueAtIndex(listID, i) as Line;
                Assert.IsNotNull(line);
            }            
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void SweepToMakeSolid()
        {
            //var model = ViewModel.Model;

            //string samplePath = Path.Combine(workingDirectory, @".\Solid\SweepToMakeSolid.dyn");
            //string testPath = Path.GetFullPath(samplePath);

            //model.Open(testPath);
            //ViewModel.Model.RunExpression();

            //var sweepNode = ViewModel.Model.CurrentWorkspace.Nodes.First(x => x is CreateSweptGeometry);
            //var result = (Solid)VisualizationManager.GetDrawablesFromNode(sweepNode).Values.First();
            //double volumeMin = 11800.0;
            //double volumeMax = 12150.0;
            //double actualVolume = result.Volume;
            //Assert.Greater(actualVolume, volumeMin);
            //Assert.Less(actualVolume, volumeMax);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void SweptBlend()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\SweptBlend.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(22, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(31, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Autodesk.DesignScript.Geomery.Solid.BySweep
            var solidID = "7a8eb5f7-083e-435b-935b-3a2284061542";
            AssertPreviewCount(solidID, 3);
            for (int i = 0; i < 3; i++)
            {
                var solid = GetPreviewValueAtIndex(solidID, i) as Solid;
                Assert.IsNotNull(solid);
            }

            //check Geometry.Scale
            var pointID = "17984c90-0215-4404-add2-075d73d156b4";
            var point = GetPreviewValue(pointID) as Point;
            Assert.IsNotNull(point);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void BoxByCenterAndDimensions()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\BoxByCenterAndDimensions.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(3, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(3, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check cuboid.ByLengths
            var cuboidID = "63d69465-1892-4226-8610-9caf86db358c";
            var cuboid = GetPreviewValue(cuboidID) as Cuboid;
            Assert.IsNotNull(cuboid);     
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void BoxByTwoCorners()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\BoxByTwoCorners.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(6, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Cuboid.ByCorners
            var cuboidID = "9f02ea11-1af5-4b88-8890-5ed22c5a5455";
            var cuboid = GetPreviewValue(cuboidID) as Cuboid;
            Assert.IsNotNull(cuboid);
            Assert.AreEqual(cuboid.ToString(), "Cuboid(Length = 4.000, Width = 4.000, Height = 4.000)");
            }
    }
}
