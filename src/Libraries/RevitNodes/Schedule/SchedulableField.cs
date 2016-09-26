
using RevitServices.Persistence;

namespace Revit.Schedule
{
    /// <summary>
    /// Revit Schedule Field
    /// </summary>
    public class SchedulableField
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.SchedulableField InternalSchedulableField
        {
            get; set;
        }

        /// <summary>
        /// Reference to the Element
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
                return this.InternalSchedulableField.GetName(DocumentManager.Instance.CurrentDBDocument);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

    }
}