using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;


namespace RevitSystemTests
{
    [TestFixture]
    class WallTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\InPlaceMass.rvt")]
        public void CreateWallByFace()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Wall\WallByFace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var wall = GetPreviewValue("e3e5735c-98d2-47ac-97fb-90a5f08c416e");
            Assert.IsNotNull(wall);
            Assert.IsTrue(wall.GetType() == typeof(Revit.Elements.FaceWall));
        }


    }
}