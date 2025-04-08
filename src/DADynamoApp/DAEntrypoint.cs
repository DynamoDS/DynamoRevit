using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using Dynamo.Models;
using DynamoPlayer;
using RevitServices.Persistence;
using RevitServices.Elements;
using RevitServices.EventHandler;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Diagnostics;

namespace DADynamoApp
{
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class DAEntrypoint : IExternalDBApplication
    {
        private DynamoModel model;
        internal static DynamoPlayerLoggerConfiguration logConfig = new DynamoPlayerLoggerConfiguration() { DynamoLogLevel = Dynamo.Logging.LogLevel.Console, LogLevel = DynamoPlayer.LogLevel.Information };

        private string LoadMessage;
        private string WorkItemFolder;

        private static EventHandlerProxy proxy;
        public static EventHandlerProxy EventHandlerProxy
        {
            get { return proxy; }
        }

        public static List<IUpdater> Updaters = new List<IUpdater>();

        private void CheckIfPythonExists(string inputFolder)
        {
            var pyDir = "python-3.9.12-embed-amd64";
            var pyRoot = Path.GetDirectoryName(model.PathManager.CommonDataDirectory);
            var searchPath = Path.Combine(pyRoot, pyDir);
            if (Directory.Exists(searchPath) && File.Exists(Path.Combine(searchPath, "python.exe")))
            {
                return;
            }
            Console.WriteLine($"<<!>> cPython not found at '{searchPath}'");
            var inputDistributionPath = Path.Combine(inputFolder, $"{pyDir}.zip");
            if (File.Exists(inputDistributionPath))
            {
                Console.WriteLine("<<!>> cPython distribution detected in input. Attempting to add it... ");
                try
                {
                    ZipFile.ExtractToDirectory(inputDistributionPath, searchPath);
                    Console.WriteLine("<<!>> cPython added.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"<<!>> Failed to add cPython: {ex.Message}");
                }
            }
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit -= CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve -= DynamoPathResolver.ResolveAssembly;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyResolve += DynamoPathResolver.ResolveAssembly;

            Console.WriteLine("<<!>> Starting to load D4DA");

            try
            {
                RevitServices.Transactions.TransactionManager.SetupManager(new RevitServices.Transactions.AutomaticTransactionStrategy());
                ElementBinder.IsEnabled = true;

                RevitServicesUpdater.Initialize(Updaters);

                DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;

                Console.WriteLine("<<!>> D4DA Loaded");

                return ExternalDBApplicationResult.Succeeded;
            }
            catch (Exception ex)
            {
                return ExternalDBApplicationResult.Failed;
            }
        }

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            Console.WriteLine("<<!>> DA event raised.");
            WorkItemFolder = Directory.GetCurrentDirectory();

            foreach (var xx in Directory.EnumerateFiles(WorkItemFolder))
                Console.WriteLine(xx);

            var dynTempDir = Path.Combine(WorkItemFolder, "dyn_tmp");
            Console.WriteLine($"<<!>> Work folder is '{WorkItemFolder}'");
            var app = e.DesignAutomationData?.RevitApp;
            Console.WriteLine("<<!>> Preparing Dynamo model. Vers 1");

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var asmLocation = Path.GetDirectoryName(hostloc);
            Console.WriteLine($"using asm at location {asmLocation}");
            Console.WriteLine($"Is Loaded {LoadMessage}");
            var curFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var cmdArgs = new Dynamo.Applications.StartupUtils.CommandLineArguments();
            cmdArgs.ASMPath = asmLocation;
            cmdArgs.DisableAnalytics = true;
            cmdArgs.ServiceMode = true;
            cmdArgs.AnalyticsInfo = new() { HostName = "Revit_DA" };
            cmdArgs.ImportedPaths = [Path.Combine(curFolder, "nodes", "RevitNodes.dll"), Path.Combine(curFolder, "nodes", "DSRevitNodesUI.dll")];
            // need this for cloud, does not work on local
            cmdArgs.UserDataFolder = Path.Combine(dynTempDir, "Dynamo Revit");
            cmdArgs.CommonDataFolder = Path.Combine(dynTempDir, "Dynamo");

            app.NewProjectDocument(Autodesk.Revit.DB.UnitSystem.Metric);

            DocumentManager.Instance.PrepareForAutomation(app);

            //true, asmLocation, "Revit_DA"
            model = Dynamo.Applications.StartupUtils.MakeCLIModel(cmdArgs);

            LoadMessage = model != null ? "loaded" : "no loaded";
            Console.WriteLine($"Checking python in {curFolder}");
            CheckIfPythonExists(curFolder);

            var playerHost = new PlayerHostDynamoDefault(model, new DynamoPlayerLogger<PlayerHostDynamoDefault>(logConfig));
            var workflows = new DynamoModelWorkflows(
                playerHost,
                new DynamoPlayerLogger<DynamoModelWorkflows>(logConfig));

            var controller = new DynamoControllerImplementation(playerHost, workflows,
                new DynamoPlayerLogger<DynamoControllerImplementation>(logConfig));

            DynamoPlayerLogger.Initialize(playerHost);

            var dynHandler = new Handler(playerHost, [new DynamoController(controller)]);

            var runContent = File.ReadAllText(Path.Combine(WorkItemFolder, "run.json"));

            var testFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var res = dynHandler.HandleRoute("POST", "/v1/graph/run", runContent);
            var output = res.Result;

            var result = Newtonsoft.Json.JsonConvert.SerializeObject(output);
            Console.WriteLine(result);

            File.WriteAllText(Path.Combine(WorkItemFolder, "result.json"), result);

            model.RunCompleted += Model_RunCompleted;
            e.Succeeded = true;
        }

        private void Model_RunCompleted(object sender, bool success)
        {
            Console.WriteLine($"Run Completed");
        }
    }
}