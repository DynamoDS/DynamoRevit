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
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class VersionLoader : IExternalApplication
    {
        ProductsManager DynamoProducts;

        public Result OnStartup(UIControlledApplication application)
        {
            // Find available Dyanmo Products
            DynamoProducts = new ProductsManager(application);
            if (DynamoProducts.Count == 0)
            {
                return Result.Failed;
            }

            // If multiple product are available then add a selection Button in the Ribbon
            if (DynamoProducts.Count > 1)
            {
                var selectBtn = new VersionSelectorButton(application, DynamoProducts.List);
                selectBtn.AddInRibbon();
                selectBtn.OnVersionSelected += LoadDynamoProduct;
            }

            // Get Persisted Dyanmo Version (by default get latest)
            Version activeVersion = Product.LASTESTDYNAMO;
            try
            {
                activeVersion = Tools.Presistence.ActiveVersion;
            }
            catch (Exception) { /* */ }

            return LoadDynamoProduct(activeVersion);
        }

        private Result LoadDynamoProduct(Version version) {
            // Start Dynamo Product
            try
            {
                DynamoProducts.Load(Tools.Presistence.ActiveVersion);
                return Result.Succeeded;
            }
            catch (Exception) { /* */ }
            return Result.Failed;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
