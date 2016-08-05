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
            Assert.IsTrue(basepoint.Equals(Autodesk.DesignScript.Geometry.Point.Origin()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void BasePoint_Valid()
        {
            var spoint = Coordinates.SurveyPoint();
            Assert.IsNotNull(spoint);
            Assert.IsTrue(spoint.Equals(Autodesk.DesignScript.Geometry.Point.Origin()));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void BasePoint_Valid()
        {
            var rot = Coordinates.ProjectRotation();
            Assert.IsNotNull(rot);
            Assert.IsTrue(rot == 0.0);
        }


    }
}

