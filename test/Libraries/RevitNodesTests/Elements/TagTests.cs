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
            var line = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(1, 1, 1));
            Assert.NotNull(line);

            var modelCurve = ModelCurve.ByCurve(line);
            Assert.NotNull(line);

            var tag = Tag.ByElement(Revit.Application.Document.Current.ActiveView, modelCurve, true, false, null);
            Assert.NotNull(tag);
          
        }




    }
}
