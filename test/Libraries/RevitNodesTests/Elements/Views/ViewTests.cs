using System.IO;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements.Views;
using RevitTestServices;
using RTF.Framework;
using RevitServices.Persistence;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    class ViewTests : RevitNodeTestBase
    {
        [Test, TestModel(@".\Empty.rvt")]
        public void ExportAsImage_GoodArgs()
        {
            var testView = CreateTestView();

            var tmp1 = Path.GetTempFileName();

            tmp1 = Path.ChangeExtension(tmp1, ".png");
            testView.ExportAsImage(tmp1);

            var tmp1Info = new FileInfo(tmp1);
            Assert.Greater(tmp1Info.Length, 0);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void ExportAsImage_BadArgs()
        {
            var testView = CreateTestView();
            Assert.Throws<System.ArgumentException>(()=>testView.ExportAsImage(""));
        }

        private static View CreateTestView()
        {
            var eye = Point.ByCoordinates(100, 100, 100);
            var target = Point.ByCoordinates(0, 1, 2);

            var v = AxonometricView.ByEyePointAndTarget(eye, target);
            var view = (Autodesk.Revit.DB.View3D)v.InternalElement;
            Assert.AreEqual(view.Name, View3D.DEFAULT_VIEW_NAME);
            Assert.False(view.CropBoxActive);

            return v;
        }

        [Test, TestModel(@".\ViewTemplateTests.rvt")]
        public void GetViewTemplate()
        {
            var rView = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .FirstOrDefault(x => x.Name == "ViewWithViewTemplate") as Autodesk.Revit.DB.ViewPlan;

            var dView = FloorPlanView.FromExisting(rView, true);
            Assert.NotNull(dView);

            var viewTemplate = dView.ViewTemplate() as FloorPlanView;
            Assert.NotNull(viewTemplate);
        }

        [Test, TestModel(@".\ViewTemplateTests.rvt")]
        public void SetViewTemplate()
        {
            var rView = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .FirstOrDefault(x => x.Name == "ViewWithoutViewTemplate") as Autodesk.Revit.DB.ViewPlan;

            var dView = FloorPlanView.FromExisting(rView, true);
            Assert.NotNull(dView);

            var viewTemplate = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .Cast<Autodesk.Revit.DB.ViewPlan>()
                .FirstOrDefault(x => x.IsTemplate && x.Name == "Architectural Plan");

            var dViewTemp = FloorPlanView.FromExisting(viewTemplate, true);
            Assert.NotNull(dViewTemp);

            dView.SetViewTemplate(dViewTemp);
            var result = dView.ViewTemplate() as FloorPlanView;
            Assert.NotNull(result);
        }

        [Test, TestModel(@".\ViewTemplateTests.rvt")]
        public void RemoveViewTemplate()
        {
            var rView = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .FirstOrDefault(x => x.Name == "ViewWithViewTemplate") as Autodesk.Revit.DB.ViewPlan;

            var dView = FloorPlanView.FromExisting(rView, true);
            Assert.NotNull(dView);

            dView.RemoveViewTemplate();
            var result = dView.ViewTemplate();
            Assert.IsNull(result);
        }

    }
}
