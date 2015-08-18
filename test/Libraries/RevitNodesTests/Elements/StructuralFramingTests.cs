using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;

using Revit.Elements;
using NUnit.Framework;
using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class StructuralFramingTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCurveLevelUpVectorAndType_ValidArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);
            
            var level = Level.ByElevation(3);
            var up = Vector.ByCoordinates(0, 0, 1);

            var structuralType = StructuralType.Beam;
            var famTyp = FamilyType.ByName("W12X26");

            var structure = StructuralFraming.ByCurveLevelUpVectorAndType(line, level, up, structuralType, famTyp);

            Assert.NotNull(structure);
            Assert.NotNull(structure.InternalElement);
            Assert.IsTrue( DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(structure.InternalElement.UniqueId) )) ;
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByCurveLevelUpVectorAndType_BadArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);

            var level = Level.ByElevation(3);
            var up = Vector.ByCoordinates(0, 0, 1);

            var structuralType = StructuralType.Beam;
            var famTyp = FamilyType.ByName("W12X26");

            Assert.Throws(typeof(System.ArgumentNullException), () => StructuralFraming.ByCurveLevelUpVectorAndType(null, level, up, structuralType, famTyp));
            Assert.Throws(typeof(System.ArgumentNullException), () => StructuralFraming.ByCurveLevelUpVectorAndType(line, null, up, structuralType, famTyp));
            Assert.Throws(typeof(System.ArgumentNullException), () => StructuralFraming.ByCurveLevelUpVectorAndType(line, level, null, structuralType, famTyp));
            Assert.Throws(typeof(System.ArgumentNullException), () => StructuralFraming.ByCurveLevelUpVectorAndType(line, level, up, structuralType, null));
        }

        [Test]
        [TestModel(@".\StructuralFramingLocationTest.rvt")]
        public void TestLocationOfStructuralFraming()
        {
            var e = ElementSelector.ByType<Autodesk.Revit.DB.FamilyInstance>(true).FirstOrDefault();
            Assert.NotNull(e);

            var beam = e as StructuralFraming;
            Assert.NotNull(beam);
            var curve = beam.Location;
            Assert.NotNull(curve.StartPoint);
            Assert.NotNull(curve.EndPoint);
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Beam_ValidArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            var lineBeam = StructuralFraming.BeamByCurve(line, level, famTyp);

            Assert.NotNull(lineBeam);
            Assert.NotNull(lineBeam.InternalElement);
            Assert.IsTrue(DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(lineBeam.InternalElement.UniqueId)));

            var curve = Arc.ByCenterPointRadiusAngle(Point.Origin(), 20, 0, 90, Vector.ZAxis());

            var arcBeam = StructuralFraming.BeamByCurve(curve, level, famTyp);

            Assert.NotNull(arcBeam);
            Assert.NotNull(arcBeam.InternalElement);
            Assert.IsTrue(DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(lineBeam.InternalElement.UniqueId)));
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Beam_BadArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BeamByCurve(null, level, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BeamByCurve(line, null, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BeamByCurve(line, level, null));
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Brace_ValidArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            var structure = StructuralFraming.BraceByCurve(line, level, famTyp);

            Assert.NotNull(structure);
            Assert.NotNull(structure.InternalElement);
            Assert.IsTrue(DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(structure.InternalElement.UniqueId)));
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Brace_BadArgs()
        {
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(5, 8, 3);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BraceByCurve(null, level, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BraceByCurve(line, null, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.BraceByCurve(line, level, null));
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Column_ValidArgs()
        {
            // A tilted column
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(1, 4, 12);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            var structure = StructuralFraming.ColumnByCurve(line, level, famTyp);

            Assert.NotNull(structure);
            Assert.NotNull(structure.InternalElement);
            Assert.IsTrue(DocumentManager.Instance.ElementExistsInDocument(
                 new ElementUUID(structure.InternalElement.UniqueId)));
        }

        [Test, TestModel(@".\Empty.rvt")]
        public void StructuralFraming_Column_BadArgs()
        {
            // A tilted column
            var start = Point.ByCoordinates(1, 2, 3);
            var end = Point.ByCoordinates(1, 4, 12);
            var line = Line.ByStartPointEndPoint(start, end);
            var level = Level.ByElevation(3);
            var famTyp = FamilyType.ByName("W12X26");

            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.ColumnByCurve(null, level, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.ColumnByCurve(line, null, famTyp));
            Assert.Throws<System.ArgumentNullException>(
                () => StructuralFraming.ColumnByCurve(line, level, null));

            // Updside down column
            end = Point.ByCoordinates(1, 2, 3);
            start = Point.ByCoordinates(1, 4, 12);
            line = Line.ByStartPointEndPoint(start, end);

            Assert.Throws<Exception>(() => StructuralFraming.ColumnByCurve(line, level, famTyp));
        }
    }
}
