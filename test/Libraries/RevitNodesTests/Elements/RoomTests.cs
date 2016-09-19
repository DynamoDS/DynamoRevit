using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;

using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class RoomTests : RevitNodeTestBase
    {
        /// <summary>
        /// Testing to create a room by location and query its name
        /// </summary>
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Create_ValidArgs()
        {
            Level lvl = Level.ByElevation(0);
            Assert.IsNotNull(lvl);

            var room = Revit.Elements.Room.ByLocation(lvl,Point.ByCoordinates(0, 0, 0),"myRoom", "myNumber");
            
            Assert.NotNull(room);
            Assert.AreEqual(room.InternalRevitElement.Name, "myRoom");      
        }

        /// <summary>
        /// Test Room Wrapper
        /// </summary>
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void CheckWrapper()
        {
            Level lvl = Level.ByElevation(100);
            Assert.IsNotNull(lvl);

            // Create a new room
            var room = Revit.Elements.Room.ByLocation(lvl, Point.ByCoordinates(100, 100, 0), "mySecondRoom", "mySecondNumber");

            Assert.NotNull(room);

            // Get the room using the Element Selector
            Element element = ElementSelector.ByElementId(room.InternalRevitElement.Id.IntegerValue);

            Assert.NotNull(element);

            // Check if the selected element is actually wrapped as a Revit.Element.Room
            Assert.IsInstanceOf(typeof(Revit.Elements.Room), element);
        }


        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void CheckBoundaryAndLocationProperty()
        {
            var level = Level.ByElevation(0);
            var wallType = WallType.ByName("Generic - 8\"");

            var line1 = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(0, 1000, 0));
            var line2 = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 1000, 0), Point.ByCoordinates(1000, 1000, 0));
            var line3 = Line.ByStartPointEndPoint(Point.ByCoordinates(1000, 1000, 0), Point.ByCoordinates(1000, 0, 0));
            var line4 = Line.ByStartPointEndPoint(Point.ByCoordinates(1000, 0, 0), Point.ByCoordinates(0, 0, 0));
            var wall1 = Wall.ByCurveAndHeight(line1, 500, level, wallType);
            var wall2 = Wall.ByCurveAndHeight(line2, 500, level, wallType);
            var wall3 = Wall.ByCurveAndHeight(line3, 500, level, wallType);
            var wall4 = Wall.ByCurveAndHeight(line4, 500, level, wallType);

            Point pt = Point.ByCoordinates(500, 500, 0);

            // Create a new room
            var room = Revit.Elements.Room.ByLocation(level, pt, "myRoom", "myNumber");

            Assert.NotNull(room);

            Assert.IsTrue(room.Location.DistanceTo(pt) < 0.001);

            var curves = room.CenterBoundary;

            Assert.IsNotNull(curves);
            Assert.IsTrue(curves.First().Count() == 4);

            double length = curves.First().First().Length;

            foreach (var curve in curves.First())
            {
                length.ShouldBeApproximately(curve.Length);
            }

        }

    }
}
