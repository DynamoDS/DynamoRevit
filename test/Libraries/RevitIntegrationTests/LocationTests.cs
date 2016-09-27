using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class LocationTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void FilterRule()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Location.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("9501a47b-25e1-4861-9f58-068c9b3d3501");
            Assert.AreEqual(type.GetType(), typeof(Autodesk.DesignScript.Geometry.Line));



        }
    }
}