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
    public class FilledRegionTests : RevitNodeTestBase
    {

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByCurves_ValidArgs()
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Revit.Application.Document.Current.InternalDocument).OfClass(typeof(Autodesk.Revit.DB.FilledRegionType));
            FilledRegionType type = FilledRegionType.FromExisting((Autodesk.Revit.DB.FilledRegionType)collector.FirstOrDefault(), true);

            Assert.NotNull(type);

            Polygon polygon = Polygon.RegularPolygon(Circle.ByCenterPointRadius(Point.ByCoordinates(0, 0, 0), 10), 4);
            Assert.NotNull(polygon);

            var reg = FilledRegion.ByCurves(Revit.Application.Document.Current.ActiveView, polygon.Curves().ToList(), type);
            Assert.NotNull(reg);

        }


        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByCircle_ValidArgs()
        {
            Autodesk.Revit.DB.FilteredElementCollector collector = new Autodesk.Revit.DB.FilteredElementCollector(Revit.Application.Document.Current.InternalDocument).OfClass(typeof(Autodesk.Revit.DB.FilledRegionType));
            FilledRegionType type = FilledRegionType.FromExisting((Autodesk.Revit.DB.FilledRegionType)collector.FirstOrDefault(), true);

            Assert.NotNull(type);

            Circle c = Circle.ByCenterPointRadius(Point.ByCoordinates(0, 0, 0), 10);
            Assert.NotNull(c);

            var reg = FilledRegion.ByCurves(Revit.Application.Document.Current.ActiveView, c.ApproximateWithArcAndLineSegments(), type);
            Assert.NotNull(reg);

        }



    }
}
