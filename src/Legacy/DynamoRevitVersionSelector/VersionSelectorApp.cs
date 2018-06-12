using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Dynamo.Applications;
using Autodesk.Revit.UI.Events;
using System.Diagnostics;
using DynamoRevitVersionSelector.Properties;

namespace Dynamo.Applications
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class VersionLoader : IExternalApplication
    {
        ProductsManager DynamoProducts;

        /// <summary>
        /// VersionSelector starting point
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // Find available Dynamo Products
            DynamoProducts = new ProductsManager(application);
            if (DynamoProducts.Count == 0)
                return Result.Failed;

            // May be needed to re-route Revit Icons commands, if no Revit product is loaded
            var Commands = new CommandsBinders(application, DynamoProducts.ContainsDynamoPlayer());
            Commands.OnDynamoClick += OnDynamoIconClick;
            Commands.OnDynamoPlayerClick += OnDynamoPlayerIconClick;
            DynamoProducts.OnBeforeLoad += new ProductsManager.BeforeLoadEventHandler(delegate () {
                Commands.UnBind();
            });

            // If multiple product are available then add a Version Selection Button in the Ribbon
            if (DynamoProducts.Count > 1)
            {
                var selectBtn = new VersionSelectorButton();
                selectBtn.AddInRibbon();
                selectBtn.OnButtonClick += OnDynamoVersionIconClick;
            }

            // Get Persisted Dynamo Version
            Version activeVersion;
            activeVersion = Tools.Presistence.ActiveVersion;
            if ((activeVersion != Product.LASTESTDYNAMO) && (!DynamoProducts.Contains(activeVersion)))
                activeVersion = Product.NODYNAMO; // default
            
            // Write it back to keep consistency
            Tools.Presistence.ActiveVersion = activeVersion;


            if (activeVersion == Product.NODYNAMO)
            {
                Commands.Bind();
            }
            else
            {
                // load expected product
                var ret = DynamoProducts.Load(activeVersion);
                if (!ret.IsSucceed) {
                    /* Error, go back in a safe position for the next time */
                    Tools.Presistence.ActiveVersion = Product.NODYNAMO;
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Event when user click on "Dynamo" Icon
        /// </summary>
        /// <remarks>Only occurs if icon associated commands has been binded</remarks>
        private void OnDynamoIconClick(object sender, ExecutedEventArgs e)
        {
            var dlg = new VersionDialog(DynamoProducts.List);
            dlg.OnSelection += new VersionDialog.SelectedEventHandler(delegate (Version v) {
                /* 1- Load Dynamo */
                DynamoProducts.Load(v);
            });
            dlg.OnSelection += new VersionDialog.SelectedEventHandler(delegate (Version v) {
                /* 2- Start Dynamo */
                Version currentversion = ProductsManager.LoadedProduct();
                if (currentversion == Product.NODYNAMO) throw new ArgumentException($"{v} is not loaded");
                UIApplication uiApp = new UIApplication(e.ActiveDocument.Application);
                DynamoProducts.Get(currentversion).ExecuteDynamoCommand(e.GetJournalData(), uiApp);
            });

            dlg.Show();
        }

        /// <summary>
        /// Event when user click on "Dynamo Player" Icon
        /// </summary>
        /// <remarks>Only occurs if icon associated commands has been binded</remarks>
        private void OnDynamoPlayerIconClick(object sender, ExecutedEventArgs e)
        {
            var dlg = new VersionDialog(DynamoProducts.List);
            dlg.OnSelection += new VersionDialog.SelectedEventHandler(delegate (Version v) {
                /* 1- Load Dynamo */
                DynamoProducts.Load(v);
            });
            dlg.OnSelection += new VersionDialog.SelectedEventHandler(delegate (Version v) {
                /* 2- Start Dynamo Player (if we can) */
                Version currentversion = ProductsManager.LoadedProduct();
                if (currentversion == Product.NODYNAMO) throw new ArgumentException($"{v} is not loaded");
                UIApplication uiApp = new UIApplication(e.ActiveDocument.Application);
                var dynamoPlayerCmdId = RevitCommandId.LookupCommandId(CommandsBinders.PLAYERID);
                uiApp.PostCommand(dynamoPlayerCmdId);
            });

            dlg.Show();
        }

        /// <summary>
        /// Event when user click on "Version selection" icon
        /// </summary>
        /// <remarks>"Version selection" icon may not be present</remarks>
        private void OnDynamoVersionIconClick()
        {
            var dlg = new VersionDialog(DynamoProducts.List);
            dlg.AddNoDynamoOption();
            dlg.OnSelection += new VersionDialog.SelectedEventHandler(delegate (Version v)
            {
                /* Load Dynamo */
                var ret = DynamoProducts.Load(v);
                if (!ret.IsSucceed)
                    return;
            
                /* Save Result */
                Tools.Presistence.ActiveVersion = v;
                
                /* UI Confirmation */
                if (ret.fromVersion != ret.afterVersion)
                {
                    var popup = new TaskDialog(Resources.DynamoVersions);
                    popup.MainInstruction = Resources.DynamoVersionSelection;
                    if (ret.Result == ProductsManager.LoadResult.ResultType.PENDING)
                    {
                        /* Need to restart Revit */
                        popup.MainContent = String.Format(Resources.NeedToRestartRevit);
                        if (ret.afterVersion != Product.NODYNAMO)
                            popup.MainContent = String.Format(Resources.ProductNeedToRestartRevit, ret.afterVersion);
                    }
                    else
                    {
                        popup.MainContent = String.Format(Resources.ProductLoaded, ret.afterVersion);
                    }
                    popup.Show();
                }
            });

            dlg.Show();
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
