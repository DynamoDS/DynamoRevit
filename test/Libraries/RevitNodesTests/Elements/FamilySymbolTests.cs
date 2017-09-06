using System;

using NUnit.Framework;

using Revit.Elements;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FamilyTypeTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByName_GoodArgs()
        {
            var famTyp = FamilyType.ByName("Box");
            Assert.NotNull(famTyp);
            Assert.AreEqual("Box", famTyp.Name);
            Assert.AreEqual("Box", famTyp.Family.Name);
        }

        [Test, Category("Failure")]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByName_BadArgs()
        {
            Assert.Throws(typeof(Exception), () => FamilyType.ByName("Turtle.BoxTurtle") );
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyType.ByName(null));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByFamilyAndName_GoodArgs()
        {
            var fam = Family.ByName("Box");
            var famTyp = FamilyType.ByFamilyAndName(fam, "Box");
            Assert.NotNull(famTyp);
            Assert.AreEqual("Box", famTyp.Name);
            Assert.AreEqual("Box", famTyp.Family.Name);
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByFamilyAndName_BadArgs()
        {
            var fam = Family.ByName("Box");
            Assert.Throws(typeof(Exception), () => FamilyType.ByFamilyAndName(fam, "Turtle"));
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyType.ByFamilyAndName(fam, null));
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyType.ByFamilyAndName(null, "Turtle"));
        }

        //TODO: Turn this test on once template files are installed on CI machines
        [Ignore]
        [TestModel(@".\empty.rvt")]
        public void ByGeometry_GoodArgs()
        {
            var cube = Autodesk.DesignScript.Geometry.Cuboid.ByCorners(
                Autodesk.DesignScript.Geometry.Point.ByCoordinates(0,0,0),
                Autodesk.DesignScript.Geometry.Point.ByCoordinates(100,100,100));

            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path = System.IO.Path.Combine(path, @"Autodesk\RVT 2017\Family Templates\English\Metric Generic Model.rft");

            var type = FamilyType.ByGeometry(cube, "MyCube", Category.ByName("Mass"), path, Material.ByName("Poche"));
            Assert.NotNull(type);
            Assert.IsTrue(typeof(FamilyType) == type.GetType());
        }
    }
}
