using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Autodesk.DesignScript.Interfaces;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.UI;
using Dynamo.Wpf.Rendering;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using TestServices;

namespace RevitSystemTests
{
    [TestFixture]
    class DisplayTests : RevitSystemTestBase
    {
        protected override void StartDynamo(TestSessionConfiguration testConfig)
        {
            base.StartDynamo(testConfig);

            //create the view
            View = new DynamoView(ViewModel);
            View.Show();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [Test, TestModel(@".\Display\ModelCurveColor.rfa")]
        public void Display_ModelCurveDrawnInBackground_HasCorrectColor()
        {
            var samplePath = Path.Combine(workingDirectory, @".\Display\ModelCurveColor.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            foreach (var n in Model.CurrentWorkspace.Nodes)
            {
                n.RenderPackagesUpdated += NodeRenderPackagesUpdated;
            }

            RunCurrentModel();
        }

        void NodeRenderPackagesUpdated(Guid guid, IEnumerable<IRenderPackage> packages)
        {
            var curvePackage = packages.FirstOrDefault(p => p.LineVertexCount > 0);
            Assert.NotNull(curvePackage);

            var expectedColor = (Color)SharedDictionaryManager.DynamoColorsAndBrushesDictionary["EdgeColor"];
            Assert.True(curvePackage.AllLineStripVerticesHaveColor(expectedColor));
        }
    }
}
