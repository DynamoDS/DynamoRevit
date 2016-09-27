using System;
using System.Linq;


using Revit.Elements;
using NUnit.Framework;
using Autodesk.Revit.DB;
using RevitTestServices;
using System.Collections.Generic;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class RevisionTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Get_ValidArgs()
        {
            PerformanceAdviser adviser = PerformanceAdviser.GetPerformanceAdviser();
            IList<PerformanceAdviserRuleId> ruleIds = adviser.GetAllRuleIds();
            var perf = Revit.Elements.PerformanceAdviserRule.ById(ruleIds[0].Guid.ToString());
            Assert.NotNull(perf);

            Assert.NotNull(perf.Description);

            Assert.NotNull(perf.Name);

            Assert.AreEqual(perf.GetType(), typeof(Revit.Elements.PerformanceAdviserRule));
        }




    }
}
