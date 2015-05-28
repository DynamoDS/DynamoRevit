using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using Revit.Elements;

using System.Linq;

using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    public class XYZTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZFromReferencePoint()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZFromReferencePoint.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(3, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(2, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check ReferencePoint.ByPoint
            var referencePointID = "8ba88ada-90b2-440f-bc7c-8963cf6a57ae";
            var referencePoint = GetPreviewValue(referencePointID) as ReferencePoint;
            Assert.IsNotNull(referencePoint);

            //check ReferencePoint.Point
            var pointID = "9e68916a-5a82-460e-9022-4e09f9898651";
            var point = GetPreviewValue(pointID) as Point;
            Assert.AreEqual(point.X, 0);
            Assert.AreEqual(point.Y, 0);
            Assert.AreEqual(point.Z, 0);
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZByPolar()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZByPolar.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(7, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(6, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Point.ByCylindricalCoordinates
            var pointsID = "ed549713-e1bb-421b-a1be-9c71530bbf7f";
            AssertPreviewCount(pointsID, 63);
            for (int i = 0; i < 63; i++)
            {
                var point = GetPreviewValueAtIndex(pointsID, i) as Point;
                Assert.IsNotNull(point);
            }           
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZBySphericalCoordinates()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZBySphericalCoordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();

            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Point.BysphericalCoordinates
            var pointID = "8e868a19-10d6-4d3c-a86d-e94cfa0c4899";
            var point = GetFlattenedPreviewValues(pointID);
            foreach (var ele in point)
            {
                Assert.AreEqual(ele.GetType(), typeof(Point));
            }
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZToPolarCoordinates()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZToPolarCoordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(7, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(6, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check XYZ to Polar Coordinate
            var cbnID = "8f623cc8-4997-497d-995b-db382db894b3";
            Assert.AreEqual(3, GetPreviewValue(cbnID));

            //check Point.ByCylindricalCoordinates
            var pointID = "8fb1a596-774f-4084-a8e2-eb637dcf9e20";
            var point = GetPreviewValue(pointID);
            Assert.AreEqual(point.ToString(), "Point(X = 0.725, Y = 1.864, Z = 3.000)");            
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZToSphericalCoordinates()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZToSphericalCoordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(12, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(13, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check XYZ to Spherical Coordinate
            var cbnID = "021905ea-d01d-487b-80a6-6d98ca740ede";
            Assert.AreEqual(System.Math.Ceiling(System.Convert.ToDouble(GetPreviewValue(cbnID))), 1);

            //check Point.BySphericalCoordinates
            var point1ID = "4225bb93-5b5d-47c6-800e-6660dc2e6035";
            var point1 = GetPreviewValue(point1ID) as Point;
            var point2ID = "c9213a06-5491-4828-a3fd-1ee7a9f2867a";
            var point2 = GetPreviewValue(point2ID) as Point;
            Assert.AreEqual(point1.ToString(), point2.ToString());            
        }
    }
}
