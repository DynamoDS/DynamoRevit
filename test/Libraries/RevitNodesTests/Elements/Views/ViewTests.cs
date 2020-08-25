using System.IO;

using Autodesk.DesignScript.Geometry;

using NUnit.Framework;

using Revit.Elements.Views;

using RevitTestServices;

using RTF.Framework;

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
            var bmp = testView.ExportAsImage(tmp1);

            var tmp1Info = new FileInfo(tmp1);
            Assert.Greater(tmp1Info.Length, 0);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void ExportAsImage_BadArgs()
        {
            var testView = CreateTestView();
            Assert.Throws<System.ArgumentException>(() => testView.ExportAsImage(""));
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

        [Test, TestModel(@".\Empty.rvt")]
        public void DuplicateView_GoodArgs()
        {
            var testView = CreateTestView();
            var newName = "Test" + testView.Name;

            var duplicateView = View.DuplicateView(testView, "Duplicate", "Test");

            Assert.NotNull(duplicateView);
            Assert.AreEqual(duplicateView.Name, newName);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void DuplicateView_BadArgs()
        {
            var testView = CreateTestView();

            Assert.Throws<System.ArgumentNullException>(() => View.DuplicateView(null));
            Assert.Throws<System.ArgumentException>(() => View.DuplicateView(testView, "duplicate"));

            var testSheet = CreateTestSheet();
            Assert.Throws<System.ArgumentException>(() => View.DuplicateView(testSheet));
        }

        private static Sheet CreateTestSheet()
        {
            RevitServices.Persistence.ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = Revit.Elements.FamilyType.ByFamilyAndName(Revit.Elements.Family.ByName(famName), famSymName);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            // Act
            var ele = Sheet.ByNameNumberTitleBlock(sheetName, sheetNumber, titleBlock);

            return ele;
        }
    }
}
