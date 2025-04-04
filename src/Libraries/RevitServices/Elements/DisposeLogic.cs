using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace RevitServices.Elements
{
    public class DisposeLogic
    {
        /// <summary>
        /// This flag is set to true in the event that you want to
        /// notify IDisposable objects whether Dynamo is shutting down.
        /// Ex. When shutting down Revit, we set IsShuttingDown to true,
        /// and we read this flag in the Dispose method. For AbstractElement
        /// elements this allows us to bypass cleanup of Revit elements.
        /// </summary>
        public static bool IsShuttingDown { get; set; }

        /// <summary>
        /// This flag is set to true when Dynamo is going to close
        /// current home workspace and set to false when home workspace is
        /// removed.
        /// </summary>
        public static bool IsClosingHomeworkspace { get; set; }

        internal void CloseDocumentsOnElementDispo(Document document, Element elementId) 
        {
#if UI_SUPPORT
            Autodesk.Revit.UI.UIDocument uIDocument = new Autodesk.Revit.UI.UIDocument(document);
            var openedViews = uIDocument.GetOpenUIViews().ToList();
            var shouldClosedViews = openedViews.FindAll(x => elementId.Id == x.ViewId);
            foreach (var v in shouldClosedViews)
            {
            if (uIDocument.GetOpenUIViews().ToList().Count() > 1)
                v.Close();
            else
                throw new InvalidOperationException(string.Format(Properties.Resources.CantCloseLastOpenView, this.ToString()));
            }
#endif
        }
    }
}
