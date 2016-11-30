using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
using Dynamo.Models;

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
        public event EventHandler<ViewActivatingEventArgs> ViewActivating;
        public event EventHandler<ViewActivatedEventArgs> ViewActivated;

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

        public void OnApplicationViewActivating(object sender, ViewActivatingEventArgs args)
        {
            InvokeEventHandler(ViewActivating, sender, args);
        }

        public void OnApplicationViewActivated(object sender, ViewActivatedEventArgs args)
        {
            InvokeEventHandler(ViewActivated, sender, args);
        }

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
