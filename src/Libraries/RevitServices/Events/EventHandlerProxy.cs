using System;
using Autodesk.Revit.DB.Events;

namespace RevitServices.EventHandler
{
    /// <summary>
    /// This is a event handler proxy class to serve as a proxy between the event publisher and
    /// the event subscriber
    /// </summary>
    public class EventHandlerProxy
    {
        public event EventHandler<DocumentOpenedEventArgs> DocumentOpened;
        public event EventHandler<DocumentClosingEventArgs> DocumentClosing;
        public event EventHandler<DocumentClosedEventArgs> DocumentClosed;
#if UI_SUPPORT
        public event EventHandler<Autodesk.Revit.UI.Events.ViewActivatingEventArgs> ViewActivating;
        public event EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs> ViewActivated;
#endif

        public void OnApplicationDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            InvokeEventHandler(DocumentOpened, sender, args);
        }

        public void OnApplicationDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            InvokeEventHandler(DocumentClosing, sender, args);
        }

        public void OnApplicationDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            InvokeEventHandler(DocumentClosed, sender, args);
        }

#if UI_SUPPORT
        public void OnApplicationViewActivating(object sender, Autodesk.Revit.UI.Events.ViewActivatingEventArgs args)
        {
            InvokeEventHandler(ViewActivating, sender, args);
        }

        public void OnApplicationViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs args)
        {
            InvokeEventHandler(ViewActivated, sender, args);
        }
#endif

        private void InvokeEventHandler<T>(EventHandler<T> eventHandler, object sender, T args) where T: EventArgs
        {
            var tempHandler = eventHandler;
            if (tempHandler != null)
            {
                tempHandler(sender, args);
            }
        }
    }
}
