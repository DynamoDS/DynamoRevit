using System.IO;
using System.Linq;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class FloorTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\DifferentTypeRooms.rvt")]
        public void FloorNodes()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\FloorNodes.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var floorTypeName = GetPreviewValue("810634a9a60f41a2ab24833354eb6f54");
            Assert.AreEqual("Standard", floorTypeName);

            var floorTypeByName = GetPreviewValue("a450f0ac27ea4802902459aa1e2b374b");
            Assert.AreEqual("FloorTest", floorTypeByName.ToString());

            var categoryName = GetPreviewValue("de31ecaf910a407bb9d94fd8fe9c67a2");
            Assert.AreEqual("Floors", categoryName);

            var floorPlanView = GetPreviewValue("8fbbb68682a2473e9289ee30c476cfe6");
            var expectedValue = "FloorPlanView(Name = Level 0(1) )";
            Assert.AreEqual(expectedValue, floorPlanView.ToString());
        }

        [Test]
        [TestModel(@".\DifferentTypeRooms.rvt")]
        public void FloorPointsAddMove()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\FloorPointsAddMove.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            //On the first run of the script, only the added point will be read
            //Assert that the new node was added
            var floorPoints1Node = AllNodes
            .FirstOrDefault(n => n.Name == "Floor.Points1");
            var floorPoints1 = GetFlattenedPreviewValues(floorPoints1Node.GUID.ToString("N"));
            Assert.AreEqual(1, floorPoints1.Count);
            var pointAddedExceptedValue = "Point(X = 1000.000, Y = 0.000, Z = 0.000)";
            Assert.AreEqual(pointAddedExceptedValue, floorPoints1[0].ToString());


            var indexFloorPoints = AllNodes
            .OfType<CoreNodeModels.Input.DoubleInput>()
            .FirstOrDefault(n => n.Name == "IndexFloorPoints");
            indexFloorPoints.Value = "4.0";

            var movePoint = AllNodes
            .OfType<CoreNodeModels.Input.DoubleInput>()
            .FirstOrDefault(n => n.Name == "MovePoint");
            movePoint.Value = "8.0";

            //!!For some reason, double run in code for this script doesn't work as expected
            //The Floor.Points remains at 1 point so in order to make it work, we save as the script, close it and open the new one
            //Manually, if we do double run, it works fine
            string tempPath = Path.Combine(workingDirectory, "newScript.dyn");
            ViewModel.SaveAsCommand.Execute(tempPath);

            ViewModel.CloseHomeWorkspaceCommand.Execute(null);
            ViewModel.OpenCommand.Execute(tempPath);

            //Now we rerun the script, the new point will be moved on Z axis,
            //and the 4 points of the floor will be read and will appear in the canvas
            RunCurrentModel();

            floorPoints1Node = AllNodes
            .FirstOrDefault(n => n.Name == "Floor.Points1");
            floorPoints1 = GetFlattenedPreviewValues(floorPoints1Node.GUID.ToString("N"));
            Assert.AreEqual(5, floorPoints1.Count);


            //Verify the moved point
            var floorPoints2Node = AllNodes
            .FirstOrDefault(n => n.Name == "Floor.Points2");
            var floorPoints2 = GetFlattenedPreviewValues(floorPoints2Node.GUID.ToString("N"));
            Assert.AreEqual(5, floorPoints2.Count);
            var pointMovedExceptedValue = "Point(X = 1000.000, Y = 0.000, Z = 2438.400)";
            Assert.AreEqual(pointMovedExceptedValue, floorPoints2[4].ToString());
        }
    }
}
