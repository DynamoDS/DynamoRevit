using System;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;

using Dynamo.Nodes;

using NUnit.Framework;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class DividedSurfaceTests : RevitSystemTestBase
    {
        [Test, Category("Failure")]
        [TestModel(@".\DividedSurface\DividedSurface.rfa")]
        public void DividedSurface()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DividedSurface\DividedSurface.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            ViewModel.OpenCommand.Execute(testPath);
            
            RunCurrentModel();
            //ViewModel.Model.RunExpression();

            //check DividedSurface
            var dividedSurfaceID = "729a8380-11f0-4700-80df-14d70accc5ba";
            var dividedSurface = GetPreviewValueAtIndex(dividedSurfaceID, 0) as Revit.Elements.DividedSurface;
            Assert.IsNotNull(dividedSurface);
            
            FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.DividedSurface));

            //did it create a divided surface?
            Assert.AreEqual(1, fec.ToElements().Count());

            var ds = (Autodesk.Revit.DB.DividedSurface)fec.ToElements()[0];
            Assert.AreEqual(5, ds.USpacingRule.Number);
            Assert.AreEqual(5, ds.VSpacingRule.Number);

            //can we change the number of divisions
            var numNode = ViewModel.Model.CurrentWorkspace.Nodes.OfType<DoubleInput>().First();
            numNode.Value = "10";
           
            RunCurrentModel();
            

            //did it create a divided surface?
            Assert.AreEqual(10, ds.USpacingRule.Number);
            Assert.AreEqual(10, ds.VSpacingRule.Number);

            //ensure there is a warning when we try to set a negative number of divisions
            numNode.Value = "-5";
           
            RunCurrentModel();
            
            Assert.Greater(ViewModel.Model.EngineController.LiveRunnerRuntimeCore.RuntimeStatus.WarningCount, 0);

           
        }
    }
}
