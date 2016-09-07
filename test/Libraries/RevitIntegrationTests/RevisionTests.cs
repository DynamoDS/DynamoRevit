using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class RevisionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Revision()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Revisions.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("98da559a-b823-4379-be25-36eddce3dd65");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.RevisionCloud));



            var rev = GetPreviewValue("4f40afa4-82b8-4860-940e-96909aa4ab40");
            Assert.AreEqual(rev.GetType(), typeof(Revit.Elements.Revision));

        }
    }
}