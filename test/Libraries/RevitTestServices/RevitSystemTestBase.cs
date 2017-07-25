using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SystemTestServices;
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
using TestServices;

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
    public class RevitSystemTestBase : SystemTestBase
    {
        private string samplesPath;
        protected string emptyModelPath1;
        protected string emptyModelPath;

        #region public methods

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
                var revitTestPathResolver = new RevitTestPathResolver();
                revitTestPathResolver.InitializePreloadedLibraries();

                DynamoRevit.RevitDynamoModel = RevitDynamoModel.Start(
                    new RevitDynamoModel.RevitStartConfiguration()
                    {
                        StartInTestMode = true,
                        GeometryFactoryPath = DynamoRevit.GetGeometryFactoryPath(testConfig.DynamoCorePath),
                        DynamoCorePath = testConfig.DynamoCorePath,
                        PathResolver = revitTestPathResolver,
                        Context = "Revit 2014",
                        SchedulerThread = new TestSchedulerThread(),
                        PackageManagerAddress = "https://www.dynamopackages.com",
                        ExternalCommandData = new DynamoRevitCommandData(RevitTestExecutive.CommandData),
                        ProcessMode = RevitTaskProcessMode
                    });

                Model = DynamoRevit.RevitDynamoModel;

                this.ViewModel = DynamoRevitViewModel.Start(
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
            ((RevitDynamoModel)this.ViewModel.Model).SetRunEnabledBasedOnContext(e.NewActiveView);
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

        /// <summary>
        /// This function gets all the family instances in the current Revit document
        /// </summary>
        /// <param name="startNewTransaction">whether do the filtering in a new transaction</param>
        /// <returns>the family instances</returns>
        protected static IList<Element> GetAllFamilyInstances(bool startNewTransaction)
        {
            if (startNewTransaction)
            {
                using (var trans = new Transaction(DocumentManager.Instance.CurrentUIDocument.Document, "FilteringElements"))
                {
                    trans.Start();

                    ElementClassFilter ef = new ElementClassFilter(typeof(FamilyInstance));
                    FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                    fec.WherePasses(ef);

                    trans.Commit();
                    return fec.ToElements();
                }
            }
            else
            {
                ElementClassFilter ef = new ElementClassFilter(typeof(FamilyInstance));
                FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
                fec.WherePasses(ef);
                return fec.ToElements();
            }
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

        protected static IList<Wall> GetAllWalls()
        {
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Wall));
            return fec.ToElements().Cast<Wall>().ToList();
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
