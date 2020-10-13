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
    class TagTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void PlaceTag()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Tag.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var tagelement = GetPreviewValue("73d7876d-c04a-418c-8f86-2e36c44d9833");

            Assert.AreEqual(typeof(Revit.Elements.Tag), tagelement.GetType());

        }     

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void PlaceTagWithOffset()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Tag.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var tagelement = GetPreviewValue("022d664d-b236-4a20-a55d-72a3beba04d5");

            Assert.AreEqual(typeof(Revit.Elements.Tag), tagelement.GetType());
            Assert.IsTrue(((tagelement as Revit.Elements.Tag)
                .InternalElement as IndependentTag)
                .TagHeadPosition.IsAlmostEqualTo(new XYZ(10, 25, 0)));

        }

        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void HeadLocation()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Tag\CanGetSetHeadPositionLeaderElbow.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var headlocation1 = GetPreviewValue("f3c8840d0da14da5afac692216eb3019") as Autodesk.DesignScript.Geometry.Point;
            var headlocation2 = GetPreviewValue("442ca4337e1d45fba00989e0db3182e8") as Autodesk.DesignScript.Geometry.Point;

            Assert.IsNotNull(headlocation1);
            Assert.IsNotNull(headlocation2);
            Assert.AreEqual(5, (headlocation2.Y - headlocation1.Y), Tolerance);
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void LeaderElbow()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Tag\CanGetSetHeadPositionLeaderElbow.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var headlocation = GetPreviewValue("f3c8840d0da14da5afac692216eb3019") as Autodesk.DesignScript.Geometry.Point;
            var leaderElbow = GetPreviewValue("0c58b6a42ea4447cad6bcd9936d85b93") as Autodesk.DesignScript.Geometry.Point;

            Assert.IsNotNull(headlocation);
            Assert.IsNotNull(leaderElbow);
            Assert.AreEqual(headlocation.X, leaderElbow.X, Tolerance);
            Assert.AreEqual(headlocation.Y, leaderElbow.Y, Tolerance);
            Assert.AreEqual(headlocation.Z, leaderElbow.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void LeaderEndCondition()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Tag\CanGetSetConditionAndLocationOfLeaderEnd.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var leaderEndCondition = GetPreviewValue("316f8fd5b05441cda070c36a1e122c81");

            Assert.AreEqual("Free", leaderEndCondition);
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void LeaderEnd()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Tag\CanGetSetConditionAndLocationOfLeaderEnd.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            var leaderEnd1 = GetPreviewValue("780005166f264fbbaf6018dd12bd8ba6") as Autodesk.DesignScript.Geometry.Point;
            var leaderEnd2 = GetPreviewValue("a067b0ce7a234d34905d23b5e31e49cf") as Autodesk.DesignScript.Geometry.Point;

            Assert.IsNotNull(leaderEnd1);
            Assert.IsNotNull(leaderEnd2);
            Assert.AreEqual(2, (leaderEnd2.Y - leaderEnd1.Y), Tolerance);
        }
    }
}