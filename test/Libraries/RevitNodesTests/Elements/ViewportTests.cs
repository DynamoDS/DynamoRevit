using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
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
        [Test]
        [TestModel(@".\Viewport\viewportTests.rvt")]
        public void CanCreateViewportOnSheet()
        {
            // Arrange
            var sheet = ElementSelector.ByElementId(179432) as Sheet;
            var view = ElementSelector.ByElementId(312) as View;
            var point = Point.ByCoordinates(0,0,0);

            var expectedViewportId = 307874;
            var expectedExceptionMessage = Revit.Properties.Resources.ViewAlreadyPlacedOnSheet;

            // Act
            var viewport = Viewport.Create(sheet, view, point);
            var viewportId = viewport.Id;
            var exceptionViewport = Assert.Throws<InvalidOperationException>(() => Viewport.Create(sheet, view, point));
            
            // Assert
            Assert.AreEqual(viewport.GetType(), typeof(Viewport));
            Assert.AreEqual(expectedViewportId, viewportId);
            Assert.AreEqual(expectedExceptionMessage, exceptionViewport.Message);
        }
    }
}
