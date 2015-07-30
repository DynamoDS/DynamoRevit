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

using DynamoRevitVersionSelector.Properties;

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
        private static UIControlledApplication uiApplication;
        private static SplitButton splitButton;
        private static RibbonPanel ribbonPanel;
        internal static List<DynamoProduct> Products { get; private set; }

        internal static string GetDynamoRevitPath(DynamoProduct product, string revitVersion)
        {
            if (product.VersionInfo < new Version(0, 7))
                return string.Empty; //0.6.3 and older version not supported for Revit2015 onwards

            return Path.Combine(product.InstallLocation, string.Format("Revit_{0}", revitVersion), "DynamoRevitDS.dll");
        }

        public Result OnStartup(UIControlledApplication application)
        {
            uiApplication = application;
            // now we have a default path, but let's look at
            // the load path file to see what was last selected
            var cachedPath = String.Empty;
            var revitVersion = application.ControlledApplication.VersionNumber;
            var fileLoc = Utils.GetVersionSaveFileLocation(revitVersion);

            if (File.Exists(fileLoc))
            {
                using (var sr = new StreamReader(fileLoc))
                {
                    cachedPath = sr.ReadToEnd().TrimEnd('\r', '\n');
                }
            }

            var revitFolder =
                new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var debugPath = revitFolder.Parent.FullName;
            var dynamoProducts = FindDynamoInstallations(debugPath);

            Products = new List<DynamoProduct>();
            int index = -1;
            foreach (var p in dynamoProducts)
            {
                var path = VersionLoader.GetDynamoRevitPath(p, revitVersion);
                if (!File.Exists(path))
                    continue;

                if (path.Equals(cachedPath))
                    index = Products.Count;

                Products.Add(p);
            }

            // If there are multiple versions installed, then create
            // a couple of push buttons in a panel to allow selection of a version.
            // If only one version is installed, no multi-selection is required.
            if (Products.Count > 1)
            {
                ribbonPanel = application.CreateRibbonPanel(Resources.DynamoVersions);

                splitButton = AddSplitButtonGroup(ribbonPanel);
                
                if(index != -1)
                    splitButton.CurrentButton = splitButton.GetItems().ElementAt(index);
            }

            string loadPath = GetDynamoRevitPath(Products.Last(), revitVersion);
            if (File.Exists(cachedPath))
                loadPath = cachedPath;
            
            if (String.IsNullOrEmpty(loadPath))
                return Result.Failed;

            if (Products.Count == 1) //If only one product is installed load the Revit App directly
            {
                var ass = Assembly.LoadFrom(loadPath);
                var revitApp = ass.CreateInstance("Dynamo.Applications.DynamoRevitApp");
                revitApp.GetType().GetMethod("OnStartup").Invoke(revitApp, new object[] { application });
            }

            return Result.Succeeded;
        }

        public static bool LaunchDynamoCommand(int index, ExternalCommandData commandData)
        {
            if (index >= Products.Count)
                return false; //Index out of range

            try
            {
                var revitVersion = commandData.Application.Application.VersionNumber;
                var p = Products[index];
                var path = GetDynamoRevitPath(p, revitVersion);
                Utils.WriteToFile(path, revitVersion);

                splitButton.Enabled = false; //Disable the split button, no more needed.
                splitButton.Visible = false; //Hide it from the UI
                //For older than 0.8.2, hide the Dynamo Versions ribbon panel
                //Otherwise rename it to "Visual Programming", so that DynamoRevit
                //will add it's button to this panel.
                if (p.VersionInfo < new Version(0, 8, 2))
                {
                    ribbonPanel.Visible = false; //Hide the panel
                }
                else
                {
                    ribbonPanel.Name = Resources.VisualProgramming;//Same as used in App Description for DynamoRevitApp
                    ribbonPanel.Title = Resources.VisualProgramming;
                }

                //Initialize application
                var ass = Assembly.LoadFrom(path);
                var revitApp = ass.CreateInstance("Dynamo.Applications.DynamoRevitApp");
                revitApp.GetType()
                    .GetMethod("OnStartup")
                    .Invoke(revitApp, new object[] { uiApplication });

                //Invoke command
                string message = string.Empty;
                var revitCmd = ass.CreateInstance("Dynamo.Applications.DynamoRevit");
                revitCmd.GetType()
                    .GetMethod("Execute")
                    .Invoke(revitCmd, new object[] { commandData, message, null });

                uiApplication = null; //release application, no more needed.
                return true;
            }
            catch (Exception)
            {
                splitButton.Enabled = true; //Ensure that the split button is enabled.
                splitButton.Visible = true; //Also the split button is visible
                ribbonPanel.Visible = true; //As well as the panel is visible
                throw;
            }
        }

        private SplitButton AddSplitButtonGroup(RibbonPanel panel)
        {
            var versionData = new SplitButtonData("versions", Resources.DynamoVersions);
            var button = panel.AddItem(versionData) as SplitButton;

            Bitmap dynamoIcon = Resources.dynamo_32x32;

            BitmapSource bitmapSource =
                Imaging.CreateBitmapSourceFromHBitmap(
                    dynamoIcon.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            PushButton item = null;
            int i = 0;
            foreach (var p in Products)
            {
                var name = p.VersionString;
                var versionInfo = p.VersionInfo;
                var text = versionInfo.ToString(3);
                
                var itemData = new PushButtonData(
                                name,
                                String.Format(Resources.DynamoVersionText, text),
                                Assembly.GetExecutingAssembly().Location,
                                String.Format("Dynamo.Applications.Command{0}", i++));

                itemData.Image = bitmapSource;
                itemData.LargeImage = bitmapSource;
                itemData.ToolTip = string.Format(Resources.DynamoVersionTooltip, text);
                item = button.AddPushButton(itemData);
            }

            button.IsSynchronizedWithCurrentItem = true;
            button.CurrentButton = item;
            return button;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private static IEnumerable<DynamoProduct> FindDynamoInstallations(string debugPath)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(debugPath, "DynamoInstallDetective.dll"));
            var type = assembly.GetType("DynamoInstallDetective.Utilities");

            var installationsMethod = type.GetMethod(
                "FindDynamoInstallations",
                BindingFlags.Public | BindingFlags.Static);

            if (installationsMethod == null)
            {
                throw new MissingMethodException("Method 'DynamoInstallDetective.Utilities.FindDynamoInstallations' not found");
            }

            var methodParams = new object[] { debugPath };
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

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command0 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(0, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(1, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(2, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(3, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(4, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command5 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(5, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command6 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(6, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command7 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(7, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command8 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.LaunchDynamoCommand(8, commandData))
                return Result.Failed;

            return Result.Succeeded;
        }
    }

    internal class Utils
    {
        internal static void WriteToFile(string loadPath, string versionName)
        {
            var path = GetVersionSaveFileLocation(versionName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var tw = new StreamWriter(path))
            {
                tw.WriteLine(loadPath);
            }
        }

        /// <summary>
        /// Return PreferenceSettings Default XML File Path if possible
        /// </summary>
        internal static string GetVersionSaveFileLocation(string versionName)
        {
            try
            {
                string appDataFolder = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.ApplicationData);

                return (Path.Combine(appDataFolder, "Dynamo", string.Format("DynamoDllForLoad_{0}.txt", versionName)));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        internal static void ShowRestartMessage(string version)
        {
            MessageBox.Show(string.Format(Resources.RestartMessage, version),
                Resources.DynamoVersions, MessageBoxButton.OK);
        }
    }
}
