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
using Dynamo.Core.Threading;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.ViewModels;

using DynamoUtilities;

using NUnit.Framework;

using RevitServices.Persistence;
using RevitServices.Threading;
using RevitServices.Transactions;

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
        /// Directory where custom node definitions are kept
        /// </summary>
        public string DefinitionsPath { get; set; }

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

            //set the custom node loader search path
            if (string.IsNullOrEmpty(DefinitionsPath))
            {
                string defsLoc = Path.Combine(
                    DynamoPathManager.Instance.Packages,
                    "Dynamo Sample Custom Nodes",
                    "dyf");
                DefinitionsPath = Path.GetFullPath(defsLoc);
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
        private string defsPath;
        protected string emptyModelPath1;
        protected string emptyModelPath;

        #region public methods

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            ((HomeWorkspaceModel)ViewModel.Model.CurrentWorkspace).RunSettings.RunType = RunType.Manually;

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
        }

        #endregion

        #region protected methods

        protected override void SetupCore()
        {
            DocumentManager.Instance.CurrentUIApplication =
                RTF.Applications.RevitTestExecutive.CommandData.Application;
            DocumentManager.Instance.CurrentUIDocument =
                RTF.Applications.RevitTestExecutive.CommandData.Application.ActiveUIDocument;

            var config = RevitTestConfiguration.LoadConfiguration();

            //get the test path
            workingDirectory = config.WorkingDirectory;

            //get the samples path
            samplesPath = config.SamplesPath;

            //set the custom node loader search path
            defsPath = config.DefinitionsPath;

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

        protected override void StartDynamo()
        {
            try
            {
                // create the transaction manager object
                TransactionManager.SetupManager(new AutomaticTransactionStrategy());

                DynamoRevit.RevitDynamoModel = RevitDynamoModel.Start(
                    new DynamoModel.StartConfiguration()
                    {
                        StartInTestMode = true,
                        DynamoCorePath = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\"),
                        Context = "Revit 2014",
                        SchedulerThread = new TestSchedulerThread()
                    });

                Model = DynamoRevit.RevitDynamoModel;

                this.ViewModel = DynamoViewModel.Start(
                    new DynamoViewModel.StartConfiguration()
                    {
                        DynamoModel = DynamoRevit.RevitDynamoModel,
                    });

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
                RTF.Applications.RevitTestExecutive.CommandData.Application;
            DocumentManager.Instance.CurrentUIDocument =
                RTF.Applications.RevitTestExecutive.CommandData.Application.ActiveUIDocument;

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
    }
}
