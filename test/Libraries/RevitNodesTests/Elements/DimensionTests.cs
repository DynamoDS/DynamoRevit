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
    public class DimensionTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Create_ValidArgs()
        {
            var line1 = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(0, 6, 0));
            ModelCurve mc1 = ModelCurve.ByCurve(line1);

            var line2 = Line.ByStartPointEndPoint(Point.ByCoordinates(2, 0, 0), Point.ByCoordinates(2, 6, 0));
            ModelCurve mc2 = ModelCurve.ByCurve(line2);

            System.Collections.Generic.List<Element> elements = new System.Collections.Generic.List<Element>(){ mc1, mc2};

            var line3 = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 3, 0), Point.ByCoordinates(2, 3, 0));

            var dim = Dimension.ByElements(Revit.Application.Document.Current.ActiveView, elements, line3);
            Assert.NotNull(dim);

            dim.InternalRevitElement.Value.Value.ShouldBeApproximately(2.0);
        }



    }
}
