using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class ScheduleOnSheetTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\ScheduleOnSheet\scheduleonsheet.rvt")]
        public void BySheetScheduleLocation()
        {
            var sheet = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(ViewSheet))
                .Where(x => x.Name.Equals("Unnamed"))
                .First() as ViewSheet;
            var scheduleView = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.ViewSchedule))
                .Where(x => x.Name.Equals("View List"))
                .First() as ViewSchedule;
            var location = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);

            var scheduleOnSheet = ScheduleOnSheet.BySheetScheduleLocation(Sheet.FromExisting(sheet,true), ScheduleView.FromExisting(scheduleView,true), location);

            Assert.AreEqual("View List", scheduleOnSheet.Name);
        }

        [Test]
        [TestModel(@".\ScheduleOnSheet\scheduleonsheet.rvt")]
        public void BySheetScheduleLocation_EmptySchedule()
        {
            var sheet = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(ViewSheet))
                .Where(x => x.Name.Equals("Unnamed"))
                .First() as ViewSheet;
            var scheduleView = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.ViewSchedule))
                .Where(x => x.Name.Equals("Room Schedule"))
                .First() as ViewSchedule;
            var location = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);

            var expectedExceptionMessage = Revit.Properties.Resources.EmptySchedule;

            var exceptionScheduleOnSheet = Assert.Throws<InvalidOperationException>(() => ScheduleOnSheet.BySheetScheduleLocation(Sheet.FromExisting(sheet, true), ScheduleView.FromExisting(scheduleView, true), location));


            Assert.AreEqual(expectedExceptionMessage, exceptionScheduleOnSheet.Message);
        }
    }
}
