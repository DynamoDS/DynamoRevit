using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class DimensionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Dimension()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Dimension.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim = GetPreviewValue("9ecb0603-0fcd-42da-b9da-ec5c57e1deaf");

            Assert.AreEqual(dim.GetType(), typeof(Revit.Elements.Dimension));


        }
    }
}