using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class ModelTextTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\ModelText\ModelText.rfa")]
        public void ModelText()
        {
            string samplePath = Path.Combine(workingDirectory, @".\ModelText\ModelText.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();
            RunCurrentModel();

            var modelText = GetFlattenedPreviewValues("f2e6e48d704648bdbdd039c1eada4375");
            Assert.IsNotNull(modelText);
            Assert.AreEqual(101, modelText.Count);
        }
    }
}
