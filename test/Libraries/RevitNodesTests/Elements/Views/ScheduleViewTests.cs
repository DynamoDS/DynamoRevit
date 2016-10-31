using System;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

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
            Assert.NotNull(fields);
        }
    }
}
