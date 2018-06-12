using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using adWin = Autodesk.Windows;

namespace Dynamo.Applications.Tools
{
    internal static class UI
    {
        static UI() {
            adWin.ComponentManager.UIElementActivated += new EventHandler<adWin.UIElementActivatedEventArgs>(OnUIEvent);
        }

        // UI Event Handling
        private delegate void EventHandler(object sender, adWin.UIElementActivatedEventArgs e);
        private static Dictionary<string, EventHandler> listener = new Dictionary<string, EventHandler>();
        private static readonly OperationRequestHandler opReq = new OperationRequestHandler();
        private static void OnUIEvent(object sender, adWin.UIElementActivatedEventArgs e)
        {
            if (e == null) return;
            if (e.Item == null) return;
            if (e.Item.Id == null) return;
            if (listener.ContainsKey(e.Item.Id))
            {
                opReq.Raise(() => {
                    listener[e.Item.Id](sender, e);
                });
            }
        }

        ///
        /// <summary>
        ///     Get all the custom Panels on a designated Revit tab.
        /// </summary>
        /// 
        /// <param name="tabName">
        ///     The name of the tab on which the panels are located.
        /// </param>
        /// 
        /// <remarks>
        ///     With official UIControlledApplication.GetRibbonPanels(string tabName),
        ///     Built-in panels would not be included. tabName would need be the name of one of the
        ///     tabs added by Autodesk.Revit.UI.UIApplication.CreateRibbonTab(System.String).
        ///     However thanks to Jeremy Tammik who brings a workaround in his blog post
        ///     http://thebuildingcoder.typepad.com/blog/2013/02/adding-a-btn-to-existing-ribbon-panel.html
        /// </remarks>  
        public static adWin.RibbonPanelCollection GetRibbonPanels(string tabName)
        {
            adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
            foreach (adWin.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Id == tabName)
                {
                    return tab.Panels;
                }
            }
            return new adWin.RibbonPanelCollection(); // empty collection
        }


        ///
        /// <summary>
        ///     This class contains information necessary to construct a push btn in the Ribbon.
        /// </summary>
        public class PushButtonData
        {
            protected adWin.RibbonButton btn;

            public adWin.RibbonButton Source
            {
                get { return btn; }
            }

            public String Text
            {
                get { return btn.Text; }
                set { btn.Text = value; }
            }

            public String ToolTip
            {
                get { return (String)btn.ToolTip; }
                set { btn.ToolTip = value; }
            }

            public System.Drawing.Bitmap Image
            {
                set
                {
                    btn.Image = ImageConversion.ToBitmapSource(value);
                    btn.LargeImage = ImageConversion.ToBitmapSource(value);
                    btn.ShowImage = true;
                }
            }

            public Boolean IsEnabled
            {
                get { return btn.IsEnabled; }
                set { btn.IsEnabled = value; }
            }

            public Boolean IsVisible
            {
                get { return btn.IsVisible; }
                set { btn.IsVisible = value; }
            }

            public delegate void OnClickEventHandler();
            public event OnClickEventHandler OnClickEvent;
            private void OnUIEvent(object sender, adWin.UIElementActivatedEventArgs e)
            {
                OnClickEvent();
            }

            ///
            /// <summary>
            ///     Constructs a new instance of PushButtonData.
            /// </summary>
            /// 
            /// <param name="id">
            ///     Unique identifier (like "ID_MY_BUTTON")
            /// </param>
            /// <param name="name">
            ///     The internal name of the new btn.
            /// </param>
            /// <param name="text">
            ///     The user visible text seen on the new btn.
            /// </param>
            /// 
            /// <exception cref="System.ArgumentNullException">Thrown when null is passed for one or more arguments</exception>
            /// <exception cref="System.ArgumentException">Thrown when an empty string is passed for one or more arguments</exception>
            public PushButtonData(string Id, string name = "", string text = "")
            {
                if (Id == null) throw new ArgumentNullException("Id", "Id shouldn't be null");
                if (String.IsNullOrWhiteSpace(Id)) throw new ArgumentException("Id", "name shouldn't be empty");
                if (String.IsNullOrWhiteSpace(name)) name = Id;
                if (String.IsNullOrWhiteSpace(text)) text = name;
                btn = new adWin.RibbonButton();
                btn.IsEnabled = true;
                btn.IsVisible = true;
                btn.Name = name;
                btn.Text = text;
                btn.Id = Id;
                btn.Size = adWin.RibbonItemSize.Large;
                btn.Orientation = System.Windows.Controls.Orientation.Vertical;
                btn.ShowImage = false;
                btn.ShowText = true;
                btn.GroupLocation = Autodesk.Private.Windows.RibbonItemGroupLocation.Last;
                btn.IsCheckable = true;
                listener.Add(btn.Id, this.OnUIEvent);
            }
        }

        private class OperationRequestHandler : IExternalEventHandler
        {
            public delegate void Operation();
            private Operation mOperation = null;
            private readonly Object opLock = new Object();
            ExternalEvent m_exEvent = null;

            public OperationRequestHandler()
            {
                m_exEvent = ExternalEvent.Create(this);
            }

            public void Raise(Operation operation)
            {
                lock (opLock) mOperation = operation;
                m_exEvent.Raise();
            }


            public String GetName()
            {
                return "UI OperationRequestHandler";
            }

            /// <summary>
            ///   The top method of the event handler.
            /// </summary>
            /// <remarks>
            ///   This is called by Revit after the corresponding
            ///   external event was raised (by the modeless form)
            ///   and Revit reached the time at which it could call
            ///   the event's handler (i.e. this object)
            /// </remarks>
            /// 
            public void Execute(UIApplication uiapp)
            {
                lock (opLock)
                {
                    mOperation();
                }
            }
        }
    }
}
