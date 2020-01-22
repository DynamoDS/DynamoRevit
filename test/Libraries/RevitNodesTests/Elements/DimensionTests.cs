
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;
using System.Collections.Generic;

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

        [Test]
        [TestModel(@".\Dimension\dimensionTests.rvt")]
        public void CanSetAndGetBelowValue()
        {
            // Arrange
            var dimension = ElementSelector.ByElementId(316269, true) as Dimension;
            var dimensionWithSegments = ElementSelector.ByElementId(316257, true) as Dimension;

            var expectedDimensionWithSegmentsValue = new List<string>()
            {
                "Below",
                "Below",
            };

            var expectedDimensionValue = new List<string>()
            {
                "Below",
            };


            // Act
            var oldDimensionBelowValue = dimension.BelowValue;
            var oldDimensionWithSegmentsBelowValue = dimensionWithSegments.BelowValue;

            var setNewBelowValueDimension = dimension.SetBelowValue("Below");
            var setNewBelowValueDimensionWithSegment = dimensionWithSegments.SetBelowValue("Below");

            var newBelowValueDimension = dimension.BelowValue;
            var newBelowValueDimensionWithSegment = dimensionWithSegments.BelowValue;

            // Assert
            Assert.AreNotEqual(oldDimensionBelowValue, newBelowValueDimension);
            Assert.AreNotEqual(oldDimensionWithSegmentsBelowValue, newBelowValueDimensionWithSegment);

            CollectionAssert.AreEqual(expectedDimensionValue, newBelowValueDimension);
            CollectionAssert.AreEqual(expectedDimensionWithSegmentsValue, newBelowValueDimensionWithSegment);
        }

        [Test]
        [TestModel(@".\Dimension\dimensionTests.rvt")]
        public void CanSetAndGetAboveValue()
        {
            // Arrange
            var dimension = ElementSelector.ByElementId(316269, true) as Dimension;
            var dimensionWithSegments = ElementSelector.ByElementId(316257, true) as Dimension;

            var expectedDimensionWithSegmentsValue = new List<string>()
            {
                "Above",
                "Above",
            };

            var expectedDimensionValue = new List<string>()
            {
                "Above",
            };


            // Act
            var oldDimensionAboveValue = dimension.AboveValue;
            var oldDimensionWithSegmentsAboveValue = dimensionWithSegments.AboveValue;

            var setNewAboveValueDimension = dimension.SetAboveValue("Above");
            var setNewAboveValueDimensionWithSegment = dimensionWithSegments.SetAboveValue("Above");

            var newAboveValueDimension = dimension.AboveValue;
            var newAboveValueDimensionWithSegment = dimensionWithSegments.AboveValue;

            // Assert
            Assert.AreNotEqual(oldDimensionAboveValue, newAboveValueDimension);
            Assert.AreNotEqual(oldDimensionWithSegmentsAboveValue, newAboveValueDimensionWithSegment);

            CollectionAssert.AreEqual(expectedDimensionValue, newAboveValueDimension);
            CollectionAssert.AreEqual(expectedDimensionWithSegmentsValue, newAboveValueDimensionWithSegment);
        }

    }
}
