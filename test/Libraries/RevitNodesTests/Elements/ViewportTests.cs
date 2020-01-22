using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ViewportTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanCreateViewportOnSheet()
        {
            // Arrange
            var sheet = ElementSelector.ByElementId(179432) as Sheet;
            var view = ElementSelector.ByElementId(312) as View;
            var point = Point.ByCoordinates(250,250);

            var expectedViewportId = 307874;
            var expectedExceptionMessage = Revit.Properties.Resources.ViewAlreadyPlacedOnSheet;

            // Act
            var viewport = Viewport.BySheetViewLocation(sheet, view, point);
            var viewportId = viewport.Id;
            var viewportLocation = viewport.InternalViewport.GetBoxCenter().ToPoint() as Point;
            var exceptionViewport = Assert.Throws<InvalidOperationException>(() => Viewport.BySheetViewLocation(sheet, view, point));
            
            // Assert
            Assert.AreEqual(viewport.GetType(), typeof(Viewport));
            Assert.AreEqual(expectedViewportId, viewportId);
            Assert.AreEqual(point.X, viewportLocation.X, Tolerance);
            Assert.AreEqual(point.Y, viewportLocation.Y, Tolerance);
            Assert.AreEqual(expectedExceptionMessage, exceptionViewport.Message);
        }
    }
}
