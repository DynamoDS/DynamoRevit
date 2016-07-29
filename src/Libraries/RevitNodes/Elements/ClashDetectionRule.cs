using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using System.Diagnostics;

namespace Revit.Elements
{
    [SupressImportIntoVM]
    public class ClashDetectionRule : IPerformanceAdviserRule
    {
        //private FailureDefinition warning;
        private FailureDefinitionId warningId;
        private List<ElementId> clashingElements;
        private Type type;

        /// <summary>
        /// Constructor
        /// </summary>
        [SupressImportIntoVM]
        public ClashDetectionRule(Type type)
        {
            warningId = Autodesk.Revit.DB.BuiltInFailures.InterferenceFailures.GeometryWarning;
            this.type = type;

        }

        [SupressImportIntoVM]
        public void ExecuteElementCheck(Document document,Autodesk.Revit.DB.Element element)
        {

            FilteredElementCollector collector = new FilteredElementCollector(document);
            ElementIntersectsElementFilter elementFilter = new ElementIntersectsElementFilter(element, false);
            collector.WherePasses(elementFilter);

            List<ElementId> excludes = new List<ElementId>();
            excludes.Add(element.Id);
            collector.Excluding(excludes);

            foreach (Autodesk.Revit.DB.Element elem in collector)
            {
                clashingElements.Add(elem.Id);
            }
        }

        [SupressImportIntoVM]
        public PerformanceAdviserRuleId Id = new PerformanceAdviserRuleId( Guid.NewGuid());

        [SupressImportIntoVM]
        public void FinalizeCheck(Document document)
        {
            try
            {
                if (clashingElements.Count == 0)
                {
                    Debug.WriteLine("No clashes");
                }
                else
                {
                    Autodesk.Revit.DB.FailureMessage fm = new Autodesk.Revit.DB.FailureMessage(warningId);

                    fm.SetFailingElements(clashingElements);

                    Autodesk.Revit.DB.Transaction failureReportingTransaction = new Autodesk.Revit.DB.Transaction(document);

                    failureReportingTransaction.Start("Failure reporting");

                    PerformanceAdviser.GetPerformanceAdviser().PostWarning(fm);

                    failureReportingTransaction.Commit();

                    clashingElements.Clear();
                }
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        /// <summary>
        /// Return rule description
        /// </summary>
        [SupressImportIntoVM]
        public string GetDescription()
        {
            return "Detects clashes between " + type.Name;
        }

        [SupressImportIntoVM]
        public ElementFilter GetElementFilter(Document document)
        {
            return new ElementClassFilter(type);
        }

        /// <summary>
        /// Return name of rule
        /// </summary>
        [SupressImportIntoVM]
        public string GetName()
        {
            return type.Name + " Clash Detection";
        }


        [SupressImportIntoVM]
        public void InitCheck(Document document)
        {
            if (clashingElements == null)
                clashingElements = new List<ElementId>();
            else
                clashingElements.Clear();

            return;
        }


        [SupressImportIntoVM]
        public bool WillCheckElements()
        {
            return true;
        }
    }
}