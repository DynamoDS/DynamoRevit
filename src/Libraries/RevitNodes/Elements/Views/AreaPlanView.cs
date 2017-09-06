using System;
using RevitServices.Persistence;
using RevitServices.Transactions;
using DynamoServices;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit Area Plan View
    /// </summary>
    [RegisterForTrace]
    public class AreaPlanView : PlanView
    {

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private AreaPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            SafeInit(() => InitAreaPlanView(view));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private AreaPlanView(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.AreaScheme areaScheme)
        {
            SafeInit(() => InitAreaPlanView(level, areaScheme));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a AreaPlanView element
        /// </summary>
        private void InitAreaPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            InternalSetPlanView(view);
        }

        /// <summary>
        /// Initialize a AreaPlanView element
        /// </summary>
        private void InitAreaPlanView(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.AreaScheme areaScheme)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var vd = CreateAreaPlan(level, areaScheme);

            InternalSetPlanView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create an Area Plan View at the given Level.
        /// </summary>
        /// <param name="level">The Level on which the AreaPlanView is based.</param>
        /// <param name="areaScheme">Area Scheme to be applied to plan view.</param>
        /// <returns>An Area Plan View if successful.</returns>
        public static AreaPlanView ByLevelAndAreaScheme(Level level, Element areaScheme)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }
            if (areaScheme == null)
            {
                throw new ArgumentNullException("level");
            }

            var scheme = areaScheme.InternalElement as Autodesk.Revit.DB.AreaScheme;

            return new AreaPlanView(level.InternalLevel, scheme);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing Element
        /// </summary>
        /// <param name="plan">Area Plan View.</param>
        /// <param name="isRevitOwned">Is Revit Owned?</param>
        /// <returns></returns>
        internal static AreaPlanView FromExisting(Autodesk.Revit.DB.ViewPlan plan, bool isRevitOwned)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }

            return new AreaPlanView(plan)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}

