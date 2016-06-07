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

    }
}
