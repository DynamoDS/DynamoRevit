using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using Dynamo.Models;
using DynamoPlayer;
using RevitServices.Persistence;
using RevitServices.Elements;
using System.Reflection;
using System.Runtime.Loader;
using System.Diagnostics;
using Dynamo.Applications;
using Dynamo.Scheduler;
using static Dynamo.Models.DynamoModel;
using System.Text.RegularExpressions;
using Greg.AuthProviders;
using Revit.Elements;
using Dynamo.PythonServices;
using DSCPython;
using Newtonsoft.Json;
using Revit.Transaction;

namespace DADynamoApp
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DAEntrypoint : IExternalDBApplication
    {
        private DynamoModel model;
        internal static DynamoPlayerLoggerConfiguration logConfig = new DynamoPlayerLoggerConfiguration() { DynamoLogLevel = Dynamo.Logging.LogLevel.Console, LogLevel = DynamoPlayer.LogLevel.Information };
        private AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
        private string DynamoPath;
        private string DynamoRevitPath;
        private ControlledApplication controlledApplication;

        private string LoadMessage;
        private string WorkItemFolder;
        private readonly string PythonDllFolder = "pythonDependencies";


        public static List<IUpdater> Updaters = new List<IUpdater>();

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            DynamoRevitPythonManager.Cleanup();

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            controlledApplication.DocumentClosing -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosing;
            controlledApplication.DocumentClosed -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosed;
            controlledApplication.DocumentOpened -= RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentOpened;

            return ExternalDBApplicationResult.Succeeded;
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            Process proc = Process.GetCurrentProcess();
            Console.WriteLine($"Dynamo exiting with Peak physical memory {proc.PeakWorkingSet64} bytes");
            if (proc.HasExited)
            {
                Console.WriteLine($"Dynamo exiting with code {proc.ExitCode}");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled exception: {e}");
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            controlledApplication = application;

            WorkItemFolder = Directory.GetCurrentDirectory();
            Console.WriteLine($"<<!>> Work folder is '{WorkItemFolder}'");

            DynamoRevitPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DynamoPath = Directory.GetParent(DynamoRevitPath).FullName;

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var hostDir = Path.GetDirectoryName(hostloc);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Console.WriteLine("<<!>> Starting to load D4DA");

            try
            {
                RevitServicesUpdater.Initialize(Updaters);
                Console.WriteLine("<<!>> D4DA Loaded");

                RevitServices.Transactions.TransactionManager.SetupManager(new RevitServices.Transactions.AutomaticTransactionStrategy());
                // TODO: do we need element binding in Design Automations?
                ElementBinder.IsEnabled = true;

                controlledApplication.DocumentClosing += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosing;
                controlledApplication.DocumentClosed += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentClosed;
                controlledApplication.DocumentOpened += RevitServices.EventHandler.EventHandlerProxy.Instance.OnApplicationDocumentOpened;

                DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;

                return ExternalDBApplicationResult.Succeeded;
            }
            catch (Exception ex)
            {
                return ExternalDBApplicationResult.Failed;
            }
        }

        private Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            return DynamoRevitAssemblyResolver.ResolveDynamoAssembly(DynamoPath, [Path.Combine(WorkItemFolder, PythonDllFolder)], args);
        }

        private static string GetRevitContext(Autodesk.Revit.ApplicationServices.Application app)
        {
            var r = new Regex(@"\b(Autodesk |Structure |MEP |Architecture )\b");
            return r.Replace(app.VersionName, "");
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            Console.WriteLine("<<!>> DA event raised.");

            // Local Change
            //WorkItemFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var dynTempDir = Path.Combine(WorkItemFolder, "dyn_tmp");
            var app = e.DesignAutomationData?.RevitApp;
            Console.WriteLine("<<!>> Preparing Dynamo model. Vers 1");

            // Startup a new project, maybe an option we can have ?
            //app.NewProjectDocument(Autodesk.Revit.DB.UnitSystem.Metric);

            Document? doc = e.DesignAutomationData?.RevitDoc;
            if (doc == null) throw new InvalidOperationException("Could not open revit document.");

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var asmLocation = Path.GetDirectoryName(hostloc);
            Console.WriteLine($"using asm at location {asmLocation}");
            Console.WriteLine($"Is Loaded {LoadMessage}");

            // need this for cloud, does not work on local
            var userDataFolder = Path.Combine(dynTempDir, "Dynamo Revit");
            var commonDataFolder = Path.Combine(dynTempDir, "Dynamo");

            DocumentManager.Instance.PrepareForAutomation(app);

            // Local Change
            //var geometryFactoryPath = @"C:\\Program Files\\Autodesk\\Revit 2025";

            var loadedLibGVersion = ASMPrealoaderUtils.PreloadAsmFromRevit(controlledApplication.SharedComponentsLocation, DynamoPath);
            var geometryFactoryPath = ASMPrealoaderUtils.GetGeometryFactoryPath(DynamoPath, loadedLibGVersion);

            PreInstallPythonDependencies();

            model = Dynamo.Applications.Models.RevitDynamoModel.Start(
                new DefaultStartConfiguration
                {
                    DynamoCorePath = DynamoPath,
                    DynamoHostPath = DynamoRevitPath,
                    GeometryFactoryPath = geometryFactoryPath,
                    PathResolver = new RevitPathResolver(userDataFolder, commonDataFolder),
                    Context = GetRevitContext(app),
                    AuthProvider = new RevitOAuth2Provider(SynchronizationContext.Current ?? new SynchronizationContext()),
                    ProcessMode = TaskProcessMode.Synchronous,
                    CLIMode = true,
                    IsHeadless = true,
                    IsServiceMode = true
                });

            LoadMessage = model != null ? "loaded" : "no loaded";

            var playerHost = new PlayerHostDynamoDefault(model, new DynamoPlayerLogger<PlayerHostDynamoDefault>(logConfig));
            var workflows = new DynamoModelWorkflows(
                playerHost,
                new DynamoPlayerLogger<DynamoModelWorkflows>(logConfig));

            var controller = new DynamoControllerImplementation(playerHost, workflows,
                new DynamoPlayerLogger<DynamoControllerImplementation>(logConfig));

            DynamoPlayerLogger.Initialize(playerHost);

            workflows.LoadDependencies(new GraphTarget() { DependenciesPath = WorkItemFolder });

            var dynHandler = new Handler(playerHost, [new DARunGraphController(controller, model, WorkItemFolder)]);

            // Default, save the rvt
            bool saveRvt = true;
            var setupReqPath = Path.Combine(WorkItemFolder, "setup.json");
            if (File.Exists(setupReqPath))
            {
                try
                {
                    var setupRequest = File.ReadAllText(setupReqPath);
                    Console.WriteLine(setupRequest);

                    SetupDARequest setupReq = JsonConvert.DeserializeObject<SetupDARequest>(setupRequest, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto});
                    saveRvt = setupReq?.SaveRvt ?? saveRvt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            var runContent = File.ReadAllText(Path.Combine(WorkItemFolder, "run.json"));
            var testFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var output = string.Empty;
            try
            {
                var res = dynHandler.HandleRoute("POST", "/v1/graph/run", runContent);
                output = res.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine(output);
            File.WriteAllText(Path.Combine(WorkItemFolder, "result.json"), output);

            Console.WriteLine($"{nameof(saveRvt)} is set to {saveRvt}");
            if (saveRvt)
            {
                try
                {
                    RevitServices.Transactions.TransactionManager.Instance.ForceCloseTransaction();
                    ModelPath path = ModelPathUtils.ConvertUserVisiblePathToModelPath("result.rvt");
                    doc.SaveAs(path, new SaveAsOptions());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            model.RunCompleted += Model_RunCompleted;
            e.Succeeded = true;
        }

        private void PreInstallPythonDependencies()
        {
            try
            {
                // Preload all python assemblies at the PythonDllFolder.
                foreach (var pyDll in Directory.EnumerateFiles(Path.Combine(WorkItemFolder, PythonDllFolder), "*.dll"))
                {
                    Assembly.LoadFrom(pyDll);
                }

                var pyIncluded = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "Python.Included");
                if (pyIncluded == null)
                {
                    throw new Exception("Could not find Python.Included assembly");
                }
                var type = pyIncluded.GetType("Python.Included.Installer");
                if (type == null)
                {
                    throw new Exception("null Installer type");
                }
                var property = type.GetProperty("INSTALL_PATH", BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    throw new Exception("null INSTALL_PATH property");
                }

                // Set the python install location to the DA workfolder (that is the only place we have wrie access)
                property.SetValue(null, WorkItemFolder);

                // Dynamo's 'VerifyEngineReferences' wants all the PythonEngine's dependencies to be in the Dynamo folder.
                // Temporary until we fix it on the Dynamo side.
                if (PythonEngineManager.Instance.AvailableEngines.Count == 0)
                {
                    PropertyInfo instanceProp = typeof(CPythonEvaluator).GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static);
                    if (instanceProp != null)
                    {
                        PythonEngine engine = (PythonEngine)instanceProp.GetValue(null);
                        if (engine == null)
                        {
                            throw new Exception($"Could not get a valid PythonEngine instance");
                        }

                        PythonEngineManager.Instance.AvailableEngines.Add(engine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not setup python " + ex.Message);
            }
        }

        private void Model_RunCompleted(object sender, bool success)
        {
            Console.WriteLine($"Run Completed");
        }
    }
}