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
    public class StructuralPlanView : PlanView
    {

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private StructuralPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            SafeInit(() => InitStructuralPlanView(view));
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private StructuralPlanView(Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitStructuralPlanView(level));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a StructuralPlanView element
        /// </summary>
        private void InitStructuralPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            InternalSetPlanView(view);
        }

        /// <summary>
        /// Initialize a StructuralPlanView element
        /// </summary>
        private void InitStructuralPlanView(Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var vd = CreatePlanView(level, ViewFamily.StructuralPlan);

            InternalSetPlanView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Structural Plan View at the given Level.
        /// </summary>
        /// <param name="level">The Level on which the StructuralPlanView is based.</param>
        /// <returns>A StructuralPlanView if successful.</returns>
        public static StructuralPlanView ByLevel(Level level)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            return new StructuralPlanView(level.InternalLevel);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing Element
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static StructuralPlanView FromExisting(Autodesk.Revit.DB.ViewPlan plan, bool isRevitOwned)
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }

            return new StructuralPlanView(plan)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
