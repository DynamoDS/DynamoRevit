using System;
using System.Linq;
using NUnit.Framework;
using RTF.Framework;
using Revit.Elements.Views;
using RevitServices.Persistence;
using RevitTestServices;
using Revit.Elements;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    class AreaPlanViewTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByLevelAndAreaScheme_ValidArgs()
        {
            const int elevation = 100;
            var level = Level.ByElevation(elevation);
            Assert.NotNull(level);

            var areaScheme = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.AreaScheme))
                .Select(x => x.ToDSType(true))
                .FirstOrDefault();   
            Assert.NotNull(areaScheme);

            var view = AreaPlanView.ByLevelAndAreaScheme(level, areaScheme);
      
            Assert.NotNull(view);
            Assert.AreEqual(level.Id, view.InternalViewPlan.GenLevel.Id.IntegerValue);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByLevelAndAreaScheme_BadArgs()
        {
            const int elevation = 100;
            var level = Level.ByElevation(elevation);
            Assert.NotNull(level);

            var areaScheme = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.AreaScheme))
                .Select(x => x.ToDSType(true))
                .FirstOrDefault();
            Assert.NotNull(areaScheme);

            Assert.Throws(typeof(ArgumentNullException), () => AreaPlanView.ByLevelAndAreaScheme(level, null));
            Assert.Throws(typeof(ArgumentNullException), () => AreaPlanView.ByLevelAndAreaScheme(null, areaScheme));
        }
    }
}
