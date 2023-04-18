using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Applications.ViewModel;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Graph.Workspaces;
using Dynamo.Logging;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.Updates;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using DynamoInstallDetective;
using Greg.AuthProviders;
using Microsoft.Win32;
using Newtonsoft.Json;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using DynUpdateManager = Dynamo.Updates.UpdateManager;
using MessageBox = System.Windows.Forms.MessageBox;
using Resources = Dynamo.Applications.Properties.Resources;

namespace RevitServices.Threading
{
    // SCHEDULER: This class will be removed once DynamoScheduler work is 
    // tested working. When that happens, all the callers will be redirected
    // to use RevitDynamoModel.DynamoScheduler directly for task scheduling.
    // 
    public static class IdlePromise
    {
        [ThreadStatic]
        private static bool idle;
        public static bool InIdleThread
        {
            get { return idle; }
            set { idle = value; }
        }

        /// <summary>
        /// Call this method to schedule a DelegateBasedAsyncTask for execution.
        /// </summary>
        /// <param name="p">The delegate to execute on the idle thread.</param>
        /// <param name="completionHandler">Event handler that will be invoked 
        /// when the scheduled DelegateBasedAsyncTask is completed. This parameter
        /// is optional.</param>
        /// 
        internal static void ExecuteOnIdleAsync(Action p,
            AsyncTaskCompletedHandler completionHandler = null)
        {
            var scheduler = DynamoRevit.RevitDynamoModel.Scheduler;
            var task = new DelegateBasedAsyncTask(scheduler, p);
            if (completionHandler != null)
                task.Completed += completionHandler;

            scheduler.ScheduleForExecution(task);
        }
    }
}
public class DynamoExternalEventHandler : IExternalEventHandler
{
    private Action callback;
    public void Execute(UIApplication app)
    {
        callback();
    }

    public string GetName()
    {
        return nameof(DynamoExternalEventHandler);
    }

    internal DynamoExternalEventHandler(Action callback)
    {
        this.callback = callback;
    }
}

namespace Dynamo.Applications
{
    /// <summary>
    /// Defines DynamoRevitCommandData class modeled after ExternalCommandData
    /// </summary>
    public class DynamoRevitCommandData
    {
        public DynamoRevitCommandData(ExternalCommandData externalCommandData)
        {
            Application = externalCommandData.Application;
            JournalData = externalCommandData.JournalData;
        }

        public DynamoRevitCommandData()
        {
        }

        // Summary:
        //     Retrieves an object that represents the current Application for external
        //     command.
        public UIApplication Application { get; set; }

        //
        // Summary:
        //     A data map that can be used to read and write data to the Autodesk Revit
        //     journal file.
        //
        // Remarks:
        //     The data map is a string to string map that can be used to store data in
        //     the Revit journal file at the end of execution of the external command. If
        //     the command is then executed from the journal file during playback this data
        //     is then passed to the external command in this Data property so the external
        //     command can execute with this passed data in a UI-less mode, hence providing
        //     non interactive journal playback for automated testing purposes. For more
        //     information on Revit's journaling features contact the Autodesk Developer
        //     Network.
        public IDictionary<string, string> JournalData { get; set; }
    }

    /// <summary>
    /// Defines startup parameters for DynamoRevitModel
    /// </summary>
    public class JournalKeys
    {
        /// <summary>
        /// The journal file can use this key to specify whether
        /// the Dynamo UI should be visible at run time.
        /// </summary>
        public const string ShowUiKey = "dynShowUI";

        /// <summary>
        /// If the journal file specifies automation mode, 
        /// Dynamo will run on the main thread without the idle loop.
        /// </summary>
        public const string AutomationModeKey = "dynAutomation";

        /// <summary>
        /// The journal file can specify a Dynamo workspace to be opened 
        /// (and executed if we are in automation mode) at run time.
        /// </summary>
        public const string DynPathKey = "dynPath";

        /// <summary>
        /// The journal file can specify if the Dynamo workspace opened 
        /// from DynPathKey will be executed or not. 
        /// If we are in automation mode the workspace will be executed regardless of this key.
        /// </summary>
        public const string DynPathExecuteKey = "dynPathExecute";

        /// <summary>
        /// The journal file can specify if the Dynamo workspace opened
        /// from DynPathKey will be forced in manual mode.
        /// </summary>
        public const string ForceManualRunKey = "dynForceManualRun";

        /// <summary>
        /// The journal file can specify if the existing UIless RevitDynamoModel
        /// needs to be shutdown before performing any action.
        /// </summary>
        public const string ModelShutDownKey = "dynModelShutDown";

        /// <summary>
        /// The journal file can specify if a check should be performed to see if the
        /// current workspaceModel already points to the Dynamo file we want to 
        /// run (or perform other tasks). If that's the case, we want to use the
        /// current workspaceModel.
        /// </summary>
        public const string DynPathCheckExisting = "dynPathCheckExisting";

        /// <summary>
        /// The journal file can specify the values of Dynamo nodes.
        /// </summary>
        public const string ModelNodesInfo = "dynModelNodesInfo";
    }

    /// <summary>
    /// Defines parameters for Dynamo nodes needed in order to update nodes
    /// values through UpdateModelValueCommand.
    /// </summary>
    public class JournalNodeKeys
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Value = "Value";
    }


    [Transaction(TransactionMode.Manual),
     Regeneration(RegenerationOption.Manual)]
    public class DynamoRevit : IExternalCommand
    {
        public static ExternalEvent SplashScreenExternalEvent { get; set; }
        public static ExternalEvent DynamoAppExternalEvent { get; set; }
        /// <summary>
        /// Based on the RevitDynamoModelState a dependent component can take certain 
        /// decisions regarding its UI and functionality.
        /// In order to be able to run a specified graph , revitDynamoModel needs to be 
        /// at least in StartedUIless state. 
        /// </summary>
        [Obsolete("This enum will be removed, please use the State property on DynamoModel along with DynamoModelState")]
        public enum RevitDynamoModelState { NotStarted, StartedUIless, StartedUI };

        private static List<Action> idleActions;
        private static DynamoRevitCommandData extCommandData;
        private static bool handledCrash;
        private static List<Exception> preLoadExceptions;
        private static Action shutdownHandler;
        private Stopwatch startupTimer;
        private Dynamo.UI.Views.SplashScreen splashScreen;

        /// <summary>
        /// The modelState tels us if the RevitDynamoModel was started and if has the
        /// the Dynamo UI attached to it or not 
        /// </summary>
        [Obsolete("This property will be removed, please use the State property on DynamoModel.")]
        public static RevitDynamoModelState ModelState
        {
            get;
            private set;
        }

        /// <summary>
        /// Get or Set the current RevitDynamoModel available in Revit context
        /// </summary>
        public static RevitDynamoModel RevitDynamoModel { get; set; }

        /// <summary>
        /// Get or Set the current DynamoViewModel available in Revit context
        /// </summary>
        public static DynamoViewModel RevitDynamoViewModel { get; private set; }

        static DynamoRevit()
        {
            idleActions = new List<Action>();
            extCommandData = null;
            RevitDynamoViewModel = null;
            RevitDynamoModel = null;
            handledCrash = false;
            ModelState = RevitDynamoModelState.NotStarted;
            preLoadExceptions = new List<Exception>();
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return ExecuteCommand(new DynamoRevitCommandData(commandData));
        }

        private void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            //push any exceptions generated before DynamoLoad to this list
            preLoadExceptions.AddRange(StartupUtils.CheckAssemblyForVersionMismatches(args.LoadedAssembly));
        }

        public Result ExecuteCommand(DynamoRevitCommandData commandData)
        {
            startupTimer = Stopwatch.StartNew();

            if (ModelState == RevitDynamoModelState.StartedUIless)
            {
                if (CheckJournalForKey(commandData, JournalKeys.ShowUiKey, true) ||
                    CheckJournalForKey(commandData, JournalKeys.ModelShutDownKey))
                {
                    //When we move from UIless to UI we prefer to start with a fresh revitDynamoModel
                    //in order to benefit from a startup sequence similar to Dynamo Revit UI launch.       
                    //Also there might be other situations which demand a new revitDynamoModel.            
                    //In order to be able to address them we process ModelShutDownKey.
                    //An example of this situation is when you have a revitDynamoModel already started and you switch 
                    //the document in Revit. Since revitDynamoModel is well connected to the previous document we need to
                    //shut it down and start a new one in order to able to run a graph in the new document.
                    RevitDynamoModel.ShutDown(false);
                    ModelState = RevitDynamoModelState.NotStarted;
                }
                else
                {
                    TryOpenAndExecuteWorkspaceInCommandData(commandData);
                    return Result.Succeeded;
                }
            }

            if (ModelState == RevitDynamoModelState.StartedUI)
            {
                TryOpenAndExecuteWorkspaceInCommandData(commandData);
                return Result.Succeeded;
            }

            HandleDebug(commandData);

            InitializeCore(commandData);
            //subscribe to the assembly load
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoad;

            // Show splash screen when dynamo is started, otherwise run UIless mode when needed.
            if (CheckJournalForKey(commandData, JournalKeys.ShowUiKey, true))
            {
                var ssEventHandler = new DynamoExternalEventHandler(new Action(() =>
                {
                    LoadDynamoView();
                }));
                //Register DynamoExternalEventHandler so Revit will call our callback
                //we requested with API access.
                SplashScreenExternalEvent = ExternalEvent.Create(ssEventHandler);

                try
                {
                    extCommandData = commandData;
                    UpdateSystemPathForProcess();

                    splashScreen = new Dynamo.UI.Views.SplashScreen();
                    //when the splashscreen is ready, raise a request to revit to call
                    //our callback within an api context.
                    splashScreen.DynamicSplashScreenReady += () =>
                    {
                        SplashScreenExternalEvent.Raise();
                    };
                    splashScreen.Closed += OnSplashScreenClosed;

                    //Set the owner for splashscreen window.
                    IntPtr mwHandle = commandData.Application.MainWindowHandle;
                    new WindowInteropHelper(splashScreen).Owner = mwHandle;

                    // show the splashscreen.
                    splashScreen.Show();

                    // Disable the Dynamo button in Revit to avoid launching multiple instances.
                    DynamoRevitApp.DynamoButtonEnabled = false;
                }
                catch (Exception ex)
                {
                    // notify instrumentation
                    Analytics.TrackException(ex, true);
                    MessageBox.Show(ex.ToString());

                    DynamoRevitApp.DynamoButtonEnabled = true;

                    //If for some reason Dynamo has crashed while startup make sure the Dynamo Model is properly shutdown.
                    if (RevitDynamoModel != null)
                    {
                        RevitDynamoModel.ShutDown(false);
                        RevitDynamoModel = null;
                    }

                    return Result.Failed;
                }

                return Result.Succeeded;
            }
            else // Run UIless mode by just initializing RevitDynamoModel
            {
                try
                {
                    extCommandData = commandData;
                    UpdateSystemPathForProcess();

                    // create core data models
                    RevitDynamoModel = InitializeCoreModel(extCommandData);
                    RevitDynamoModel.UpdateManager.RegisterExternalApplicationProcessId(Process.GetCurrentProcess().Id);
                    RevitDynamoModel.Logger.Log("SYSTEM", string.Format("Environment Path:{0}", Environment.GetEnvironmentVariable("PATH")));

                    // handle initialization steps after RevitDynamoModel is created in UIless mode
                    RevitDynamoModel.HandlePostInitialization();
                    ModelState = RevitDynamoModelState.StartedUIless;

                    //unsubscribe to the assembly load
                    AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;
                }
                catch (Exception ex)
                {
                    // notify instrumentation
                    Analytics.TrackException(ex, true);
                    MessageBox.Show(ex.ToString());

                    DynamoRevitApp.DynamoButtonEnabled = true;

                    //If for some reason Dynamo has crashed while startup make sure the Dynamo Model is properly shutdown.
                    if (RevitDynamoModel != null)
                    {
                        RevitDynamoModel.ShutDown(false);
                        RevitDynamoModel = null;
                    }

                    return Result.Failed;
                }

                return Result.Succeeded;
            }

        }

        private void LoadDynamoView()
        {
            // create core data models
            RevitDynamoModel = InitializeCoreModel(extCommandData);
            RevitDynamoModel.UpdateManager.RegisterExternalApplicationProcessId(Process.GetCurrentProcess().Id);
            RevitDynamoModel.Logger.Log("SYSTEM", string.Format("Environment Path:{0}", Environment.GetEnvironmentVariable("PATH")));

            // handle initialization steps after RevitDynamoModel is created.
            RevitDynamoModel.HandlePostInitialization();
            ModelState = RevitDynamoModelState.StartedUIless;

            // show the window
            if (CheckJournalForKey(extCommandData, JournalKeys.ShowUiKey, true))
            {
                RevitDynamoViewModel = InitializeCoreViewModel(RevitDynamoModel);

                // Let the host (e.g. Revit) control the rendering mode
                var save = RenderOptions.ProcessRenderMode;

                splashScreen.DynamoView = InitializeCoreView(extCommandData);

                RenderOptions.ProcessRenderMode = save;
                RevitDynamoModel.Logger.Log(Dynamo.Applications.Properties.Resources.WPFRenderMode + RenderOptions.ProcessRenderMode.ToString());

                ModelState = RevitDynamoModelState.StartedUI;
                // Disable the Dynamo button to prevent a re-run
                DynamoRevitApp.DynamoButtonEnabled = false;
            }

            //foreach preloaded exception send a notification to the Dynamo Logger
            //these are messages we want the user to notice.
            preLoadExceptions.ForEach(x => RevitDynamoModel.Logger.LogNotification
            (RevitDynamoModel.GetType().ToString(),
            x.GetType().ToString(),
            DynamoApplications.Properties.Resources.MismatchedAssemblyVersionShortMessage,
            x.Message));

            TryOpenAndExecuteWorkspaceInCommandData(extCommandData);

            //unsubscribe to the assembly load
            AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;
            Analytics.TrackStartupTime("DynamoRevit", startupTimer.Elapsed, ModelState.ToString());

            splashScreen.OnRequestStaticSplashScreen();
            splashScreen.DynamicSplashScreenReady -= LoadDynamoView;
        }

        /// <summary>
        /// Add the main exec path to the system PATH
        /// This is required to pickup certain dlls.
        /// </summary>
        private static void UpdateSystemPathForProcess()
        {
            var path =
                    Environment.GetEnvironmentVariable(
                        "Path",
                        EnvironmentVariableTarget.Process) + ";" + DynamoRevitApp.DynamoCorePath;
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);
        }

        #region Initialization

        /// <summary>
        /// DynamoShapeManager.dll is a companion assembly of Dynamo core components,
        /// we do not want a static reference to it (since the Revit add-on can be 
        /// installed anywhere that's outside of Dynamo), we do not want a duplicated 
        /// reference to it. Here we use reflection to obtain GetGeometryFactoryPath
        /// method, and call it to get the geometry factory assembly path.
        /// </summary>
        /// <param name="corePath">The path where DynamoShapeManager.dll can be 
        /// located.</param>
        /// <returns>Returns the full path to geometry factory assembly.</returns>
        /// 
        [Obsolete("Please use the overload which specifies the version of the geometry library to load")]
        public static string GetGeometryFactoryPath(string corePath)
        {
            var dynamoAsmPath = Path.Combine(corePath, "DynamoShapeManager.dll");
            var assembly = Assembly.LoadFrom(dynamoAsmPath);
            if (assembly == null)
                throw new FileNotFoundException("File not found", dynamoAsmPath);

            var utilities = assembly.GetType("DynamoShapeManager.Utilities");
            var getGeometryFactoryPath = utilities.GetMethod("GetGeometryFactoryPath2");

            //if old method is called default to 224.4.0
            return (getGeometryFactoryPath.Invoke(null,
                new object[] { corePath, new Version(224, 4, 0) }) as string);
        }


        public static string GetGeometryFactoryPath(string corePath, Version version)
        {
            var dynamoAsmPath = Path.Combine(corePath, "DynamoShapeManager.dll");
            var assembly = Assembly.LoadFrom(dynamoAsmPath);
            if (assembly == null)
                throw new FileNotFoundException("File not found", dynamoAsmPath);

            var utilities = assembly.GetType("DynamoShapeManager.Utilities");
            var getGeometryFactoryPath = utilities.GetMethod("GetGeometryFactoryPath2");

            return (getGeometryFactoryPath.Invoke(null,
                new object[] { corePath, version }) as string);
        }



        private static void PreloadDynamoCoreDlls()
        {
            // Assume Revit Install folder as look for root. Assembly name is compromised.
            var assemblyList = new[]
            {
                "SDA\\bin\\ICSharpCode.AvalonEdit.dll"
            };

            foreach (var assembly in assemblyList)
            {
                var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly);
                if (File.Exists(assemblyPath))
                    Assembly.LoadFrom(assemblyPath);
            }
        }

        private static RevitDynamoModel InitializeCoreModel(DynamoRevitCommandData commandData)
        {
            // Temporary fix to pre-load DLLs that were also referenced in Revit folder. 
            // To do: Need to align with Revit when provided a chance.
            PreloadDynamoCoreDlls();
            var corePath = DynamoRevitApp.DynamoCorePath;
            var dynamoRevitExePath = Assembly.GetExecutingAssembly().Location;
            var dynamoRevitRoot = Path.GetDirectoryName(dynamoRevitExePath);// ...\Revit_xxxx\ folder

            // get Dynamo Revit Version
            var dynRevitVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var umConfig = UpdateManagerConfiguration.GetSettings(new DynamoRevitLookUp());
            var revitUpdateManager = new DynUpdateManager(umConfig);
            revitUpdateManager.HostVersion = dynRevitVersion; // update RevitUpdateManager with the current DynamoRevit Version
            revitUpdateManager.HostName = "Dynamo Revit";
            if (revitUpdateManager.Configuration is IDisableUpdateConfig)
                (revitUpdateManager.Configuration as IDisableUpdateConfig).DisableUpdates = true;

            Debug.Assert(umConfig.DynamoLookUp != null);

            var userDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                "Dynamo", "Dynamo Revit");
            var commonDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData),
                "Autodesk", "RVT " + commandData.Application.Application.VersionNumber, "Dynamo");

            bool isAutomationMode = CheckJournalForKey(extCommandData, JournalKeys.AutomationModeKey);

            // when Dynamo runs on top of Revit we must load the same version of ASM as revit
            // so tell Dynamo core we've loaded that version.
            var loadedLibGVersion = PreloadAsmFromRevit();


            return RevitDynamoModel.Start(
            new RevitDynamoModel.RevitStartConfiguration()
            {
                DynamoCorePath = corePath,
                DynamoHostPath = dynamoRevitRoot,
                GeometryFactoryPath = GetGeometryFactoryPath(corePath, loadedLibGVersion),
                PathResolver = new RevitPathResolver(userDataFolder, commonDataFolder),
                Context = GetRevitContext(commandData),
                SchedulerThread = new RevitSchedulerThread(commandData.Application),
                StartInTestMode = isAutomationMode,
                AuthProvider = new RevitOAuth2Provider(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher)),
                ExternalCommandData = commandData,
                UpdateManager = revitUpdateManager,
                ProcessMode = isAutomationMode ? TaskProcessMode.Synchronous : TaskProcessMode.Asynchronous
            });
        }

        internal static Version PreloadAsmFromRevit()
        {
            var asmLocation = AppDomain.CurrentDomain.BaseDirectory;
            Version libGVersion = findRevitASMVersion(asmLocation);
            var dynCorePath = DynamoRevitApp.DynamoCorePath;
            // Get the corresponding libG preloader location for the target ASM loading version.
            // If there is exact match preloader version to the target ASM version, use it, 
            // otherwise use the closest below.
            var preloaderLocation = DynamoShapeManager.Utilities.GetLibGPreloaderLocation(libGVersion, dynCorePath);

            // [Tech Debt] (Will refactor the code later)
            // The LibG version maybe different in Dynamo and Revit, using the one which is in Dynamo.
            Version preLoadLibGVersion = PreloadLibGVersion(preloaderLocation);
            DynamoShapeManager.Utilities.PreloadAsmFromPath(preloaderLocation, asmLocation);
            return preLoadLibGVersion;
        }

        // [Tech Debt] (Will refactor the code later)
        /// <summary>
        /// Return the preload version of LibG.
        /// </summary>
        /// <param name="preloaderLocation"></param>
        /// <returns></returns>
        internal static Version PreloadLibGVersion(string preloaderLocation)
        {
            preloaderLocation = new DirectoryInfo(preloaderLocation).Name;
            var regExp = new Regex(@"^libg_(\d\d\d)_(\d)_(\d)$", RegexOptions.IgnoreCase);

            var match = regExp.Match(preloaderLocation);
            if (match.Groups.Count == 4)
            {
                return new Version(
                    Convert.ToInt32(match.Groups[1].Value),
                    Convert.ToInt32(match.Groups[2].Value),
                    Convert.ToInt32(match.Groups[3].Value));
            }

            return new Version();
        }

        /// <summary>
        /// Returns the version of ASM which is installed with Revit at the requested path.
        /// This version number can be used to load the appropriate libG version.
        /// </summary>
        /// <param name="asmLocation">path where asm dlls are located, this is usually the product(Revit) install path</param>
        /// <returns></returns>
        internal static Version findRevitASMVersion(string asmLocation)
        {
            var lookup = new InstalledProductLookUp("Revit", "ASMAHL*.dll");
            var product = lookup.GetProductFromInstallPath(asmLocation);
            var libGversion = new Version(product.VersionInfo.Item1, product.VersionInfo.Item2, product.VersionInfo.Item3);
            return libGversion;
        }

        private static DynamoViewModel InitializeCoreViewModel(RevitDynamoModel revitDynamoModel)
        {
            var viewModel = DynamoRevitViewModel.Start(
                new DynamoViewModel.StartConfiguration()
                {
                    DynamoModel = revitDynamoModel,
                    WatchHandler =
                        new RevitWatchHandler(revitDynamoModel.PreferenceSettings)
                });
            return viewModel;
        }

        private static DynamoView InitializeCoreView(DynamoRevitCommandData commandData)
        {
            IntPtr mwHandle = commandData.Application.MainWindowHandle;
            var dynamoView = new DynamoView(RevitDynamoViewModel);

            new WindowInteropHelper(dynamoView).Owner = mwHandle;

            handledCrash = false;

            //Register DynamoExternalEventHandler so Revit will call our callback
            //we requested with API access.

            var appEventHandler = new DynamoExternalEventHandler(new Action(() =>
            {
                UpdateLibraryLayoutSpec();
            }));

            DynamoAppExternalEvent = ExternalEvent.Create(appEventHandler);

            dynamoView.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            dynamoView.Closed += OnDynamoViewClosed;
            dynamoView.Loaded += (o, e) => DynamoAppExternalEvent.Raise();

            return dynamoView;
        }

        private static void AddSyncWithRevitControls(DynamoView dynamoView)
        {
            // Get the RunSettingsControl field from the DynamoWindow
            var runsettingField = dynamoView.GetType().GetField("RunSettingsControl", BindingFlags.Instance | BindingFlags.NonPublic);

            // Get the value from the field, this should be of type UserControl
            var runsettingsValue = runsettingField.GetValue(dynamoView) as UserControl;

            // Get the grid from the RunSettingsControl
            var runsettingsGrid = runsettingsValue.Content as System.Windows.Controls.Grid;

            // Get StackPanel from the RunSettingsControls main grid, this is where all of its UIElements are defined
            var runsettingStackPanel = runsettingsGrid.Children.OfType<StackPanel>().FirstOrDefault();

            var srcDic = Dynamo.UI.SharedDictionaryManager.DynamoModernDictionary;

            var toggleItem = new System.Windows.Controls.Primitives.ToggleButton
            {
                Width = 40,
                Height = 20,
                IsChecked = true,
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                ToolTip = Resources.SyncWithRevitToolTip
            };

            toggleItem.SetValue(System.Windows.Controls.Primitives.ToggleButton.StyleProperty, srcDic["EllipseToggleButton1"]);

            toggleItem.Click += OnReadOnlyModeToggleChecked;

            var toggleLabel = new Label
            {
                Content = Resources.SyncWithRevit,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new System.Windows.Thickness(0, 0, 0, 0),
                ToolTip = Resources.SyncWithRevitToolTip
            };
            toggleLabel.SetValue(Label.ForegroundProperty, srcDic["PreferencesWindowFontColor"]);

            // Add a new control to the RunSettingsControls StackPanel
            runsettingStackPanel.Children.Add(toggleItem);
            runsettingStackPanel.Children.Add(toggleLabel);
        }

        private static void OnReadOnlyModeToggleChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = sender as System.Windows.Controls.Primitives.ToggleButton;
            if (!toggle.IsChecked.HasValue ||
                toggle.IsChecked.Value != TransactionManager.Instance.DisableTransactions)
                return;
            TransactionManager.Instance.DisableTransactions = !toggle.IsChecked.Value;
        }

        /// <summary>
        /// Updates the Libarary Layout spec to include layout for Revit nodes. 
        /// The Revit layout spec is embeded as resource "LayoutSpecs.json".
        /// </summary>
        private static void UpdateLibraryLayoutSpec()
        {
            //Get the library view customization service to update spec
            var customization = RevitDynamoModel.ExtensionManager.Service<ILibraryViewCustomization>();
            if (customization == null) return;

            if (shutdownHandler == null && extCommandData != null)
            {
                //Make sure to notify customization for application closing, so that 
                //the CEF can be shutdown for clean Revit exit
                shutdownHandler = () => customization.OnAppShutdown();
                extCommandData.Application.ApplicationClosing += (o, _) => shutdownHandler();
            }

            //Register the icon resource
            customization.RegisterResourceStream("/icons/Category.Revit.svg",
                GetResourceStream("Dynamo.Applications.Resources.Category.Revit.svg"));

            //Read the revitspec from the resource stream
            LayoutSpecification revitspec;
            using (Stream stream = GetResourceStream("Dynamo.Applications.Resources.LayoutSpecs.json"))
            {
                revitspec = LayoutSpecification.FromJSONStream(stream);
            }

            //The revitspec should have only one section, add all its child elements to the customization
            var elements = revitspec.sections.First().childElements;

            // Extend it with the layoutSpecs from internal nodes
            var internalNodesLayouts = DynamoRevitInternalNodes.GetLayoutSpecsFiles();
            foreach (var layoutSpecsFile in internalNodesLayouts)
            {
                try
                {
                    LayoutSpecification spec = LayoutSpecification.FromJSONString(File.ReadAllText(layoutSpecsFile));
                    var revitSection = spec.sections.First();
                    var revitCategory = revitSection.childElements.First();

                    var revitCategoryToExtend = elements.First(elem => elem.text == "Revit");
                    revitCategoryToExtend.childElements.AddRange(revitCategory.childElements);
                }
                catch (Exception)
                {
                    Console.WriteLine(string.Format("Exception while trying to load {0}", layoutSpecsFile));
                }
            }

            customization.AddElements(elements); //add all the elements to default section
        }

        /// <summary>
        /// Reads the embeded resource stream by given name
        /// </summary>
        /// <param name="resource">Fully qualified name of the embeded resource.</param>
        /// <returns>The resource Stream if successful else null</returns>
        private static Stream GetResourceStream(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resource);
            return stream;
        }

        private static bool initializedCore;
        private static void InitializeCore(DynamoRevitCommandData commandData)
        {
            if (initializedCore) return;

            // Change the locale that LibG depends on.
            StringBuilder sb = new StringBuilder("LANGUAGE=");
            var revitLocale = System.Globalization.CultureInfo.CurrentUICulture.ToString();
            sb.Append(revitLocale.Replace("-", "_"));
            _putenv(sb.ToString());

            InitializeDocumentManager(commandData);

            initializedCore = true;
        }

        private static void InitializeDocumentManager(DynamoRevitCommandData commandData)
        {
            if (DocumentManager.Instance.CurrentUIApplication == null)
                DocumentManager.Instance.CurrentUIApplication = commandData.Application;
        }

        #endregion

        #region Helpers

        private static void HandleDebug(DynamoRevitCommandData commandData)
        {
            if (commandData.JournalData != null && commandData.JournalData.ContainsKey("debug"))
            {
                if (Boolean.Parse(commandData.JournalData["debug"]))
                    Debugger.Launch();
            }
        }

        private static bool CheckJournalForKey(DynamoRevitCommandData commandData, string key, bool defaultReturn = false)
        {
            var result = defaultReturn;

            if (commandData.JournalData == null)
            {
                return result;
            }

            if (commandData.JournalData.ContainsKey(key))
            {
                bool.TryParse(commandData.JournalData[key], out result);
            }

            return result;
        }

        private static void TryOpenAndExecuteWorkspaceInCommandData(DynamoRevitCommandData commandData)
        {
            if (commandData.JournalData == null)
            {
                return;
            }

            if (commandData.JournalData.ContainsKey(JournalKeys.DynPathKey))
            {
                bool isAutomationMode = CheckJournalForKey(commandData, JournalKeys.AutomationModeKey);
                bool forceManualRun = CheckJournalForKey(commandData, JournalKeys.ForceManualRunKey);

                bool useExistingWorkspace = false;
                if (CheckJournalForKey(commandData, JournalKeys.DynPathCheckExisting))
                {
                    WorkspaceModel currentWorkspace = RevitDynamoModel.CurrentWorkspace;
                    if (currentWorkspace.FileName.Equals(commandData.JournalData[JournalKeys.DynPathKey],
                        StringComparison.OrdinalIgnoreCase))
                    {
                        useExistingWorkspace = true;
                    }
                }

                if (!useExistingWorkspace) //if use existing is false, open the specified workspace
                {
                    if (ModelState == RevitDynamoModelState.StartedUIless)
                    {
                        RevitDynamoModel.OpenFileFromPath(commandData.JournalData[JournalKeys.DynPathKey], forceManualRun);
                    }
                    else
                    {
                        RevitDynamoViewModel.OpenIfSavedCommand.Execute(new Dynamo.Models.DynamoModel.OpenFileCommand(commandData.JournalData[JournalKeys.DynPathKey], forceManualRun));
                        RevitDynamoViewModel.ShowStartPage = false;
                    }
                }

                //If we have information about the nodes and their values we want to push those values after the file is opened.
                if (commandData.JournalData.ContainsKey(JournalKeys.ModelNodesInfo))
                {
                    try
                    {
                        var allNodesInfo = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(commandData.JournalData[JournalKeys.ModelNodesInfo]);
                        if (allNodesInfo != null)
                        {
                            foreach (var nodeInfo in allNodesInfo)
                            {
                                if (nodeInfo.ContainsKey(JournalNodeKeys.Id) &&
                                    nodeInfo.ContainsKey(JournalNodeKeys.Name) &&
                                    nodeInfo.ContainsKey(JournalNodeKeys.Value))
                                {
                                    var modelCommand = new DynamoModel.UpdateModelValueCommand(nodeInfo[JournalNodeKeys.Id],
                                                                                               nodeInfo[JournalNodeKeys.Name],
                                                                                               nodeInfo[JournalNodeKeys.Value]);
                                    modelCommand.Execute(RevitDynamoModel);
                                }
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Exception while trying to update nodes with new values");
                    }
                }

                //If we are in automation mode the model will run anyway (on the main thread 
                //without the idle loop) regardless of the DynPathExecuteKey.
                if (!isAutomationMode && commandData.JournalData.ContainsKey(JournalKeys.DynPathExecuteKey))
                {
                    bool executePath = false;
                    bool.TryParse(commandData.JournalData[JournalKeys.DynPathExecuteKey], out executePath);
                    if (executePath)
                    {
                        HomeWorkspaceModel modelToRun = RevitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                        if (modelToRun != null)
                        {
                            modelToRun.Run();
                            return;
                        }
                    }
                }

            }
        }

        public static string GetRevitContext(DynamoRevitCommandData commandData)
        {
            var r = new Regex(@"\b(Autodesk |Structure |MEP |Architecture )\b");
            string context = r.Replace(commandData.Application.Application.VersionName, "");

            //they changed the application version name conventions for vasari
            //it no longer has a version year so we can't compare it to other versions
            if (context == "Vasari")
                context = "Vasari 2014";

            return context;
        }

        [DllImport("msvcrt.dll")]
        public static extern int _putenv(string env);

        #endregion

        #region Exception

        /// <summary>
        ///     A method to deal with unhandle exceptions.  Executes right before Revit crashes.
        ///     Dynamo is still valid at this time, but further work may cause corruption.  Here,
        ///     we run the ExitCommand, allowing the user to save all of their work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">Info about the exception</param>
        private static void Dispatcher_UnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            // only handle a single crash per Dynamo sesh, this should be reset in the initial command
            if (handledCrash)
                return;

            handledCrash = true;

            string exceptionMessage = args.Exception.Message;

            try
            {
                Dynamo.Logging.Analytics.TrackException(args.Exception, true);

                RevitDynamoModel.Logger.LogError("Dynamo Unhandled Exception");
                RevitDynamoModel.Logger.LogError(exceptionMessage);
            }
            catch { }

            try
            {
                DynamoModel.IsCrashing = true;
                RevitDynamoModel.OnRequestsCrashPrompt(
                    RevitDynamoModel,
                    new CrashPromptArgs(args.Exception.Message + "\n\n" + args.Exception.StackTrace));
                RevitDynamoViewModel.Exit(false); // don't allow cancellation
            }
            catch { }
            finally
            {
                args.Handled = true;
                DynamoRevitApp.DynamoButtonEnabled = true;
            }
        }

        #endregion

        #region Shutdown

        /// <summary>
        ///     Executes after Dynamo closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDynamoViewClosed(object sender, EventArgs e)
        {
            var view = (DynamoView)sender;

            view.Dispatcher.UnhandledException -= Dispatcher_UnhandledException;
            view.Closed -= OnDynamoViewClosed;

            DynamoRevitApp.DynamoButtonEnabled = true;

            //the model is shutdown when DynamoView is closed
            ModelState = RevitDynamoModelState.NotStarted;
        }

        /// <summary>
        ///     Executes after the splash screen closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSplashScreenClosed(object sender, EventArgs e)
        {
            var view = (Window)sender;

            view.Dispatcher.UnhandledException -= Dispatcher_UnhandledException;
            view.Closed -= OnSplashScreenClosed;

            DynamoRevitApp.DynamoButtonEnabled = true;

            //the model is shutdown when splash screen is closed
            ModelState = RevitDynamoModelState.NotStarted;
        }

        #endregion

        /// <summary>
        /// Add an action to run when the application is in the idle state
        /// </summary>
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
                lock (idleActions)
                {
                    idleActions.Add(a);
                }
            }
        }
    }

    internal class DynamoRevitLookUp : DynamoLookUp
    {
        public override IEnumerable<string> GetDynamoInstallLocations()
        {
            const string regKey64 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\";
            //Open HKLM for 64bit registry
            var regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            //Open Windows/CurrentVersion/Uninstall registry key
            regKey = regKey.OpenSubKey(regKey64);

            //Get "InstallLocation" value as string for all the subkey that starts with "Dynamo"
            return regKey.GetSubKeyNames().Where(s => s.StartsWith("Dynamo")).Select(
                (s) => regKey.OpenSubKey(s).GetValue("InstallLocation") as string);
        }

        public override IEnumerable<string> GetDynamoUserDataLocations()
        {
            var appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var paths = new List<string>();
            //Pre 1.0 Dynamo Studio user data was stored at %appdata%\Dynamo\
            var dynamoFolder = Path.Combine(appDatafolder, "Dynamo");
            if (Directory.Exists(dynamoFolder))
            {
                paths.AddRange(Directory.EnumerateDirectories(dynamoFolder));
            }

            //From 1.0 onwards Dynamo Studio user data is stored at %appdata%\Dynamo\Dynamo Revit\
            var revitFolder = Path.Combine(dynamoFolder, "Dynamo Revit");
            if (Directory.Exists(revitFolder))
            {
                paths.AddRange(Directory.EnumerateDirectories(revitFolder));
            }

            return paths;
        }
    }

    /// <summary>
    /// Defines parameters used for loading internal Dynamo Revit packages
    /// </summary>
    [Serializable()]
    public class InternalPackage
    {
        /// <summary>
        /// keeps the path to the node file
        /// </summary>
        public string NodePath { get; set; }

        /// <summary>
        /// keeps the path to the layoutSpecs.json file
        /// </summary>
        public string LayoutSpecsPath { get; set; }
    }
    internal static class DynamoRevitInternalNodes
    {
        private const string InternalNodesDir = "nodes";
        private static IEnumerable<string> GetAllInternalPackageFiles()
        {
            string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string currentAssemblyDir = Path.GetDirectoryName(currentAssemblyPath);

            string internalNodesDir = Path.Combine(currentAssemblyDir, InternalNodesDir);
            if (false == Directory.Exists(internalNodesDir))
            {
                return new List<string>();
            }

            string[] internalNodesFolders = Directory.GetDirectories(internalNodesDir);

            List<string> internalPackageFiles = new List<string>();
            foreach (string dir in internalNodesFolders)
            {
                string internalPackageFile = Path.Combine(dir, "internalPackage.xml");
                if (true == File.Exists(internalPackageFile))
                {
                    internalPackageFiles.Add(internalPackageFile);
                }
            }
            return internalPackageFiles;
        }
        private static IEnumerable<InternalPackage> ParseinternalPackageFiles(IEnumerable<string> internalPackageFiles)
        {
            List<InternalPackage> internalPackages = new List<InternalPackage>();

            foreach (string internalPackageFile in internalPackageFiles)
            {
                try
                {
                    string internalPackageDir = Path.GetDirectoryName(internalPackageFile);
                    using (StreamReader reader = new StreamReader(internalPackageFile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(InternalPackage));
                        InternalPackage intPackage = serializer.Deserialize(reader) as InternalPackage;

                        // convert to absolute path, if needed
                        if (false == Path.IsPathRooted(intPackage.NodePath))
                        {
                            intPackage.NodePath = Path.Combine(internalPackageDir, intPackage.NodePath);
                        }

                        // convert to absolute path, if needed
                        if (false == Path.IsPathRooted(intPackage.LayoutSpecsPath))
                        {
                            intPackage.LayoutSpecsPath = Path.Combine(internalPackageDir, intPackage.LayoutSpecsPath);
                        }

                        internalPackages.Add(intPackage);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(string.Format("Exception while trying to parse internalPackage file {0}", internalPackageFile));
                }
            }

            return internalPackages;
        }
        internal static IEnumerable<string> GetNodesToPreload()
        {
            IEnumerable<string> internalPackageFiles = GetAllInternalPackageFiles();
            return ParseinternalPackageFiles(internalPackageFiles).Select(pkg => pkg.NodePath);
        }
        internal static IEnumerable<string> GetLayoutSpecsFiles()
        {
            IEnumerable<string> internalPackageFiles = GetAllInternalPackageFiles();
            return ParseinternalPackageFiles(internalPackageFiles).Select(pkg => pkg.LayoutSpecsPath);
        }
    }
}