using Autodesk.Revit.DB;
using NUnit.Framework;
using Revit.Elements.InternalUtilities;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using System;
using FamilyInstance = Revit.Elements.FamilyInstance;
using FamilyType = Revit.Elements.FamilyType;
using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FamilyInstanceTests : RevitNodeTestBase
    {
        public Autodesk.Revit.DB.XYZ InternalLocation(Autodesk.Revit.DB.FamilyInstance instance)
        {
            return (instance.Location as LocationPoint).Point;
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByCoordinates_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            var famSym = FamilyType.ByName("Box");
            var famInst = FamilyInstance.ByCoordinates(famSym, 0, 1, 2);
            Assert.NotNull(famInst);

            var position = famInst.Location;

            position.ShouldBeApproximately(Point.ByCoordinates(0, 1, 2));
    
            // no unit conversion
            var internalPos =
                InternalLocation(famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance);

            (internalPos * UnitConverter.HostToDynamoFactor(UnitType.UT_Length)).ShouldBeApproximately(
                Point.ByCoordinates(0, 1, 2));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByPoint_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            var famSym = FamilyType.ByName("Box");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famSym, pt);
            Assert.NotNull(famInst);

            var position = famInst.Location;

            position.ShouldBeApproximately(Point.ByCoordinates(0, 1, 2));

            // no unit conversion
            var internalPos =
                InternalLocation(famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance);

            (internalPos * UnitConverter.HostToDynamoFactor(UnitType.UT_Length)).ShouldBeApproximately(
                Point.ByCoordinates(0, 1, 2));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByPoint_NullFamilySymbol()
        {
            var pt = Point.ByCoordinates(0, 1, 2);
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyInstance.ByPoint(null, pt));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByPoint_NullPoint()
        {
            var famSym = FamilyType.ByName("Box");
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyInstance.ByPoint(famSym, null));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByCoordinates_NullFamilySymbol()
        {
            var pt = Point.ByCoordinates(0, 1, 2);
            Assert.Throws(typeof(System.ArgumentNullException), () => FamilyInstance.ByCoordinates(null, 0, 1, 2));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void Rotate_ZAxis()
        {
            var famSym = FamilyType.ByName("Box");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famSym, pt);
            Assert.NotNull(famInst);
          
            var transform = famInst.InternalFamilyInstance.GetTransform();
            double[] rotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
            Assert.AreEqual(0.0, rotationAngles[0]);
          
            RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument.Regenerate();
          
            famInst.SetRotation(30);
            transform = famInst.InternalFamilyInstance.GetTransform();
            TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
            Assert.AreEqual(30.0, rotationAngles[0] * 180 / Math.PI, 1.0e-6);
          
            famInst.SetRotation(60);
            transform = famInst.InternalFamilyInstance.GetTransform();
            TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
            Assert.AreEqual(60.0, rotationAngles[0] * 180 / Math.PI, 1.0e-6);
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void FacingOrientation()
        {
            var famSym = FamilyType.ByName("Box");
            var pt = Point.ByCoordinates(0, 1, 2);
            var famInst = FamilyInstance.ByPoint(famSym, pt);
            Assert.NotNull(famInst);

            var dir = famInst.FacingOrientation;
            dir.IsAlmostEqualTo(Vector.ByCoordinates(0.0, 0.0, 1.0));
        }

        [Test]
        [TestModel(@".\WallFamily.rvt")]
        public void ByFace_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            var familytype1 = FamilyType.ByName("WallFamily");
            var plane1 = Autodesk.DesignScript.Geometry.Plane.XY();
            var rectangle1 = Autodesk.DesignScript.Geometry.Rectangle.ByWidthLength(plane1, 1000, 1000);
            var surface1 = Autodesk.DesignScript.Geometry.Surface.ByPatch(rectangle1);
            var importinstance1 = Revit.Elements.ImportInstance.ByGeometry(surface1);
            var t2 = importinstance1.Geometry();
            var s = t2[0] as Autodesk.DesignScript.Geometry.Surface;
            var point1 = Point.ByCoordinates(0, 0, 0);
            var vector1 = Vector.YAxis();
            var famInst = FamilyInstance.ByFace(familytype1, s, point1, vector1);

            Assert.NotNull(famInst);

            var position = famInst.Location;
            position.ShouldBeApproximately(point1);
        }
    }
}
