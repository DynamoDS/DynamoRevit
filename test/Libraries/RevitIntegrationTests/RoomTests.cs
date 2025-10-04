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

        private const double Tolerance = 0.001;
        [Test]
        [TestModel(@".\Room2025.rvt")]
        public void LocationAndVolumeRoom()
        {
        string samplePath = Path.Combine(workingDirectory, @".\Script\LocationAndVolumeRoom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var roomLocation = GetPreviewValue("3594f81581cd4b8b9e68ef308eefcaa3");
            var expectedLocation = "Point(X = 5895.952, Y = 1686.784, Z = 0.000)";
            Assert.AreEqual(expectedLocation, roomLocation.ToString());

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

            var thirdLineCenterBoundary = "Line(StartPoint = Point(X = 10568.339, Y = -1574.928, Z = 0.000), EndPoint = Point(X = 10568.339, Y = 5125.072, Z = 0.000), " +
                "Direction = Vector(X = 0.000, Y = 6700.000, Z = 0.000, Length = 6700.000))";
            Assert.AreEqual(thirdLineCenterBoundary, roomCenterBoundary[3].ToString());

            var thirdLineCoreBoundary = "Line(StartPoint = Point(X = 10435.839, Y = -1424.928, Z = 0.000), EndPoint = Point(X = 10435.839, Y = 4992.572, Z = 0.000), " +
                "Direction = Vector(X = 0.000, Y = 6417.500, Z = 0.000, Length = 6417.500))";
            Assert.AreEqual(thirdLineCoreBoundary, roomCoreBoundary[3].ToString());

            var thirdLineCoreCenterBoundary = "Line(StartPoint = Point(X = 10485.839, Y = -1574.928, Z = 0.000), EndPoint = Point(X = 10485.839, Y = 5042.572, Z = 0.000), " +
                "Direction = Vector(X = 0.000, Y = 6617.500, Z = 0.000, Length = 6617.500))";
            Assert.AreEqual(thirdLineCoreCenterBoundary, roomCoreCenterBoundary[3].ToString());

            var thirdLineFinishBoundary = "Line(StartPoint = Point(X = 10423.339, Y = -1424.928, Z = 0.000), EndPoint = Point(X = 10423.339, Y = 4980.072, Z = 0.000), " +
                "Direction = Vector(X = 0.000, Y = 6405.000, Z = 0.000, Length = 6405.000))";
            Assert.AreEqual(thirdLineFinishBoundary, roomFinishBoundary[3].ToString());
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