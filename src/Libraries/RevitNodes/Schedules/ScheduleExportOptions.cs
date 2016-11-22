using Autodesk.DesignScript.Runtime;

namespace Revit.Schedules
{
    /// <summary>
    ///     View Schedule Export Options
    /// </summary>
    public class ScheduleExportOptions
    {

        #region Internal Properties

        /// <summary>
        ///     Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.ViewScheduleExportOptions InternalScheduleExportOptions
        {
            get; set;
        }

        /// <summary>
        ///     Reference to the Element
        /// </summary>
        internal ScheduleExportOptions(Autodesk.Revit.DB.ViewScheduleExportOptions internalScheduleExportOptions)
        {
            this.InternalScheduleExportOptions = internalScheduleExportOptions;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        ///     Creates View Schedule Export Options element.
        /// </summary>
        /// <param name="columnHeaders">How to export column headers. Default is MultipleRows.</param>
        /// <param name="fieldDelimiter">How to delimit fields. Default is Tab.</param>
        /// <param name="headersFootersBlanks">Whether to export group headers, footers, and blank lines. Default is false.</param>
        /// <param name="textQualifier">How to qualify text fields. Default is DoubleQuote.</param>
        /// <param name="title">Whether or not to export the schedule title. Default is false.</param>
        /// <returns name="exportOptions">View Schedule Export Options.</returns>
        public static ScheduleExportOptions ByProperties(
            string columnHeaders = "MultipleRows",
            string fieldDelimiter = "\t",
            bool headersFootersBlanks = false,
            string textQualifier = "DoubleQuote",
            bool title = false
            )
        {
            Autodesk.Revit.DB.ViewScheduleExportOptions exportOptions = new Autodesk.Revit.DB.ViewScheduleExportOptions();

            var h = (Autodesk.Revit.DB.ExportColumnHeaders)System.Enum.Parse(typeof(Autodesk.Revit.DB.ExportColumnHeaders), columnHeaders);
            var tq = (Autodesk.Revit.DB.ExportTextQualifier)System.Enum.Parse(typeof(Autodesk.Revit.DB.ExportTextQualifier), textQualifier);

            exportOptions.ColumnHeaders = h;
            exportOptions.FieldDelimiter = fieldDelimiter;
            exportOptions.HeadersFootersBlanks = headersFootersBlanks;
            exportOptions.TextQualifier = tq;
            exportOptions.Title = title;

            return new ScheduleExportOptions(exportOptions);
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Options for exporting column headers in schedule view export.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum ExportColumnHeaders
        {
            None,
            OneRow,
            MultipleRows
        }

        /// <summary>
        ///     Options for the text qualifier character in schedule view export.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum ExportTextQualifier
        {
            None,
            Quote,
            DoubleQuote
        }

        #endregion
    }
}
