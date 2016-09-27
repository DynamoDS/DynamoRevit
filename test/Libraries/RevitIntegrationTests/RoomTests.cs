using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class RoomTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Room()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Room.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("2774f84d-e233-46a1-aa8d-3aa884757207");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.Room));


        }
    }
}