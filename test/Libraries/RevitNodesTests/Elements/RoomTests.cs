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

            var room = Revit.Elements.Room.ByLocation(lvl, Point.ByCoordinates(0, 0, 0), "myRoom", "myNumber");

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


    }
}
