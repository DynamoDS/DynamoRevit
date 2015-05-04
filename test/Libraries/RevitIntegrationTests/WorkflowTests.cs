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
        //this method will be deleted on Monday after Mr.Ritesh add it to RevitSystemTestBase
        protected static IList<Autodesk.Revit.DB.Wall> GetAllWalls()
        {
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(Autodesk.Revit.DB.Wall));
            return fec.ToElements().Cast<Autodesk.Revit.DB.Wall>().ToList();
        }
        [Test]
        [TestModel(@".\empty.rvt")]
        public void Wall()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\Wall.dyn");
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
                Assert.IsNotNull(allwalls);
            }
            var wallFromRevit = GetAllWalls();
            Assert.AreEqual(4, wallFromRevit.Count);
        }
    }
}

