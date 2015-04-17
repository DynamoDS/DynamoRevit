using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class FamilyTests : RevitSystemTestBase
    {   
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

            Assert.IsTrue(IsFuzzyEqual(pnt.X, -31.453185696, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, -115.515869423, 1.0e-6));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 26.806669948, 1.0e-6));
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
           Assert.AreEqual(30.0, rotationAngles[0] * 180 / System.Math.PI, 1.0e-6);
        }
    }
}
