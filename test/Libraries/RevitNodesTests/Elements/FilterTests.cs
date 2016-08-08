using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;

using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FilterTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void CreateFilterRule_ValidArgs()
        {
            Element wall = Revit.Elements.ElementSelector.ByElementId(205280, true);
            Parameter p = new Parameter(wall.InternalElement.LookupParameter("Comments"));

            var filterRule = Revit.Filter.FilterRule.ByRuleType(Revit.Filter.FilterRule.FilterType.BeginsWith, "my", p);
            Assert.NotNull(filterRule);

        }


        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void CreateParameterFilterElement_ValidArgs()
        {

            Element wall = Revit.Elements.ElementSelector.ByElementId(205280, true);
            Parameter p = new Parameter(wall.InternalElement.LookupParameter("Comments"));

            var filterRule = Revit.Filter.FilterRule.ByRuleType(Revit.Filter.FilterRule.FilterType.BeginsWith, "my", p);

            var filter = Revit.Filter.ParameterFilterElement.ByRules("myFilter",
                new System.Collections.Generic.List<Category>() { Category.ByName("Walls") },
                new System.Collections.Generic.List<Revit.Filter.FilterRule>() { filterRule });

            Assert.NotNull(filter);
            Assert.AreEqual(filter.Name, "myFilter");

        }


        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void CreateOverrideGraphicSettings_ValidArgs()
        {


            Revit.Filter.OverrideGraphicSettings overrides = Revit.Filter.OverrideGraphicSettings.ByProperties(null, null, null, null, null, null, null, null);

            Element wall = Revit.Elements.ElementSelector.ByElementId(205280, true);
            Parameter p = new Parameter(wall.InternalElement.LookupParameter("Comments"));

            var filterRule = Revit.Filter.FilterRule.ByRuleType(Revit.Filter.FilterRule.FilterType.BeginsWith, "my", p);

            var filter = Revit.Filter.ParameterFilterElement.ByRules("myFilter",
                new System.Collections.Generic.List<Category>() { Category.ByName("Walls") },
                new System.Collections.Generic.List<Revit.Filter.FilterRule>() { filterRule });

            Assert.NotNull(filter);
            Assert.AreEqual(filter.Name, "myFilter");


            Revit.Application.Document.Current.ActiveView.AddFilter(filter);
            Revit.Application.Document.Current.ActiveView.SetFilterOverrides(filter, overrides);

            var appliedOverrides = Revit.Application.Document.Current.ActiveView.FilterOverrides(filter);

            Assert.NotNull(appliedOverrides);

        }


    }
}
