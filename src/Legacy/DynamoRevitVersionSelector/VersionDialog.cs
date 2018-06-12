using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using DynamoRevitVersionSelector.Properties;

namespace Dynamo.Applications
{
    internal class VersionDialog
    {
        public delegate void SelectedEventHandler(Version version);
        public event SelectedEventHandler OnSelection;

        private List<Product> mProductList;

        /// <summary>
        /// Dialog to provide a Dyanmo Version Picker
        /// </summary>
        /// <param name="ProductList">List of product to show up</param>
        public VersionDialog(List<Product> ProductList)
        {
            mProductList = ProductList;
        }

        /// <summary>
        /// Add the option to select "No Dynamo"
        /// </summary>
        public void AddNoDynamoOption()
        {
            mProductList.Add(new Product(Product.NODYNAMO));
        }

        /// <summary>
        /// Show modal dialog
        /// </summary>
        public void Show()
        {
            // Latest version available
            Version latestVersion = mProductList.First().Version;

            // Current Version loaded in appDomain
            Version currentVersion = ProductsManager.LoadedProduct();
             
            // Selected Version
            Version selectedVersion = Tools.Presistence.ActiveVersion;
            if (selectedVersion == Product.LASTESTDYNAMO) selectedVersion = latestVersion;

            // Creates a Revit task dialog to communicate information to the user.
            TaskDialog mainDialog = new TaskDialog(Resources.DynamoVersions);
            mainDialog.MainInstruction = Resources.DynamoVersionSelection;
            mainDialog.MainContent = Resources.VersionSelectionContent;

            // Add commmandLink options to task dialog
            int id = (int)TaskDialogCommandLinkId.CommandLink1;
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

                // overwrite version by LASTESTDYNAMO, to keep tracking the latest version
                if (version == latestVersion) version = Product.LASTESTDYNAMO;

                // Propagate Version selection
                OnSelection(version);
            }
            catch (Exception) { /* */ }
        }
    }
}
