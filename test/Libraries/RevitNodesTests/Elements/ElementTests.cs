using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryReferences;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using Element = Revit.Elements.Element;
using FamilyType = Revit.Elements.FamilyType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class ElementTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\materials.rvt")]
        public void SetParameterByName_Element_CanSuccessfullySetMaterialByElement()
        {

            var mat = Revit.Elements.Material.ByName("Glass");

            var ele = ElementSelector.ByType<Autodesk.Revit.DB.FamilyInstance>(true).First();

            var paramName = "Body Material";
            var elemId0 = ele.GetParameterValueByName(paramName);

            Assert.AreNotEqual( mat.Id, elemId0 );

            ele.SetParameterByName(paramName, mat);

            DocumentManager.Regenerate();

            var elemId1 = ele.GetParameterValueByName(paramName) as Element;

            Assert.AreEqual(mat.InternalElement.Id, elemId1.InternalElement.Id);

        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullyDeleteElement()
        {
            // Id of wall
            const int wallId = 184176;

            // Get element from document
            using (Element wall = ElementSelector.ByElementId(wallId, true))
            {
                Assert.IsNotNull(wall);

                // Delete Element
                int[] deleted = Element.Delete(wall);

                // Confirm list of elements represent the wall requested to delete. 
                Assert.AreEqual(1, deleted.Length);
                Assert.AreEqual(wallId, deleted[0]);
            }
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySetAndGetElement()
        {
            var wall = ElementSelector.ByElementId(184176, true);
            var famSym = FamilyType.ByName("18\" x 18\"");

            var name = "Column";
            wall.SetParameterByName(name, famSym);
            var sym = wall.GetParameterValueByName(name) as Element;
            Assert.NotNull(sym);
            Assert.AreEqual(sym.Name, "18\" x 18\"");
        }

        [Test] 
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySetElementParamWithUnitType()
        {
            var wall = ElementSelector.ByElementId(184176, true);

            SetExpectedWallHeight(wall, 45.5);
            GetExpectedWallHeight(wall, 45.5);

            // Change project to meters
            var units = DocumentManager.Instance.CurrentDBDocument.GetUnits();
            units.SetFormatOptions(UnitType.UT_Length, new FormatOptions(DisplayUnitType.DUT_METERS));
            DocumentManager.Instance.CurrentDBDocument.SetUnits(units);

            SetExpectedWallHeight(wall, 45.5);
            GetExpectedWallHeight(wall, 45.5);
        }

        private static void SetExpectedWallHeight(Element wall, double value)
        {
            var name = "Unconnected Height";
            wall.SetParameterByName(name, value);
        }

        private static void GetExpectedWallHeight(Element wall, double value)
        {
            var name = "Unconnected Height";
            var height = (double)(wall.GetParameterValueByName(name));

            Assert.NotNull(height);
            height.ShouldBeApproximately(value);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullyGetElementParamWithUnitType()
        {
            var wall = ElementSelector.ByElementId(184176, true);
            GetExpectedWallHeight(wall, 20);
        }

        #region Face/Solid Extraction

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void Solids_ExtractsSolidAccountingForInstanceTransform()
        {
            var ele = ElementSelector.ByElementId(46874, true);
            var solids = ele.Solids;

            Assert.AreEqual(1, solids.Length);

            var bbox = BoundingBox.ByGeometry(solids);

            bbox.MaxPoint.ShouldBeApproximately(-210.846457, -26.243438, 199.124016, 1e-2);
            bbox.MinPoint.ShouldBeApproximately(-304.160105, -126.243438, 0, 1e-2);
        }

        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void Faces_ExtractsFacesAccountingForInstanceTransform()
        {
            var ele = ElementSelector.ByElementId(46874, true);
            var faces = ele.Faces;

            Assert.AreEqual(6, faces.Length);

            var bbox = BoundingBox.ByGeometry(faces);

            bbox.MaxPoint.ShouldBeApproximately(-210.846457, -26.243438, 199.124016, 1e-2);
            bbox.MinPoint.ShouldBeApproximately(-304.160105, -126.243438, 0, 1e-2);

            var refs = faces.Select(x => ElementFaceReference.TryGetFaceReference(x));

            foreach (var refer in refs)
            {
                Assert.AreEqual(46874, refer.InternalReference.ElementId.IntegerValue);
            }
        }
        
        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void Geometry_ExtractsSolidAccountingForInstanceTransform()
        {
            var ele = ElementSelector.ByElementId(46874, true);
            var objects = ele.Geometry();
            Assert.AreEqual(1, objects.Length);

            var solids = objects.OfType<Autodesk.DesignScript.Geometry.Solid>();
            Assert.AreEqual(1, solids.Count());

            var bbox = BoundingBox.ByGeometry(solids);

            bbox.MaxPoint.ShouldBeApproximately(-210.846457, -26.243438, 199.124016, 1e-2);
            bbox.MinPoint.ShouldBeApproximately(-304.160105, -126.243438, 0, 1e-2);
        }
        
        [Test]
        [TestModel(@".\AdaptiveComponents.rfa")]
        public void ElementFaceReferences_ExtractsExpectedReferences()
        {
            var ele = ElementSelector.ByElementId(46874, true);
            var refs = ele.ElementFaceReferences;

            Assert.AreEqual(6, refs.Length);

            foreach (var refer in refs)
            {
                Assert.AreEqual(46874, refer.InternalReference.ElementId.IntegerValue);
            }
        }

        #endregion

        #region Curve extraction

        [Test]
        [TestModel(@".\projectWithNestedNonSharedAdaptiveComponent.rvt")]
        public void Curves_ExtractsCurvesFromNestedNonSharedFamilyInstanceAccountingForInstanceTransform()
        {
            var ele = ElementSelector.ByElementId(186006, true);
            var crvs = ele.Curves;

            Assert.AreEqual(4, crvs.Length);
            Assert.AreEqual(4, crvs.OfType<Autodesk.DesignScript.Geometry.Line>().Count());

            var bbox = BoundingBox.ByGeometry(crvs);

            bbox.MinPoint.ShouldBeApproximately(-103.697, -88.156, 0, 1e-2);
            bbox.MaxPoint.ShouldBeApproximately(83.445, 108.963, 0, 1e-2);

            var refs = crvs.Select(x => ElementCurveReference.TryGetCurveReference(x));

            foreach (var refer in refs)
            {
                Assert.AreEqual(186006, refer.InternalReference.ElementId.IntegerValue);
            }
        }

        [Test]
        [TestModel(@".\GetCurvesFromFamilyInstance.rfa")]
        public void Curves_ExtractsCurvesAccountingForInstanceTransform()
        {
            var ele = ElementSelector.ByElementId(32107, true);
            var crvs = ele.Curves;

            Assert.AreEqual(4, crvs.Length);
            Assert.AreEqual(4, crvs.OfType<Autodesk.DesignScript.Geometry.Line>().Count());

            var bbox = BoundingBox.ByGeometry(crvs);

            bbox.MaxPoint.ShouldBeApproximately(50.0, 0, 0, 1e-3);
            bbox.MinPoint.ShouldBeApproximately(0, -100.0, 0, 1e-3);

            var refs = crvs.Select(x => ElementCurveReference.TryGetCurveReference(x));

            foreach (var refer in refs)
            {
                Assert.AreEqual(32107, refer.InternalReference.ElementId.IntegerValue);
            }
        }

        [Test]
        [TestModel(@".\GetCurvesFromFamilyInstance.rfa")]
        public void ElementCurveReferences_ExtractsExpectedReferences()
        {
            var ele = ElementSelector.ByElementId(32107, true);
            var refs = ele.ElementCurveReferences;

            Assert.AreEqual(4, refs.Length);

            foreach (var refer in refs)
            {
                Assert.AreEqual(32107, refer.InternalReference.ElementId.IntegerValue);
            }
        }

        #endregion

        [Test]
        [TestModel((@".\Element\elementComponents.rvt"))]
        public void CanGetElementChildElements()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(316153, true);
            var window = ElementSelector.ByElementId(319481, true);
            var beamSystem = ElementSelector.ByElementId(319537, true);
            var stair = ElementSelector.ByElementId(316246, true);
            var railing = ElementSelector.ByElementId(319643, true);

            var expectedExceptionMessageWallChildElement = Revit.Properties.Resources.NoChildElements;
            var expectedWindowChildElement = new List<int>() { 319484, 319485 };
            var expectedBeamChildElements = new List<int>() { 319563, 319575, 319577, 319579, 319581 };
            var expectedStairChildElements = new List<int>() { 316286, 316288, 316289 };
            var expectedRailingChildElements = new List<int>() { 319683 };

            // Act
            var resultWindowChildElements = window.GetChildElements().Select(x => x.Id).ToList();
            var resultBeamSystemChildElements = beamSystem.GetChildElements().Select(x => x.Id).ToList();
            var resultStairChildElements = stair.GetChildElements().Select(x => x.Id).ToList();
            var resultRailingChildElements = railing.GetChildElements().Select(x => x.Id).ToList();
            var wallChildElementsException = Assert.Throws<System.NullReferenceException>(() => wall.GetChildElements());

            // Assert
            Assert.AreEqual(wallChildElementsException.Message, expectedExceptionMessageWallChildElement);
            CollectionAssert.AreEqual(expectedWindowChildElement, resultWindowChildElements);
            CollectionAssert.AreEqual(expectedBeamChildElements, resultBeamSystemChildElements);
            CollectionAssert.AreEqual(expectedStairChildElements, resultStairChildElements);
            CollectionAssert.AreEqual(expectedRailingChildElements, resultRailingChildElements);

        }

        [Test]
        [TestModel((@".\Element\elementComponents.rvt"))]
        public void CanGetElementParentElement()
        {
            // Arrange
            var wall = ElementSelector.ByElementId(316153, true);
            var window = ElementSelector.ByElementId(319485, true);
            var beam = ElementSelector.ByElementId(319579, true);

            var expectedExceptionMessageWallSubComponents = Revit.Properties.Resources.NoParentElement;
            var expectedWindowParentElement = 319481;
            var expectedBeamParentElement = 319537;

            // Act
            var wallParentElementException = Assert.Throws<System.InvalidOperationException>(() => wall.GetParentElement());
            var resultWindowParentElement = window.GetParentElement().Id;
            var resultBeamParentElement = beam.GetParentElement().Id;

            // Assert
            Assert.AreEqual(wallParentElementException.Message, expectedExceptionMessageWallSubComponents);
            Assert.AreEqual(expectedWindowParentElement, resultWindowParentElement);
            Assert.AreEqual(expectedBeamParentElement, resultBeamParentElement);
        }
        
        [Test]
        [TestModel((@".\Element\elementTransform.rvt"))]
        public void CanTransformElement()
        {
            // Arrange
            var delta = 0.001;
            var originPoint = Coordinates.BasePoint();  
            var fromCS = CoordinateSystem.ByOrigin(originPoint);
            var translatedOriginPoint = originPoint.Translate(Vector.XAxis(), 10000) as Autodesk.DesignScript.Geometry.Point;
            var contextCS = CoordinateSystem.ByOrigin(translatedOriginPoint).Rotate(translatedOriginPoint,
                                                                                    Vector.ZAxis(),
                                                                                    25);
            var wall = ElementSelector.ByElementId(316150);
            var rectangularColumn = ElementSelector.ByElementId(318266);
            var steelColumn = ElementSelector.ByElementId(316180);
            var lineBasedFamily = ElementSelector.ByElementId(317296);

            var expectedRectangularColumnLocation = Autodesk.DesignScript.Geometry.Point.ByCoordinates(5317.185,-364.393,0);
            var expectedLineBasedFamilyLocation = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point.ByCoordinates(6192.436, 989.836, 0),
                                                                                                           Autodesk.DesignScript.Geometry.Point.ByCoordinates(7220.897, 2215.507, 0));

            var originalRectangularColumnLocation = Autodesk.DesignScript.Geometry.Point.ByCoordinates(-5924.398, -81.245, 0); ;
            var originalLineBasedFamilyLocation = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Autodesk.DesignScript.Geometry.Point.ByCoordinates(-4324.398, 118.755, 0),
                                                                                                           Autodesk.DesignScript.Geometry.Point.ByCoordinates(-2724.398, 118.755, 0));

            // Act
            var transformlineBasedFamily = lineBasedFamily.Transform(fromCS, contextCS);
            var transformedlineBasedFamilyLocation = transformlineBasedFamily.GetLocation() as Autodesk.DesignScript.Geometry.Line;
            var transformRectangularColumn = rectangularColumn.Transform(fromCS, contextCS);
            var transformedRectangularColumnLocation = transformRectangularColumn.GetLocation() as Autodesk.DesignScript.Geometry.Point;
            var wallEx = Assert.Throws<System.Exception>(() => wall.Transform(fromCS, contextCS));
            var steelColumnEx = Assert.Throws<System.Exception>(() => steelColumn.Transform(fromCS, contextCS));


            // Assert - Elements have moved
            Assert.AreNotEqual(originalRectangularColumnLocation.X, transformedRectangularColumnLocation.X);
            Assert.AreNotEqual(originalRectangularColumnLocation.Y, transformedRectangularColumnLocation.Y);
            Assert.AreNotEqual(originalLineBasedFamilyLocation.StartPoint.X, transformedlineBasedFamilyLocation.StartPoint.X);
            Assert.AreNotEqual(originalLineBasedFamilyLocation.StartPoint.Y, transformedlineBasedFamilyLocation.StartPoint.Y);

            // Assert - Elements are in correct position
            Assert.AreEqual(expectedRectangularColumnLocation.X, transformedRectangularColumnLocation.X, delta);
            Assert.AreEqual(expectedRectangularColumnLocation.Y, transformedRectangularColumnLocation.Y, delta);
            Assert.AreEqual(expectedLineBasedFamilyLocation.StartPoint.X, transformedlineBasedFamilyLocation.StartPoint.X, delta);
            Assert.AreEqual(expectedLineBasedFamilyLocation.StartPoint.Y, transformedlineBasedFamilyLocation.StartPoint.Y, delta);
        }
    }
}
