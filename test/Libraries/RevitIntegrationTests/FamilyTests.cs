using System.IO;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using NUnit.Framework;
using RevitTestServices;

using RTF.Framework;
using FamilyInstance = Revit.Elements.FamilyInstance;

namespace RevitSystemTests
{
    [TestFixture]
    class FamilyTests : RevitSystemTestBase
    {
        public const double Epsilon = 1e-6;

        [Test]
        [TestModel(@".\Family\GetFamilyInstancesByType.rvt")]
        public void GetFamilyInstancesByType()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Family\GetFamilyInstancesByType.dyn");
            string testPath = Path.GetFullPath(samplePath);


            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            Assert.AreEqual(100, GetPreviewValue("5eac6ab9-e736-49a9-a90a-8b6d93676813"));
        }

        [Test]
        [TestModel(@".\Family\GetFamilyInstanceLocation.rvt")]
        public void GetFamilyInstanceLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\GetFamilyInstanceLocation.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            AssertPreviewCount("4274fd18-23b8-4c5c-9006-14d927fa3ff3", 100);

        }

        [Test]
        [TestModel(@".\Family\AC_locationStandAlone.rfa")]
        public void CanLocateAdaptiveComponent()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Family\AC_locationStandAlone.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var pnt = GetPreviewValue("79dde258-ddce-49b7-9700-da21b2d5a9ae") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, -31.453185696, Epsilon));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, -115.515869423, Epsilon));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 26.806669948, Epsilon));
        }

        [Test]
        [TestModel(@".\Family\AC_locationInDividedSurface.rfa")]
        public void CanLocateAdaptiveComponentInDividedSurface()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\AC_locationInDividedSurface.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            AssertPreviewCount("76076507-d16e-4480-802c-14ba87d88f81", 25);
        }

        [Test]
        [TestModel(@".\Family\SetFamilyInstanceRotation.rvt")]
        public void SetFamilyInstanceRotation()
        {
           string samplePath = Path.Combine(workingDirectory, @".\Family\SetFamilyInstanceRotation.dyn");
           string testPath = Path.GetFullPath(samplePath);

           ViewModel.OpenCommand.Execute(testPath);

           RunCurrentModel();
           var elId = new Autodesk.Revit.DB.ElementId(187030);
           var famInst = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument.GetElement(elId) as Autodesk.Revit.DB.FamilyInstance;
           var transform = famInst.GetTransform();
           double[] rotationAngles;
           Revit.Elements.InternalUtilities.TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
           Assert.AreEqual(30.0, rotationAngles[0] * 180 / System.Math.PI, Epsilon);
        }

        [Test]
        [TestModel(@".\Family\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\FamilyInstancePlacementByFace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var famInst = GetPreviewValue("e8dbd9fa-c0fd-4a5c-9cd8-2616f98285c8") as FamilyInstance;
            Assert.IsNotNull(famInst);

            var pnt = GetPreviewValue("31171a34-edd6-4c81-b821-2ad0bae36aab") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            var pos = famInst.Location;

            Assert.AreEqual(pnt.X, pos.X, Epsilon, "X property does not match");
            Assert.AreEqual(pnt.Y, pos.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(pnt.Z, pos.Z, Epsilon, "Z property does not match");

            var elId = new Autodesk.Revit.DB.ElementId(259536);
            var internalFamInst = famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Assert.AreEqual(elId, internalFamInst.HostFace.ElementId);
        }
    }
}
