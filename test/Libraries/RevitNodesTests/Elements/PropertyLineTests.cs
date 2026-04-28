using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;
using DBArcDirection = Autodesk.Revit.DB.PropertyTableEntryArcDirection;
using DBCurveType = Autodesk.Revit.DB.PropertyTableEntryCurveType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class PropertyLineTests : RevitNodeTestBase
    {
        private const double Tolerance = 1e-6;

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

        private static PolyCurve MakeVerticalSquare(double size = 100)
        {
            var lines = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(size, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(size, 0, 0), Point.ByCoordinates(size, 0, size)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(size, 0, size), Point.ByCoordinates(0, 0, size)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, size), Point.ByCoordinates(0, 0, 0))
            };
            return PolyCurve.ByJoinedCurves(lines, 0.001, false);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCurveLoops_PolyCurve_CreatesSketchBasedPropertyLine()
        {
            var outline = MakeSquareOutline();

            var propertyLine = PropertyLine.ByCurveLoops(outline);

            Assert.NotNull(propertyLine);
            Assert.IsTrue(propertyLine.IsSketchBased);
            Assert.IsTrue(propertyLine.IsClosedLoop);
            Assert.AreEqual(1, propertyLine.Boundary.Length);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCurveLoops_CurveArray_CreatesSketchBasedPropertyLine()
        {
            var outline = new[]
            {
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 0, 0), Point.ByCoordinates(100, 0, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 0, 0), Point.ByCoordinates(100, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(100, 100, 0), Point.ByCoordinates(0, 100, 0)),
                Line.ByStartPointEndPoint(Point.ByCoordinates(0, 100, 0), Point.ByCoordinates(0, 0, 0))
            };

            var propertyLine = PropertyLine.ByCurveLoops(outline);

            Assert.NotNull(propertyLine);
            Assert.IsTrue(propertyLine.IsSketchBased);
            Assert.IsTrue(propertyLine.IsClosedLoop);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCurveLoops_NullArguments_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => PropertyLine.ByCurveLoops((PolyCurve)null));
            Assert.Throws<ArgumentNullException>(() => PropertyLine.ByCurveLoops((Curve[])null));
            Assert.Throws<ArgumentNullException>(() => PropertyLine.ByPolyCurves(null));
            Assert.Throws<ArgumentException>(() => PropertyLine.ByPolyCurves(Array.Empty<PolyCurve>()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void IsValidBoundary_ReturnsFalseForVerticalLoop()
        {
            var verticalOutline = MakeVerticalSquare();

            Assert.IsFalse(PropertyLine.IsValidBoundary(new[] { verticalOutline }));
            Assert.IsFalse(PropertyLine.IsValidBoundary(null));
            Assert.IsFalse(PropertyLine.IsValidBoundary(Array.Empty<PolyCurve>()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void IsValidBoundary_ReturnsTrueForHorizontalSquare()
        {
            var outline = MakeSquareOutline();

            Assert.IsTrue(PropertyLine.IsValidBoundary(new[] { outline }));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByPropertyTableEntries_FourLines_CreatesTableBasedPropertyLine()
        {
            // Bearings are in radians; build a closed square: east, north, west, south.
            var entries = new[]
            {
                PropertyTableEntry.ByLine(50.0, Math.PI / 2),
                PropertyTableEntry.ByLine(50.0, 0.0),
                PropertyTableEntry.ByLine(50.0, 3 * Math.PI / 2),
                PropertyTableEntry.ByLine(50.0, Math.PI)
            };

            var propertyLine = PropertyLine.ByPropertyTableEntries(entries);

            Assert.NotNull(propertyLine);
            Assert.IsFalse(propertyLine.IsSketchBased);
            Assert.AreEqual(4, propertyLine.PropertyTable.Length);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByPropertyTableEntries_NullOrEmpty_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => PropertyLine.ByPropertyTableEntries(null));
            Assert.Throws<ArgumentException>(() => PropertyLine.ByPropertyTableEntries(Array.Empty<PropertyTableEntry>()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void IsValidPropertyTable_ReturnsTrueForValidEntries()
        {
            var entries = new[]
            {
                PropertyTableEntry.ByLine(50.0, Math.PI / 2),
                PropertyTableEntry.ByLine(50.0, 0.0),
                PropertyTableEntry.ByLine(50.0, 3 * Math.PI / 2),
                PropertyTableEntry.ByLine(50.0, Math.PI)
            };

            Assert.IsTrue(PropertyLine.IsValidPropertyTable(entries));
            Assert.IsFalse(PropertyLine.IsValidPropertyTable(null));
            Assert.IsFalse(PropertyLine.IsValidPropertyTable(Array.Empty<PropertyTableEntry>()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyTableEntry_ByLine_RoundTripsValues()
        {
            const double distance = 75.0;
            const double bearing = Math.PI / 3;
            const int id = 7;

            var entry = PropertyTableEntry.ByLine(distance, bearing, id);

            Assert.AreEqual(distance, entry.Distance, Tolerance);
            Assert.AreEqual(bearing, entry.Bearing, Tolerance);
            Assert.AreEqual(id, entry.Id);
            Assert.AreEqual(DBCurveType.Line, entry.CurveType);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyTableEntry_ByArc_RoundTripsValues()
        {
            const double distance = 40.0;
            const double bearing = Math.PI / 4;
            const double radius = 30.0;
            const int id = 11;

            var entry = PropertyTableEntry.ByArc(distance, bearing, radius, "Left", id);

            Assert.AreEqual(distance, entry.Distance, Tolerance);
            Assert.AreEqual(bearing, entry.Bearing, Tolerance);
            Assert.AreEqual(radius, entry.Radius, Tolerance);
            Assert.AreEqual(DBCurveType.Arc, entry.CurveType);
            Assert.AreEqual(DBArcDirection.Left, entry.ArcDirection);
            Assert.AreEqual(id, entry.Id);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyTableEntry_ByArc_InvalidDirectionThrows()
        {
            Assert.Throws<ArgumentException>(
                () => PropertyTableEntry.ByArc(40.0, 0.0, 30.0, "Sideways"));
            Assert.Throws<ArgumentNullException>(
                () => PropertyTableEntry.ByArc(40.0, 0.0, 30.0, null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyTableEntry_ByValues_RoundTripsValues()
        {
            const double distance = 60.0;
            const double bearing = Math.PI;
            const double radius = 35.0;

            var entry = PropertyTableEntry.ByValues(
                distance, bearing, DBCurveType.Arc, radius, DBArcDirection.Right, id: 3);

            Assert.AreEqual(distance, entry.Distance, Tolerance);
            Assert.AreEqual(bearing, entry.Bearing, Tolerance);
            Assert.AreEqual(radius, entry.Radius, Tolerance);
            Assert.AreEqual(DBCurveType.Arc, entry.CurveType);
            Assert.AreEqual(DBArcDirection.Right, entry.ArcDirection);
            Assert.AreEqual(3, entry.Id);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void PropertyTableEntry_IsValid_RejectsArcWithRadiusBelowHalfDistance()
        {
            // Arc radius must exceed half the chord distance; 5 < 50/2.
            Assert.IsFalse(PropertyTableEntry.IsValid(
                distance: 50.0,
                bearing: 0.0,
                curveType: DBCurveType.Arc,
                arcRadius: 5.0,
                arcDirection: DBArcDirection.Right));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ConvertToTable_OnSketchBasedPropertyLine_TurnsItTableBased()
        {
            var outline = MakeSquareOutline();
            var propertyLine = PropertyLine.ByCurveLoops(outline);

            Assume.That(propertyLine.IsSketchBased, Is.True);
            Assume.That(propertyLine.IsValidToConvert, Is.True);

            propertyLine.ConvertToTable();

            Assert.IsFalse(propertyLine.IsSketchBased);
            Assert.IsTrue(propertyLine.PropertyTable.Length > 0);
        }
    }
}
