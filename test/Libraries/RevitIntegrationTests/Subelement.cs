using System.Collections;
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

            // The subelement layout is checked loosely — exact counts would have to track every Revit change.
            var subelemCat = GetFlattenedPreviewValues("062360f4d4514b70b3da0b0b73c1ee01");
            Assert.GreaterOrEqual(subelemCat.Count(c => c.ToString() == "Analytical Duct Segments"), 1);
            Assert.GreaterOrEqual(subelemCat.Count(c => c.ToString() == "Duct Connectors"), 2);

            var subelemElement = GetFlattenedPreviewValues("8e50e4cab1bd4517abc6d3ca81d11110");
            Assert.IsTrue(subelemElement.All(e => e.ToString() == "Duct"));

            // Both nodes replicate over the same subelement list, so ids[i] and values[i]
            // belong to the same subelement.
            var allParams = GetPreviewCollection("01312066e09b4b1e86b76bad85944e9f");
            Assert.AreEqual(subelemCat.Count, allParams.Count);
            var paramValues = GetPreviewCollection("c0448b187dc74bce8e0ecbfba591d0fc");
            Assert.AreEqual(allParams.Count, paramValues.Count);
            for (int i = 0; i < allParams.Count; i++)
            {
                var ids = ((IEnumerable)allParams[i]).Cast<object>().ToList();
                var values = ((IEnumerable)paramValues[i]).Cast<object>().ToList();
                Assert.IsNotEmpty(ids);
                Assert.AreEqual(ids.Count, values.Count);
                Assert.IsTrue(values.Any(v => v.ToString() == "Supply Air"));
            }
        }
    }
}
