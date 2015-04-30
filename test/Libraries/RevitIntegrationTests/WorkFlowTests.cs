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
    public class WorkflowTests : RevitSystemTestBase
    {

        protected static IList<Autodesk.Revit.DB.Wall> GetAllWalls()
        {
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.Wall));
            return fec.ToElements().Cast<Autodesk.Revit.DB.Wall>().ToList();
        }
        

        [Test]
        [TestModel(@".\empty.rvt")]
        public void Workflow_test01()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\WorkFlow_test01.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            var model = ViewModel.Model;

            RunCurrentModel();
            Assert.AreEqual(9, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(9, model.CurrentWorkspace.Connectors.Count());
    
            // Check for Wall
            var wall = "1367ca93-ad6b-459f-a264-61ebe1eb5edd";
            AssertPreviewCount(wall, 4);

            // get all Walls.
            for (int i = 0; i <= 3; i++)
            {
                var allwalls = GetPreviewValueAtIndex(wall, i) as Wall;
              //  System.Console.WriteLine(allwalls.ToString());
                Assert.IsNotNull(allwalls);

            }

            var wallFromRevit = GetAllWalls();
            Assert.AreEqual(4, wallFromRevit.Count);

        }


        [Test]
        [TestModel(@".\Workflow\basic.rvt")]
        public void Workflow_Truss()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoBasic\01 Truss.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
        }

        [Test]
        [TestModel(@".\Workflow\empty.rfa")]
        public void Workflow_ListLacing()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoBasic\02 List Lacing.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\Workflow\empty.rfa")]
        public void Workflow_AttractorPoint()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoBasic\03 Attractor Point.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }


        [Test]
        [TestModel(@".\Workflow\basic.rvt")]
        public void Workflow_StructureFrame()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\01_StructureFraming.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }


        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Workflow_AdaptiveComponent1()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\02_Adaptive Component Placement 1.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Workflow_AdaptiveComponent2()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\03_Adaptive Component Placement 2.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Workflow_ImportSolid()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\04_ImportSolid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\Workflow\DynamoRevit\DynamoSample.rvt")]
        public void Workflow_PlaceFamiliesByLevel()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\DynamoRevit\05_PlaceFamiliesByLevle_SetParameters.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

        [Test]
        [TestModel(@".\Workflow\RevitProject\tower.rvt")]
        public void Workflow_PlaceFamiliesByLevel2()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\RevitProject\01 Ellipse Tower v1.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

        }

       
        
    }
}
