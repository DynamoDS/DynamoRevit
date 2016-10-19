using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class FilledRegionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void FilledRegion()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\FilledRegion.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("eb337891-f493-4123-9f2d-814361863ec4");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.FilledRegionType));

            var element = GetPreviewValue("2649b29c-9743-4872-a5f2-ac4892d64676");
            Assert.AreEqual(element.GetType(), typeof(Revit.Elements.FilledRegion));


        }
    }
}