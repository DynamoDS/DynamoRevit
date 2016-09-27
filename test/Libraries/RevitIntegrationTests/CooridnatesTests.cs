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
    class CoordinatesTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void GetSurveyPoint()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Coordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(GetPreviewValue("ad5bb1c3-00eb-472c-92a8-d9dafe805939"), Autodesk.DesignScript.Geometry.Point.Origin());
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void GetBasePoint()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Coordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(GetPreviewValue("81ad46c7-29f2-4913-a1f5-832a39b62bcd"), Autodesk.DesignScript.Geometry.Point.Origin());
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void GetRotation()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Coordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual(GetPreviewValue("77ce2b6f-5d4a-4f45-85b8-bc2c1bac9134"), 0.0);
        }



    }
}