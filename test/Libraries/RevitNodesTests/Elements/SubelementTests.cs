using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class SubelementTests : RevitNodeTestBase
    {
        private const Int64 m_testElbowId = 976586L;

        [Test]
        [TestModel(@".\Subelement\DuctSegmentFlow.rvt")]
        public void CanSetCoefficientOnDuctSegment()
        {
            ElementId lossCoeffParamId = new ElementId(BuiltInParameter.RBS_LOSS_COEFFICIENT);
            ElementId overrideParamId = new ElementId(BuiltInParameter.MEP_SEGMENT_OVERRIDE);
            double newValue = 1.0;
            var elbow = ElementSelector.ByElementId(m_testElbowId, true);
            foreach (Revit.Elements.Subelement subelem in elbow.Subelements)
            {
                int intValue = (int)SegmentOverrideType.OverrideLossCoefficient;
                subelem.SetParameterValue(overrideParamId, new IntegerParameterValue(intValue));
                subelem.SetParameterValue(lossCoeffParamId, new DoubleParameterValue(newValue));
            }

            foreach (Revit.Elements.Subelement subelem in elbow.Subelements)
            {
                var flowValue = subelem.GetParameterValue(lossCoeffParamId);
                Assert.AreEqual((double)flowValue, newValue);

                Assert.AreEqual(subelem.Element.Id, m_testElbowId);
                Assert.AreEqual(subelem.Category.Id, (long)BuiltInCategory.OST_DuctAnalyticalSegments);
            }
        }
    }
}
