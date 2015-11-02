using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Core.Threading;
using DSCoreNodesUI.Input;
using NUnit.Framework;
using Revit.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using Dynamo.Nodes;

namespace RevitSystemTests
{
    /// <summary>
    /// Test cases in this fixture will be supposed to run in an asynchronous mode
    /// </summary>
    [TestFixture]
    public class AsynchronousModeTests : RevitSystemTestBase
    {
        /// <summary>
        /// Override the implementation from the base class so that the scheduler
        /// will run the tasks asyncoronously
        /// </summary>
        protected override TaskProcessMode RevitTaskProcessMode
        {
            get
            {
                return TaskProcessMode.Asynchronous;
            }
        }

        /// <summary>
        /// Run the model but will wait for the asynchronous tasks to be completed
        /// </summary>
        private void AsyncRunCurrentModel()
        {
            RunCurrentModel();

            // Run till all asyncronous tasks are completed
            var scheduler = ViewModel.Model.Scheduler;
            while (scheduler.ProcessNextTask(false)) { }
        }

        [Test]
        [Category("RegressionTests")]
        [TestModel(@".\empty.rvt")]
        public void MAGN_8187()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Async\MAGN_8187.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            AsyncRunCurrentModel();

            //ensure that the level count is 3
            var levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            levelColl.OfClass(typeof(Autodesk.Revit.DB.Level));
            Assert.AreEqual(levelColl.ToElements().Count(), 3);
        }
    }
}