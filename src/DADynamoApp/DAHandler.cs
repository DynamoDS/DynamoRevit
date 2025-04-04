using DesignAutomationFramework;
using Autodesk.Revit.ApplicationServices;
using System.Reflection;
using Dynamo.Models;
using System.IO.Compression;
using DynamoPlayer;
using RevitServices.Persistence;

namespace DADynamoApp
{
    public class DynamoADApp
    {
        private ControlledApplication ControlledApplication;
        private DynamoModel model;
        internal static DynamoPlayerLoggerConfiguration logConfig = new DynamoPlayerLoggerConfiguration() { DynamoLogLevel = Dynamo.Logging.LogLevel.Console, LogLevel = DynamoPlayer.LogLevel.Information };

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

        private bool Loaded = false;
        private string LoadMessage;
        private string WorkItemFolder;

        public void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            Console.WriteLine("<<!>> DA event raised.");
            WorkItemFolder = Directory.GetCurrentDirectory();
            var dynTempDir = Path.Combine(WorkItemFolder, "dyn_tmp");
            Console.WriteLine($"<<!>> Work folder is '{WorkItemFolder}'");
            var app = e.DesignAutomationData?.RevitApp;
            Console.WriteLine("<<!>> Preparing Dynamo model. Vers 1");


            // Initialize Dynamo Core
            // Preload ASM from Revit
            // Preload Core libs from DynamoCore
            // Preload NodeModels and libs from DynamoRevit
            // Setup design automation
            // Startup Player run request

            var hostloc = typeof(Autodesk.Revit.ApplicationServices.Application).Assembly.Location;
            var asmLocation = Path.GetDirectoryName(hostloc);
            Console.WriteLine($"using asm at location {asmLocation}");

            var curFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;


            Console.WriteLine($"Is Loaded {LoadMessage}");

            var cmdArgs = new Dynamo.Applications.StartupUtils.CommandLineArguments();
            cmdArgs.ASMPath = asmLocation;
            cmdArgs.DisableAnalytics = true;
            cmdArgs.ServiceMode = true;
            cmdArgs.AnalyticsInfo = new() { HostName = "Revit_DA" };
            cmdArgs.ImportedPaths = [Path.Combine(curFolder, "nodes", "RevitNodes.dll"), Path.Combine(curFolder, "nodes", "DSRevitNodesUI.dll")];
            cmdArgs.UserDataFolder = Path.Combine(dynTempDir, "Dynamo Revit");
            cmdArgs.CommonDataFolder = Path.Combine(dynTempDir, "Dynamo");

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


            var testFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var res = dynHandler.HandleRoute("POST", "/v1/graph/run", File.ReadAllText(Path.Combine(testFolder, "test.json")));
            var output = res.Result;

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(output));

            model.RunCompleted += Model_RunCompleted;

            e.Succeeded = true;
        }

        private void Model_RunCompleted(object sender, bool success)
        {
            Console.WriteLine($"Run Completed");
        }
    }
}