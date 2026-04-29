using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class SketchTests : RevitNodeTestBase
    {
        private static PolyCurve MakeSquareOutline(double size = 100)
        {
            var lines = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(size, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(size, 0, 0), Point.ByCoordinates(size, size, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(size, size, 0), Point.ByCoordinates(0, size, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, size, 0), Point.ByCoordinates(0, 0, 0))
            };
            return PolyCurve.ByJoinedCurves(lines, 0.001, false);
        }

        private static Sketch MakeSketchFromPropertyLine(double size = 100)
        {
            var propertyLine = PropertyLine.ByCurveLoops(MakeSquareOutline(size));
            Assume.That(propertyLine, Is.Not.Null, "PropertyLine should be created.");
            Assume.That(propertyLine.IsSketchBased, Is.True, "PropertyLine should be sketch-based.");
            return propertyLine.Sketch;
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyLineSketch_ReturnsTypedSketchWrapper()
        {
            var propertyLine = PropertyLine.ByCurveLoops(MakeSquareOutline());

            Sketch sketch = propertyLine.Sketch;

            Assert.NotNull(sketch);
            Assert.IsInstanceOf<Sketch>(sketch);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Sketch_SketchPlane_ReturnsValidSketchPlane()
        {
            var sketch = MakeSketchFromPropertyLine();

            SketchPlane sketchPlane = sketch.SketchPlane;

            Assert.NotNull(sketchPlane);
            Assert.NotNull(sketchPlane.Plane);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Sketch_Owner_ReturnsHostingPropertyLine()
        {
            var propertyLine = PropertyLine.ByCurveLoops(MakeSquareOutline());

            var owner = propertyLine.Sketch.Owner;

            Assert.NotNull(owner);
            Assert.AreEqual(propertyLine.InternalElement.Id, owner.InternalElement.Id);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Sketch_Elements_ReturnsAtLeastOneSketchElement()
        {
            var sketch = MakeSketchFromPropertyLine();

            var elements = sketch.Elements;

            Assert.NotNull(elements);
            Assert.IsTrue(elements.Length > 0,
                "A sketch backing a closed-loop PropertyLine should contain at least one model curve.");
            Assert.IsTrue(elements.All(e => e != null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Sketch_Profile_ReturnsAtLeastOneClosedLoop()
        {
            var sketch = MakeSketchFromPropertyLine();

            PolyCurve[] profile = sketch.Profile;

            Assert.NotNull(profile);
            Assert.IsTrue(profile.Length > 0, "Profile should contain at least one curve loop.");
            Assert.IsTrue(profile.All(p => p != null));
            Assert.IsTrue(profile[0].IsClosed,
                "The boundary loop of a closed-square PropertyLine should be closed.");
        }
    }
}
