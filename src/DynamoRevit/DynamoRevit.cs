
using Greg.AuthProviders;

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
using System.Windows.Threading;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Applications.ViewModel;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Core.Threading;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.Services;
using Dynamo.UpdateManager;
using Dynamo.ViewModels;

using RevitServices.Persistence;
using RevitServices.Transactions;
using RevitServices.Threading;

using MessageBox = System.Windows.Forms.MessageBox;
using DynUpdateManager = Dynamo.UpdateManager.UpdateManager;
using Microsoft.Win32;


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
    [Transaction(TransactionMode.Manual),
     Regeneration(RegenerationOption.Manual)]
    public class DynamoRevit : IExternalCommand
    {
        enum Versions { ShapeManager = 220 }

        private static ExternalCommandData extCommandData;
        private static DynamoViewModel dynamoViewModel;
        private static RevitDynamoModel revitDynamoModel;
        private static bool handledCrash;

        // These fields are used to store information that
        // is pulled from the journal file.
        private static bool shouldShowUi = true;
        private static bool isAutomationMode;

        /// <summary>
        /// The journal file can use this key to specify whether
        /// the Dynamo UI should be visible at run time.
        /// </summary>
        private const string JournalShowUiKey = "dynShowUI";

        /// <summary>
        /// If the journal file specifies automation mode, 
        /// Dynamo will run on the main thread without the idle loop.
        /// </summary>
        private const string JounralAutomationModeKey = "dynAutomation";

        /// <summary>
        /// The journal file can specify a Dynamo workspace to 
        /// be opened (and executed) at run time.
        /// </summary>
        private const string JournalDynPathKey = "dynPath";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            HandleDebug(commandData);
            
            InitializeCore(commandData);

            try
            {
                extCommandData = commandData;
                shouldShowUi = CheckJournalForUiDisplay(extCommandData);
                isAutomationMode = CheckJournalForAutomationMode(extCommandData);

                // create core data models
                revitDynamoModel = InitializeCoreModel(extCommandData);
                dynamoViewModel = InitializeCoreViewModel(revitDynamoModel);

                UpdateSystemPathForProcess();

                revitDynamoModel.Logger.Log("SYSTEM", string.Format("Environment Path:{0}", Environment.GetEnvironmentVariable("PATH")));

                // handle initialization steps after RevitDynamoModel is created.
                revitDynamoModel.HandlePostInitialization();

                // show the window
                if (shouldShowUi)
                {
                    InitializeCoreView().Show();
                }

                TryOpenWorkspaceInCommandData(extCommandData);

                // Disable the Dynamo button to prevent a re-run
                DynamoRevitApp.DynamoButton.Enabled = false;
            }
            catch (Exception ex)
            {
                // notify instrumentation
                InstrumentationLogger.LogException(ex);
                StabilityTracking.GetInstance().NotifyCrash();

                MessageBox.Show(ex.ToString());

                DynamoRevitApp.DynamoButton.Enabled = true;

                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private static void UpdateSystemPathForProcess()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            var parentDirectory = Directory.GetParent(assemblyDirectory);
            var corePath = parentDirectory.FullName;

            // Add the main exec path to the system PATH
            // This is required to pickup certain dlls.
            var path =
                    Environment.GetEnvironmentVariable(
                        "Path",
                        EnvironmentVariableTarget.Process) + ";" + corePath;
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

        private static RevitDynamoModel InitializeCoreModel(ExternalCommandData commandData)
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            var parentDirectory = Directory.GetParent(assemblyDirectory);
            var corePath = parentDirectory.FullName;

            var umConfig = UpdateManagerConfiguration.GetSettings(new DynamoRevitLookUp());
            Debug.Assert(umConfig.DynamoLookUp != null);

            return RevitDynamoModel.Start(
                new RevitDynamoModel.RevitStartConfiguration()
                {
                    GeometryFactoryPath = GetGeometryFactoryPath(corePath),
                    PathResolver = new RevitPathResolver(),
                    Context = GetRevitContext(commandData),
                    SchedulerThread = new RevitSchedulerThread(commandData.Application),
                    StartInTestMode = isAutomationMode,
                    AuthProvider = new RevitOxygenProvider(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher)),
                    ExternalCommandData = commandData,
                    UpdateManager = new DynUpdateManager(umConfig),
                });
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

            return dynamoView;
        }

        private static bool initializedCore;
        private static void InitializeCore(ExternalCommandData commandData)
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

        private static void InitializeDocumentManager(ExternalCommandData commandData)
        {
            if (DocumentManager.Instance.CurrentUIApplication == null)
                DocumentManager.Instance.CurrentUIApplication = commandData.Application;
        }

        #endregion

        #region Helpers

        private void HandleDebug(ExternalCommandData commandData)
        {
            if (commandData.JournalData != null && commandData.JournalData.ContainsKey("debug"))
            {
                if (Boolean.Parse(commandData.JournalData["debug"]))
                    Debugger.Launch();
            }
        }

        private static bool CheckJournalForUiDisplay(ExternalCommandData commandData)
        {
            var result = true;

            if (commandData.JournalData == null)
            {
                return result;
            }

            if (commandData.JournalData.ContainsKey(JournalShowUiKey))
            {
                bool.TryParse(commandData.JournalData[JournalShowUiKey], out result);
            }

            return result;
        }

        private static bool CheckJournalForAutomationMode(ExternalCommandData commandData)
        {
            var result = false;

            if (commandData.JournalData == null)
            {
                return result;
            }

            if (commandData.JournalData.ContainsKey(JounralAutomationModeKey))
            {
                result = bool.TryParse(commandData.JournalData[JounralAutomationModeKey], out result);
            }

            return result;
        }

        private static void TryOpenWorkspaceInCommandData(ExternalCommandData commandData)
        {
            if(commandData.JournalData == null)
            {
                return;
            }

            if (commandData.JournalData.ContainsKey(JournalDynPathKey))
            {
                revitDynamoModel.OpenFileFromPath(commandData.JournalData[JournalDynPathKey]);
            }  
        }

        private static string GetRevitContext(ExternalCommandData commandData)
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
                InstrumentationLogger.LogException(args.Exception);
                StabilityTracking.GetInstance().NotifyCrash();

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
                DynamoRevitApp.DynamoButton.Enabled = true;
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

            DynamoRevitApp.DynamoButton.Enabled = true;
        }

        #endregion
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
    }
}
