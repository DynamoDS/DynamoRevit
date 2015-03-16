using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DynamoRevitVersionSelector;
using DynamoRevitVersionSelector.Properties;

using DynamoVersionSelector.Utilities;


namespace Dynamo.Applications
{
    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class VersionLoader : IExternalApplication
    {
        public static List<IInstalledProduct> Products { get; private set; }

        public static string GetDynamoRevitPath(IInstalledProduct product, string revitVersion)
        {
            if (product.VersionInfo.Item1 == 0 && product.VersionInfo.Item2 < 7)
                return string.Empty; //0.6.3 and older version not supported for Revit2015 onwards

            return Path.Combine(product.InstallLocation, string.Format("Revit_{0}", revitVersion), "DynamoRevitDS.dll");
        }

        public Result OnStartup(UIControlledApplication application)
        {
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
            var dynamoProducts = DynamoProducts.FindDynamoInstallations(debugPath);

            Products = new List<IInstalledProduct>();
            int index = -1;
            foreach (var p in dynamoProducts.Products)
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
                RibbonPanel ribbonPanel = application.CreateRibbonPanel(Resources.DynamoVersions);

                var button = AddSplitButtonGroup(ribbonPanel);
                if(index != -1)
                    button.CurrentButton = button.GetItems().ElementAt(index);
            }

            string loadPath = GetDynamoRevitPath(Products.Last(), revitVersion);
            if (File.Exists(cachedPath))
                loadPath = cachedPath;
            
            if (String.IsNullOrEmpty(loadPath))
                return Result.Failed;

            var ass = Assembly.LoadFrom(loadPath);
            var revitApp = ass.CreateInstance("Dynamo.Applications.DynamoRevitApp");
            revitApp.GetType().GetMethod("OnStartup").Invoke(revitApp, new object[] { application });

            return Result.Succeeded;
        }

        public static bool CacheApplicationPath(int index, string revitVersion)
        {
            if (index >= Products.Count)
                return false; //Index out of range

            var p = Products[index];
            var path = GetDynamoRevitPath(p, revitVersion);
            Utils.WriteToFile(path, revitVersion);

            Utils.ShowRestartMessage(p.VersionString);
            return true;
        }

        private SplitButton AddSplitButtonGroup(RibbonPanel panel)
        {
            var versionData = new SplitButtonData("versions", Resources.DynamoVersions);
            var button = panel.AddItem(versionData) as SplitButton;

            PushButton item = null;
            int i = 0;
            foreach (var p in Products)
            {
                var name = p.VersionString;
                var versionInfo = p.VersionInfo;
                var text = string.Format("{0}.{1}.{2}", versionInfo.Item1, versionInfo.Item2, versionInfo.Item3);

                var itemData = new PushButtonData(
                                name,
                                String.Format(Resources.DynamoVersionText, text),
                                Assembly.GetExecutingAssembly().Location,
                                String.Format("Dynamo.Applications.Command{0}", i++));
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

    }

    [Transaction(TransactionMode.Automatic)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command0 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!VersionLoader.CacheApplicationPath(0, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(1, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(2, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(3, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(4, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(5, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(6, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(7, commandData.Application.Application.VersionNumber))
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
            if (!VersionLoader.CacheApplicationPath(8, commandData.Application.Application.VersionNumber))
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
