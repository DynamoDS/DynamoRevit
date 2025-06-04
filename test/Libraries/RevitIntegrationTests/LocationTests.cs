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
        public void Location()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Location\Location.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var getLocationNode = GetPreviewValue("9501a47b-25e1-4861-9f58-068c9b3d3501");
            Assert.AreEqual(getLocationNode.GetType(), typeof(Autodesk.DesignScript.Geometry.Line));

            var expectedlocation = "Line(StartPoint = Point(X = 0.000, Y = 0.000, Z = 0.000), " +
                "EndPoint = Point(X = 0.000, Y = 300.000, Z = 0.000), Direction = Vector(X = 0.000, Y = 300.000, Z = 0.000, Length = 300.000))";
            Assert.AreEqual(getLocationNode.ToString(), expectedlocation);

        }
    }
}