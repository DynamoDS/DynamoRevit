using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Dynamo.Applications.Properties;
using Dynamo.Utilities;

using DynamoUtilities;

using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;

using MessageBox = System.Windows.Forms.MessageBox;

namespace Dynamo.Applications
{
    

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic),
     Regeneration(RegenerationOption.Manual)]
    public class DynamoRevitApp : IExternalApplication
    {
        private static readonly string assemblyName = Assembly.GetExecutingAssembly().Location;
        private static ResourceManager res;
        public static ControlledApplication ControlledApplication;
        public static List<IUpdater> Updaters = new List<IUpdater>();
        internal static PushButton DynamoButton;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                SubscribeAssemblyResolvingEvent();

                ControlledApplication = application.ControlledApplication;

                TransactionManager.SetupManager(new AutomaticTransactionStrategy());
                ElementBinder.IsEnabled = true;

                res = Resources.ResourceManager;

                // Create new ribbon panel
                RibbonPanel ribbonPanel =
                    application.CreateRibbonPanel(res.GetString("App_Description"));

                DynamoButton =
                    (PushButton)
                        ribbonPanel.AddItem(
                            new PushButtonData(
                                "Dynamo 0.7",
                                res.GetString("App_Name"),
                                assemblyName,
                                "Dynamo.Applications.DynamoRevit"));

                Bitmap dynamoIcon = Resources.logo_square_32x32;

                BitmapSource bitmapSource =
                    Imaging.CreateBitmapSourceFromHBitmap(
                        dynamoIcon.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                DynamoButton.LargeImage = bitmapSource;
                DynamoButton.Image = bitmapSource;

                RegisterAdditionalUpdaters(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            UnsubscribeAssemblyResolvingEvent();

            return Result.Succeeded;
        }

        // should be handled by the ModelUpdater class. But there are some
        // cases where the document modifications handled there do no catch
        // certain document interactions. Those should be registered here.
        /// <summary>
        ///     Register some document updaters. Generally, document updaters
        /// </summary>
        /// <param name="application"></param>
        private static void RegisterAdditionalUpdaters(UIControlledApplication application)
        {
            var sunUpdater = new SunPathUpdater(application.ActiveAddInId);

            if (!UpdaterRegistry.IsUpdaterRegistered(sunUpdater.GetUpdaterId()))
                UpdaterRegistry.RegisterUpdater(sunUpdater);

            var sunFilter = new ElementClassFilter(typeof(SunAndShadowSettings));
            var filterList = new List<ElementFilter> { sunFilter };
            ElementFilter filter = new LogicalOrFilter(filterList);
            UpdaterRegistry.AddTrigger(
                sunUpdater.GetUpdaterId(),
                filter,
                Element.GetChangeTypeAny());
            Updaters.Add(sunUpdater);
        }

        private void SubscribeAssemblyResolvingEvent()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        private void UnsubscribeAssemblyResolvingEvent()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
        }

        /// <summary>
        /// Handler to the ApplicationDomain's AssemblyResolve event.
        /// If an assembly's location cannot be resolved, an exception is
        /// thrown. Failure to resolve an assembly will leave Dynamo in 
        /// a bad state, so we should throw an exception here which gets caught 
        /// by our unhandled exception handler and presents the crash dialogue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string assemblyPath = string.Empty;
            try
            {
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                var parentDirectory = Directory.GetParent(assemblyDirectory);

                // First check the core path
                assemblyPath = Path.Combine(parentDirectory.FullName, new AssemblyName(args.Name).Name + ".dll");
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("There location of the assembly, {0} could not be resolved for {1loading.", assemblyPath), ex);
            }
        }
    }
}