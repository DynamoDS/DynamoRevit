using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using DynamoRevitVersionSelector.Properties;
using Autodesk.Revit.UI.Events;
using Dynamo.Applications.Tools;

namespace Dynamo.Applications
{
    internal class VersionSelectorButton
    {
        public delegate void OnClickEventHandler();
        public event OnClickEventHandler OnButtonClick;

        public void AddInRibbon()
        {
            foreach (var panel in Tools.UI.GetRibbonPanels("Manage"))
            {
                if(panel.Source.Id == "visualprogramming_shr")
                {
                    var btn = new Tools.UI.PushButtonData("ID_DYANMO_CHANGEVERSION");
                    btn.Text = Resources.VersionSelectionBtnText;
                    btn.ToolTip = Resources.VersionSelectionBtnToolTip;
                    btn.Image = Resources.dynamo_setting_32x32;
                    btn.OnClickEvent += ButtonClick;
                    panel.Source.Items.Add(btn.Source);
                }
            }
        }

        private void ButtonClick()
        {
            OnButtonClick();
        }
    }
}
