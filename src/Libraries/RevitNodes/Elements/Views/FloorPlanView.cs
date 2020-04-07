﻿using System;
using DynamoServices;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements.Views
{
    /// <summary>
    /// A Revit ViewPlan
    /// </summary>
    [RegisterForTrace]
    public class FloorPlanView : PlanView
    {

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private FloorPlanView(Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitFloorPlanView(level));
        }

        /// <summary>
        /// Create a Revit Floor Plan from Autodesk View Plan
        /// </summary>
        private FloorPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            SafeInit(() => InitFloorPlanView(view));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a FloorPlanView element
        /// </summary>
        private void InitFloorPlanView(Autodesk.Revit.DB.ViewPlan view)
        {
            InternalSetPlanView(view);
        }

        /// <summary>
        /// Initialize a FloorPlanView element
        /// </summary>
        private void InitFloorPlanView(Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var vd = CreatePlanView(level, Autodesk.Revit.DB.ViewFamily.FloorPlan);

            InternalSetPlanView(vd);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Floor Plan at a given Level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static FloorPlanView ByLevel(Level level)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            return new FloorPlanView( level.InternalLevel );
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Create from existing Element
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FloorPlanView FromExisting( Autodesk.Revit.DB.ViewPlan plan, bool isRevitOwned )
        {
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }

            return new FloorPlanView(plan)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}

