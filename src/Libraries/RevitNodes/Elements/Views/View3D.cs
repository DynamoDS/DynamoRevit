using System;
using DynamoServices;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit View3D
    /// </summary>
    [RegisterForTrace]
    public class View3D : AbstractView3D
    {
        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private View3D(Autodesk.Revit.DB.View3D view)
        {
            SafeInit(() => Init3DView(view));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a 3d view
        /// </summary>
        private void Init3DView(Autodesk.Revit.DB.View3D view)
        {
            InternalSetView3D(view);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static View3D FromExisting(Autodesk.Revit.DB.View3D view, bool isRevitOwned)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            return new View3D(view)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
