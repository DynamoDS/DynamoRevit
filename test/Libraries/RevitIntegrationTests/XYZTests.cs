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

            Assert.AreEqual(4, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(5, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check ReferencePoint.ByPoint
            var referencePointID = "8ba88ada-90b2-440f-bc7c-8963cf6a57ae";
            var referencePoint = GetPreviewValue(referencePointID) as ReferencePoint;
            Assert.IsNotNull(referencePoint);

            //check ReferencePoint.Point
            var pointID = "9e68916a-5a82-460e-9022-4e09f9898651";
            var point = GetPreviewValue(pointID) as Point;
            Assert.AreEqual(point.X, 20);
            Assert.AreEqual(point.Y, 5);
            Assert.AreEqual(point.Z, -8);
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void XYZByCylindricalCoordinates()
        {
            string samplePath = Path.Combine(workingDirectory, @".\XYZ\XYZByCylindricalCoordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();

            Assert.AreEqual(7, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(6, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Point.ByCylindricalCoordinates
            var pointsID = "ed549713-e1bb-421b-a1be-9c71530bbf7f";
            AssertPreviewCount(pointsID, 63);
            for (int i = 0; i < 63; i++)
            {
                var point = GetPreviewValueAtIndex(pointsID, i) as Point;
                Assert.IsNotNull(point);
            }

            var expectedValue = "Point(X = 1.081, Y = 1.683, Z = 10.000)";
            Assert.AreEqual(expectedValue, GetPreviewValueAtIndex(pointsID, 10).ToString());
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

            Assert.AreEqual(11, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check Point.BysphericalCoordinates
            var pointID = "8e868a19-10d6-4d3c-a86d-e94cfa0c4899";
            var point = GetFlattenedPreviewValues(pointID);
            Assert.AreEqual(896, point.Count());
            foreach (var elem in point)
            {
                Assert.AreEqual(elem.GetType(), typeof(Point));
                Assert.IsNotNull(elem as Point);
            }

            var expectedValue = "Point(X = 0.085, Y = 0.181, Z = 1.990)";
            Assert.AreEqual(expectedValue, point[33].ToString());
        }
    }
}