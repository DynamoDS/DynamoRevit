using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// Performance Adviser Failure Message
    /// </summary>
    public class FailureMessage
    {
        internal Autodesk.Revit.DB.FailureMessage InternalElement
        { 
            get; set;
        }

        [SupressImportIntoVM]
        public FailureMessage(Autodesk.Revit.DB.FailureMessage message)
        {
            this.InternalElement = message;
        }


        /// <summary>
        /// The description of the message.
        /// </summary>
        public string Description
        {
            get
            {
                return InternalElement.GetDescriptionText();
            }
        }

        /// <summary>
        /// The Failing Elements of the message.
        /// </summary>
        public ICollection<Element> FailingElements
        {
            get
            {
                var failingIds = InternalElement.GetFailingElements();
                var failingElements = new List<Element>();

                foreach (var failingId in failingIds)
                {
                    if (failingId != ElementId.InvalidElementId)
                    {
                        var element = DocumentManager.Instance.CurrentDBDocument.GetElement(failingId);
                        failingElements.Add(element.ToDSType(true));
                    }
                }

                return failingElements;
            }
        }


        /// <summary>
        /// Get the failure severity.
        /// </summary>
        public string Severity
        {
            get
            {
                return InternalElement.GetSeverity().ToString();
            }
        }



        public override string ToString()
        {
            return string.Format("{0} : {1}", InternalElement.GetSeverity(), InternalElement.GetDescriptionText());
        }

        
    }
}
