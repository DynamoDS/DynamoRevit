using System.Linq;
using NUnit.Framework;
using Revit.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements.Views
{
    [TestFixture]
    public class LegendTests : RevitNodeTestBase
    {
        [Test, TestModel(@".\View\Legend.rvt")]
        public void GetLegend()
        {
            var legend = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.View))
                .Cast<Autodesk.Revit.DB.View>()
                .FirstOrDefault(x => x.ViewType == Autodesk.Revit.DB.ViewType.Legend);
            Assert.NotNull(legend);

            var wrappedLegend = legend.ToDSType(true);
            Assert.AreEqual(wrappedLegend.GetType(), typeof(Revit.Elements.Views.Legend));
        }
    }
}
