using System;
using System.IO;
using System.Linq;

using Autodesk.Revit.DB;

using Dynamo.Nodes;

using NUnit.Framework;

using RevitServices.Persistence;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class DividedSurfaceTests : SystemTest
    {
        [Test, Category("Failure")]
        [TestModel(@".\DividedSurface\DividedSurface.rfa")]
        public void DividedSurface()
        {
            throw new NotImplementedException("LC Modularization");

            /*
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DividedSurface\DividedSurface.dyn");
            string testPath = Path.GetFullPath(samplePath);
            
            ViewModel.OpenCommand.Execute(testPath);
            
            RunCurrentModel();
            //ViewModel.Model.RunExpression();

            FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(DividedSurface));

            //did it create a divided surface?
            Assert.AreEqual(1, fec.ToElements().Count());

            var ds = (DividedSurface)fec.ToElements()[0];
            Assert.AreEqual(5, ds.USpacingRule.Number);
            Assert.AreEqual(5, ds.VSpacingRule.Number);

            //can we change the number of divisions
            var numNode = ViewModel.Model.CurrentWorkspace.Nodes.OfType<DoubleInput>().First();
            numNode.Value = "10";
           //LC: Modularization
            RunCurrentModel();
            

            //did it create a divided surface?
            Assert.AreEqual(10, ds.USpacingRule.Number);
            Assert.AreEqual(10, ds.VSpacingRule.Number);

            //ensure there is a warning when we try to set a negative number of divisions
            numNode.Value = "-5";
           //LC: Modularization
            RunCurrentModel();
            
            Assert.Greater(ViewModel.Model.EngineController.LiveRunnerCore.RuntimeStatus.WarningCount, 0);
        
             */
        }
    }
}
