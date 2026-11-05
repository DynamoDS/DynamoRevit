using System.Collections.Generic;
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

            // Both nodes replicate over the same subelement list, so allParams[i] and
            // paramValues[i] belong to the same subelement.
            var allParams = GetPreviewValuesPerSubelement("01312066e09b4b1e86b76bad85944e9f");
            Assert.AreEqual(subelemCat.Count, allParams.Count);
            var paramValues = GetPreviewValuesPerSubelement("c0448b187dc74bce8e0ecbfba591d0fc");
            Assert.AreEqual(allParams.Count, paramValues.Count);
            for (int i = 0; i < allParams.Count; i++)
            {
                Assert.IsNotEmpty(allParams[i]);
                Assert.AreEqual(allParams[i].Count, paramValues[i].Count);
                Assert.IsTrue(paramValues[i].Any(v => v.ToString() == "Supply Air"));
            }
        }

        /// <summary>
        /// Returns the node's output as one value list per subelement, without flattening.
        /// </summary>
        private List<List<object>> GetPreviewValuesPerSubelement(string guid)
        {
            var data = GetRuntimeMirror(GetVarName(guid)).GetData();
            Assert.IsTrue(data.IsCollection);
            return data.GetElements()
                .Select(e =>
                {
                    Assert.IsTrue(e.IsCollection);
                    return e.GetElements().Select(x => x.Data).ToList();
                })
                .ToList();
        }
    }
}
