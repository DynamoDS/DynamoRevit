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
        /// Testing to create a room by location and query its name and type
        /// </summary>
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Create_ValidArgs()
        {
            Level lvl = Level.ByElevation(0);
            Assert.IsNotNull(lvl);
            CreateDefaultBoundary();

            var room = Revit.Elements.Room.ByLocation(lvl,Point.ByCoordinates(0, 0, 0),"myRoom", "myNumber");
            
            Assert.NotNull(room);
            Assert.AreEqual("myRoom myNumber",room.InternalRevitElement.Name);
            Assert.AreEqual(typeof(Revit.Elements.Room), room.GetType());
        }


        /// <summary>
        /// Create a default boundary for rooms
        /// </summary>
        private void CreateDefaultBoundary()
        { 
            Autodesk.Revit.DB.CurveArray curves = new Autodesk.Revit.DB.CurveArray();
            curves.Append(Autodesk.Revit.DB.Line.CreateBound(new Autodesk.Revit.DB.XYZ(-100,100,0), new Autodesk.Revit.DB.XYZ(100,100,0)));
            curves.Append(Autodesk.Revit.DB.Line.CreateBound(new Autodesk.Revit.DB.XYZ(100,100,0), new Autodesk.Revit.DB.XYZ(100,-100,0)));
            curves.Append(Autodesk.Revit.DB.Line.CreateBound(new Autodesk.Revit.DB.XYZ(100,-100,0), new Autodesk.Revit.DB.XYZ(-100,-100,0)));
            curves.Append(Autodesk.Revit.DB.Line.CreateBound(new Autodesk.Revit.DB.XYZ(-100,-100,0), new Autodesk.Revit.DB.XYZ(-100,100,0)));

            var plane = Autodesk.Revit.DB.SketchPlane.Create(Revit.Application.Document.Current.InternalDocument, new Autodesk.Revit.DB.Plane(Autodesk.Revit.DB.XYZ.BasisZ, Autodesk.Revit.DB.XYZ.Zero));
            Revit.Application.Document.Current.InternalDocument.Create.NewRoomBoundaryLines(plane,curves, Revit.Application.Document.Current.ActiveView.InternalView);
        }

    }
}
