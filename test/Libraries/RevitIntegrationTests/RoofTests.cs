using System.IO;
using NUnit.Framework;
using RTF.Framework;
using RevitTestServices;
using Dynamo.Graph.Nodes.ZeroTouch;
using System.Linq;

namespace RevitSystemTests
{
    [TestFixture]
    class RoofTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByOutlineExtrusionTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Roof\ByOutLineExtrusionTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("9daa9b583bdf4659901779b540fdeecc");
            Assert.AreEqual(typeof(Revit.Elements.Roof), type.GetType());
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByOutlineTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Roof\ByOutlineTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("c6d16ef7c551467a8d46cd307b2a465b");
            Assert.AreEqual(typeof(Revit.Elements.Roof), type.GetType());
        }

        [Test]
        [TestModel(@".\DifferentTypeRooms.rvt")]
        public void RoofAddAndMovePoint()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RoofAddAndMovePoint.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            var roofByOutline = GetFlattenedPreviewValues("58315d46b9934fafb62bceaca05b2255");
            Assert.AreEqual(typeof(Revit.Elements.Roof), roofByOutline[0].GetType());

            var roofPoints1 = AllNodes
            .FirstOrDefault(n => n.Name == "roof.Points1");

            var roofByOutlineNode = GetNode<DSFunction>("58315d46-b993-4faf-b62b-ceaca05b2255");
            var roofPoints1Node = GetNode<DSFunction>("0a224bb8-6557-43fc-bb6f-b5c1e002bbf0");

            //Connect the output of Roof.ByOutlineTypeAndLevel node to the input of Roof.Points1 node
            MakeConnector(roofByOutlineNode, roofPoints1Node, 0, 0);
            RunCurrentModel();

            var roofPoints1Value = GetFlattenedPreviewValues(roofPoints1Node.GUID.ToString("N"));
            Assert.AreEqual(7, roofPoints1Value.Count);
            var firstPoint1 = roofPoints1Value[0] as Autodesk.DesignScript.Geometry.Point;
            Assert.AreEqual(-9976.661, firstPoint1.X, Tolerance);
            Assert.AreEqual(-7119.928, firstPoint1.Y, Tolerance);
            Assert.AreEqual(4350.500, firstPoint1.Z, Tolerance);

            var roofPoints2 = AllNodes
            .FirstOrDefault(n => n.Name == "roof.Points2");
            var roofPoints2Node = GetNode<DSFunction>("198154aa-94e4-4857-aa4c-6980dd1eef98");

            //Connect the output of Roof.ByOutlineTypeAndLevel node to the input of Roof.Points2 node
            MakeConnector(roofByOutlineNode, roofPoints2Node, 0, 0);
            RunCurrentModel();

            var roofPoints2Value = GetFlattenedPreviewValues(roofPoints2Node.GUID.ToString("N"));
            Assert.AreEqual(7, roofPoints2Value.Count);
            var firstPoint2 = roofPoints2Value[0] as Autodesk.DesignScript.Geometry.Point;
            Assert.AreEqual(-9976.661, firstPoint2.X, Tolerance);
            Assert.AreEqual(-7119.928, firstPoint2.Y, Tolerance);
            Assert.AreEqual(4960.100, firstPoint2.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void RoofByOutlineExtrusionTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RoofByOutlineExtrusionTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var roofByOutline = GetPreviewValue("541167eee9a9407faf6e036fecbc61c0");
            Assert.AreEqual(typeof(Revit.Elements.Roof), roofByOutline.GetType());

            var roofTypeName = GetPreviewValue("495506b35585413389c1486863480f32");
            Assert.AreEqual("Roof_Generic-400mm", roofTypeName);
        }
    }
}
