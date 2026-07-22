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
    /// <summary>
    /// Smoke tests for OOTB D4R sample scripts. Resolves .dyn files from
    /// DynamoForRevit\samples\{locale}\Revit\ via <see cref="ResolveSamplePath"/>.
    /// </summary>
    [TestFixture]
    class OOTB_D4R_SampleTests : RevitSystemTestBase
    {
        private static string ResolveSamplePath(string scriptFileName)
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Deployed layout: {parentDir}\samples\{locale}\Revit\ (not doc/distrib/Samples)
            string parentDir = Path.GetDirectoryName(assemblyDir);
            string samplesFolder = Path.Combine(parentDir, "samples");

            if (Directory.Exists(samplesFolder))
            {
                foreach (var locale in new[] { System.Globalization.CultureInfo.CurrentUICulture.Name, "en-US" }.Distinct())
                {
                    string localePath = Path.Combine(samplesFolder, locale, "Revit");
                    if (Directory.Exists(localePath))
                    {
                        var resolved = Path.Combine(localePath, scriptFileName);
                        if (File.Exists(resolved))
                            return resolved;
                    }
                }
            }

            var hint = Directory.Exists(samplesFolder)
                ? $"Contents:{string.Concat(Directory.EnumerateFileSystemEntries(samplesFolder).OrderBy(e => e).Select(e => $"\n  {e}"))}"
                : Directory.Exists(parentDir)
                    ? $"Parent dir contents:{string.Concat(Directory.EnumerateDirectories(parentDir).OrderBy(e => e).Select(e => $"\n  {e}"))}"
                    : string.Empty;

            throw new FileNotFoundException(
                $"Cannot locate OOTB D4R sample script '{scriptFileName}'.\n" +
                $"Samples folder: {samplesFolder}\n{hint}");
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
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Points()
        {
            OpenAndRunSample("Revit Geometry Creation Points.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Curves()
        {
            OpenAndRunSample("Revit Geometry Creation Curves.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void Revit_Geometry_Creation_Solids()
        {
            OpenAndRunSample("Revit Geometry Creation Solids.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void Revit_Geometry_Creation_Surfaces()
        {
            OpenAndRunSample("Revit Geometry Creation Surfaces.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void Revit_Adaptive_Component_Placement()
        {
            OpenAndRunSample("Revit Adaptive Component Placement.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\SampleModel.rvt")]
        public void Revit_Color()
        {
            OpenAndRunSample("Revit Color.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\SampleModel.rvt")]
        public void Revit_Floors_and_Framing()
        {
            OpenAndRunSample("Revit Floors and Framing.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\SampleModel.rvt")]
        public void Revit_Import_Solid()
        {
            OpenAndRunSample("Revit Import Solid.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\SampleModel.rvt")]
        public void Revit_Place_Families_By_Level_Set_Parameters()
        {
            OpenAndRunSample("Revit Place Families By Level Set Parameters.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\StructuralFraming\StructuralFraming.rvt")]
        public void Revit_Structural_Framing()
        {
            OpenAndRunSample("Revit Structural Framing.dyn");
        }
    }
}
