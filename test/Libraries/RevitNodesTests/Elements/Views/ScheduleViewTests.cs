using System;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    class ScheduleViewTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCategoryNameType_ValidArgs()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString());
            Assert.NotNull(view);

            Assert.IsTrue(DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(view.InternalElement.UniqueId)));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCategoryNameType_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ScheduleView.CreateSchedule(null, "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString()));
            Assert.Throws(typeof(ArgumentNullException), () => ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), null, ScheduleView.ScheduleType.KeySchedule.ToString()));
            Assert.Throws(typeof(ArgumentNullException), () => ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void GetSchedulableField()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString());
            Assert.NotNull(view);

            // should always return at least one field
            var fields = view.SchedulableFields;
            Assert.Greater(fields.Count, 0);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ExportViewSchedule_ValidArgs()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString());
            Assert.NotNull(view);

            var path = Path.GetTempFileName();
            path = Path.ChangeExtension(path, ".tsv");

            var options = new Revit.Schedules.ScheduleExportOptions(new Autodesk.Revit.DB.ViewScheduleExportOptions());
            Assert.NotNull(options);

            var exportView = view.Export(path, options);
            var pathInfo = new FileInfo(path);
            Assert.Greater(pathInfo.Length, 0);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ExportViewSchedule_NullArgs()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString());
            Assert.NotNull(view);

            var options = new Revit.Schedules.ScheduleExportOptions(new Autodesk.Revit.DB.ViewScheduleExportOptions());
            Assert.NotNull(options);

            var path = Path.GetTempFileName();
            path = Path.ChangeExtension(path, ".tsv");

            Assert.Throws(typeof(ArgumentNullException), () => view.Export(null, options));
            Assert.Throws(typeof(ArgumentNullException), () => view.Export(path, null));
        }
    }
}
