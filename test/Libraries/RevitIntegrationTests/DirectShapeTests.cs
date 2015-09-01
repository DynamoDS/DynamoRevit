using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using System.Linq;
namespace RevitSystemTests
{
    [TestFixture]
    class DirectShapeTests : RevitSystemTestBase
    {
        
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

            var pnt = GetPreviewValue("76e793d7-ba52-429e-9bed-8f1e9493d505") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 10.0, 1.0e-6));

        }

        [Test, Category("Failure")]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanUpdateDirectShapeGeo()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\UpdateGeo.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var pnt = GetPreviewValue("76e793d7-ba52-429e-9bed-8f1e9493d505") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, 10.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 10.0, 1.0e-6));

            //now move the geo in Dynamo

            var input = model.CurrentWorkspace.Nodes.Where(x => x.GUID.ToString() == "e1f9f94a-f705-40f2-8410-6931deca3648").First();
            input.UpdateValue(new Dynamo.Models.UpdateValueParams("Value", "20.0"));

            Assert.AreEqual((input as Dynamo.Nodes.DoubleInput).Value, "20.0");

            var pnt2 = GetPreviewValue("76e793d7-ba52-429e-9bed-8f1e9493d505") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt2);

            Assert.IsTrue(IsFuzzyEqual(pnt2.X, 20.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt2.Y, 20.0, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt2.Z, 20.0, 1.0e-6));

        }

        [Test,Category("Failure")]
        [TestModel(@".\DirectShape\singleDirectShape.rvt")]
        public void CanUpdateDirectShapeCategory()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\DirectShape\UpdateCat.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            System.Threading.Thread.Sleep(100);
            var cat = GetPreviewValue("ede05d2f-5fc4-4486-94dd-e2b2f230fe25") as Revit.Elements.Category;
            Assert.IsNotNull(cat);

            //now update the category in Dynamo

            var input = GetNode<Dynamo.Models.NodeModel>("292eb358-fd48-4466-91ad-14bf303449f5");
            input.UpdateValue(new Dynamo.Models.UpdateValueParams("value", "OST_GenericModel"));
            RunCurrentModel();

            var cat2 = GetPreviewValue("ede05d2f-5fc4-4486-94dd-e2b2f230fe25") as Revit.Elements.Category;
            Assert.IsNotNull(cat2);

            Assert.AreEqual("Generic Model", cat2.Name);

        }
    }
}
