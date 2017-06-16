using System;
using Autodesk.DesignScript.Geometry;
using Revit.Elements;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ReferencePlaneTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLine_ValidArgs()
        {
            var line = Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(1, 0, 0));
            Assert.NotNull(line);

            var refPlane = ReferencePlane.ByLine(line);

            Assert.NotNull(refPlane);
            Assert.NotNull(refPlane.Plane);
            Assert.NotNull(refPlane.ElementPlaneReference);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByStartPointEndPoint_ValidArgs()
        {
            var refPlane = ReferencePlane.ByStartPointEndPoint(Point.ByCoordinates(1, 0, 0),
                Point.ByCoordinates(1, 1, 1));

            Assert.NotNull(refPlane);
            Assert.NotNull(refPlane.Plane);
            Assert.NotNull(refPlane.ElementPlaneReference);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByPoints_ValidArgs()
        {
            var refPlane = ReferencePlane.ByPoints(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(0, 10, 0),
                Point.ByCoordinates(0, 0, 10));

            Assert.NotNull(refPlane);
            Assert.NotNull(refPlane.Plane);
            Assert.NotNull(refPlane.ElementPlaneReference);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void BySketchPlaneView_ValidArgs()
        {
            var plane = Plane.ByOriginNormal(Point.ByCoordinates(0, 0, 0), Vector.ZAxis());
            Assert.NotNull(plane);

            var sketchPlane = SketchPlane.ByPlane(plane);
            Assert.NotNull(sketchPlane);
            
            var view = (Revit.Elements.Views.View)DocumentManager.Instance.CurrentDBDocument.ActiveView.ToDSType(true);
            Assert.NotNull(view);

            var refPlane = ReferencePlane.BySketchPlaneAndView(sketchPlane, view);

            Assert.NotNull(refPlane);
            Assert.NotNull(refPlane.Plane);
            Assert.NotNull(refPlane.ElementPlaneReference);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByStartPointEndPointNormal_ValidArgs()
        {
            var refPlane = ReferencePlane.ByStartPointEndPointNormal(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(0, 10, 0), 
                Vector.ZAxis());

            Assert.NotNull(refPlane);
            Assert.NotNull(refPlane.Plane);
            Assert.NotNull(refPlane.ElementPlaneReference);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByLine_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByLine(null));
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByStartPointEndPoint_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByStartPointEndPoint(Point.ByCoordinates(1, 1, 1), null));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByStartPointEndPoint(null, Point.ByCoordinates(1, 1, 1)));
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByPoints_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByPoints(Point.ByCoordinates(1, 1, 1), Point.ByCoordinates(1, 1, 1), null));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByPoints(Point.ByCoordinates(1, 1, 1), null, Point.ByCoordinates(1, 1, 1)));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByPoints(null, Point.ByCoordinates(1, 1, 1), Point.ByCoordinates(1, 1, 1)));
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void BySketchPlaneView_NullArgs()
        {
            var sketchPlane = SketchPlane.ByPlane(Plane.ByOriginNormal(Point.ByCoordinates(0, 0, 0), Vector.ZAxis()));
            var view = (Revit.Elements.Views.View)DocumentManager.Instance.CurrentDBDocument.ActiveView.ToDSType(true);

            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.BySketchPlaneAndView(sketchPlane, null));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.BySketchPlaneAndView(null, view));
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByStartPointEndPointNormal_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByStartPointEndPointNormal(Point.ByCoordinates(1, 1, 1), Point.ByCoordinates(1, 1, 1), null));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByStartPointEndPointNormal(Point.ByCoordinates(1, 1, 1), null, Vector.ZAxis()));
            Assert.Throws(typeof(ArgumentNullException), () => ReferencePlane.ByStartPointEndPointNormal(null, Point.ByCoordinates(1, 1, 1), Vector.ZAxis()));
        }
    }
}
