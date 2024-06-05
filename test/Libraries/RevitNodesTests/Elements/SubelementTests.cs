using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class SubelementTests : RevitNodeTestBase
    {
        private const long m_testElbowId = 976586L;

        [Test]
        [TestModel(@".\Subelement\DuctSegmentFlow.rvt")]
        public void CanGetSubelementFromElement()
        {
            var elbow = ElementSelector.ByElementId(m_testElbowId, true);
            var subelems = elbow.Subelements;
            Assert.Greater(subelems.Length, 0);
            foreach(Revit.Elements.Subelement subelem in subelems)
            {
                Assert.Equals(subelem.Element.Id, m_testElbowId);
                Assert.Equals(subelem.Category.Id, ((long)BuiltInCategory.OST_DuctAnalyticalSegments));
            }
        }

        [Test]
        [TestModel(@".\Subelement\DuctSegmentFlow.rvt")]
        public void CanSetCoefficientOnDuctSegment()
        {
            ElementId lossCoeffParamId = new ElementId(BuiltInParameter.RBS_LOSS_COEFFICIENT);
            double newValue = 1.0;
            var elbow = ElementSelector.ByElementId(m_testElbowId, true);
            foreach (Revit.Elements.Subelement subelem in elbow.Subelements)
            {
                var allparams = subelem.GetAllParameters();
                Assert.Greater(allparams.Count, 0);
                subelem.SetParameterValue(lossCoeffParamId, new DoubleParameterValue(newValue));
            }

            DocumentManager.Regenerate();

            foreach (Revit.Elements.Subelement subelem in elbow.Subelements)
            {
                var flowValue = subelem.GetParameterValue(lossCoeffParamId);
                Assert.AreEqual((double)flowValue, newValue);
            }
        }
    }
}
