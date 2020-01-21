using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryReferences;

using RevitServices.Persistence;
using RevitServices.Transactions;
using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class ElevationMarkerTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanCreateElevationMarker()
        {
            // Arrange
            var room = ElementSelector.ByElementId(316289, true);
            var elevationViewFamilyType = ElementSelector.ByElementId(86228, true);

            var expectedType = typeof(Revit.Elements.ElevationMarker);
            var expectedLocation = Autodesk.DesignScript.Geometry.Point.ByCoordinates(1392.616, 915.356, 0.000);
            var elevationMarkerLocation = room.GetLocation() as Autodesk.DesignScript.Geometry.Point;

            // Act
            var elevationMarker = Revit.Elements.ElevationMarker.ByViewTypeLocation(elevationViewFamilyType, elevationMarkerLocation, 100);

            var newElevationMarkerType = elevationMarker.GetType();

            // Assert
            Assert.AreEqual(expectedType, newElevationMarkerType);
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanCreateElevationViewsByMarkerIndex()
        {
            // Arrange
            var elevationMarker = ElementSelector.ByElementId(327339, true) as Revit.Elements.ElevationMarker;
            var planView = ElementSelector.ByElementId(312, true) as Revit.Elements.Views.View;

            var expectedViewName = "Elevation 8 - c";
            var index = 2;

            // Act
            var elevationView = elevationMarker.CreateElevationByMarkerIndex(planView, index);
            var elevationViewName = elevationView.Name;

            // Assert
            Assert.AreEqual(expectedViewName, elevationViewName);
            Assert.AreEqual(false, elevationMarker.InternalMarker.IsAvailableIndex(index));
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanGetElevationMarkerViewCount()
        {
            // Arrange
            var elevationMarker = ElementSelector.ByElementId(327385, true) as Revit.Elements.ElevationMarker;

            var expectedViewCount = 3;

            // Act
            var elevationMarkerViewCount = elevationMarker.CurrentViewCount;

            // Assert
            Assert.AreEqual(expectedViewCount, elevationMarkerViewCount);
            
        }

        [Test]
        [TestModel(@".\ElevationMarker\elevationMarkerTests.rvt")]
        public void CanGetElevationMarkerViewByIndex()
        {
            // Arrange
            var elevationMarker = ElementSelector.ByElementId(327385, true) as Revit.Elements.ElevationMarker;

            var expectedViewId = 327396;

            // Act
            var elevationMarkerView = elevationMarker.GetView(1);
            var elevationMarkerViewId = elevationMarkerView.Id;

            // Assert
            Assert.AreEqual(expectedViewId, elevationMarkerViewId);
        }
    }
}
