using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using System.Collections.Generic;

namespace RevitSystemTests
{
    [TestFixture]
    public class AreaTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\Area\areaTests.rvt")]
        public void CanGetAreaBoundaries()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Area\canGetAreaBoundaries.dyn");
            string testPath = Path.GetFullPath(samplePath);

            var expectedCurves = new List<Autodesk.DesignScript.Geometry.Curve>()
            {

                Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(6223.036, 3184.616, 0.000),
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(-8076.964, 3184.616, 0.000)),
                Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(-8221.964, 3039.616, 0.000),
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(-8221.964, -2660.384, 0.000)),
                Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(-8076.964, -2805.384, 0.000),
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(6223.036, -2805.384, 0.000)),
                Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(6368.036, -2660.384, 0.000),
                    Autodesk.DesignScript.Geometry.Point.ByCoordinates(6368.036, 3039.616, 0.000)),

            };

            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var areaBoundaries = GetFlattenedPreviewValues("295e59563b2c4f4fbed6e9fbeffb53ac");

            // Assert
            AssertListOfCurves(expectedCurves, areaBoundaries);
        }

        private static void AssertListOfCurves(List<Autodesk.DesignScript.Geometry.Curve> expectedCurves, List<object> areaBoundaries)
        {
            Assert.AreEqual(expectedCurves.Count(), areaBoundaries.Count());
            for (int i = 0; i < areaBoundaries.Count(); i++)
            {
                var expected = expectedCurves[i];
                var actual = (Autodesk.DesignScript.Geometry.Curve)areaBoundaries[i];

                var expectedStartPoint = expected.StartPoint;
                var expectedEndPoint = expected.EndPoint;
                var actualStartPoint = actual.StartPoint;
                var actualEndPoint = actual.EndPoint;

                Assert.AreEqual(expectedStartPoint.X, actualStartPoint.X, Tolerance);
                Assert.AreEqual(expectedStartPoint.Y, actualStartPoint.Y, Tolerance);
                Assert.AreEqual(expectedStartPoint.Z, actualStartPoint.Z, Tolerance);

                Assert.AreEqual(expectedEndPoint.X, actualEndPoint.X, Tolerance);
                Assert.AreEqual(expectedEndPoint.Y, actualEndPoint.Y, Tolerance);
                Assert.AreEqual(expectedEndPoint.Z, actualEndPoint.Z, Tolerance);

            }
        }
    }
}
