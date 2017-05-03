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

        [Test]
        [TestModel(@".\Room_Properties.rvt")]
        public void RoomProperties()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Room_Properties.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var area = GetPreviewValue("cfe548b0-42bf-4480-b561-0ace4fc4d68d");

            Assert.AreEqual(12.87, (double)area, 1e-9);

            var height = GetPreviewValue("636279f6-d676-4fe8-9d57-fa2c8bc28af6");
            Assert.AreEqual(4000, (double)height, 1e-9);

            var number = GetPreviewValue("47871f8b-994e-4fab-8153-0e8b0fb925c4");
            Assert.AreEqual("101", number);
        }
    }
}