﻿using System;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    class ScheduleViewTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCategoryNameType_ValidArgs()
        {
            var view = CreateTestView();

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
            var view = CreateTestView();

            // should always return at least one field
            var fields = view.SchedulableFields;
            Assert.Greater(fields.Count, 0);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ExportViewSchedule_ValidArgs()
        {
            var view = CreateTestView();

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
            var view = CreateTestView();

            var options = new Revit.Schedules.ScheduleExportOptions(new Autodesk.Revit.DB.ViewScheduleExportOptions());
            Assert.NotNull(options);

            var path = Path.GetTempFileName();
            path = Path.ChangeExtension(path, ".tsv");

            Assert.Throws(typeof(ArgumentException), () => view.Export(null, options));
            Assert.Throws(typeof(ArgumentException), () => view.Export(path, null));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void RemoveFields_ValidArgs()
        {
            var view = CreateTestView();

            // Key Schedule by default always has one field on creation
            var fields = view.Fields;
            Assert.Greater(fields.Count, 0);

            // remove all fields
            view.RemoveFields(fields);
            var currentFields = view.Fields;
            Assert.AreEqual(currentFields.Count, 0);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void AddFields_ValidArgs()
        {
            var view = CreateTestView();

            // remove all fields
            var fields = view.Fields;
            view.RemoveFields(fields);
            Assert.AreEqual(view.Fields.Count, 0);

            // get a list of schedulable fields and add Key Name field back
            var schedulableFields = view.SchedulableFields;
            Assert.Greater(schedulableFields.Count, 0);

            var fieldToAdd = schedulableFields.Where(x => x.Name == "Key Name").FirstOrDefault();
            Assert.NotNull(fieldToAdd);

            view.AddFields(new List<Revit.Schedules.SchedulableField>() { fieldToAdd });
            Assert.Greater(view.Fields.Count, 0);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void AddScheduleFilter_ValidArgs()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_Walls"), "WallSchedule_Test", ScheduleView.ScheduleType.RegularSchedule.ToString());
            Assert.NotNull(view);

            // get a list of schedulable fields and add "Width" field to schedule
            var schedulableFields = view.SchedulableFields;
            Assert.Greater(schedulableFields.Count, 0);

            var fieldToAdd = schedulableFields.Where(x => x.Name == "Width").FirstOrDefault();
            Assert.NotNull(fieldToAdd);

            view.AddFields(new List<Revit.Schedules.SchedulableField>() { fieldToAdd });
            Assert.Greater(view.Fields.Count, 0);

            // create new schedule filter and add it to schedule
            var field = view.Fields.Where(x => x.Name == "Width").FirstOrDefault();
            Assert.NotNull(field);

            var filter = Revit.Schedules.ScheduleFilter.ByFieldTypeAndValue(field, Autodesk.Revit.DB.ScheduleFilterType.Equal.ToString(), 0.1);
            Assert.NotNull(filter);

            view.AddFilters(new List<Revit.Schedules.ScheduleFilter>() { filter });
            Assert.Greater(view.ScheduleFilters.Count, 0);
        }

        [Test]
        [TestModel(@".\SampleModel.rvt")]
        public void ClearAllScheduleFilters_ValidArgs()
        {
            var rView = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.ViewSchedule))
                .Cast<Autodesk.Revit.DB.ViewSchedule>()
                .Where(x => x.Name == "Wall Schedule")
                .FirstOrDefault();

            var view = ScheduleView.FromExisting(rView, true);
            Assert.NotNull(view);

            // remove all schedule filters
            view.ClearAllFilters();
            Assert.AreEqual(view.ScheduleFilters.Count, 0);
        }


        private static ScheduleView CreateTestView()
        {
            var view = ScheduleView.CreateSchedule(Category.ByName("OST_GenericModel"), "KeySchedule_Test", ScheduleView.ScheduleType.KeySchedule.ToString());
            Assert.NotNull(view);

            return view;
        }
    }
}
