using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class AppearanceAssetElementTests : RevitSystemTestBase
    {
        [Test, TestModel(@".\Empty.rvt")]
        public void GetSetTextureImage()
        {
            string samplePath = Path.Combine(workingDirectory, @".\AppearanceAssetElement\GetSetTextureImage.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            string expectedFileName = "TextureImageTest.txt";

            var filePath = GetPreviewValue("6808334fdedf4e9984d8a57a0675fb0f") as string;
            string fileName = Path.GetFileName(filePath);

            Assert.AreEqual(expectedFileName, fileName);
        }
    }
}
