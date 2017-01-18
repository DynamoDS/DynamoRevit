using System;
using System.IO;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using CoreNodeModels.Input;
using Dynamo.Nodes;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;


namespace RevitSystemTests
{
    [TestFixture]
    class PerformanceAdviserTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void GetPerformanceAdviserRules()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\\PerformanceAdviser\\PerformanceAdviser.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            object rule = GetPreviewValue("4afe4fdd-4663-4bc1-923c-48210fff043f");
            Assert.IsTrue(typeof(Revit.Elements.PerformanceAdviserRule) == rule.GetType());
            Revit.Elements.PerformanceAdviserRule prule = rule as Revit.Elements.PerformanceAdviserRule;

            object name = GetPreviewValue("0d24a270-9640-4cda-b6fe-8850be7ba0d8");
            Assert.AreEqual(name, prule.Name);

            object desc = GetPreviewValue("6dfe67fb-cbad-40c0-be17-35b94ddba6aa");
            Assert.AreEqual(desc, prule.Description);

            object enabled = GetPreviewValue("5e619344-900e-4bc9-931a-52a85cd270d2");
            Assert.AreEqual(enabled, prule.Enabled);


        }





    }
}