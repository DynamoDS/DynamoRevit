using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using GeoTimeZone;

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
            if (timeZone.IsDaylightSavingTime(utc))
            {
                offset++;
            }

            // Remove user local offset. Just leave pure revit datetime.
            return utc.AddHours(offset);
        }

        private readonly static Dictionary<string, string> olsonWindowsTimes = new Dictionary<string, string>()
    {
        { "Africa/Bangui", "W. Central Africa Standard Time" },
        { "Africa/Cairo", "Egypt Standard Time" },
        { "Africa/Casablanca", "Morocco Standard Time" },
        { "Africa/Harare", "South Africa Standard Time" },
        { "Africa/Johannesburg", "South Africa Standard Time" },
        { "Africa/Lagos", "W. Central Africa Standard Time" },
        { "Africa/Monrovia", "Greenwich Standard Time" },
        { "Africa/Nairobi", "E. Africa Standard Time" },
        { "Africa/Windhoek", "Namibia Standard Time" },
        { "America/Anchorage", "Alaskan Standard Time" },
        { "America/Argentina/San_Juan", "Argentina Standard Time" },
        { "America/Asuncion", "Paraguay Standard Time" },
        { "America/Bahia", "Bahia Standard Time" },
        { "America/Bogota", "SA Pacific Standard Time" },
        { "America/Buenos_Aires", "Argentina Standard Time" },
        { "America/Caracas", "Venezuela Standard Time" },
        { "America/Cayenne", "SA Eastern Standard Time" },
        { "America/Chicago", "Central Standard Time" },
        { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
        { "America/Cuiaba", "Central Brazilian Standard Time" },
        { "America/Denver", "Mountain Standard Time" },
        { "America/Fortaleza", "SA Eastern Standard Time" },
        { "America/Godthab", "Greenland Standard Time" },
        { "America/Guatemala", "Central America Standard Time" },
        { "America/Halifax", "Atlantic Standard Time" },
        { "America/Indianapolis", "US Eastern Standard Time" },
        { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
        { "America/La_Paz", "SA Western Standard Time" },
        { "America/Los_Angeles", "Pacific Standard Time" },
        { "America/Mexico_City", "Mexico Standard Time" },
        { "America/Montevideo", "Montevideo Standard Time" },
        { "America/New_York", "Eastern Standard Time" },
        { "America/Noronha", "UTC-02" },
        { "America/Phoenix", "US Mountain Standard Time" },
        { "America/Regina", "Canada Central Standard Time" },
        { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
        { "America/Santiago", "Pacific SA Standard Time" },
        { "America/Sao_Paulo", "E. South America Standard Time" },
        { "America/St_Johns", "Newfoundland Standard Time" },
        { "America/Tijuana", "Pacific Standard Time" },
        { "Antarctica/McMurdo", "New Zealand Standard Time" },
        { "Atlantic/South_Georgia", "UTC-02" },
        { "Asia/Almaty", "Central Asia Standard Time" },
        { "Asia/Amman", "Jordan Standard Time" },
        { "Asia/Baghdad", "Arabic Standard Time" },
        { "Asia/Baku", "Azerbaijan Standard Time" },
        { "Asia/Bangkok", "SE Asia Standard Time" },
        { "Asia/Beirut", "Middle East Standard Time" },
        { "Asia/Calcutta", "India Standard Time" },
        { "Asia/Colombo", "Sri Lanka Standard Time" },
        { "Asia/Damascus", "Syria Standard Time" },
        { "Asia/Dhaka", "Bangladesh Standard Time" },
        { "Asia/Dubai", "Arabian Standard Time" },
        { "Asia/Irkutsk", "North Asia East Standard Time" },
        { "Asia/Jerusalem", "Israel Standard Time" },
        { "Asia/Kabul", "Afghanistan Standard Time" },
        { "Asia/Kamchatka", "Kamchatka Standard Time" },
        { "Asia/Karachi", "Pakistan Standard Time" },
        { "Asia/Katmandu", "Nepal Standard Time" },
        { "Asia/Kolkata", "India Standard Time" },
        { "Asia/Krasnoyarsk", "North Asia Standard Time" },
        { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
        { "Asia/Kuwait", "Arab Standard Time" },
        { "Asia/Magadan", "Magadan Standard Time" },
        { "Asia/Muscat", "Arabian Standard Time" },
        { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
        { "Asia/Oral", "West Asia Standard Time" },
        { "Asia/Rangoon", "Myanmar Standard Time" },
        { "Asia/Riyadh", "Arab Standard Time" },
        { "Asia/Seoul", "Korea Standard Time" },
        { "Asia/Shanghai", "China Standard Time" },
        { "Asia/Singapore", "Singapore Standard Time" },
        { "Asia/Taipei", "Taipei Standard Time" },
        { "Asia/Tashkent", "West Asia Standard Time" },
        { "Asia/Tbilisi", "Georgian Standard Time" },
        { "Asia/Tehran", "Iran Standard Time" },
        { "Asia/Tokyo", "Tokyo Standard Time" },
        { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
        { "Asia/Vladivostok", "Vladivostok Standard Time" },
        { "Asia/Yakutsk", "Yakutsk Standard Time" },
        { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
        { "Asia/Yerevan", "Armenian Standard Time" },
        { "Atlantic/Azores", "Azores Standard Time" },
        { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
        { "Atlantic/Reykjavik", "Greenwich Standard Time" },
        { "Australia/Adelaide", "Cen. Australia Standard Time" },
        { "Australia/Brisbane", "E. Australia Standard Time" },
        { "Australia/Darwin", "AUS Central Standard Time" },
        { "Australia/Hobart", "Tasmania Standard Time" },
        { "Australia/Perth", "W. Australia Standard Time" },
        { "Australia/Sydney", "AUS Eastern Standard Time" },
        { "Etc/GMT", "UTC" },
        { "Etc/GMT+11", "UTC-11" },
        { "Etc/GMT+12", "Dateline Standard Time" },
        { "Etc/GMT+2", "UTC-02" },
        { "Etc/GMT-12", "UTC+12" },
        { "Europe/Amsterdam", "W. Europe Standard Time" },
        { "Europe/Athens", "GTB Standard Time" },
        { "Europe/Belgrade", "Central Europe Standard Time" },
        { "Europe/Berlin", "W. Europe Standard Time" },
        { "Europe/Brussels", "Romance Standard Time" },
        { "Europe/Budapest", "Central Europe Standard Time" },
        { "Europe/Dublin", "GMT Standard Time" },
        { "Europe/Helsinki", "FLE Standard Time" },
        { "Europe/Istanbul", "GTB Standard Time" },
        { "Europe/Kiev", "FLE Standard Time" },
        { "Europe/London", "GMT Standard Time" },
        { "Europe/Minsk", "E. Europe Standard Time" },
        { "Europe/Moscow", "Russian Standard Time" },
        { "Europe/Paris", "Romance Standard Time" },
        { "Europe/Sarajevo", "Central European Standard Time" },
        { "Europe/Warsaw", "Central European Standard Time" },
        { "Indian/Mauritius", "Mauritius Standard Time" },
        { "Pacific/Apia", "Samoa Standard Time" },
        { "Pacific/Auckland", "New Zealand Standard Time" },
        { "Pacific/Fiji", "Fiji Standard Time" },
        { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
        { "Pacific/Guam", "West Pacific Standard Time" },
        { "Pacific/Honolulu", "Hawaiian Standard Time" },
        { "Pacific/Pago_Pago", "UTC-11" },
        { "Pacific/Port_Moresby", "West Pacific Standard Time" },
        { "Pacific/Tongatapu", "Tonga Standard Time" }
    };

        /// <summary>
        /// Converts an Olson time zone ID to a Windows time zone ID.
        /// </summary>
        /// <param name="olsonTimeZoneId">An Olson time zone ID. See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html. </param>
        /// <returns>
        /// The TimeZoneInfo corresponding to the Olson time zone ID, 
        /// or null if you passed in an invalid Olson time zone ID.
        /// </returns>
        /// <remarks>
        /// See http://unicode.org/repos/cldr-tmp/trunk/diff/supplemental/zone_tzid.html
        /// </remarks>
        private static TimeZoneInfo OlsonTimeZoneToTimeZoneInfo(string olsonTimeZoneId)
        {
            var windowsTimeZoneId = default(string);
            var windowsTimeZone = default(TimeZoneInfo);
            if (olsonWindowsTimes.TryGetValue(olsonTimeZoneId, out windowsTimeZoneId))
            {
                try { windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return windowsTimeZone;
        }
    }
}
