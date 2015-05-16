using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using Autodesk.DesignScript.Interfaces;
using Dynamo;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.UI;
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
            RunCurrentModel();

            var packages = AllNodes.SelectMany<NodeModel,IRenderPackage>(n => n.RenderPackages);
            
            var curvePackage = packages.FirstOrDefault(p => p.LineVertexCount > 0);
            Assert.NotNull(curvePackage);

            var expectedColor = (Color)SharedDictionaryManager.DynamoColorsAndBrushesDictionary["EdgeColor"];
            Assert.True(curvePackage.AllLineStripVerticesHaveColor(expectedColor));
        }
    }

    internal static class RenderPackageExtensions
    {
        public static bool AllLineStripVerticesHaveColor(this IRenderPackage package, Color color)
        {
            if (!package.LineStripVertices.Any())
            {
                return false;
            }

            for (var i = 0; i < package.LineStripVertexColors.Count(); i += 4)
            {
                if (color.R != package.LineStripVertexColors.ElementAt(i) ||
                    color.G != package.LineStripVertexColors.ElementAt(i+1) ||
                    color.B != package.LineStripVertexColors.ElementAt(i+2) ||
                    color.A != package.LineStripVertexColors.ElementAt(i+3))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
