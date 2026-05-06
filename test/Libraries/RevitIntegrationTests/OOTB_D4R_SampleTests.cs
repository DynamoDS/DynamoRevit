using System;
using System.IO;
// using System.IO.Compression;  // Needed when zip extraction (priorities 2/3) is enabled
using System.Linq;
using System.Reflection;
using Dynamo.Graph.Nodes;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    /// <summary>
    /// Tests for the Out-of-the-Box (OOTB) D4R sample scripts shipped as part of the
    /// revit-d4r-content-samples artifact.
    ///
    /// The .dyn files are resolved at runtime via <see cref="ResolveSamplePath"/> which checks
    /// the already-deployed samples at DynamoForRevit\samples\{locale}\Revit\.
    ///
    /// Future consideration: zip-based extraction is available as commented code below
    /// (priorities 2 and 3) for scenarios where the samples are shipped as a zip artifact.
    /// </summary>
    [TestFixture]
    class OOTB_D4R_SampleTests : RevitSystemTestBase
    {
        private static string ResolveSamplePath(string scriptFileName)
        {
            // Assembly.GetExecutingAssembly().Location is the standard pattern used throughout
            // the test framework (RevitSystemTestBase, SystemTest, CoreTests, RegressionTests).
            // RTF always loads test assemblies from the deployed DynamoForRevit\Revit\ folder.
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // NOTE: We intentionally do NOT use the base class SamplesPath (which points to
            // doc/distrib/Samples/ for Dynamo core samples). The D4R OOTB samples are deployed
            // at DynamoForRevit\samples\{locale}\Revit\ alongside the plugin.
            //
            // When deployed to Revit (via RTF):
            //   assemblyDir = DynamoForRevit\Revit\
            //   parentDir   = DynamoForRevit\
            string parentDir = Path.GetDirectoryName(assemblyDir);

            string samplesFolder = Path.Combine(parentDir, "samples");

            // Priority 1: already-deployed samples — probe current culture then en-US fallback
            if (Directory.Exists(samplesFolder))
            {
                var localesToProbe = new[]
                {
                    System.Globalization.CultureInfo.CurrentUICulture.Name, // e.g. "fr-FR"
                    "en-US"
                }.Distinct();

                foreach (var locale in localesToProbe)
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

            #region Future: zip-based extraction (priorities 2 and 3)
            // // Priority 2: zip file — use version-keyed cache in temp to avoid staleness
            // if (Directory.Exists(samplesFolder))
            // {
            //     var zipFiles = Directory.GetFiles(
            //             samplesFolder,
            //             "revit-d4r-content-samples-*-net10.zip")
            //         .OrderBy(path => path)
            //         .ToArray();
            //
            //     if (zipFiles.Length > 1)
            //     {
            //         var matches = zipFiles.Select(path => $"\n  {path}");
            //         throw new InvalidOperationException(
            //             $"Multiple revit-d4r-content-samples archives were found in '{samplesFolder}', " +
            //             $"so the OOTB D4R sample source is ambiguous. Ensure exactly one matching zip is present." +
            //             $"{string.Concat(matches)}");
            //     }
            //
            //     if (zipFiles.Length == 1)
            //     {
            //         // Derive cache folder name from zip filename to invalidate on version change
            //         string zipName = Path.GetFileNameWithoutExtension(zipFiles[0]);
            //         string extractedSamplesPath = Path.Combine(Path.GetTempPath(), "D4RSamples", zipName);
            //
            //         // If already extracted for this version, use cached
            //         var resolved = Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
            //         if (File.Exists(resolved))
            //             return resolved;
            //
            //         // Extract to staging dir, then move into place to avoid partial cache
            //         string stagingPath = extractedSamplesPath + "_staging_" + Guid.NewGuid().ToString("N")[..8];
            //         try
            //         {
            //             ZipFile.ExtractToDirectory(zipFiles[0], stagingPath);
            //             if (Directory.Exists(extractedSamplesPath))
            //                 Directory.Delete(extractedSamplesPath, recursive: true);
            //             Directory.Move(stagingPath, extractedSamplesPath);
            //         }
            //         catch
            //         {
            //             // Clean up staging on failure so next run can retry
            //             if (Directory.Exists(stagingPath))
            //                 Directory.Delete(stagingPath, recursive: true);
            //             throw;
            //         }
            //
            //         resolved = Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
            //         if (File.Exists(resolved))
            //             return resolved;
            //     }
            // }
            //
            // // Priority 3: reuse any previously cached extraction (zip may have been removed)
            // string cacheRoot = Path.Combine(Path.GetTempPath(), "D4RSamples");
            // if (Directory.Exists(cacheRoot))
            // {
            //     var cached = Directory.GetDirectories(cacheRoot)
            //         .OrderByDescending(d => Directory.GetLastWriteTime(d))
            //         .Select(d => Path.Combine(d, @"Samples\en-US\Revit", scriptFileName))
            //         .FirstOrDefault(File.Exists);
            //     if (cached != null)
            //         return cached;
            // }
            #endregion

            // Provide a useful diagnostic message to help locate the issue
            if (Directory.Exists(samplesFolder))
            {
                var entries = Directory.EnumerateFileSystemEntries(samplesFolder)
                    .OrderBy(e => e)
                    .Select(e => $"\n  {e}");
                throw new FileNotFoundException(
                    $"Cannot locate OOTB D4R sample script '{scriptFileName}'.\n" +
                    $"Checked samples folder: {samplesFolder}\n" +
                    $"Contents:{string.Concat(entries)}");
            }
            else
            {
                var parentEntries = Directory.Exists(parentDir)
                    ? Directory.EnumerateDirectories(parentDir).OrderBy(e => e).Select(e => $"\n  {e}")
                    : Enumerable.Empty<string>();
                throw new FileNotFoundException(
                    $"Cannot locate OOTB D4R sample script '{scriptFileName}'.\n" +
                    $"Expected samples folder not found: {samplesFolder}\n" +
                    $"Parent dir contents:{string.Concat(parentEntries)}");
            }
        }

        private void OpenAndRunSample(string scriptFileName)
        {
            string samplePath = ResolveSamplePath(scriptFileName);
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            Assert.IsTrue(
                ViewModel.Model.CurrentWorkspace.Nodes.Any(),
                $"Graph '{scriptFileName}' opened but contains no nodes — file may not have loaded correctly.");

            AssertNoDummyNodes();

            RunCurrentModel();

            var errorNodes = ViewModel.Model.CurrentWorkspace.Nodes.Where(
                n => n.State == ElementState.Error || n.State == ElementState.Warning).ToList();

            if (errorNodes.Any())
            {
                var details = string.Join("\n",
                    errorNodes.Select(n => $"  [{n.State}] {n.Name} ({n.GUID})"));
                Assert.Fail(
                    $"After RunCurrentModel(), {errorNodes.Count} node(s) are in error/warning state " +
                    $"in '{scriptFileName}':\n{details}");
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
