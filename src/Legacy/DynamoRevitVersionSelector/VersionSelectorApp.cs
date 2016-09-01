using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using DynamoRevitVersionSelector.Properties;
using Microsoft.Win32;

namespace Dynamo.Applications
{
    internal struct DynamoProduct
    {
        public string InstallLocation;
        public Version VersionInfo;

        public string VersionString
        {
            get
            {
                return string.Format(
                    @"Dynamo {0}", VersionInfo.ToString(4));
            }
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class VersionLoader : IExternalApplication
    {
        private static UIControlledApplication uiControlledApplication;
        private AddInCommandBinding dynamoCommand;
        private AddInCommandBinding dynamoPlayerCommand;
        internal List<DynamoProduct> Products { get; private set; }
        //PlaylistProducts represent a subset of available Dynamo Products who are supporting Playlist feature.        
        private List<DynamoProduct> PlaylistProducts { get; set; }

        //Minimum version requirements needed by Playlist feature for Dynamo and Revit.
        private const int MinDynamoMajorVersionForPlaylist = 1;
        private const int MinDynamoMinorVersionForPlaylist = 2;
        private const int MinRevitVersionForPlaylist = 2017;


        internal static string GetDynamoRevitPath(DynamoProduct product, string revitVersion)
        {
            if (product.VersionInfo < new Version(0, 7))
                return string.Empty; //0.6.3 and older version not supported for Revit2015 onwards

            return Path.Combine(product.InstallLocation, string.Format("Revit_{0}", revitVersion), "DynamoRevitDS.dll");
        }

        public Result OnStartup(UIControlledApplication application)
        {
            uiControlledApplication = application;
            var revitVersion = application.ControlledApplication.VersionNumber;

            var revitFolder =
                new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var debugPath = revitFolder.Parent.FullName;
            var dynamoProducts = FindDynamoRevitInstallations(debugPath, revitVersion);

            Products = new List<DynamoProduct>();
            PlaylistProducts = new List<DynamoProduct>();
            foreach (var p in dynamoProducts)
            {
                var path = VersionLoader.GetDynamoRevitPath(p, revitVersion);
                if (!File.Exists(path))
                    continue;

                Products.Add(p);

                if (p.VersionInfo.Major >= MinDynamoMajorVersionForPlaylist
                    && p.VersionInfo.Major >= MinDynamoMinorVersionForPlaylist)
                {
                    if (Convert.ToInt64(revitVersion) >= MinRevitVersionForPlaylist)
                    {
                        PlaylistProducts.Add(p);
                    }
                }
            }

            if (Products.Count == 0)
                return Result.Failed;

            Result preliminaryLoadResult = Result.Succeeded;

            // If there are multiple versions installed, then do the command 
            // binding to prompt for version selector task dialog for user to 
            // select specific Dynamo version.
            if (Products.Count > 1)
            {
                var dynamoCmdId = RevitCommandId.LookupCommandId("ID_VISUAL_PROGRAMMING_DYNAMO");
                dynamoCommand = application.CreateAddInCommandBinding(dynamoCmdId);
                dynamoCommand.CanExecute += canExecute;
                dynamoCommand.BeforeExecuted += beforeExecuted;
                dynamoCommand.Executed += executed;

                if (PlaylistProducts.Count >= 1)
                {
                    var dynamoPlayerCmdId = RevitCommandId.LookupCommandId("ID_PLAYLIST_DYNAMO");
                    if (dynamoPlayerCmdId != null)
                    {
                        dynamoPlayerCommand = application.CreateAddInCommandBinding(dynamoPlayerCmdId);
                        dynamoPlayerCommand.CanExecute += canExecute;
                        dynamoPlayerCommand.BeforeExecuted += beforeExecuted;
                        dynamoPlayerCommand.Executed += executedPlaylist;
                    }
                }
            }
            else //If only one product is installed load the Revit App directly
            {
                preliminaryLoadResult = loadProduct(Products.First(), revitVersion);
            }

            return preliminaryLoadResult;
        }

        void executed(object sender, ExecutedEventArgs e)
        {
            var product = PromptVersionSelectorDialog(Products);
            if(product.HasValue)
                LaunchDynamoCommand(product.Value, e);
        }

        void executedPlaylist(object sender, ExecutedEventArgs e)
        {
            if (PlaylistProducts.Count == 0)
                return;

            DynamoProduct productToLaunch = PlaylistProducts.First();
            if (PlaylistProducts.Count > 1)
            {
                var product = PromptVersionSelectorDialog(PlaylistProducts);
                if (product.HasValue)
                {
                    productToLaunch = product.Value;
                }
            }

            LaunchDynamoCommand(productToLaunch, e, true);
        }

        void beforeExecuted(object sender, BeforeExecutedEventArgs e)
        {
            e.UsingCommandData = true;
        }

        void canExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = e.ActiveDocument != null;
        }

        private Result loadProduct(DynamoProduct product, string revitVersion)
        {
            string loadPath = GetDynamoRevitPath(product, revitVersion);

            if (String.IsNullOrEmpty(loadPath))
                return Result.Failed;

            var ass = Assembly.LoadFrom(loadPath);
            var revitApp = ass.CreateInstance("Dynamo.Applications.DynamoRevitApp");
            revitApp.GetType().GetMethod("OnStartup").Invoke(revitApp, new object[] { uiControlledApplication });

            return Result.Succeeded;
        }

        /// <summary>
        /// Prompts for version selection task dialog
        /// </summary>
        /// <param name="availableProducts">Choice list of Dynamo Products available for selection</param>
        /// <returns>DynamoProduct to launch or null</returns>
        private DynamoProduct? PromptVersionSelectorDialog(List<DynamoProduct> availableProducts)
        {
            // Creates a Revit task dialog to communicate information to the user.
            TaskDialog mainDialog = new TaskDialog(Resources.DynamoVersions);
            mainDialog.MainInstruction = Resources.DynamoVersionSelection;
            mainDialog.MainContent = Resources.VersionSelectionContent;

            // Add commmandLink options to task dialog
            int id = (int)TaskDialogCommandLinkId.CommandLink1;
            var revitVersion = uiControlledApplication.ControlledApplication.VersionNumber;
            //Get the default selection data
            var selectorData = VersionSelectorData.ReadFromRegistry(revitVersion);
            var selectedVersion = selectorData.SelectedVersion.ToString(2);
            TaskDialogResult defaultResult = TaskDialogResult.CommandLink1;
            foreach (var item in availableProducts)
            {
                var versionText = String.Format(Resources.DynamoVersionText, item.VersionInfo.ToString(3));
                mainDialog.AddCommandLink((TaskDialogCommandLinkId)id, versionText, item.InstallLocation);
                if (item.VersionInfo.ToString(2) == selectedVersion)
                    defaultResult = (TaskDialogResult)id;

                id++;
            }

            // Set common buttons and default button. If no CommonButton or CommandLink is added,
            // task dialog will show a Close button by default
            mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
            mainDialog.DefaultButton = defaultResult;

            TaskDialogResult tResult = mainDialog.Show();
            if (tResult == TaskDialogResult.Close)
                return null;

            var index = (int)tResult - (int)TaskDialogCommandLinkId.CommandLink1;
            return availableProducts[index];
        }


        /// <summary>
        /// Launches specific version of Dynamo command
        /// </summary>
        /// <param name="product">DynamoProduct to launch</param>
        /// <param name="e">Command executed event argument</param>
        /// <param name="playlist">Launch the Playlist app or just Dynamo</param>
        /// <returns>true for success</returns>
        private bool LaunchDynamoCommand(DynamoProduct product, ExecutedEventArgs e,bool playlist = false)
        {
            if (uiControlledApplication == null)
                throw new InvalidOperationException();

            var revitVersion = uiControlledApplication.ControlledApplication.VersionNumber;
            var path = GetDynamoRevitPath(product, revitVersion);
            var data = new VersionSelectorData() { RevitVersion = revitVersion, SelectedVersion = product.VersionInfo };
            data.WriteToRegistry();

            //Initialize application
            var ass = Assembly.LoadFrom(path);
            var revitApp = ass.CreateInstance("Dynamo.Applications.DynamoRevitApp");
            if (null == revitApp)
                return false;
            
            //Remove the command binding, because now DynamoRevitApp will 
            //do the command binding for DynamoRevit command.
            RemoveCommandBinding();

            var type = revitApp.GetType();
            var result = type.GetMethod("OnStartup").Invoke(revitApp, new object[] { uiControlledApplication });
            if ((Result)result != Result.Succeeded)
                return false;

            UIApplication uiApp = new UIApplication(e.ActiveDocument.Application);

            if (!playlist)
            {
                //Invoke command
                string message = string.Empty;
                result = type.GetMethod("ExecuteDynamoCommand").Invoke(revitApp, new object[] { e.GetJournalData(), uiApp });
                if ((Result)result != Result.Succeeded)
                    return false;
            }
            else
            {
                //Dependent components have done a re-binding of Playlist command in order to be executed in their context.
                //In order to lunch the Playlist we just need to post the command to the Revit application.
                var dynamoPlayerCmdId = RevitCommandId.LookupCommandId("ID_PLAYLIST_DYNAMO");
                if (dynamoPlayerCmdId != null)
                    uiApp.PostCommand(dynamoPlayerCmdId);
            }

            uiControlledApplication = null; //release application, no more needed.
            return true;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            RemoveCommandBinding();
            return Result.Succeeded;
        }

        private void RemoveCommandBinding()
        {
            if (null != dynamoCommand)
            {
                uiControlledApplication.RemoveAddInCommandBinding(dynamoCommand.RevitCommandId);
                dynamoCommand.BeforeExecuted -= beforeExecuted;
                dynamoCommand.CanExecute -= canExecute;
                dynamoCommand.Executed -= executed;
                dynamoCommand = null;
            }

            if (null != dynamoPlayerCommand)
            {
                uiControlledApplication.RemoveAddInCommandBinding(dynamoPlayerCommand.RevitCommandId);
                dynamoPlayerCommand.BeforeExecuted -= beforeExecuted;
                dynamoPlayerCommand.CanExecute -= canExecute;
                dynamoPlayerCommand.Executed -= executedPlaylist;
                dynamoPlayerCommand = null;
            }                

        }

        private static IEnumerable<DynamoProduct> FindDynamoRevitInstallations(string debugPath, string revitVersion)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(debugPath, "DynamoInstallDetective.dll"));
            var type = assembly.GetType("DynamoInstallDetective.Utilities");

            var installationsMethod = type.GetMethod(
                "LocateDynamoInstallations",
                BindingFlags.Public | BindingFlags.Static);

            if (installationsMethod == null)
            {
                throw new MissingMethodException("Method 'DynamoInstallDetective.Utilities.LocateDynamoInstallations' not found");
            }

            Func<string, string> fileLocator =
                p => Path.Combine(p, string.Format("Revit_{0}", revitVersion), "DynamoRevitDS.dll");

            var methodParams = new object[] { debugPath, fileLocator };

            var installs = installationsMethod.Invoke(null, methodParams) as IEnumerable;
            if(null == installs)
                return null;

            return
                installs.Cast<KeyValuePair<string, Tuple<int, int, int, int>>>()
                    .Select(
                        p => new DynamoProduct() { InstallLocation = p.Key, 
                            VersionInfo = new Version(p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4) });
        }

    }

    /// <summary>
    /// VersionSelectorData: A class to keep the selected version data to be stored in registry.
    /// </summary>
    internal class VersionSelectorData
    {
        const string RegKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
        
        /// <summary>
        /// Constructor
        /// </summary>
        public VersionSelectorData()
        { 
            ThisDynamoVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Reads the version selection data for specific revit version from 
        /// registry and returns a new VersionSelectorData.
        /// </summary>
        /// <param name="revitVersion">Revit version</param>
        /// <returns>VersionSelectorData</returns>
        public static VersionSelectorData ReadFromRegistry(string revitVersion)
        {
            var data = new VersionSelectorData() { RevitVersion = revitVersion };
            data.SelectedVersion = data.ThisDynamoVersion; //Default initialization

            try
            {
                var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                regKey = regKey.OpenSubKey(RegKey64);

                var key = string.Format(@"Dynamo {0}.{1}", data.ThisDynamoVersion.Major, data.ThisDynamoVersion.Minor);
                regKey = regKey.OpenSubKey(key);
                if (regKey == null)
                    return data;

                var version = regKey.GetValue(data.RevitVersion);

                Version selectedVersion = null;
                if (Version.TryParse(version as string, out selectedVersion))
                    data.SelectedVersion = selectedVersion;
            }
            catch (SystemException) { }

            return data;
        }

        /// <summary>
        /// Saves this data to Registry
        /// </summary>
        public void WriteToRegistry()
        {
            try
            {
                var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                regKey = regKey.OpenSubKey(RegKey64);

                var key = string.Format(@"Dynamo {0}.{1}", ThisDynamoVersion.Major, ThisDynamoVersion.Minor);
                regKey = regKey.OpenSubKey(key, true);
                if(null != regKey)
                    regKey.SetValue(RevitVersion, SelectedVersion.ToString(), RegistryValueKind.String);
            }
            catch (SystemException) { }
        }

        /// <summary>
        /// Gets the version of this Dynamo
        /// </summary>
        public Version ThisDynamoVersion { get; private set; }

        /// <summary>
        /// Revit version string
        /// </summary>
        public string RevitVersion { get; set; }

        /// <summary>
        /// Gets the version of Dynamo that was selected to run.
        /// </summary>
        public Version SelectedVersion { get; set; }
    }
}
