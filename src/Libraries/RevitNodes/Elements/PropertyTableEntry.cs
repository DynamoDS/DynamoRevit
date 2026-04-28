using System;
using Autodesk.DesignScript.Runtime;
using DBPropertyTableEntry = Autodesk.Revit.DB.PropertyTableEntry;
using DBCurveType = Autodesk.Revit.DB.PropertyTableEntryCurveType;
using DBArcDirection = Autodesk.Revit.DB.PropertyTableEntryArcDirection;

namespace Revit.Elements
{
    /// <summary>
    /// A row of a table-driven Revit PropertyLine element.
    /// </summary>
    public class PropertyTableEntry
    {
        #region Internal properties

        /// <summary>
        /// Internal handle on the underlying Revit PropertyTableEntry.
        /// </summary>
        [SupressImportIntoVM]
        internal DBPropertyTableEntry InternalPropertyTableEntry { get; private set; }

        #endregion

        #region Private constructors

        private PropertyTableEntry(DBPropertyTableEntry entry)
        {
            InternalPropertyTableEntry = entry;
        }

        private PropertyTableEntry(double distance, double bearing, DBCurveType curveType, double arcRadius, DBArcDirection arcDirection, int id)
        {
            if (!DBPropertyTableEntry.IsValidPropertyTableEntry(distance, bearing, curveType, arcRadius, arcDirection))
            {
                throw new ArgumentException(Properties.Resources.PropertyTableEntryInvalid);
            }

            InternalPropertyTableEntry = DBPropertyTableEntry.Create(distance, bearing, curveType, arcRadius, arcDirection, id);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The distance, in feet, between the start and end points of this entry.
        /// </summary>
        public double Distance => InternalPropertyTableEntry.Distance;

        /// <summary>
        /// The bearing angle, in radians (0 to 2*PI), of this entry.
        /// </summary>
        public double Bearing => InternalPropertyTableEntry.Bearing;

        /// <summary>
        /// The arc radius, in feet, when this entry is an arc. Unused for line entries.
        /// </summary>
        public double Radius => InternalPropertyTableEntry.Radius;

        /// <summary>
        /// The curve type represented by this entry, either Line or Arc.
        /// </summary>
        public DBCurveType CurveType => InternalPropertyTableEntry.CurveType;

        /// <summary>
        /// For arc entries, indicates whether the arc bulges to the Left or the Right of the line segment.
        /// </summary>
        public DBArcDirection ArcDirection => InternalPropertyTableEntry.ArcDirection;

        /// <summary>
        /// The unique identifier of this entry within its property table.
        /// </summary>
        public int Id => InternalPropertyTableEntry.Id;

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates a line-segment PropertyTableEntry.
        /// </summary>
        /// <param name="distance">Distance, in feet, between the segment's start and end points.</param>
        /// <param name="bearing">Bearing angle, in radians (0 to 2*PI).</param>
        /// <param name="id">Optional unique identifier for this entry; defaults to -1.</param>
        public static PropertyTableEntry ByLine(double distance, double bearing, int id = -1)
        {
            return new PropertyTableEntry(distance, bearing, DBCurveType.Line, 0.0, DBArcDirection.Right, id);
        }

        /// <summary>
        /// Creates an arc-segment PropertyTableEntry.
        /// </summary>
        /// <param name="distance">Distance, in feet, between the segment's start and end points.</param>
        /// <param name="bearing">Bearing angle, in radians (0 to 2*PI), of the chord.</param>
        /// <param name="radius">Arc radius, in feet. Must be greater than half of <paramref name="distance"/>.</param>
        /// <param name="direction">"Left" or "Right" — which side of the chord the arc bulges towards.</param>
        /// <param name="id">Optional unique identifier for this entry; defaults to -1.</param>
        public static PropertyTableEntry ByArc(double distance, double bearing, double radius, string direction = "Right", int id = -1)
        {
            if (string.IsNullOrEmpty(direction))
            {
                throw new ArgumentNullException(nameof(direction));
            }

            if (!Enum.TryParse(direction, true, out DBArcDirection parsedDirection))
            {
                throw new ArgumentException(Properties.Resources.PropertyTableEntryInvalid, nameof(direction));
            }

            return new PropertyTableEntry(distance, bearing, DBCurveType.Arc, radius, parsedDirection, id);
        }

        /// <summary>
        /// Creates a PropertyTableEntry by explicitly specifying every Revit-API field.
        /// </summary>
        /// <param name="distance">Distance, in feet, between the segment's start and end points.</param>
        /// <param name="bearing">Bearing angle, in radians (0 to 2*PI).</param>
        /// <param name="curveType">Curve type for this entry.</param>
        /// <param name="arcRadius">Arc radius, in feet (only used when <paramref name="curveType"/> is Arc).</param>
        /// <param name="arcDirection">Arc direction (only used when <paramref name="curveType"/> is Arc).</param>
        /// <param name="id">Optional unique identifier for this entry; defaults to -1.</param>
        public static PropertyTableEntry ByValues(
            double distance,
            double bearing,
            DBCurveType curveType,
            double arcRadius = 0.0,
            DBArcDirection arcDirection = DBArcDirection.Right,
            int id = -1)
        {
            return new PropertyTableEntry(distance, bearing, curveType, arcRadius, arcDirection, id);
        }

        /// <summary>
        /// Returns true if the supplied values would form a valid PropertyTableEntry.
        /// </summary>
        public static bool IsValid(
            double distance,
            double bearing,
            DBCurveType curveType,
            double arcRadius = 0.0,
            DBArcDirection arcDirection = DBArcDirection.Right)
        {
            return DBPropertyTableEntry.IsValidPropertyTableEntry(distance, bearing, curveType, arcRadius, arcDirection);
        }

        #endregion

        #region Internal helpers

        [SupressImportIntoVM]
        internal static PropertyTableEntry FromExisting(DBPropertyTableEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            return new PropertyTableEntry(entry);
        }

        #endregion

        /// <inheritdoc/>
        public override string ToString()
        {
            return CurveType == DBCurveType.Arc
                ? $"PropertyTableEntry(Arc, Distance={Distance}, Bearing={Bearing}, Radius={Radius}, ArcDirection={ArcDirection}, Id={Id})"
                : $"PropertyTableEntry(Line, Distance={Distance}, Bearing={Bearing}, Id={Id})";
        }
    }
}
