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
    public class RevisionCloudTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ByCurve_ValidArgs()
        {
            Polygon polygon = Polygon.RegularPolygon(Circle.ByCenterPointRadius(Point.ByCoordinates(0, 0, 0), 100), 5);
            Assert.NotNull(polygon);

            Revision rev1 = Revision.ByName("myName", "01.01.1970", "myDesc", false, "me", "to");
            Assert.NotNull(rev1);

            Assert.IsInstanceOf(typeof(Revit.Elements.Revision), rev1);

            var revCloud = RevisionCloud.ByCurve(Revit.Application.Document.Current.ActiveView,polygon.Curves().ToList(),rev1);
            Assert.NotNull(revCloud);

            Assert.IsInstanceOf(typeof(Revit.Elements.RevisionCloud), revCloud);

            var curveGeo = revCloud.Curves;
            Assert.NotNull(curveGeo);

            Assert.AreEqual(revCloud.InternalRevitElement.RevisionId.IntegerValue, rev1.InternalElement.Id.IntegerValue);
        }



    }
}
