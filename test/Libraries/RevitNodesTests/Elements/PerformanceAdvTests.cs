using System.Collections.Generic;
using Autodesk.Revit.DB;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class PerformanceAdviserTests : RevitNodeTestBase
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

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Execute_ValidResult()
        {
            PerformanceAdviser adviser = PerformanceAdviser.GetPerformanceAdviser();
            IList<PerformanceAdviserRuleId> ruleIds = adviser.GetAllRuleIds();
            var perf = Revit.Elements.PerformanceAdviserRule.ById(ruleIds[0].Guid.ToString());
            List<PerformanceAdviserRule> rules = new List<PerformanceAdviserRule>(){perf};

            var messages = Revit.Elements.PerformanceAdviserRule.Execute(rules);

            foreach (var msg in messages)
            {
                Assert.IsTrue(msg.GetType() == typeof(Revit.Elements.FailureMessage));
                Assert.IsNotNull(msg.Description);
                Assert.IsNotNull(msg.Severity);
            }
        }


    }
}
