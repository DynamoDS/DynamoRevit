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
    class GlobalParameterTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void CreateGlobalParameterAndTestProperties()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Parameter\GlobalParameter.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(GetPreviewValue("97d5b4ba-76e4-40e9-b1b2-8ce98e642a40"), "MyGlobalParameter");
            Assert.AreEqual(GetPreviewValue("04ccd2fc-45bb-4953-9306-b7eb21d991a0"), "Text");
            Assert.AreEqual(GetPreviewValue("bb83a545-0c80-4f34-b987-da1d3d0dc2b7"), true);
        }




    }
}