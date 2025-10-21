using System.IO;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class SubelementTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\SubelementTests.rvt")]
        public void GetParameterValueOfSubelement()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\GetParameterValueOfSubelement.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var subelemCat = GetFlattenedPreviewValues("062360f4d4514b70b3da0b0b73c1ee01");
            Assert.AreEqual("Analytical Duct Segments", subelemCat[0].ToString());

            var subelemElement = GetFlattenedPreviewValues("8e50e4cab1bd4517abc6d3ca81d11110");
            Assert.AreEqual("Duct", subelemElement[0].ToString());

            var getAllParams = GetFlattenedPreviewValues("01312066e09b4b1e86b76bad85944e9f");
            Assert.AreEqual(20, getAllParams.Count);

            var getParamValue = GetFlattenedPreviewValues("c0448b187dc74bce8e0ecbfba591d0fc");
            Assert.AreEqual(20, getParamValue.Count);
            Assert.AreEqual("Supply Air", getParamValue[3].ToString());
        }
    }
}
