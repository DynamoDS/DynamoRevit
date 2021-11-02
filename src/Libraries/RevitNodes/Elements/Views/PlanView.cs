using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Revit.Elements.Views;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// Base class for Revit Plan views
    /// </summary>
    [SupressImportIntoVM]
    public class PlanView : View
    {

        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit element
        /// </summary>
        internal Autodesk.Revit.DB.ViewPlan InternalViewPlan
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalViewPlan; }
        }

        /// <summary>
        /// Set Internal Element from a exsiting element.
        /// </summary>
        /// <param name="element"></param>
        internal override void SetInternalElement(Autodesk.Revit.DB.Element element)
        {
            InternalSetPlanView(element as Autodesk.Revit.DB.ViewPlan);
        }

        #endregion

        #region Protected mutators

        /// <summary>
        /// Set the InternalViewPlan property and the associated element id and unique id
        /// </summary>
        /// <param name="plan">ViewPlan</param>
        protected void InternalSetPlanView(Autodesk.Revit.DB.ViewPlan plan)
        {
            InternalViewPlan = plan;
            InternalElementId = plan.Id;
            InternalUniqueId = plan.UniqueId;
        }

        #endregion

        #region Protected helper methods

        protected static Autodesk.Revit.DB.ViewPlan CreatePlanView(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.ViewFamily planType)
        {
            var viewFam = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == planType);

            if (viewFam == null)
            {
                throw new Exception(Properties.Resources.ViewPlan_ViewFamilyNotFound);
            }

            return Autodesk.Revit.DB.ViewPlan.Create(Document, viewFam.Id, level.Id);
        }

        protected static Autodesk.Revit.DB.ViewPlan CreateAreaPlan(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.AreaScheme areaScheme)
        {
            return Autodesk.Revit.DB.ViewPlan.CreateAreaPlan(Document, areaScheme.Id, level.Id);
        }

        #endregion

    }
}
