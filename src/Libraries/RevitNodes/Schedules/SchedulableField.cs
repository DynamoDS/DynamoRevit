
using RevitServices.Persistence;

namespace Revit.Schedules
{
    /// <summary>
    ///     Revit Schedule Field
    /// </summary>
    public class SchedulableField
    {

        #region Internal Properties

        /// <summary>
        ///     Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.SchedulableField InternalSchedulableField
        {
            get; set;
        }

        /// <summary>
        ///     Internal constructor. Used by the public static constructor to construct an instance of the SchedulableField class.
        /// </summary>
        /// <param name="internalSchedulableField"></param>
        internal SchedulableField(Autodesk.Revit.DB.SchedulableField internalSchedulableField)
        {
            this.InternalSchedulableField = internalSchedulableField;
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
                var doc = DocumentManager.Instance.CurrentDBDocument;
                return this.InternalSchedulableField.GetName(doc);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

    }
}