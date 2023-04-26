using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit 3D View Template
    /// </summary>
    [RegisterForTrace]
    public class View3DTemplate : View3D
    {

        #region Private constructors

        /// <summary>
        /// Create a Revit Floor Plan from Autodesk View Plan
        /// </summary>
        private View3DTemplate(Autodesk.Revit.DB.View3D view)
        {
            SafeInit(() => Init3DView(view), true);
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a FloorPlanView element
        /// </summary>
        private void Init3DView(Autodesk.Revit.DB.View3D view)
        {
            InternalSetView3D(view);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing Element
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static View3DTemplate FromExisting(Autodesk.Revit.DB.View3D plan, bool isRevitOwned)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("3d_view");
            }

            return new View3DTemplate(plan)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
