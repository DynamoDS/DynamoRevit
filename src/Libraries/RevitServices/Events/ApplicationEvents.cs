using Autodesk.Revit.DB.Events;
using System;

namespace RevitServices.Events
{
    internal static class ApplicationEvents
    {
        public static event EventHandler<DocumentOpenedEventArgs> DocumentOpened;
        public static event EventHandler<DocumentClosingEventArgs> DocumentClosing;
        public static event EventHandler<DocumentClosedEventArgs> DocumentClosed;
#if !DESIGN_AUTOMATION
        public static event EventHandler<Autodesk.Revit.UI.Events.ViewActivatingEventArgs> ViewActivating;
        public static event EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs> ViewActivated;
#endif

        public static void OnApplicationDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            InvokeEventHandler(DocumentOpened, sender, args);
        }

        public static void OnApplicationDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            InvokeEventHandler(DocumentClosing, sender, args);
        }

        public static void OnApplicationDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            InvokeEventHandler(DocumentClosed, sender, args);
        }

#if !DESIGN_AUTOMATION
        public static void OnApplicationViewActivating(object sender, Autodesk.Revit.UI.Events.ViewActivatingEventArgs args)
        {
            InvokeEventHandler(ViewActivating, sender, args);
        }

        public static void OnApplicationViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs args)
        {
            InvokeEventHandler(ViewActivated, sender, args);
        }
#endif

        private static void InvokeEventHandler<T>(EventHandler<T> eventHandler, object sender, T args) where T : EventArgs
        {
            var tempHandler = eventHandler;
            if (tempHandler != null)
            {
                tempHandler(sender, args);
            }
        }
    }
}
