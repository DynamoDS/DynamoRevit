
namespace Revit.Schedules
{
    /// <summary>
    ///     Revit Schedule Field
    /// </summary>
    public class ScheduleField
    {

        #region Internal Properties

        /// <summary>
        ///     Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.ScheduleField InternalScheduleField
        {
            get; set;
        }

        /// <summary>
        ///     Internal constructor. Used by the public static constructor to construct an instance of the ScheduleField class.
        /// </summary>
        /// <param name="internalScheduleField"></param>
        internal ScheduleField(Autodesk.Revit.DB.ScheduleField internalScheduleField)
        {
            this.InternalScheduleField = internalScheduleField;
        }

        #endregion

        #region Public properties

        /// <summary>
        ///     Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.InternalScheduleField.GetName();
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

    }
}