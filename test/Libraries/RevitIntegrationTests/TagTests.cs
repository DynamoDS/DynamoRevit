using System;
using System.IO;
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
    class TagTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void PlaceTag()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Tag.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var tagelement = GetPreviewValue("73d7876d-c04a-418c-8f86-2e36c44d9833");

            Assert.AreEqual(typeof(Revit.Elements.Tag), tagelement.GetType());

        }



    }
}