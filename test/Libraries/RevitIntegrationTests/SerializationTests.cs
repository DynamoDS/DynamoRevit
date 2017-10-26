using System.IO;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using Revit.Elements;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using System;

namespace RevitSystemTests
{
    [TestFixture]
    class SerializationTests : RevitSystemTestBase
    {
        /// <summary>
        /// This function gathers all dyn and dyf in 
        /// workingDirectory into paths array.
        /// </summary>
        /// <returns>Test files paths array</returns>
        public object[] FindWorkspaces()
        {
            var config = RevitTestConfiguration.LoadConfiguration();

            //get the test path
            workingDirectory = config.WorkingDirectory;
            var di = new DirectoryInfo(workingDirectory);
            var fis = new string[] { "*.dyn", "*.dyf" }
            .SelectMany(i => di.GetFiles(i, SearchOption.AllDirectories))
            .ToArray();

            //di.GetFiles("*.dyn, *.dyf", SearchOption.AllDirectories);
            return fis.Select(fi => fi.FullName).ToArray();
        }

        protected string JsonFolderName = "DynamoRevitJsons";

        [Test, TestModel(@".\empty.rfa"), TestCaseSource("FindWorkspaces")]
        public void SaveXMLTestFilesToJson(string filePath)
        {
            ViewModel.OpenCommand.Execute(filePath);
            AssertNoDummyNodes();

            bool isCustomNodeWorkspace = filePath.Contains(".dyf");

            var tempPath = Path.GetTempPath();
            var jsonFolder = Path.Combine(tempPath, JsonFolderName);

            if (!System.IO.Directory.Exists(jsonFolder))
            {
                System.IO.Directory.CreateDirectory(jsonFolder);
            }

            // Get file name
            var fileNameSameStructure = filePath.Split(new string[] { "\\test" }, StringSplitOptions.None).Last();
            var jsonPath = isCustomNodeWorkspace? jsonFolder + fileNameSameStructure + ".dyf" : jsonFolder + fileNameSameStructure + ".dyn";
            if (File.Exists(jsonPath))
            {
                File.Delete(jsonPath);
            }
            ViewModel.SaveAsCommand.Execute(jsonPath);
        }
    }
}
