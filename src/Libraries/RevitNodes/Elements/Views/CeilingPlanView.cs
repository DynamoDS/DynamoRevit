using System;

using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit ViewPlan
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class CeilingPlanView : PlanView
    {

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private CeilingPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            SafeInit(() => InitCeilingPlanView(view));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private CeilingPlanView(Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitCeilingPlanView(level));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a CeilingPlanView element
        /// </summary>
        private void InitCeilingPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            InternalSetPlanView(view);
        }

        /// <summary>
        /// Initialize a CeilingPlanView element
        /// </summary>
        private void InitCeilingPlanView(Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var vd = CreatePlanView(level, ViewFamily.CeilingPlan);

            InternalSetPlanView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Floor Plan at a given Level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static CeilingPlanView ByLevel(Level level)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            return new CeilingPlanView(level.InternalLevel);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing Element
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static CeilingPlanView FromExisting(Autodesk.Revit.DB.ViewPlan plan, bool isRevitOwned)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }

            return new CeilingPlanView(plan)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
