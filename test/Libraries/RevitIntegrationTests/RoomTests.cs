using System.IO;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using Autodesk.Revit.DB.Architecture;


namespace RevitSystemTests
{
    [TestFixture]
    class RoomTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Room()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Room.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("2774f84d-e233-46a1-aa8d-3aa884757207");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.Room));
        }

        [Test]
        [TestModel(@".\Room2025.rvt")]
        public void IsInsideRoom()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\IsInsideRoom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var isInsideRoom = GetFlattenedPreviewValues("747d3bf362054cb49ab8169f569cb027");
            Assert.AreEqual(2, isInsideRoom.Count);
            Assert.AreEqual(true, isInsideRoom[0]);
            Assert.AreEqual(false, isInsideRoom[1]);
        }

        [Test]
        [TestModel(@".\Room2025.rvt")]
        public void LocationAndVolumeRoom()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\LocationAndVolumeRoom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var roomLocation = GetPreviewValue("3594f81581cd4b8b9e68ef308eefcaa3") as Autodesk.DesignScript.Geometry.Point;
            Assert.AreEqual(5895.952, roomLocation.X, Tolerance);
            Assert.AreEqual(1686.784, roomLocation.Y, Tolerance);
            Assert.AreEqual(0.000, roomLocation.Z, Tolerance);

            var roomVolume = GetPreviewValue("478907639b054374a68dadb7f3cfc26e");
            var expectedVolume = 241.077;
            Assert.AreEqual(expectedVolume, (double)roomVolume, Tolerance);
        }

        [Test]
        [TestModel(@".\Room2025.rvt")]
        public void RoomBoundaries()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RoomBoundaries.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //Read all the rooms present in the project
            var allElementsByCat = GetFlattenedPreviewValues("0ebf1b7ac58e4c61a0c83e24b64d01f4");
            Assert.AreEqual(2, allElementsByCat.Count);

            //Verify the results for Room center/core/core-center/finish boundary are correct
            var roomCenterBoundary = GetFlattenedPreviewValues("1f649b1fcce44408884b0499f3a935fd");
            var roomCoreBoundary = GetFlattenedPreviewValues("18c8aa4c1d3c46bc8582ec8de9f5780d");
            var roomCoreCenterBoundary = GetFlattenedPreviewValues("ec94778c47834c98885a1eef2cd61661");
            var roomFinishBoundary = GetFlattenedPreviewValues("9b4a96a367764876b1db51e33f194ecb");

            //All of the nodes should return the same number of boundary curves
            Assert.AreEqual(roomCenterBoundary.Count, roomCoreBoundary.Count);
            Assert.AreEqual(roomCoreCenterBoundary.Count, roomFinishBoundary.Count);

            //Center Boundary assertions
            var thirdLineCenterBoundary1 = "Line(StartPoint = Point(X = 10568.339, Y = -1574.928, Z = 0.000), EndPoint = Point(X = 10568.339, Y = 5125.072, Z = 0.000), " +
                "Direction = Vector(X = 0.000, Y = 6700.000, Z = 0.000, Length = 6700.000))";
            var thirdLineCenterBoundary = roomCenterBoundary[3] as Autodesk.DesignScript.Geometry.Line;
            Assert.AreEqual(10568.339, thirdLineCenterBoundary.StartPoint.X, Tolerance);
            Assert.AreEqual(-1574.928, thirdLineCenterBoundary.StartPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCenterBoundary.StartPoint.Z, Tolerance);
            Assert.AreEqual(10568.339, thirdLineCenterBoundary.EndPoint.X, Tolerance);
            Assert.AreEqual(5125.072, thirdLineCenterBoundary.EndPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCenterBoundary.EndPoint.Z, Tolerance);
            Assert.AreEqual(6700.000, thirdLineCenterBoundary.Direction.Length, Tolerance);

            //Core Boundary assertions
            var thirdLineCoreBoundary = roomCoreBoundary[3] as Autodesk.DesignScript.Geometry.Line;
            Assert.AreEqual(10435.839, thirdLineCoreBoundary.StartPoint.X, Tolerance);
            Assert.AreEqual(-1424.928, thirdLineCoreBoundary.StartPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCoreBoundary.StartPoint.Z, Tolerance);
            Assert.AreEqual(10435.839, thirdLineCoreBoundary.EndPoint.X, Tolerance);
            Assert.AreEqual(4992.572, thirdLineCoreBoundary.EndPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCoreBoundary.EndPoint.Z, Tolerance);
            Assert.AreEqual(6417.500, thirdLineCoreBoundary.Direction.Length, Tolerance);

            //Core Center Boundary assertions
            var thirdLineCoreCenterBoundary = roomCoreCenterBoundary[3] as Autodesk.DesignScript.Geometry.Line;
            Assert.AreEqual(10485.839, thirdLineCoreCenterBoundary.StartPoint.X, Tolerance);
            Assert.AreEqual(-1574.928, thirdLineCoreCenterBoundary.StartPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCoreCenterBoundary.StartPoint.Z, Tolerance);
            Assert.AreEqual(10485.839, thirdLineCoreCenterBoundary.EndPoint.X, Tolerance);
            Assert.AreEqual(5042.572, thirdLineCoreCenterBoundary.EndPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineCoreCenterBoundary.EndPoint.Z, Tolerance);
            Assert.AreEqual(6617.500, thirdLineCoreCenterBoundary.Direction.Length, Tolerance);


            //Finish Boundary assertions
            var thirdLineFinishBoundary = roomFinishBoundary[3] as Autodesk.DesignScript.Geometry.Line;
            Assert.AreEqual(10423.339, thirdLineFinishBoundary.StartPoint.X, Tolerance);
            Assert.AreEqual(-1424.928, thirdLineFinishBoundary.StartPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineFinishBoundary.StartPoint.Z, Tolerance);
            Assert.AreEqual(10423.339, thirdLineFinishBoundary.EndPoint.X, Tolerance);
            Assert.AreEqual(4980.072, thirdLineFinishBoundary.EndPoint.Y, Tolerance);
            Assert.AreEqual(0.000, thirdLineFinishBoundary.EndPoint.Z, Tolerance);
            Assert.AreEqual(6405.000, thirdLineFinishBoundary.Direction.Length, Tolerance);
        }

        [Test]
        [TestModel(@".\Room2025.rvt")]
        public void SetNameAndNumberRoom()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\SetNameAndNumberRoom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            int roomIdValue = 315201;
            ElementId roomId = new ElementId(roomIdValue);

            //Read the proprieties of the room from Revit canvas and verify that the name and number have been correctly updated
            if (DocumentManager.Instance.CurrentDBDocument is Document document)
            {
                if (document.GetElement(roomId) is Room room)
                {
                    string roomName = room.Name;
                    Assert.AreEqual("NewRoom 25", roomName);

                    string roomNumber = room.Number;
                    Assert.AreEqual("25", roomNumber);
                }
            }
        }


        [Test]
        [TestModel(@".\DifferentTypeRooms.rvt")]
        public void RoomsByStatus()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RoomsByStatus.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

          var roomsStatus = GetFlattenedPreviewValues("ac214652ff8a4bb782f704abe67115bf");
          Assert.AreEqual(8, roomsStatus.Count);
        }
    }
}