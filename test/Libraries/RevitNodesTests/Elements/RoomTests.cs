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

            var plane = Autodesk.Revit.DB.SketchPlane.Create(Revit.Application.Document.Current.InternalDocument, Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(Autodesk.Revit.DB.XYZ.BasisZ, Autodesk.Revit.DB.XYZ.Zero));
            Revit.Application.Document.Current.InternalDocument.Create.NewRoomBoundaryLines(plane,curves, Revit.Application.Document.Current.ActiveView.InternalView);
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
