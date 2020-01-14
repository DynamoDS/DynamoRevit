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

        internal Warning(Autodesk.Revit.DB.FailureMessage warning)
        {
            InternalWarning = warning;
        }

        [IsVisibleInDynamoLibrary(false)]
        public static List<Warning> GetWarningByGuid(string guid)
        {
            var warnings = DocumentManager.Instance.CurrentDBDocument
                .GetWarnings()
                .GroupBy(warn => warn.GetFailureDefinitionId().Guid.ToString())
                .ToList();

            var keys = warnings.Select(key => key.First().GetFailureDefinitionId().Guid.ToString());
            var dict = keys.Zip(warnings, (k, v) => new { Key = k, Value = v })
                .ToDictionary(x => x.Key, x => x.Value);
            return dict[guid].Select(warning => new Warning(warning)).ToList();
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
            get{ return this.InternalWarning.GetSeverity().ToString(); }
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
            return this.InternalWarning
                .GetFailingElements()
                .Select(x => DocumentManager.Instance.CurrentDBDocument.GetElement(x).ToDSType(true))
                .ToList();
        }

    }
}
