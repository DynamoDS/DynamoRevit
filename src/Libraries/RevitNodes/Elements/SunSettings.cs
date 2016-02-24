using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using GeoTimeZone;

using NodaTime.TimeZones;

using Revit.GeometryConversion;

using Point = Autodesk.DesignScript.Geometry.Point;

namespace Revit.Elements
{
    public class SunSettings : Element
    {
        [IsVisibleInDynamoLibrary(false)]
        public static SunSettings Current()
        {
            return new SunSettings(Document.ActiveView.SunAndShadowSettings);
        }

        private SunSettings(SunAndShadowSettings settings)
        {
            SafeInit(() => InitSunSettings(settings));
        }

        private void InitSunSettings(SunAndShadowSettings settings)
        {
            InternalSunAndShadowSettings = settings;
            InternalElementId = settings.Id;
            InternalUniqueId = settings.UniqueId;
            IsRevitOwned = true;
        }

        internal SunAndShadowSettings InternalSunAndShadowSettings { get; private set; }

        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalSunAndShadowSettings; }
        }

        /// <summary>
        ///     Calculates the direction of the sun.
        /// </summary>
        public Vector SunDirection
        {
            get
            {
                // Rotations are + CW from Y (North)
                // This contradicts the Revit API documentation
                // but matches the Sun Path diagram in Revit.
                var initialDirection = Vector.YAxis();

                var azimuth =
                    InternalSunAndShadowSettings.GetFrameAzimuth(
                        InternalSunAndShadowSettings.ActiveFrame);

                var altitude =
                    InternalSunAndShadowSettings.GetFrameAltitude(
                        InternalSunAndShadowSettings.ActiveFrame);

                var cs = CoordinateSystem.Identity()
                    .Rotate(Point.Origin(), Vector.ZAxis(), -azimuth.ToDegrees());

                var sunDirection =
                    initialDirection.Transform(cs.Rotate(cs.Origin, cs.XAxis, altitude.ToDegrees()));

                return sunDirection.Scale(100);
            }
        }

        /// <summary>
        ///     Extracts the Altitude.
        /// </summary>
        public double Altitude
        {
            get { return InternalSunAndShadowSettings.GetFrameAltitude(InternalSunAndShadowSettings.ActiveFrame).ToDegrees(); }

        }

        /// <summary>
        ///     Extracts the Azimuth.
        /// </summary>
        public double Azimuth
        {
            get { return InternalSunAndShadowSettings.GetFrameAzimuth(InternalSunAndShadowSettings.ActiveFrame).ToDegrees(); }

        }


        /// <summary>
        ///     Gets the Start Date and Time of the solar study given in the local time of the solar study location.
        /// </summary>
        public DateTime StartDateTime
        {
            get
            {
                string timeZoneName = TimeZoneLookup.GetTimeZone(
                    InternalSunAndShadowSettings.Latitude.ToDegrees(),
                    InternalSunAndShadowSettings.Longitude.ToDegrees()).Result;
                var timeZone = OlsonTimeZoneToTimeZoneInfo(timeZoneName);

                return TranslateTime(InternalSunAndShadowSettings.StartDateAndTime, timeZone);
            }

        }

        /// <summary>
        ///     Gets the End Date and Time of the solar study given in the local time of the solar study location.
        /// </summary>
        public DateTime EndDateTime
        {
            get
            {
                string timeZoneName = TimeZoneLookup.GetTimeZone(
                    InternalSunAndShadowSettings.Latitude.ToDegrees(),
                    InternalSunAndShadowSettings.Longitude.ToDegrees()).Result;
                var timeZone = OlsonTimeZoneToTimeZoneInfo(timeZoneName);

                return TranslateTime(InternalSunAndShadowSettings.EndDateAndTime, timeZone);
            }
        }

        /// <summary>
        ///     Gets the Date and Time for the current frame of the solar study given in the local time of the solar study location.
        /// </summary>
        public DateTime CurrentDateTime
        {
            get
            {
                string timeZoneName = TimeZoneLookup.GetTimeZone(
                    InternalSunAndShadowSettings.Latitude.ToDegrees(),
                    InternalSunAndShadowSettings.Longitude.ToDegrees()).Result;
                var timeZone = OlsonTimeZoneToTimeZoneInfo(timeZoneName);

                return TranslateTime(InternalSunAndShadowSettings.ActiveFrameTime, timeZone);
            }
        }

        public override string ToString()
        {
            return string.Format(
                "Name: {0}, Alt: {1}, Azim: {2}",
                InternalSunAndShadowSettings.Name,
                InternalSunAndShadowSettings.Altitude.ToDegrees(),
                InternalSunAndShadowSettings.Azimuth.ToDegrees()
                );
        }

        /// <summary>
        /// Fix for: http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-8993
        /// 
        /// StartDateAndTime, EndDateAndTime and ActiveFrameTime should return local datetime,
        /// that is set in Revit project configuration.
        /// But it returns customized datetime + user timezone offset, which is incorrect.
        /// Althought in documentation it's said, 
        /// that "The output value will be in Coordinated Universal Time (UTC), 
        /// but input may be in local time as well."
        /// There is no means to return local time.
        /// </summary>
        internal static DateTime TranslateTime(DateTime utc, TimeZoneInfo timeZone)
        {
            // Get user local hours offset.
            var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).Hours;

            if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.UtcNow))
            {
                offset--;
            }

            // If it's summer time, we should add one more hour.
            // For some reasons Revit API doesn't do it.
            if (timeZone != null && timeZone.IsDaylightSavingTime(utc))
            {
                offset++;
            }

            // Remove user local offset. Just leave pure revit datetime.
            return utc.AddHours(offset);
        }

        /// <summary>
        /// Converts an Olson time zone ID to a Windows time zone ID.
        /// </summary>
        /// <param name="olsonTimeZoneId">An Olson time zone ID. See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html. </param>
        /// <returns>
        /// The TimeZoneInfo corresponding to the Olson time zone ID, 
        /// or null if you passed in an invalid Olson time zone ID.
        /// </returns>
        private static TimeZoneInfo OlsonTimeZoneToTimeZoneInfo(string olsonTimeZoneId)
        {
            var mappings = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones;
            var map = mappings.FirstOrDefault(x =>
                x.TzdbIds.Any(z => z.Equals(olsonTimeZoneId, StringComparison.OrdinalIgnoreCase)));
            return map == null ? null : TimeZoneInfo.FindSystemTimeZoneById(map.WindowsId);
        }
    }
}
