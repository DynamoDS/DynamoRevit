using DynamoServices;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit Legend View Wrapper
    /// </summary>
    [RegisterForTrace]
    public class Legend : View
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.View InternalLegend
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement => InternalLegend;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Legend(Autodesk.Revit.DB.View view)
        {
            SafeInit(() => InitLegend(view));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a Legend element
        /// </summary>
        private void InitLegend(Autodesk.Revit.DB.View view)
        {
            InternalSetLegend(view);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the Internal Legend properties and the associated Element Id and Unique Id
        /// </summary>
        /// <param name="view"></param>
        private void InternalSetLegend(Autodesk.Revit.DB.View view)
        {
            InternalLegend = view;
            InternalElementId = view.Id;
            InternalUniqueId = view.UniqueId;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a View from a user selected Element.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Legend FromExisting(Autodesk.Revit.DB.View view, bool isRevitOwned)
        {
            return new Legend(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}