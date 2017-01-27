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
    class CurtainSystemTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\InPlaceMass.rvt")]
        public void CreateCurtainSystem()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\CurtainSystem\CurtainSystem.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var system = GetPreviewValue("4d37de22-e10a-4c55-a3a7-5275c3d06f0e");
            Assert.IsNotNull(system);
            Assert.IsTrue(typeof(Revit.Elements.CurtainSystem) == system.GetType());
        }




    }
}