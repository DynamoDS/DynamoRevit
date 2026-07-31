using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Dynamo.Graph.Nodes;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class OOTB_D4R_SampleTests : RevitSystemTestBase
    {
        private static string ResolveSamplePath(string scriptFileName)
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string parentDir = Path.GetDirectoryName(assemblyDir);
            string resolved = Path.Combine(parentDir, "samples", "en-US", "Revit", scriptFileName);
            if (File.Exists(resolved))
                return resolved;

            throw new FileNotFoundException(
                $"Cannot locate OOTB D4R sample script '{scriptFileName}' under {Path.Combine(parentDir, "samples")}.");
        }

        private void OpenAndRunSample(string scriptFileName)
        {
            string testPath = Path.GetFullPath(ResolveSamplePath(scriptFileName));

            ViewModel.OpenCommand.Execute(testPath);

            Assert.IsTrue(
                ViewModel.Model.CurrentWorkspace.Nodes.Any(),
                $"Graph '{scriptFileName}' opened but contains no nodes — file may not have loaded correctly.");

            AssertNoDummyNodes();

            var brokenConnectors = ViewModel.Model.CurrentWorkspace.Connectors
                .Where(c => c.Start == null || c.End == null).ToList();
            Assert.IsFalse(brokenConnectors.Any(),
                $"Graph '{scriptFileName}' has {brokenConnectors.Count} broken connector(s).");

            RunCurrentModel();

            var errorNodes = ViewModel.Model.CurrentWorkspace.Nodes
                .Where(n => n.State == ElementState.Error || n.State == ElementState.Warning).ToList();
            if (errorNodes.Any())
            {
                var first = errorNodes[0];
                var msg = first.NodeInfos
                    .FirstOrDefault(i => i.State == ElementState.Error || i.State == ElementState.Warning)?.Message;
                Assert.Fail(
                    $"After RunCurrentModel(), {errorNodes.Count} node(s) in error/warning in '{scriptFileName}'. " +
                    $"First: [{first.State}] {first.Name}" + (msg != null ? $": {msg}" : string.Empty));
            }
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void Revit_Geometry_Creation_Points()
        {
            OpenAndRunSample("Revit Geometry Creation Points.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void Revit_Geometry_Creation_Curves()
        {
            OpenAndRunSample("Revit Geometry Creation Curves.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Solids()
        {
            OpenAndRunSample("Revit Geometry Creation Solids.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Surfaces()
        {
            OpenAndRunSample("Revit Geometry Creation Surfaces.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\DynamoSample_2021.rvt")]
        public void OOTB_Revit_Adaptive_Component_Placement()
        {
            OpenAndRunSample("Revit Adaptive Component Placement.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void OOTB_Revit_Color()
        {
            OpenAndRunSample("Revit Color.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void OOTB_Revit_Floors_and_Framing()
        {
            OpenAndRunSample("Revit Floors and Framing.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void Revit_Import_Solid()
        {
            OpenAndRunSample("Revit Import Solid.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void Revit_Place_Families_By_Level_Set_Parameters()
        {
            OpenAndRunSample("Revit Place Families By Level Set Parameters.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Snowdon Towers Sample Architectural.rvt")]
        public void Revit_Structural_Framing()
        {
            OpenAndRunSample("Revit Structural Framing.dyn");
        }
    }
}
