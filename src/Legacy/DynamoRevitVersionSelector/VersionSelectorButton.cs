using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using DynamoRevitVersionSelector.Properties;

namespace Dynamo.Applications
{
    internal class VersionSelectorButton
    {
        private List<Product> mProductList;
        private UIControlledApplication uiControlledApplication;

        public delegate Result OnVersionSelectedEventHandler(Version version);
        public event OnVersionSelectedEventHandler OnVersionSelected;

        public VersionSelectorButton(UIControlledApplication application, List<Product> ProductList)
        {
            uiControlledApplication = application;
            mProductList = ProductList;
        }

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
                    btn.OnClickEvent += OnClick;
                    panel.Source.Items.Add(btn.Source);
                }
            }
        }

        private void OnClick()
        {
            // Latest version available
            Version latestVersion = mProductList.First().Version;

            // Current Version loaded in appDomain
            Version currentVersion = latestVersion;
            var isCurrentVersion = ProductsManager.IsProductLoaded(ref currentVersion);

            // Selected Version
            Version selectedVersion = Tools.Presistence.ActiveVersion;
            if (selectedVersion == Product.LASTESTDYNAMO) selectedVersion = latestVersion;

            // Creates a Revit task dialog to communicate information to the user.
            TaskDialog mainDialog = new TaskDialog(Resources.DynamoVersions);
            mainDialog.MainInstruction = Resources.DynamoVersionSelection;
            mainDialog.MainContent = Resources.VersionSelectionContent;

            // Add commmandLink options to task dialog
            int id = (int)TaskDialogCommandLinkId.CommandLink1;
            var revitVersion = uiControlledApplication.ControlledApplication.VersionNumber;
            TaskDialogResult defaultResult = TaskDialogResult.CommandLink1;
            foreach (var item in mProductList)
            {
                item.isCurrent = (item.Version == currentVersion);
                mainDialog.AddCommandLink((TaskDialogCommandLinkId)id, item.Title, item.SubTitle);
                if (item.Version == selectedVersion) defaultResult = (TaskDialogResult)id;
                id++;
            }
            mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
            mainDialog.DefaultButton = defaultResult;

            TaskDialogResult tResult = mainDialog.Show();
            if (tResult == TaskDialogResult.Close)
                return;

            var index = (int)tResult - (int)TaskDialogCommandLinkId.CommandLink1;
            try
            {
                var version = mProductList[index].Version;
                if (version == latestVersion) version = Product.LASTESTDYNAMO;

                // Save Result
                Tools.Presistence.ActiveVersion = version;
                OnVersionSelected(version);
            }
            catch (Exception) { /* */ }

        }
    }
}
