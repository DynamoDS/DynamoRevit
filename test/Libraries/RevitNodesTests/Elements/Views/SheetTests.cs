using System;

using System.Linq;

using Autodesk.DesignScript.Geometry;

using NUnit.Framework;

using Revit.Elements;
using Revit.Elements.Views;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    class SheetTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockAndViews_ValidArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var pt2 = Point.ByCoordinates(100, 100, 0);
            var famInst2 = FamilyInstance.ByPoint(famTyp, pt2);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);
            var view2 = SectionView.ByBoundingBox(famInst2.BoundingBox);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            var ele = Sheet.ByNameNumberTitleBlockAndViews( sheetName, sheetNumber, titleBlock, new[] {view, view2});

            Assert.NotNull(ele);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockAndViews_BadArgs()
        {

            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var pt2 = Point.ByCoordinates(100, 100, 0);
            var famInst2 = FamilyInstance.ByPoint(famTyp, pt2);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);
            var view2 = SectionView.ByBoundingBox(famInst2.BoundingBox);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndViews(null, sheetNumber, titleBlock, new[] { view, view2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndViews(sheetName, null, titleBlock, new[] { view, view2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndViews(sheetName, sheetNumber, null, new[] { view, view2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndViews(sheetName, sheetNumber, titleBlock, null));
        }
        
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockAndView_ValidArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            var ele = Sheet.ByNameNumberTitleBlockAndView(sheetName, sheetNumber, titleBlock, view);

            Assert.NotNull(ele);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockAndView_BadArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndView(null, sheetNumber, titleBlock, view));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndView(sheetName, null, titleBlock, view));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndView(sheetName, sheetNumber, null, view));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockAndView(sheetName, sheetNumber, titleBlock, null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlock_ValidArgs()
        {
            // Arrange
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            // Act
            var ele = Sheet.ByNameNumberTitleBlock(sheetName, sheetNumber, titleBlock);

            // Assert
            Assert.NotNull(ele);
            Assert.AreEqual(sheetName, ele.SheetName);
            Assert.AreEqual(sheetNumber, ele.SheetNumber);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlock_BadArgs()
        {
            // Arrange
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlock(null, sheetNumber, titleBlock));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlock(sheetName, null, titleBlock));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlock(sheetName, sheetNumber, null));
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void DuplicateSheet_ValidArgs()
        {
            var sheets = ElementSelector.ByType<Autodesk.Revit.DB.ViewSheet>(true).Where(x => (x as Sheet).SheetNumber.Equals("A001"));
            var sheet = sheets.First() as Sheet;

            var newSheetNumber = "Test" + sheet.SheetNumber;

            var duplicateSheet = Sheet.DuplicateSheet(sheet, true, true, "Duplicate", "Test");

            Assert.NotNull(duplicateSheet);
            Assert.AreEqual(duplicateSheet.SheetNumber, newSheetNumber);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void DuplicateSheet_BadArgs()
        {
            var sheets = ElementSelector.ByType<Autodesk.Revit.DB.ViewSheet>(true).Where(x => (x as Sheet).SheetNumber.Equals("A001"));
            var sheet = sheets.First() as Sheet;

            Assert.Throws(typeof(ArgumentNullException), () => Sheet.DuplicateSheet(null, true, true, "Duplicate", "Test"));
            Assert.Throws(typeof(ArgumentException), () => Sheet.DuplicateSheet(sheet, true, true, "duplicate"));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockViewsAndLocations_ValidArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var pt2 = Point.ByCoordinates(100, 100, 0);
            var famInst2 = FamilyInstance.ByPoint(famTyp, pt2);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);
            var view2 = SectionView.ByBoundingBox(famInst2.BoundingBox);

            var location = Point.ByCoordinates(0, 0, 0);
            var location2 = Point.ByCoordinates(0, 1, 0);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            var ele = Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, titleBlock, new[] { view, view2 }, new[] { location, location2 });

            Assert.NotNull(ele);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockViewsAndLocations_BadArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var pt2 = Point.ByCoordinates(100, 100, 0);
            var famInst2 = FamilyInstance.ByPoint(famTyp, pt2);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);
            var view2 = SectionView.ByBoundingBox(famInst2.BoundingBox);

            var location = Point.ByCoordinates(0, 0, 0);
            var location2 = Point.ByCoordinates(0, 1, 0);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(null, sheetNumber, titleBlock, new[] { view, view2 }, new[] { location, location2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, null, titleBlock, new[] { view, view2 }, new[] { location, location2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, null, new[] { view, view2 }, new[] { location, location2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, titleBlock, null, new[] { location, location2 }));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, titleBlock, new[] { view, view2 }, null));
            Assert.Throws(typeof(ArgumentException), () => Sheet.ByNameNumberTitleBlockViewsAndLocations(sheetName, sheetNumber, titleBlock, new[] { view, view2 }, new[] { location }));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockViewAndLocation_ValidArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);

            var location = Point.ByCoordinates(0, 0, 0);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            var ele = Sheet.ByNameNumberTitleBlockViewAndLocation(sheetName, sheetNumber, titleBlock, view, location);

            Assert.NotNull(ele);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlockViewAndLocation_BadArgs()
        {
            ElementBinder.IsEnabled = false;

            var famSymName = "E1 30x42 Horizontal";
            var famName = "E1 30 x 42 Horizontal";
            var titleBlock = FamilyType.ByFamilyAndName(Family.ByName(famName), famSymName);

            var famTyp = FamilyType.ByName("Kousa Dogwood - 10'");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famTyp, pt);

            var view = SectionView.ByBoundingBox(famInst.BoundingBox);

            var location = Point.ByCoordinates(0, 0, 0);

            var sheetName = "Poodle";
            var sheetNumber = "A1";

            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewAndLocation(null, sheetNumber, titleBlock, view, location));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewAndLocation(sheetName, null, titleBlock, view, location));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewAndLocation(sheetName, sheetNumber, null, view, location));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewAndLocation(sheetName, sheetNumber, titleBlock, null, location));
            Assert.Throws(typeof(ArgumentNullException), () => Sheet.ByNameNumberTitleBlockViewAndLocation(sheetName, sheetNumber, titleBlock, view, null));
        }
    }
}