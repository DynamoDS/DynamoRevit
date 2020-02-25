using System;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.InternalUtilities;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using FamilyInstance = Revit.Elements.FamilyInstance;
using FamilyType = Revit.Elements.FamilyType;
using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FamilyInstanceTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.001;

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
        [TestModel(@".\Empty.rvt")]
        public void CanPlaceFamilyInstanceByTypeAndCoordinateSystem()
        {
            // Arrange
            var famType = ElementSelector.ByElementId(131610, true) as FamilyType;
            var coordinateSystem = CoordinateSystem.ByOrigin(0, 0)
                .Rotate(Autodesk.DesignScript.Geometry.Plane.ByOriginNormal(Point.ByCoordinates(0,0), Vector.ByCoordinates(0,0,1)), 45);

            var expectedFacingOrientation = Vector.ByCoordinates(-0.707, 0.707, 0.000);

            // Act
            var familyInstance = FamilyInstance.ByCoordinateSystem(famType, coordinateSystem);
            var facingOrientation = familyInstance.InternalFamilyInstance.FacingOrientation;

            // Assert
            Assert.AreEqual(expectedFacingOrientation.X, facingOrientation.X, Tolerance);
            Assert.AreEqual(expectedFacingOrientation.Y, facingOrientation.Y, Tolerance);
            Assert.AreEqual(expectedFacingOrientation.Z, facingOrientation.Z, Tolerance);
        }

        [Test]
        [TestModel(@".\FamilyInstance\familyInstanceTests.rvt")]
        public void CanGetFamilyInstanceRoom()
        {
            // Arrange
            var famInstance = ElementSelector.ByElementId(316162, true) as FamilyInstance;
            var famInstanceOutsideRoom = ElementSelector.ByElementId(316286, true) as FamilyInstance;

            var expectedRoomId = 316151;

            // Act
            var familyInstanceRoom = famInstance.Room;
            var roomId = familyInstanceRoom.Id;
            var outsideRoom = famInstanceOutsideRoom.Room;

            // Assert
            Assert.AreEqual(expectedRoomId, roomId);
            Assert.IsNull(outsideRoom);
        }

        [Test]
        [TestModel(@".\FamilyInstance\familyInstanceTests.rvt")]
        public void CanGetFamilyInstanceSpace()
        {
            // Arrange
            var famInstance = ElementSelector.ByElementId(316162, true) as FamilyInstance;
            var famInstanceOutsideSpace = ElementSelector.ByElementId(316286, true) as FamilyInstance;

            var expectedSpaceId = 316157;

            // Act
            var familyInstanceSpace = famInstance.Space;
            var spaceId = familyInstanceSpace.Id;
            var outsideSpace = famInstanceOutsideSpace.Space;

            // Assert
            Assert.AreEqual(expectedSpaceId, spaceId);
            Assert.IsNull(outsideSpace);
        }

    }
}
