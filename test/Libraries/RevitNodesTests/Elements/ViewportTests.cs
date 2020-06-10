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
            var sheet = ElementSelector.ByType<Autodesk.Revit.DB.ViewSheet>(true).First() as Sheet;
            var view = CreateTestView();
            var point = Point.ByCoordinates(250,250);

            var expectedExceptionMessage = Revit.Properties.Resources.ViewAlreadyPlacedOnSheet;

            // Act
            var viewport = Viewport.BySheetViewLocation(sheet, view, point);
            var viewportLocation = viewport.InternalViewport.GetBoxCenter().ToPoint() as Point;
            var exceptionViewport = Assert.Throws<InvalidOperationException>(() => Viewport.BySheetViewLocation(sheet, view, point));
            
            // Assert
            Assert.AreEqual(viewport.GetType(), typeof(Viewport));
            Assert.AreEqual(point.X, viewportLocation.X, Tolerance);
            Assert.AreEqual(point.Y, viewportLocation.Y, Tolerance);
            Assert.AreEqual(expectedExceptionMessage, exceptionViewport.Message);
        }

        private static View CreateTestView()
        {
            var eye = Point.ByCoordinates(100, 100, 100);
            var target = Point.ByCoordinates(0, 1, 2);

            var v = AxonometricView.ByEyePointAndTarget(eye, target);
            var view = (Autodesk.Revit.DB.View3D)v.InternalElement;
            Assert.AreEqual(view.Name, View3D.DEFAULT_VIEW_NAME);
            Assert.False(view.CropBoxActive);

            return v;
        }
    }
}
