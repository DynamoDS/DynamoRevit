using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    /// <summary>
    /// Tests for the Out-of-the-Box (OOTB) D4R sample scripts shipped as part of the
    /// revit-d4r-content-samples artifact.
    ///
    /// The .dyn files are resolved at runtime via <see cref="SetupUnzip"/> which checks,
    /// in order:
    ///   1. An already-extracted D4RSamples folder next to the installed samples directory.
    ///   2. A revit-d4r-content-samples-*-net10.zip in the installed samples directory.
    ///   3. The already-deployed samples at DynamoForRevit\samples\en-US\Revit\.
    /// </summary>
    [TestFixture]
    class OOTB_D4R_Tests : RevitSystemTestBase
    {
        private static string SetupUnzip(string scriptFileName)
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // When deployed to Revit:
            //   assemblyDir = DynamoForRevit\Revit\
            //   parentDir   = DynamoForRevit\
            string parentDir = Path.GetDirectoryName(assemblyDir);

            string samplesFolder = Path.Combine(parentDir, "samples");
            string extractedSamplesPath = Path.Combine(samplesFolder, "D4RSamples");
            string installedSamplesPath = Path.Combine(samplesFolder, @"en-US\Revit");

            // Priority 1: zip was already extracted from a previous test run
            if (Directory.Exists(extractedSamplesPath))
            {
                var resolved = Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
                if (File.Exists(resolved))
                    return resolved;
            }

            // Priority 2: zip file is present — extract on first use
            if (Directory.Exists(samplesFolder))
            {
                var zipFiles = Directory.GetFiles(
                        samplesFolder,
                        "revit-d4r-content-samples-*-net10.zip")
                    .OrderBy(path => path)
                    .ToArray();

                if (zipFiles.Length > 1)
                {
                    var matches = zipFiles.Select(path => $"\n  {path}");
                    throw new InvalidOperationException(
                        $"Multiple revit-d4r-content-samples archives were found in '{samplesFolder}', " +
                        $"so the OOTB D4R sample source is ambiguous. Ensure exactly one matching zip is present." +
                        $"{string.Concat(matches)}");
                }

                if (zipFiles.Length == 1)
                {
                    ZipFile.ExtractToDirectory(zipFiles[0], extractedSamplesPath);
                    var resolved = Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
                    if (File.Exists(resolved))
                        return resolved;
                }
            }

            // Priority 3: already-deployed samples (standard Revit install)
            if (Directory.Exists(installedSamplesPath))
            {
                var resolved = Path.Combine(installedSamplesPath, scriptFileName);
                if (File.Exists(resolved))
                    return resolved;
            }

            // Provide a useful diagnostic message to help locate the issue
            if (Directory.Exists(samplesFolder))
            {
                var entries = Directory.EnumerateFileSystemEntries(samplesFolder)
                    .Select(e => $"\n  {e}");
                throw new FileNotFoundException(
                    $"Cannot locate OOTB D4R sample script '{scriptFileName}'.\n" +
                    $"Checked samples folder: {samplesFolder}\n" +
                    $"Contents:{string.Concat(entries)}");
            }
            else
            {
                var parentEntries = Directory.Exists(parentDir)
                    ? Directory.EnumerateDirectories(parentDir).Select(e => $"\n  {e}")
                    : Enumerable.Empty<string>();
                throw new FileNotFoundException(
                    $"Cannot locate OOTB D4R sample script '{scriptFileName}'.\n" +
                    $"Expected samples folder not found: {samplesFolder}\n" +
                    $"Parent dir contents:{string.Concat(parentEntries)}");
            }
        }

        private void OpenAndRunSample(string scriptFileName)
        {
            string samplePath = SetupUnzip(scriptFileName);
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
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
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Adaptive_Component_Placement()
        {
            OpenAndRunSample("Revit Adaptive Component Placement.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Color()
        {
            OpenAndRunSample("Revit Color.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Floors_and_Framing()
        {
            OpenAndRunSample("Revit Floors and Framing.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Import_Solid()
        {
            OpenAndRunSample("Revit Import Solid.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Place_Families_By_Level_Set_Parameters()
        {
            OpenAndRunSample("Revit Place Families By Level Set Parameters.dyn");
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Structural_Framing()
        {
            OpenAndRunSample("Revit Structural Framing.dyn");
        }
    }
}
