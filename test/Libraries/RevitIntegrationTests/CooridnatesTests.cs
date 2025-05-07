using System.IO;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;



namespace RevitSystemTests
{
    [TestFixture]
    class CoordinatesTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeViewNew.rvt")]
        public void GetProjectCoordinates()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Coordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            //expected values of Survey and Base points
            //you will see that if you click on Survey or Base point, the values will be different - that's because
            //the points are calculated relative to the internal origin which was changed at the rotation of the project
            //the values will be the same as the ones from the annotations
            var expectedSurveyPoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(3.000, -5.000, 0.000);
            var expectedBasePoint = Autodesk.DesignScript.Geometry.Point.ByCoordinates(-5.000, -5.000, 0.000);

            var surveyPoint = GetPreviewValue("ad5bb1c3-00eb-472c-92a8-d9dafe805939") as Autodesk.DesignScript.Geometry.Point;
            var basePoint = GetPreviewValue("81ad46c7-29f2-4913-a1f5-832a39b62bcd") as Autodesk.DesignScript.Geometry.Point;
            var rotationPoint = GetPreviewValue("77ce2b6f-5d4a-4f45-85b8-bc2c1bac9134");

            const double Tolerance = 0.001;
            //Verify every coordinate of Survey Point
            Assert.AreEqual(expectedSurveyPoint.X, surveyPoint.X, Tolerance);
            Assert.AreEqual(expectedSurveyPoint.Y, surveyPoint.Y, Tolerance);
            Assert.AreEqual(expectedSurveyPoint.Z, surveyPoint.Z, Tolerance);

            //Verify every coordinate of Base Point
            Assert.AreEqual(expectedBasePoint.X, basePoint.X, Tolerance);
            Assert.AreEqual(expectedBasePoint.Y, basePoint.Y, Tolerance);
            Assert.AreEqual(expectedBasePoint.Z, basePoint.Z, Tolerance);

            //Verify the rotation has a double value and is equal to 270 degrees
            double? rotation = rotationPoint as double?;
            Assert.IsNotNull(rotation);
            Assert.AreEqual(rotation.Value, 270.0, Tolerance);
        }

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void GetProjectCoordinatesZeroCoord()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Coordinates.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var surveyPoint = GetPreviewValue("ad5bb1c3-00eb-472c-92a8-d9dafe805939") as Autodesk.DesignScript.Geometry.Point;
            var basePoint = GetPreviewValue("81ad46c7-29f2-4913-a1f5-832a39b62bcd") as Autodesk.DesignScript.Geometry.Point;
            var rotationPoint = GetPreviewValue("77ce2b6f-5d4a-4f45-85b8-bc2c1bac9134");

            Assert.AreEqual(surveyPoint, Autodesk.DesignScript.Geometry.Point.Origin());
            Assert.AreEqual(basePoint, Autodesk.DesignScript.Geometry.Point.Origin());
            Assert.AreEqual(rotationPoint, 0.0);
        }
    }
}