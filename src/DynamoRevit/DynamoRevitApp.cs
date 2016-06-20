using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Dynamo.Applications.Properties;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using MessageBox = System.Windows.Forms.MessageBox;
using Dynamo.Models;
using RevitServices.EventHandler;
using System.Collections;

namespace Dynamo.Applications
{
    

    [Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic),
     Regeneration(RegenerationOption.Manual)]
    public class DynamoRevitApp : IExternalApplication
    {
        private static readonly string assemblyName = Assembly.GetExecutingAssembly().Location;
        private static ResourceManager res;
        public static ControlledApplication ControlledApplication;
        public static UIControlledApplication UIControlledApplication;
        public static List<IUpdater> Updaters = new List<IUpdater>();
        public static string DynamoCorePath
        {
            get
            {
                if (string.IsNullOrEmpty(dynamopath))
                {
                    dynamopath = GetDynamoCorePath();
                }
                return dynamopath;
            }
        }

        /// <summary>
        /// Finds the Dynamo Core path by looking into registery or potentially a config file.
        /// </summary>
        /// <returns>The root folder path of Dynamo Core.</returns>
        private static string GetDynamoCorePath()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var dynamoRevitRootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(assemblyName));
            var dynamoRoot = GetDynamoRoot(dynamoRevitRootDirectory);
            
            var assembly = Assembly.LoadFrom(Path.Combine(dynamoRevitRootDirectory, "DynamoInstallDetective.dll"));
            var type = assembly.GetType("DynamoInstallDetective.Utilities");

            var installationsMethod = type.GetMethod(
                "FindDynamoInstallations",
                BindingFlags.Public | BindingFlags.Static);

            if (installationsMethod == null)
            {
                throw new MissingMethodException("Method 'DynamoInstallDetective.Utilities.FindDynamoInstallations' not found");
            }

            var methodParams = new object[] { dynamoRoot };

            var installs = installationsMethod.Invoke(null, methodParams) as IEnumerable;
            if (null == installs)
                return string.Empty;

            return installs.Cast<KeyValuePair<string, Tuple<int, int, int, int>>>()
                .Where(p => p.Value.Item1 == version.Major && p.Value.Item2 == version.Minor)
                .Select(p=>p.Key)
                .LastOrDefault();
        }

        /// <summary>
        /// Gets Dynamo Root folder from the given DynamoRevit root.
        /// </summary>
        /// <param name="dynamoRevitRoot">The root folder of DynamoRevit binaries</param>
        /// <returns>The root folder path of Dynamo Core</returns>
        private static string GetDynamoRoot(string dynamoRevitRoot)
        {
            //TODO: use config file to setup Dynamo Path for debug builds.

            //When there is no config file, just replace DynamoRevit by Dynamo 
            //from the 'dynamoRevitRoot' folder.
            var parent = new DirectoryInfo(dynamoRevitRoot);
            var path =  string.Empty;
            while(null != parent && parent.Name != @"DynamoRevit")
            {
                path = Path.Combine(parent.Name, path);
                parent = Directory.GetParent(parent.FullName);
            }
            
            return parent != null ? Path.Combine(Path.GetDirectoryName(parent.FullName), @"Dynamo", path) : dynamoRevitRoot;
        }

        private static string dynamopath;
        private static readonly Queue<Action> idleActionQueue = new Queue<Action>(10);
        private static EventHandlerProxy proxy;
        private AddInCommandBinding dynamoCommand;

        public Result OnStartup(UIControlledApplication application)
        {
            // Revit2015+ has disabled hardware acceleration for WPF to
            // avoid issues with rendering certain elements in the Revit UI. 
            // Here we get it back, by setting the ProcessRenderMode to Default,
            // signifying that we want to use hardware rendering if it's 
            // available.

            RenderOptions.ProcessRenderMode = RenderMode.Default;

            try
            {
                if (false == TryResolveDynamoCore())
                    return Result.Failed;

                UIControlledApplication = application;
                ControlledApplication = application.ControlledApplication;

                SubscribeAssemblyEvents();
                SubscribeApplicationEvents();

                TransactionManager.SetupManager(new AutomaticTransactionStrategy());
                ElementBinder.IsEnabled = true;

                var dynamoCmdId = RevitCommandId.LookupCommandId("ID_VISUAL_PROGRAMMING_DYNAMO");
                dynamoCommand = application.CreateAddInCommandBinding(dynamoCmdId);
                dynamoCommand.CanExecute += canExecute;
                dynamoCommand.BeforeExecuted += beforeExecuted;
                dynamoCommand.Executed += executed;
                DynamoButtonEnabled = true; //initialize
            
                RegisterAdditionalUpdaters(application);

                RevitServicesUpdater.Initialize(DynamoRevitApp.Updaters);
                SubscribeDocumentChangedEvent();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return Result.Failed;
            }
        }

        /// <summary>
        /// Executes Dynamo command to show the Dynamo UI
        /// </summary>
        /// <param name="journalData">Journal data passed for the command</param>
        /// <param name="application">Active session of Revit UI application</param>
        /// <returns></returns>
        public Result ExecuteDynamoCommand(IDictionary<string,string> journalData, UIApplication application)
        {
            var data = new DynamoRevitCommandData()
            {
                JournalData = journalData,
                Application = application
            };
            var cmd = new DynamoRevit();
            return cmd.ExecuteCommand(data);
        }

        void executed(object sender, ExecutedEventArgs e)
        {
            ExecuteDynamoCommand(e.GetJournalData(), new UIApplication(e.ActiveDocument.Application));
        }

        void beforeExecuted(object sender, BeforeExecutedEventArgs e)
        {
            e.UsingCommandData = true;
        }

        void canExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = DynamoButtonEnabled && e.ActiveDocument != null;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            if(dynamoCommand != null)
            {
                dynamoCommand.BeforeExecuted -= beforeExecuted;
                dynamoCommand.CanExecute -= canExecute;
                dynamoCommand.Executed -= executed;
                dynamoCommand = null;
            }

            UnsubscribeAssemblyEvents();
            UnsubscribeApplicationEvents();
            UnsubscribeDocumentChangedEvent();
            RevitServicesUpdater.DisposeInstance();

            return Result.Succeeded;
        }

        private static void OnApplicationIdle(object sender, IdlingEventArgs e)
        {
            if (!idleActionQueue.Any())
                return;

            Action pendingAction = null;
            lock (idleActionQueue)
            {
                pendingAction = idleActionQueue.Dequeue();
            }

            if (pendingAction != null)
                pendingAction();
        }


        /// <summary>
        /// Add an action to run when the application is in the idle state
        /// </summary>
        /// <param name="a"></param>
        public static void AddIdleAction(Action a)
        {
            // If we are running in test mode, invoke 
            // the action immediately.
            if (DynamoModel.IsTestMode)
            {
                a.Invoke();
            }
            else
            {
                lock (idleActionQueue)
                {
                    idleActionQueue.Enqueue(a);
                }
            }
        }

        public static EventHandlerProxy EventHandlerProxy
        {
            get { return proxy; }
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

        private void SubscribeApplicationEvents()
        {
            UIControlledApplication.Idling += OnApplicationIdle;

            proxy = new EventHandlerProxy();

            UIControlledApplication.ViewActivated += proxy.OnApplicationViewActivated;
            UIControlledApplication.ViewActivating += proxy.OnApplicationViewActivating;

            ControlledApplication.DocumentClosing += proxy.OnApplicationDocumentClosing;
            ControlledApplication.DocumentClosed += proxy.OnApplicationDocumentClosed;
            ControlledApplication.DocumentOpened += proxy.OnApplicationDocumentOpened;
        }

        private void UnsubscribeApplicationEvents()
        {
            UIControlledApplication.Idling -= OnApplicationIdle;

            UIControlledApplication.ViewActivated -= proxy.OnApplicationViewActivated;
            UIControlledApplication.ViewActivating -= proxy.OnApplicationViewActivating;

            ControlledApplication.DocumentClosing -= proxy.OnApplicationDocumentClosing;
            ControlledApplication.DocumentClosed -= proxy.OnApplicationDocumentClosed;
            ControlledApplication.DocumentOpened -= proxy.OnApplicationDocumentOpened;

            proxy = null;
        }

        private void SubscribeAssemblyEvents()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

      
        private void UnsubscribeAssemblyEvents()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
            //AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;
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
            var assemblyPath = string.Empty;
            var assemblyName = new AssemblyName(args.Name).Name + ".dll";

            try
            {
                assemblyPath = Path.Combine(DynamoRevitApp.DynamoCorePath, assemblyName);
                if(File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }

                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                // Try "Dynamo 0.x\Revit_20xx" folder first...
                assemblyPath = Path.Combine(assemblyDirectory, assemblyName);
                if (!File.Exists(assemblyPath))
                {
                    // If assembly cannot be found, try in "Dynamo 0.x" folder.
                    var parentDirectory = Directory.GetParent(assemblyDirectory);
                    assemblyPath = Path.Combine(parentDirectory.FullName, assemblyName);
                }

                return (File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("The location of the assembly, {0} could not be resolved for loading.", assemblyPath), ex);
            }
        }
        
        private void SubscribeDocumentChangedEvent()
        {
            ControlledApplication.DocumentChanged += RevitServicesUpdater.Instance.ApplicationDocumentChanged;
        }

        private void UnsubscribeDocumentChangedEvent()
        {
            ControlledApplication.DocumentChanged -= RevitServicesUpdater.Instance.ApplicationDocumentChanged;
        }

        public static bool DynamoButtonEnabled
        {
            get;
            set;
        }

        private bool TryResolveDynamoCore()
        {
            if (string.IsNullOrEmpty(DynamoCorePath))
            {
                var fvi = FileVersionInfo.GetVersionInfo(assemblyName);

                if (MessageBoxResult.OK ==
                    System.Windows.MessageBox.Show(
                        string.Format(Resources.DynamoCoreNotFoundDialogMessage,
                            fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart),
                        Resources.DynamoCoreNotFoundDialogTitle,
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Error))
                {
                    System.Diagnostics.Process.Start("http://dynamobim.org/download/");
                }
                return false;
            }
            return true;
        }
    }
}