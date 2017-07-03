using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
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
using DynUpdateManager = Dynamo.Updates.UpdateManager;
using MessageBox = System.Windows.Forms.MessageBox;

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
        enum Versions { ShapeManager = 222 };

        /// <summary>
        /// Based on the RevitDynamoModelState a dependent component can take certain 
        /// decisions regarding its UI and functionality.
        /// In order to be able to run a specified graph , revitDynamoModel needs to be 
        /// at least in StartedUIless state. 
        /// </summary>
        public enum RevitDynamoModelState { NotStarted, StartedUIless, StartedUI };

        private static List<Action> idleActions;
        private static DynamoRevitCommandData extCommandData;
        private static DynamoViewModel dynamoViewModel;
        private static RevitDynamoModel revitDynamoModel;
        private static bool handledCrash;
        private static List<Exception> preLoadExceptions;
        private static Action shutdownHandler;

        /// <summary>
        /// The modelState tels us if the RevitDynamoModel was started and if has the
        /// the Dynamo UI attached to it or not 
        /// </summary>
        public static RevitDynamoModelState ModelState
        {
            get;
            private set;
        }

        static DynamoRevit()
        {
            idleActions = new List<Action>();
            extCommandData = null;
            dynamoViewModel = null;
            revitDynamoModel = null;
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
            var startupTimer = Stopwatch.StartNew();
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
                    revitDynamoModel.ShutDown(false);
                    ModelState = RevitDynamoModelState.NotStarted;
                }
                else
                {
                    TryOpenAndExecuteWorkspaceInCommandData(commandData);
                    return Result.Succeeded;
                }
            }

            if(ModelState == RevitDynamoModelState.StartedUI)
            {
                TryOpenAndExecuteWorkspaceInCommandData(commandData);
                return Result.Succeeded;
            }

            HandleDebug(commandData);

            InitializeCore(commandData);
            //subscribe to the assembly load
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoad;

            try
            {
                extCommandData = commandData;

                UpdateSystemPathForProcess();

                // create core data models
                revitDynamoModel = InitializeCoreModel(extCommandData);
                revitDynamoModel.UpdateManager.RegisterExternalApplicationProcessId(Process.GetCurrentProcess().Id);
                revitDynamoModel.Logger.Log("SYSTEM", string.Format("Environment Path:{0}", Environment.GetEnvironmentVariable("PATH")));

                // handle initialization steps after RevitDynamoModel is created.
                revitDynamoModel.HandlePostInitialization();
                ModelState = RevitDynamoModelState.StartedUIless;

                // show the window
                if (CheckJournalForKey(extCommandData, JournalKeys.ShowUiKey, true))
                {
                    dynamoViewModel = InitializeCoreViewModel(revitDynamoModel);

                    // Let the host (e.g. Revit) control the rendering mode
                    var save = RenderOptions.ProcessRenderMode;
                    InitializeCoreView().Show();
                    RenderOptions.ProcessRenderMode = save;
                    revitDynamoModel.Logger.Log(Dynamo.Applications.Properties.Resources.WPFRenderMode + RenderOptions.ProcessRenderMode.ToString());

                    ModelState = RevitDynamoModelState.StartedUI;
                    // Disable the Dynamo button to prevent a re-run
                    DynamoRevitApp.DynamoButtonEnabled = false;
                }

                //foreach preloaded exception send a notification to the Dynamo Logger
                //these are messages we want the user to notice.
                preLoadExceptions.ForEach(x => revitDynamoModel.Logger.LogNotification
                (revitDynamoModel.GetType().ToString(),
                x.GetType().ToString(),
                DynamoApplications.Properties.Resources.MismatchedAssemblyVersionShortMessage,
                x.Message));

                TryOpenAndExecuteWorkspaceInCommandData(extCommandData);

                //unsubscribe to the assembly load
                AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;
                Analytics.TrackStartupTime("DynamoRevit", startupTimer.Elapsed, ModelState.ToString());
            }
            catch (Exception ex)
            {
                // notify instrumentation
                Analytics.TrackException(ex, true);

                MessageBox.Show(ex.ToString());

                DynamoRevitApp.DynamoButtonEnabled = true;
                
                //If for some reason Dynamo has crashed while startup make sure the Dynamo Model is properly shutdown.
                if (revitDynamoModel != null)
                {
                    revitDynamoModel.ShutDown(false);
                    revitDynamoModel = null;
                }

                return Result.Failed;
            }

            return Result.Succeeded;
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

        public static RevitDynamoModel RevitDynamoModel
        {
            get
            {
                return revitDynamoModel;
            }
            set
            {
                revitDynamoModel = value;
            }
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
        public static string GetGeometryFactoryPath(string corePath)
        {
            var dynamoAsmPath = Path.Combine(corePath, "DynamoShapeManager.dll");
            var assembly = Assembly.LoadFrom(dynamoAsmPath);
            if (assembly == null)
                throw new FileNotFoundException("File not found", dynamoAsmPath);

            var utilities = assembly.GetType("DynamoShapeManager.Utilities");
            var getGeometryFactoryPath = utilities.GetMethod("GetGeometryFactoryPath");

            return (getGeometryFactoryPath.Invoke(null,
                new object[] { corePath, Versions.ShapeManager }) as string);
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
                if(File.Exists(assemblyPath))
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
            var revitVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var umConfig = UpdateManagerConfiguration.GetSettings(new DynamoRevitLookUp());
            var revitUpdateManager = new DynUpdateManager(umConfig);
            revitUpdateManager.HostVersion = revitVersion; // update RevitUpdateManager with the current DynamoRevit Version
            revitUpdateManager.HostName = "Dynamo Revit";

            Debug.Assert(umConfig.DynamoLookUp != null);

            var userDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), 
                "Dynamo", "Dynamo Revit");
            var commonDataFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData), 
                "Dynamo", "Dynamo Revit");

            bool isAutomationMode = CheckJournalForKey(extCommandData,JournalKeys.AutomationModeKey);

            PreloadAsmFromRevit();

            return RevitDynamoModel.Start(
                new RevitDynamoModel.RevitStartConfiguration()
                {
                    DynamoCorePath = corePath,
                    DynamoHostPath = dynamoRevitRoot,
                    GeometryFactoryPath = GetGeometryFactoryPath(corePath),
                    PathResolver = new RevitPathResolver(userDataFolder, commonDataFolder),
                    Context = GetRevitContext(commandData),
                    SchedulerThread = new RevitSchedulerThread(commandData.Application),
                    StartInTestMode = isAutomationMode,
                    AuthProvider = new RevitOxygenProvider(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher)),
                    ExternalCommandData = commandData,
                    UpdateManager = revitUpdateManager,
                    ProcessMode = isAutomationMode ? TaskProcessMode.Synchronous : TaskProcessMode.Asynchronous
                });
        }

        private static void PreloadAsmFromRevit()
        {
            var asmLocation = AppDomain.CurrentDomain.BaseDirectory;

            var lookup = new InstalledProductLookUp("Revit", "ASMAHL*.dll");
            var product = lookup.GetProductFromInstallPath(asmLocation);

            var dynCorePath = DynamoRevitApp.DynamoCorePath;
            var libGFolderName = string.Format("libg_{0}", product.VersionInfo.Item1);
            var preloaderLocation = Path.Combine(dynCorePath, libGFolderName);

            DynamoShapeManager.Utilities.PreloadAsmFromPath(preloaderLocation, asmLocation);
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

        private static DynamoView InitializeCoreView()
        {
            IntPtr mwHandle = Process.GetCurrentProcess().MainWindowHandle;
            var dynamoView = new DynamoView(dynamoViewModel);
            new WindowInteropHelper(dynamoView).Owner = mwHandle;

            handledCrash = false;

            dynamoView.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            dynamoView.Closed += OnDynamoViewClosed;
            dynamoView.Loaded += (o, e) => UpdateLibraryLayoutSpec();

            return dynamoView;
        }

        /// <summary>
        /// Updates the Libarary Layout spec to include layout for Revit nodes. 
        /// The Revit layout spec is embeded as resource "LayoutSpecs.json".
        /// </summary>
        private static void UpdateLibraryLayoutSpec()
        {
            //Get the library view customization service to update spec
            var customization = revitDynamoModel.ExtensionManager.Service<ILibraryViewCustomization>();
            if (customization == null) return;

            if(shutdownHandler == null && extCommandData != null)
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

            InitializeAssemblies();
            InitializeDocumentManager(commandData);

            initializedCore = true;
        }

        private static void InitializeAssemblies()
        {
            RevitAssemblyLoader.LoadAll();
            AppDomain.CurrentDomain.AssemblyResolve +=
                Analyze.Render.AssemblyHelper.ResolveAssemblies;
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

                if (ModelState == RevitDynamoModelState.StartedUIless)
                {
                    revitDynamoModel.OpenFileFromPath(commandData.JournalData[JournalKeys.DynPathKey], forceManualRun);
                }
                else
                {
                    dynamoViewModel.OpenIfSavedCommand.Execute(new Dynamo.Models.DynamoModel.OpenFileCommand(commandData.JournalData[JournalKeys.DynPathKey], forceManualRun));
                    dynamoViewModel.ShowStartPage = false;
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
                                    modelCommand.Execute(revitDynamoModel);
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
                        HomeWorkspaceModel modelToRun = revitDynamoModel.CurrentWorkspace as HomeWorkspaceModel;
                        if (modelToRun != null)
                        {
                            modelToRun.Run();
                            return;
                        }
                    }
                }

            }
        }

        private static string GetRevitContext(DynamoRevitCommandData commandData)
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

                revitDynamoModel.Logger.LogError("Dynamo Unhandled Exception");
                revitDynamoModel.Logger.LogError(exceptionMessage);
            }
            catch { }

            try
            {
                DynamoModel.IsCrashing = true;
                revitDynamoModel.OnRequestsCrashPrompt(
                    revitDynamoModel,
                    new CrashPromptArgs(args.Exception.Message + "\n\n" + args.Exception.StackTrace));
                dynamoViewModel.Exit(false); // don't allow cancellation
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

            AppDomain.CurrentDomain.AssemblyResolve -=
                Analyze.Render.AssemblyHelper.ResolveAssemblies;

            DynamoRevitApp.DynamoButtonEnabled = true;

            //the model is shutdown when DynamoView is closed
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
}
