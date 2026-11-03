using System.IO;
using System.Linq;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class SubelementTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\DuctSegmentFlow.rvt")]
        public void GetParameterValueOfSubelement()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\GetParameterValueOfSubelement.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            // The selected duct exposes one analytical duct segment (21 parameters)
            // and two duct connectors (14 parameters each); the graph flattens
            // the parameters of all subelements.
            const int expectedSegmentCount = 1;
            const int expectedConnectorCount = 2;
            const int expectedSubelementCount = expectedSegmentCount + expectedConnectorCount;
            const int segmentParameterCount = 21;
            const int connectorParameterCount = 14;
            const int expectedParameterCount =
                segmentParameterCount + expectedConnectorCount * connectorParameterCount;

            var subelemCat = GetFlattenedPreviewValues("062360f4d4514b70b3da0b0b73c1ee01");
            Assert.AreEqual(expectedSubelementCount, subelemCat.Count);
            Assert.AreEqual(expectedSegmentCount, subelemCat.Count(c => c.ToString() == "Analytical Duct Segments"));
            Assert.AreEqual(expectedConnectorCount, subelemCat.Count(c => c.ToString() == "Duct Connectors"));

            var subelemElement = GetFlattenedPreviewValues("8e50e4cab1bd4517abc6d3ca81d11110");
            Assert.AreEqual(expectedSubelementCount, subelemElement.Count);
            Assert.IsTrue(subelemElement.All(e => e.ToString() == "Duct"));

            var getAllParams = GetFlattenedPreviewValues("01312066e09b4b1e86b76bad85944e9f");
            Assert.AreEqual(expectedParameterCount, getAllParams.Count);

            var getParamValue = GetFlattenedPreviewValues("c0448b187dc74bce8e0ecbfba591d0fc");
            Assert.AreEqual(expectedParameterCount, getParamValue.Count);
            Assert.AreEqual(expectedSubelementCount, getParamValue.Count(v => v.ToString() == "Supply Air"));
        }
    }
}
