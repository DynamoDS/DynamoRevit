using System.IO;
using DADynamoApp;
using Dynamo.Graph.Workspaces;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class DALoggerTests : RevitSystemTestBase
    {
        private StringWriter capturedOutput;
        private TextWriter originalOutput;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            capturedOutput = new StringWriter();
            originalOutput = Console.Out;
            Console.SetOut(capturedOutput);
        }

        [TearDown]
        public override void TearDown()
        {
            Console.SetOut(originalOutput);
            capturedOutput?.Dispose();
            base.TearDown();
        }

        /// <summary>
        /// Verifies that when profiling handlers are set up (mirroring the pattern in DAEntrypoint.SetupProfilingHandlers),
        /// node execution begin and end events are logged to stdout via DALogger.
        /// </summary>
        [Test]
        [TestModel(@".\empty.rfa")]
        public void NodeExecutionEventsAreLoggedToStdout()
        {
            var daLogger = new DALogger(TempFolder);

            // Mirror the WorkspaceOpened subscription from DAEntrypoint.SetupProfilingHandlers
            Model.WorkspaceOpened += ws =>
            {
                if (ws is not HomeWorkspaceModel homeWorkspace) return;

                homeWorkspace.EvaluationStarted += (sender, args) =>
                {
                    homeWorkspace.EngineController.EnableProfiling(true, homeWorkspace, homeWorkspace.Nodes);
                };

                foreach (var node in homeWorkspace.Nodes)
                {
                    node.NodeExecutionBegin += nm => daLogger.Log($"Node {nm.Name} started execution.");
                    node.NodeExecutionEnd += nm => daLogger.Log($"Node {nm.Name} finished execution.");
                }
            };

            string dynPath = Path.GetFullPath(Path.Combine(workingDirectory, @".\Core\SanityCheck.dyn"));
            ViewModel.OpenCommand.Execute(dynPath);
            RunCurrentModel();

            var output = capturedOutput.ToString();
            StringAssert.Contains("Point.ByCoordinates started execution.", output);
            StringAssert.Contains("Point.ByCoordinates finished execution.", output);
        }
    }
}
