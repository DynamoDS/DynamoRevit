using System.IO;
using System.Linq;

using Autodesk.Revit.DB;

using NUnit.Framework;
using Revit.GeometryConversion;
using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class LinkElementTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\LinkElement\Model.rvt")]
        public void GetBoundingBox()
        {
            var samplePath = Path.Combine(workingDirectory, @".\LinkElement\BoundingBox.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var boundingbox = GetPreviewValue("23f965f67fa1455b8cc5798cf44f982d");

            Assert.AreEqual(boundingbox.GetType(), typeof(Autodesk.DesignScript.Geometry.BoundingBox));

            var bb = boundingbox as Autodesk.DesignScript.Geometry.BoundingBox;

            var min = new XYZ(-14.079682327109818, 10.576967122508846, 0);
            var max = new XYZ(18.586984339556878, 11.243633789175512, 20.0);

            Assert.AreEqual(0.0, min.DistanceTo(bb.MinPoint.ToRevitType()), 1e-6);
            Assert.AreEqual(0.0, max.DistanceTo(bb.MaxPoint.ToRevitType()), 1e-6);
        }
    }
}
