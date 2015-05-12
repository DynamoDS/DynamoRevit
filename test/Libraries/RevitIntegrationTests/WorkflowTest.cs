using System.IO;
using System.Linq;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using Dynamo.Models;
using RTF.Framework;
using Autodesk.DesignScript.Geometry;
using RevitServices.Persistence;
using System.Collections.Generic;
using RevitServices.Transactions;

namespace RevitSystemTests
{
    [TestFixture]
    public class WorkflowTest : RevitSystemTestBase
    {

        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-02.rvt")]
        public void Test_Vignette_02_Adaptive_Family()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-02-Adaptive-Family.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(13, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(15, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoints
            var adaptiveComp = "700fd421-636c-4cd9-8604-36f027f045ee";
            var adaptiveCompVlaue = GetPreviewValue(adaptiveComp) as AdaptiveComponent;
            Assert.IsNotNull(adaptiveComp);

            //check Flatten
            var flattenId = "14719205-f9d8-40a0-9a04-2760c5c816be";
            AssertPreviewCount(flattenId, 8);
            for (int i = 0; i < 8; i++)
            {
                var element = GetPreviewValueAtIndex(flattenId, i) as Point;
                Assert.AreEqual(System.Math.Abs(element.X), 50);
                Assert.AreEqual(System.Math.Abs(element.Y), 100);
                if (i >= 4)
                {
                    Assert.AreEqual(System.Math.Abs(element.Z), 80);
                }
                else
                {
                    Assert.AreEqual(System.Math.Abs(element.Z), 0);
                }
            }
        }


        [Test]
        [TestModel(@".\Workflow\Vignette\Vignette-02.rvt")]
        public void Test_Vignette_02_Adaptive_Family_Mult()
        {
            // Create automation for Dynamo files running in Dynamo Revit
            // http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-7346
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Vignette\Vignette-02-Adaptive-Families-Mult.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            var model = ViewModel.Model;
            Assert.AreEqual(23, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(27, model.CurrentWorkspace.Connectors.Count());

            //check AdaptiveComponent.ByPoints
            var adaptiveComp = "700fd421-636c-4cd9-8604-36f027f045ee";
            AssertPreviewCount(adaptiveComp, 9);
            for (int i = 0; i < 9; i++)
            {
                var element = GetPreviewValueAtIndex(adaptiveComp, i) as AdaptiveComponent;
                Assert.IsNotNull(element);
            }


        }
    }
}