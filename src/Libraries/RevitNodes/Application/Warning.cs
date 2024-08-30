using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Application
{
    public class Warning
    {
        /// <summary>
        /// Internal reference to the Document
        /// </summary>
        internal Autodesk.Revit.DB.FailureMessage InternalWarning { get; private set; }

        internal Warning(Autodesk.Revit.DB.FailureMessage failure)
        {
            InternalWarning = failure;
        }

        [IsVisibleInDynamoLibrary(false)]
        public static List<Warning> GetWarningByGuid(string guid)
        {
            return DocumentManager.Instance.CurrentDBDocument
                .GetWarnings()
                .Where(warn => warn.GetFailureDefinitionId().Guid.ToString() == guid)
                .Select(x => new Warning(x))
                .ToList();
        }

        /// <summary>
        /// Retrieves the description text of the warning.
        /// </summary>
        public string Description
        {
            get { return this.InternalWarning.GetDescriptionText(); }
        }

        /// <summary>
        /// Retrieves the severity of the Warning.
        /// </summary>
        public string Severity
        {
            get{ return Autodesk.Revit.DB.LabelUtils.GetFailureSeverityName(InternalWarning.GetSeverity());
            }
        }

        /// <summary>
        /// Returns a list of all warnings from the document.
        /// </summary>
        /// <param name="document">The document to get the warnings from</param>
        /// <returns>List of all warnings</returns>
        public static List<Warning> GetWarnings(Document document)
        {
            return document.InternalDocument.GetWarnings().Select(x => new Warning(x)).ToList();
        }

        /// <summary>
        /// Retrieves a list of the elements that have caused the failure.
        /// </summary>
        /// <returns>The elements that have caused the failure.</returns>
        public List<Element> GetFailingElements()
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            List<Autodesk.Revit.DB.ElementId> FailingElements = new List<Autodesk.Revit.DB.ElementId>(this.InternalWarning.GetFailingElements());
            FailingElements.AddRange(this.InternalWarning.GetAdditionalElements());
            return FailingElements
                .Select(x => doc.GetElement(x).ToDSType(true))
                .ToList();
        }

    }
}
