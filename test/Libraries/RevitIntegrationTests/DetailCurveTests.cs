using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class DetailCurveTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void DetailCurve()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\DetailCurve.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var dim = GetPreviewValue("f36fee12-d733-4544-b02d-76c5bc9cdbac");

            Assert.AreEqual(dim.GetType(), typeof(Revit.Elements.DetailCurve));


        }
    }
}