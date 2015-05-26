using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;

using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class MaterialTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Material\GetMaterialByName.rfa")]
        public void GetMaterialByName()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Material\GetMaterialByName.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();
            Assert.AreEqual(13, ViewModel.Model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(12, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check material name
            var material1ID = "bfb04f5e-9c61-419b-a50d-e183f19c402c";
            Assert.IsNotNull(GetPreviewValue(material1ID) as Material);

            var material2ID = "09a8c02a-377d-408a-9dda-df9977c9b685";
            Assert.IsNotNull(GetPreviewValue(material2ID) as Material);

            var material3ID = "a72bd284-0eec-4a24-8674-07ba3c387e86";
            Assert.IsNotNull(GetPreviewValue(material3ID) as Material);

            //check Element.SetParameterByName
            var element1ID = "996fded3-74d5-4d65-96f4-100c6fbf45f7";
            var element1 = GetPreviewValue(element1ID) as Element;
            Assert.IsNotNull(element1);

            var element2ID = "e64ce205-a3d3-4d2a-827e-461f47316b83";
            var element2 = GetPreviewValue(element2ID) as Element;
            Assert.IsNotNull(element2);

            var element3ID = "bfe01e63-2cb4-4b2e-8dae-64d9b64a9969";
            var element3 = GetPreviewValue(element3ID) as Element;
            Assert.IsNotNull(element3);
        }
    }
}
