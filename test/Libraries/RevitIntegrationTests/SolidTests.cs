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

        [Test]
        [TestModel(@".\empty.rfa")]
        public void RevolveSolid()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\RevolveSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(33, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(41, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Solid.ByRevolve
            var solidID = "e6179aa194db4c85a8eb988b61c36d03";
            var solid = GetPreviewValue(solidID) as Solid;
            Assert.IsNotNull(solid);

            //check Polycurve.ByThickeningCurve
            var polyCurveID = "37c32bbebac04b5280c7b5ccbe0c978b";
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
            var listID = "623a22b534df45429c791ae68f5bf706";
            AssertPreviewCount(listID, 3);
            for (int i = 0; i < 3; i++)
            {
                var line = GetPreviewValueAtIndex(listID, i) as Line;
                Assert.IsNotNull(line);
            }
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

            //check Solid.BySweep
            var solidID = "eca5b608c2314aa38ad0d3530e309bb8";
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

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ExtrudeMultiple()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\ExtrudeMultiple.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(17, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(18, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check both Rectangle.ByWidthLength nodes
            var rectangleID1 = "11639f45aaa449599b274a1f3c13cced";
            var rectangle1 = GetPreviewValue(rectangleID1) as Rectangle;
            Assert.AreEqual(rectangle1.ToString(), "Rectangle(Width = 5.000, Height = 5.000)");

            var rectangleID2 = "1ba7c2e9153d4c099d787a7e460b565a";
            var rectangle2 = GetPreviewValue(rectangleID2) as Rectangle;
            Assert.AreEqual(rectangle2.ToString(), "Rectangle(Width = 2.000, Height = 2.000)");

            //check Curve.ExtrudeAsSolid
            var solidID = "321139d19a7649f1a3048b2e6a358a30";
            AssertPreviewCount(solidID, 2);
            for (int i = 0; i < 2; i++)
            {
                var solid = GetPreviewValueAtIndex(solidID, i) as Solid;
                Assert.IsNotNull(solid);
                Assert.IsTrue(solid.Volume > 0);
            }
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void Extrusion()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Solid\Extrusion.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Curve.ExtrudeAsSolid
            var solidID = GetFlattenedPreviewValues("321139d19a7649f1a3048b2e6a358a30");
            Assert.AreEqual(1, solidID.Count());
            var solid = solidID.FirstOrDefault() as Solid;
            Assert.IsNotNull(solid);
            Assert.IsTrue(solid.Volume > 0);
        }
    }
}