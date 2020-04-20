﻿using System.IO;
using System.Linq;

using Autodesk.Revit.DB;

using NUnit.Framework;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class RayBounceTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\RayBounce\RayBounce.rvt")]
        public void RayBounce()
        {
            var samplePath = Path.Combine(workingDirectory, @".\RayBounce\RayBounce.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            
            RunCurrentModel();

            //ensure that the bounce curve count is the same
            var curveColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document, DocumentManager.Instance.CurrentUIDocument.ActiveView.Id);
            curveColl.OfClass(typeof(CurveElement));
            Assert.AreEqual(curveColl.ToElements().Count(), 33);
        }

        [Test, Category("IntegrationTests")]
        [TestModel(@".\RayBounce\SunStudy.rvt")]
        public void RayBounce_SunStudy()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\RayBounce\Raybounce_Template_cleaned.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(24, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(26, model.CurrentWorkspace.Connectors.Count());

            RunCurrentModel();

            // Validation for Reference Points.
            var modelCurve = "64b62b8e-a07e-477e-ba5d-9e33eb03debf";
            AssertPreviewCount(modelCurve, 45);

            for (int i = 0; i <= 42; i++)
            {
                var curve = GetPreviewValueAtIndex(modelCurve, i) as Revit.Elements.ModelCurve;
                Assert.IsNotNull(curve);
            }
        }

    }
}
