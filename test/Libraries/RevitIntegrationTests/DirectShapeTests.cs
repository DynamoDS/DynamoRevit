using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using System.Linq;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using RevitServices.Materials;
namespace RevitSystemTests
{
    [TestFixture]
    class DirectShapeTests : RevitSystemTestBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            //setup the material manager just for tests
            var mgr = MaterialsManager.Instance;
            mgr.InitializeForActiveDocumentOnIdle();
        }
        
        [Test]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanGetDirectShapeInstances()
        {
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\getDirectShapesFromDocument.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            Assert.AreEqual("shape123test", GetPreviewValue("47a28c19-6eea-414d-9c50-d7e125fbf81f"));
            Assert.AreEqual("shape123test", GetPreviewValue("8d10aa34-45d5-405d-8ad8-8b277d0291f7"));

        }
        
        [Test]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanGetDirectShapeCentroid()
        {
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\getDirectShapeCentroid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            AssertNoDummyNodes();
            RunCurrentModel();

            var pnt = GetPreviewValue("53eb0459-4cd3-4131-b7ce-c4e689c57248") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 10.0, 1.0e-6));

        }

        [Test,Category("Failure")]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanUpdateDirectShapeGeo()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\getDirectShapeCentroid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var pnt = GetPreviewValue("53eb0459-4cd3-4131-b7ce-c4e689c57248") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 10.0, 1.0e-6));

            //now move the geo in Dynamo

            var input = model.CurrentWorkspace.Nodes.Where(x => x.GUID.ToString() == "06e1461a-2eb4-4069-a581-93914d500115").First();
            input.UpdateValue(new UpdateValueParams("Value", "20.0"));

            Assert.AreEqual((input as CoreNodeModels.Input.DoubleInput).Value, "20.0");

            var pnt2 = GetPreviewValue("53eb0459-4cd3-4131-b7ce-c4e689c57248") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt2);

            Assert.IsTrue(IsFuzzyEqual(pnt2.X, 20.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt2.Y, 20.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt2.Z, 20.0, 1.0e-6));

        }

        [Test]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanUpdateDirectShapeCategory()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\getDirectShapeCentroid.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var cat = GetPreviewValue("097a1c71-884d-413a-bd75-ff0001eeceb5") as Revit.Elements.Category;
            Assert.IsNotNull(cat);

            //now update the category in Dynamo

            var input = GetNode<NodeModel>("9b4ad043-369e-49a1-bdd7-adab9777abc9");
            Assert.DoesNotThrow(() => { input.UpdateValue(new UpdateValueParams("Value", "OST_GenericModel")); });
            RunCurrentModel();

            var cat2 = GetPreviewValue("097a1c71-884d-413a-bd75-ff0001eeceb5") as Revit.Elements.Category;
            Assert.IsNotNull(cat2);

            Assert.AreEqual("Generic Models", cat2.Name);

        }
    }
}
