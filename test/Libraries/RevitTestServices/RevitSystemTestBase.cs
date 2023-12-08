using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Dynamo.Applications;
using Dynamo.Applications.Models;
using Dynamo.Applications.ViewModel;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.ViewModels;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;
using RTF.Applications;
using SystemTestServices;
using TestServices;
using Dynamo.Configuration;
using RTF.Framework;
using Dynamo.Controls;
using Dynamo.Interfaces;
using Dynamo.Updates;

namespace RevitTestServices
{
    public class RevitTestConfiguration
    {
        /// <summary>
        /// Directory where test files are kept
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Directory where Samples are kept
        /// </summary>
        public string SamplesPath { get; set; }

        /// <summary>
        /// TestConfiguration file name
        /// </summary>
        private const string TEST_CONFIGURATION_FILE_S = "RevitTestConfiguration.xml";

        /// <summary>
        /// Loads configuration
        /// </summary>
        /// <returns></returns>
        public static RevitTestConfiguration LoadConfiguration()
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string assDir = fi.DirectoryName;
            var config = new RevitTestConfiguration();
            try
            {
                var configPath = Path.Combine(assDir, TEST_CONFIGURATION_FILE_S);
                if (File.Exists(configPath))
                {
                    var serializer = new XmlSerializer(typeof(RevitTestConfiguration));
                    using (var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read))
                    {
                        config = serializer.Deserialize(fs) as RevitTestConfiguration;
                    }
                }
#if DEBUG
                else
                {
                    config.SetDefaultValuesToUninitializedProperties();
                    config.Save(configPath);
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            config.SetDefaultValuesToUninitializedProperties();
            return config;
        }

        private void SetDefaultValuesToUninitializedProperties()
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string assDir = fi.DirectoryName;

            //It is used to get the path of journal file.
            //The path to the journal files is different. 
            //We have to get where each journal file runs when Revit gets it,
            //and then get the location of the required Revit files and dynamo files.
            if (RevitSystemTestBase.IsJournalReplaying())
            {
                string journalPath = RevitTestExecutive.CommandData.Application.Application.RecordingJournalFilename;
                WorkingDirectory = Path.GetDirectoryName(journalPath);
            }

            //get the test path
            if (string.IsNullOrEmpty(WorkingDirectory))
            {
                string testsLoc = Path.Combine(assDir, @"..\..\..\..\test\System\");
                WorkingDirectory = Path.GetFullPath(testsLoc);
            }

            //get the samples path
            if (string.IsNullOrEmpty(SamplesPath))
            {
                string samplesLoc = Path.Combine(assDir, @"..\..\..\..\doc\distrib\Samples\");
                SamplesPath = Path.GetFullPath(samplesLoc);
            }
        }

        private void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(RevitTestConfiguration));
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(fs, this);
            }
        }
    }

    public class TestSchedulerThread : ISchedulerThread
    {
        public void Initialize(IScheduler owningScheduler)
        {

        }

        public void Shutdown()
        {

        }
    }

    [TestFixture]
    public class RevitSystemTestBase : SystemTestBase, IRTFSetup
    {
        private string samplesPath;
        protected string emptyModelPath1;
        protected string emptyModelPath;

        #region public methods

        protected void WrapOf(RevitSystemTestBase initedInstance)
        {
            pathResolver = initedInstance.pathResolver;
            workingDirectory = initedInstance.workingDirectory;
            //preloader = other.preloader;
            //assemblyResolver = other.assemblyResolver;
            //originalBuiltinPackagesDirectory = other.originalBuiltinPackagesDirectory;
            ViewModel = initedInstance.ViewModel;
            View = initedInstance.View;
            Model = initedInstance.Model;
            //TempFolder = other.TempFolder;

            samplesPath = initedInstance.samplesPath;
            emptyModelPath = initedInstance.emptyModelPath;
            emptyModelPath1 = initedInstance.emptyModelPath1;
        }

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            ((HomeWorkspaceModel)ViewModel.Model.CurrentWorkspace).RunSettings.RunType = RunType.Manual;

            DocumentManager.Instance.CurrentUIApplication.ViewActivating += CurrentUIApplication_ViewActivating;
        }

        [TearDown]
        public override void TearDown()
        {
            DocumentManager.Instance.CurrentUIApplication.ViewActivating -= CurrentUIApplication_ViewActivating;

            // Automatic transaction strategy requires that we 
            // close the transaction if it hasn't been closed by 
            // by the end of an evaluation. It is possible to 
            // run the test framework without running Dynamo, so
            // we ensure that the transaction is closed here.
            TransactionManager.Instance.ForceCloseTransaction();

            base.TearDown();
        }

        #endregion

        #region protected methods

        protected override void SetupCore()
        {
            DocumentManager.Instance.CurrentUIApplication =
                RevitTestExecutive.CommandData.Application;
            DocumentManager.Instance.CurrentUIDocument =
                RevitTestExecutive.CommandData.Application.ActiveUIDocument;

            var config = RevitTestConfiguration.LoadConfiguration();

            //get the test path
            workingDirectory = config.WorkingDirectory;

            //get the samples path
            samplesPath = config.SamplesPath;

            emptyModelPath = Path.Combine(workingDirectory, "empty.rfa");

            if (DocumentManager.Instance.CurrentUIApplication.Application.VersionNumber.Contains("2014") &&
                DocumentManager.Instance.CurrentUIApplication.Application.VersionName.Contains("Vasari"))
            {
                emptyModelPath = Path.Combine(workingDirectory, "emptyV.rfa");
                emptyModelPath1 = Path.Combine(workingDirectory, "emptyV1.rfa");
            }
            else
            {
                emptyModelPath = Path.Combine(workingDirectory, "empty.rfa");
                emptyModelPath1 = Path.Combine(workingDirectory, "empty1.rfa");
            }
        }

        /// <summary>
        /// Indicates whether it is in journal replaying mode.
        /// </summary>
        /// <returns>Whether journal is replaying or not.</returns>
        public static bool IsJournalReplaying()
        {
            var method = typeof(Autodesk.Revit.UI.UIFabricationUtils).GetMethod("IsJournalReplaying", BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                return (bool)method.Invoke(null, null);
            }
            return false;
        }

        protected override void StartDynamo(TestSessionConfiguration testConfig)
        {
            try
            {
                UpdateSystemPathForProcess();

                // create the transaction manager object
                TransactionManager.SetupManager(new AutomaticTransactionStrategy());

                // Note that there is another data member pathResolver in base class 
                // SystemTestBase. That pathResolver will be used only in StartDynamo
                // of the base class, here a local instance of pathResolver is used.
                // 
                var commandData = new DynamoRevitCommandData(RevitTestExecutive.CommandData);
                var userDataFolder = Path.Combine(Environment.GetFolderPath(
                  Environment.SpecialFolder.ApplicationData),
                  "Dynamo", "Dynamo Revit");
                var commonDataFolder = Path.Combine(Environment.GetFolderPath(
                  Environment.SpecialFolder.CommonApplicationData),
                  "Autodesk", "RVT " + commandData.Application.Application.VersionNumber, "Dynamo");

                // Set Path Resolver's user data folder and common data folder with DynamoRevit runtime.
                var pathResolverParams = new TestPathResolverParams()
                {
                    UserDataRootFolder = userDataFolder,
                    CommonDataRootFolder = commonDataFolder
                };
                RevitTestPathResolver revitTestPathResolver = new RevitTestPathResolver(pathResolverParams);
                revitTestPathResolver.InitializePreloadedLibraries();

                // Get the preloaded DynamoRevit Custom Nodes, and Add them to Preload Libraries.
                var preloadedLibraries = new List<string>();
                GetLibrariesToPreload(preloadedLibraries);
                if (preloadedLibraries.Any())
                {
                    foreach (var preloadedLibrary in preloadedLibraries)
                    {
                        if (Directory.Exists(preloadedLibrary))
                            revitTestPathResolver.AddNodeDirectory(preloadedLibrary);
                        else if (File.Exists(preloadedLibrary))
                            revitTestPathResolver.AddPreloadLibraryPath(preloadedLibrary);
                    }
                }

                // Init DynamoTestPath to get DynamoSettings.xml which under user data folder
                PreferenceSettings.DynamoTestPath = string.Empty;
                //preload ASM and instruct dynamo to load that version of libG.
                var requestedLibGVersion = DynamoRevit.PreloadAsmFromRevit();

                DynamoRevit.RevitDynamoModel = RevitDynamoModel.Start(
                    new RevitDynamoModel.RevitStartConfiguration()
                    {
                        StartInTestMode = true,
                        GeometryFactoryPath = DynamoRevit.GetGeometryFactoryPath(testConfig.DynamoCorePath, requestedLibGVersion),
                        DynamoCorePath = testConfig.DynamoCorePath,
                        PathResolver = revitTestPathResolver,
                        Context = DynamoRevit.GetRevitContext(commandData),
                        SchedulerThread = new TestSchedulerThread(),
                        PackageManagerAddress = "https://www.dynamopackages.com",
                        ExternalCommandData = commandData,
                        ProcessMode = RevitTaskProcessMode
                    });

                Model = DynamoRevit.RevitDynamoModel;

                ViewModel = DynamoRevitViewModel.Start(
                    new DynamoViewModel.StartConfiguration()
                    {
                        DynamoModel = DynamoRevit.RevitDynamoModel,
                    });

                var vm3D = ViewModel.Watch3DViewModels.FirstOrDefault(vm => vm is RevitWatch3DViewModel);
                if (vm3D != null)
                {
                    vm3D.Active = false;
                }

                // Because the test framework does not work in the idle thread. 
                // We need to trick Dynamo into believing that it's in the idle
                // thread already.
                IdlePromise.InIdleThread = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        protected virtual TaskProcessMode RevitTaskProcessMode
        {
            get
            {
                return TaskProcessMode.Synchronous;
            }
        }

        protected override TestSessionConfiguration GetTestSessionConfiguration()
        {
            return new TestSessionConfiguration(Dynamo.Applications.DynamoRevitApp.DynamoCorePath);
        }

        protected void OpenSampleDefinition(string relativeFilePath)
        {
            string samplePath = Path.Combine(samplesPath, relativeFilePath);
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
        }

        protected IEnumerable<NodeModel> AllNodes
        {
            get
            {
                return this.Model.Workspaces.SelectMany(x => x.Nodes);
            }
        }

        private void CurrentUIApplication_ViewActivating(object sender, ViewActivatingEventArgs e)
        {
            ((RevitDynamoModel)ViewModel.Model).SetRunEnabledBasedOnContext(e.NewActiveView);
        }

        /// <summary>
        /// Opens and activates a new model.
        /// </summary>
        /// <param name="modelPath"></param>
        protected static UIDocument OpenAndActivateNewModel(string modelPath)
        {
            DocumentManager.Instance.CurrentUIApplication.OpenAndActivateDocument(modelPath);
            return DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument;
        }

        /// <summary>
        /// Opens and activates a new model, and closes the old model.
        /// </summary>
        protected static void SwapCurrentModel(string modelPath)
        {
            DocumentManager.Instance.CurrentUIApplication =
                RevitTestExecutive.CommandData.Application;
            DocumentManager.Instance.CurrentUIDocument =
                RevitTestExecutive.CommandData.Application.ActiveUIDocument;

            Document initialDoc = DocumentManager.Instance.CurrentUIApplication.ActiveUIDocument.Document;
            DocumentManager.Instance.CurrentUIApplication.OpenAndActivateDocument(modelPath);
            initialDoc.Close(false);
        }

        #endregion

        protected void MakeConnector(NodeModel start, NodeModel end, int portStart, int portEnd)
        {
            var cmdStart = new DynamoModel.MakeConnectionCommand(start.GUID, portStart, PortType.Output,
                DynamoModel.MakeConnectionCommand.Mode.Begin);
            this.Model.ExecuteCommand(cmdStart);

            var cmdend = new DynamoModel.MakeConnectionCommand(end.GUID, portEnd, PortType.Input,
                DynamoModel.MakeConnectionCommand.Mode.End);
            this.Model.ExecuteCommand(cmdend);
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
    }
}
