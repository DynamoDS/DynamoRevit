using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Dynamo.Graph.Nodes;
using Dynamo.Tests;

using NUnit.Framework;
using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;
using System;
using CoreNodeModels.Input;

namespace RevitSystemTests
{
    [TestFixture]
    class LevelTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Level\Level.rvt")]
        public void Level()
        {
            var samplePath = Path.Combine(workingDirectory, @".\Level\Level.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            //ensure that the level count is the same
            var levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            levelColl.OfClass(typeof(Level));
            Assert.AreEqual(levelColl.ToElements().Count(), 6);

            //change the number and run again
            var cbnNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<CodeBlockNodeModel>();
            var cmd = new Dynamo.Models.DynamoModel.UpdateModelValueCommand(Guid.Empty, cbnNode.GUID, "Code", "0..20..2");
            Model.ExecuteCommand(cmd);

            RunCurrentModel();

            //ensure that the level count is the same
            levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            levelColl.OfClass(typeof(Level));
            Assert.AreEqual(levelColl.ToElements().Count(), 11);
       }

        //this test runs in automatic to attempt to reproduce the defect in MAGN 8187
        [Test,Ignore("Because it throws exceptions during nodeModified")]
        [TestModel(@".\Level\Level.rvt")]
        public void SetAllLevelsToSameName()
        {
            var samplePath = Path.Combine(workingDirectory, @".\Level\SameNameLevels.dyn");
            var testPath = Path.GetFullPath(samplePath);
      
            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();

            //ensure that the level count is the same
            var levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            levelColl.OfClass(typeof(Level));
            Assert.AreEqual(levelColl.ToElements().Count(),6);

            var firstLevelID = levelColl.ToElementIds().First().IntegerValue;

            //change the name and run again
            var stringNode = ViewModel.Model.CurrentWorkspace.FirstNodeFromWorkspace<StringInput>();
            stringNode.Value = "aNewName";
            
            //ensure that the first level has new name
            levelColl = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            levelColl.OfClass(typeof(Level));
            Assert.AreEqual(levelColl.ToElements().First().Name,"aNewName");
            Assert.AreEqual(levelColl.ToElements().Last().Name, "aNewName(5)");

            var firstLevelIDModified = levelColl.ToElementIds().First().IntegerValue;

            //assert that the elementId of the first level is unchanged... and rebinding has succeeded.
            //while this is true... currently this test throws exceptions during nodeModified runs
            //so the infinite loop would not occur even before the fix, so I have marked the test ignore.
            Assert.AreEqual(firstLevelID, firstLevelIDModified);
            
        }

        [Test]
        [Category("IntegrationTests")]
        [TestModel(@".\empty.rfa")]
        public void CreateLevelsUsingAllLevelCreationNodes()
        {
            /* This Test Case serves as a perpose of Smoke Test For Level creation. 
             Curently it is marked as Integration Tests */

            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Level\Levels.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            // check all the nodes and connectors are loaded
            Assert.AreEqual(9, model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(8, model.CurrentWorkspace.Connectors.Count());

            var levelByElevationAndName = GetPreviewValue
                                    ("f004b19e-f67a-4422-8e4e-5fd4eeea4dff") as Revit.Elements.Level;
            Assert.IsNotNull(levelByElevationAndName);

            var levelByLevelAndOffset = GetPreviewValue
                                    ("bc16e986-6b1e-4e50-845c-970782509145") as Revit.Elements.Level;
            Assert.IsNotNull(levelByLevelAndOffset);

            var levelByLevelOffsetAndName = GetPreviewValue
                                    ("c3894b49-270a-4936-8d68-8ede789fe9f2") as Revit.Elements.Level;
            Assert.IsNotNull(levelByLevelOffsetAndName);

            var levelByElevation = GetPreviewValue
                                    ("eb310915-91b0-481e-ae45-3764207c8b95") as Revit.Elements.Level;
            Assert.IsNotNull(levelByElevation);

        }


    }
}
