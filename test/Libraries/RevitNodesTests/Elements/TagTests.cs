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
    public class TagTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByElement_ValidArgs()
        {
            var line = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(1000, 0, 0));
            Assert.NotNull(line);

            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Revit.Application.Document.Current.InternalDocument).OfClass(typeof(Autodesk.Revit.DB.WallType));
            Revit.Elements.WallType wt = Revit.Elements.WallType.FromExisting((Autodesk.Revit.DB.WallType)collector.FirstOrDefault(), true);

            var wall = Wall.ByCurveAndHeight(line, 300, Level.ByElevation(100), wt);
            Assert.NotNull(wall);


            var tag = Tag.ByElement(Revit.Application.Document.Current.ActiveView, wall, true, false, Autodesk.Revit.DB.HorizontalAlignmentStyle.Center, Autodesk.Revit.DB.VerticalAlignmentStyle.Middle, Vector.ByCoordinates(0, 0, 0));
            Assert.NotNull(tag);
          
        }




    }
}
