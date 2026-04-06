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
        /// <summary>
        /// Locates an OOTB sample .dyn file by checking the standard deployment locations
        /// used for the revit-d4r-content-samples artifact.
        /// </summary>
        /// <param name="scriptFileName">
        /// The filename of the .dyn script as shipped, e.g. "Revit Color.dyn".
        /// </param>
        /// <returns>Absolute path to the .dyn file.</returns>
        public static string SetupUnzip(string scriptFileName)
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
                return Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
            }

            // Priority 2: zip file is present — extract on first use
            if (Directory.Exists(samplesFolder))
            {
                var zipFiles = Directory.GetFiles(
                    samplesFolder,
                    "revit-d4r-content-samples-*-net10.zip");

                if (zipFiles.Length > 0)
                {
                    ZipFile.ExtractToDirectory(zipFiles[0], extractedSamplesPath);
                    return Path.Combine(extractedSamplesPath, @"Samples\en-US\Revit", scriptFileName);
                }
            }

            // Priority 3: already-deployed samples (standard Revit install)
            if (Directory.Exists(installedSamplesPath))
            {
                return Path.Combine(installedSamplesPath, scriptFileName);
            }

            // Provide a useful diagnostic message to help locate the issue
            if (Directory.Exists(samplesFolder))
            {
                var entries = Directory.EnumerateFileSystemEntries(samplesFolder)
                    .Select(e => $"\n  {e}");
                throw new DirectoryNotFoundException(
                    $"Cannot locate OOTB D4R sample scripts.\n" +
                    $"Checked samples folder: {samplesFolder}\n" +
                    $"Contents:{string.Concat(entries)}");
            }
            else
            {
                var parentEntries = Directory.Exists(parentDir)
                    ? Directory.EnumerateDirectories(parentDir).Select(e => $"\n  {e}")
                    : Enumerable.Empty<string>();
                throw new DirectoryNotFoundException(
                    $"Cannot locate OOTB D4R sample scripts.\n" +
                    $"Expected samples folder not found: {samplesFolder}\n" +
                    $"Parent dir contents:{string.Concat(parentEntries)}");
            }
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Points()
        {
            string samplePath = SetupUnzip("Revit Geometry Creation Points.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Curves()
        {
            string samplePath = SetupUnzip("Revit Geometry Creation Curves.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Solids()
        {
            string samplePath = SetupUnzip("Revit Geometry Creation Solids.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\empty.rfa")]
        public void Revit_Geometry_Creation_Surfaces()
        {
            string samplePath = SetupUnzip("Revit Geometry Creation Surfaces.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Adaptive_Component_Placement()
        {
            string samplePath = SetupUnzip("Revit Adaptive Component Placement.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Color()
        {
            string samplePath = SetupUnzip("Revit Color.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Floors_and_Framing()
        {
            string samplePath = SetupUnzip("Revit Floors and Framing.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Import_Solid()
        {
            string samplePath = SetupUnzip("Revit Import Solid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Place_Families_By_Level_Set_Parameters()
        {
            string samplePath = SetupUnzip("Revit Place Families By Level Set Parameters.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }

        [Test, Category("SmokeTests")]
        [TestModel(@".\Samples\DynamoSample_2021.rvt")]
        public void Revit_Structural_Framing()
        {
            string samplePath = SetupUnzip("Revit Structural Framing.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();
        }
    }
}
