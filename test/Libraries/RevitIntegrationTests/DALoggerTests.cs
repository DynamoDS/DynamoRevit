using System;
using System.IO;
using DADynamoApp;
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

        // TODO: Refactor DAEntrypoint so that all post-model-startup logic lives in a separate
        // internal DAApp class that accepts a DynamoModel in its constructor. DAEntrypoint would
        // just create the model and hand it to new DAApp(model). This test could then do:
        //   var app = new DAApp(ViewModel.Model);
        //   app.SetupProfilingHandlers();
        // ...which tests the actual DA code path without needing to create a DAEntrypoint at all,
        // and makes it straightforward to test graph running too (pass a work item folder).

        /// <summary>
        /// Verifies that DAEntrypoint.SetupProfilingHandlers wires up node execution events that
        /// call DALogger.SerializeNodeOutputs and write profiling output to stdout.
        /// </summary>
        [Test]
        [TestModel(@".\empty.rfa")]
        public void SetupProfilingHandlers_WritesNodeOutputToStdout()
        {
            // Use the actual production method — SetupProfilingHandlers subscribes to WorkspaceOpened
            // and wires all the profiling/serialization handlers on the model we already have.
            var entrypoint = new DAEntrypoint();
            entrypoint.SetupProfilingHandlers(Model);

            string dynPath = Path.GetFullPath(Path.Combine(workingDirectory, @".\Core\SanityCheck.dyn"));
            ViewModel.OpenCommand.Execute(dynPath);
            RunCurrentModel();

            var output = capturedOutput.ToString();

            // Profiling start/end lines must appear
            StringAssert.Contains("Node Point.ByCoordinates started execution.", output);
            StringAssert.Contains("Node Point.ByCoordinates finished execution.", output);

            // DALogger.SerializeNodeOutputs must have returned something for the Point node
            StringAssert.Contains("Node Point.ByCoordinates outputs:", output);
        }
    }
}
