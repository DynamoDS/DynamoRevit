using System;
using Revit.Elements;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{

    [TestFixture]
    public class CoordinatesTests : RevitNodeTestBase
    {

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void BasePoint_Valid()
        {
            var basepoint = Coordinates.BasePoint();
            Assert.IsNotNull(basepoint);
            basepoint.ShouldBeApproximately(Autodesk.DesignScript.Geometry.Point.Origin());
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void SurveyPoint_Valid()
        {
            var spoint = Coordinates.SurveyPoint();
            Assert.IsNotNull(spoint);
            spoint.ShouldBeApproximately(Autodesk.DesignScript.Geometry.Point.Origin());
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ProjectRotationPoint_Valid()
        {
            var rot = Coordinates.ProjectRotation();
            Assert.IsNotNull(rot);
            rot.ShouldBeApproximately(0.0);
        }


    }
}

