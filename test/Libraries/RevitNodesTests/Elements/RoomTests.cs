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
        [Test]
        [TestModel(@".\emptyAnnotativeView.rfa")]
        public void Create_ValidArgs()
        {
            Level lvl = Level.ByElevation(0);
            Assert.IsNotNull(lvl);

            var room = Revit.Elements.Room.Create(lvl,Point.ByCoordinates(0, 0, 0),"myRoom", "myNumber");
            
            Assert.NotNull(room);
            Assert.AreEqual(room.InternalRevitElement.Name, "myRoom");      
        }

    }
}
