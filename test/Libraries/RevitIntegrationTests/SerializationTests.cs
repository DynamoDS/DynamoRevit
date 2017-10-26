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
        private string JsonFolderName = "DynamoRevitJsons";

        [Test, TestModel(@".\empty.rfa")]
        public void SaveXMLTestFilesToJson()
        {
            // Get all old XML DYNs
            var di = new DirectoryInfo(workingDirectory);
            var fis = new string[] { "*.dyn", "*.dyf" }
            .SelectMany(i => di.GetFiles(i, SearchOption.AllDirectories))
            .ToArray();
            string[] XmlFilePaths = fis.Select(fi => fi.FullName).ToArray();

            // Create new Folder under temp to store Json
            var tempPath = Path.GetTempPath();
            var jsonFolder = Path.Combine(tempPath, JsonFolderName);

            if (!System.IO.Directory.Exists(jsonFolder))
            {
                System.IO.Directory.CreateDirectory(jsonFolder);
            }

            foreach (var filePath in XmlFilePaths)
            {
                // Open XMLs in new Dynamo Core which should take care of
                // Migration but element binding might be lost
                ViewModel.OpenCommand.Execute(filePath);

                // Get new file path under temp folder
                var fileNameSameStructure = filePath.Split(new string[] { "\\test" }, StringSplitOptions.None).Last();
                var jsonPath = jsonFolder + fileNameSameStructure;
                if (File.Exists(jsonPath))
                {
                    File.Delete(jsonPath);
                }

                // Re-save dyn, if folder does not exist, create it
                FileInfo fileInfo = new FileInfo(jsonPath);
                if (!fileInfo.Exists)
                    Directory.CreateDirectory(fileInfo.Directory.FullName);

                ViewModel.SaveAsCommand.Execute(jsonPath);
            }
        }
    }
}
