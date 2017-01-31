using System.Collections.Generic;
using Autodesk.Revit.DB;
using NUnit.Framework;
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
            var perf = Revit.PerformanceAdviser.PerformanceAdviserRule.ById(ruleIds[0].Guid.ToString());
            Assert.NotNull(perf);

            Assert.NotNull(perf.Description);

            Assert.NotNull(perf.Name);

            Assert.AreEqual(perf.GetType(), typeof(Revit.PerformanceAdviser.PerformanceAdviserRule));
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Execute_ValidResult()
        {
            PerformanceAdviser adviser = PerformanceAdviser.GetPerformanceAdviser();
            IList<PerformanceAdviserRuleId> ruleIds = adviser.GetAllRuleIds();
            var perf = Revit.PerformanceAdviser.PerformanceAdviserRule.ById(ruleIds[0].Guid.ToString());
            List<Revit.PerformanceAdviser.PerformanceAdviserRule> rules = new List<Revit.PerformanceAdviser.PerformanceAdviserRule>(){perf};

            var messages = Revit.PerformanceAdviser.PerformanceAdviserRule.Execute(rules);

            foreach (var msg in messages)
            {
                Assert.IsTrue(msg.GetType() == typeof(Revit.PerformanceAdviser.FailureMessage));
                Assert.IsNotNull(msg.Description);
                Assert.IsNotNull(msg.Severity);
            }
        }


    }
}
