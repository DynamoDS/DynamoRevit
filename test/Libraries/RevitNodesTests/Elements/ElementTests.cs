using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryReferences;

using RevitServices.Persistence;
using RevitServices.Transactions;
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

            Assert.AreNotEqual(mat.Id, elemId0);

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

        #region Pin settings

        /// <summary>
        /// gets the pinned status of an element from the model
        /// and checks if IsPinned is the correct value
        /// </summary>
        [Test]
        [TestModel(@".\Element\elementPinned.rvt")]
        public void CanSuccessfullyGetElementPinnedStatus()
        {
            var pinnedElement = ElementSelector.ByElementId(184176, true);
            AssertElementPinnedStatusIs(pinnedElement, true);

            var unPinnedElement = ElementSelector.ByElementId(184324, true);
            AssertElementPinnedStatusIs(unPinnedElement, false);
        }

        /// <summary>
        /// Checks if Pin status can be set correctly
        /// </summary>
        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySetElementPinnedStatus()
        {
            var elem = ElementSelector.ByElementId(184176, true);
            Assert.IsNotNull(elem);

            bool originalPinStatus = elem.IsPinned;

            elem.SetPinnedStatus(true);
            Assert.AreNotEqual(originalPinStatus, elem.IsPinned);

            elem.SetPinnedStatus(false);
            Assert.AreEqual(originalPinStatus, elem.IsPinned);
        }

        private static void AssertElementPinnedStatusIs(
            Element element,
            bool expectedValue)
        {
            bool pinStatus = element.IsPinned;

            Assert.NotNull(pinStatus);
            Assert.AreEqual(expectedValue, pinStatus);
        }
        #endregion

        #region Join tests

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanSuccessfullyGetJoinedElementsFromElement()
        {
            // Arrange - get element from model
            var element = ElementSelector.ByElementId(184176, true);
            int[] expectedIds = new int[] { 207960, 208259, 208422 };

            // Act
            IEnumerable<Element> joinedElements = element.GetJoinedElements();
            var joinedElementIds = joinedElements
                .Select(x => x.Id)
                .ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedIds, joinedElementIds);
        }

        [TestModel(@".\Element\hostedElements.rvt")]
        public void CanSuccessfullyGetHostedElements()
        {
            // Arrange - Select the wall element in revit by it Id
            var elem = ElementSelector.ByElementId(261723, true);
            // Model element names
            string windowFamName = "600 x 3100";
            string curtainWallFamName = "Curtain Wall";
            string wallOpeningFamName = "Rectangular Straight Wall Opening";
            // Expected hosted elements lists
            var famNamesWithShadowsInserts = new[] { windowFamName, windowFamName, windowFamName };
            var famNamesWithEverything = new[] { curtainWallFamName, windowFamName, windowFamName, windowFamName, wallOpeningFamName };
            var famNamesWithOpeningsShadows = new[] { windowFamName, windowFamName, windowFamName, wallOpeningFamName };
            var famNamesWithShadowEmbeddedWallsInserts = new[] { curtainWallFamName, windowFamName, windowFamName, windowFamName };

            // Act - Invoke GetHostedElements with all possible cobinations
            var hostedElementsIncludeNothing = elem.GetHostedElements();
            var hostedElementsIncludeEverything = elem.GetHostedElements(true, true, true, true);

            var hostedElementsIncludeOpenings = elem.GetHostedElements(true, false, false, false);
            var hostedElementsIncludeOpeningsAndShadows = elem.GetHostedElements(true, true, false, false);
            var hostedElementsIncludeOpeningsAndShadowsAndEmbeddedWalls = elem.GetHostedElements(true, true, true, false);

            var hostedElementsIncludeShadows = elem.GetHostedElements(false, true, false, false);
            var hostedElementsIncludeShadowsAndEmbeddedWalls = elem.GetHostedElements(false, true, true, false);
            var hostedElementsIncludeShadowsAndEmbeddedWallsAndEmbeddedInserts = elem.GetHostedElements(false, true, true, true);

            var hostedElementsIncludeEmbeddedWalls = elem.GetHostedElements(false, false, true, false);
            var hostedElementsIncludeEmbeddedWallsAndEmbeddedInserts = elem.GetHostedElements(false, false, true, true);
            var hostedElementsIncludeEmbeddedWallsAndEmbeddedInsertsAndOpenings = elem.GetHostedElements(true, false, true, true);

            var hostedElementsIncludeEmbeddedInserts = elem.GetHostedElements(false, false, false, true);
            var hostedElementsIncludeSEmbeddedInsertsAndOpenings = elem.GetHostedElements(true, false, false, true);
            var hostedElementsIncludeEmbeddedInsertsAndOpeningsAndShadows = elem.GetHostedElements(true, true, false, true);

            //Assert all combinations has the right amount of output elements
            Assert.AreEqual(3, hostedElementsIncludeNothing.Count());
            Assert.AreEqual(5, hostedElementsIncludeEverything.Count());

            Assert.AreEqual(4, hostedElementsIncludeOpenings.Count());
            Assert.AreEqual(4, hostedElementsIncludeOpeningsAndShadows.Count());
            Assert.AreEqual(5, hostedElementsIncludeOpeningsAndShadowsAndEmbeddedWalls.Count());

            Assert.AreEqual(3, hostedElementsIncludeShadows.Count());
            Assert.AreEqual(4, hostedElementsIncludeShadowsAndEmbeddedWalls.Count());
            Assert.AreEqual(4, hostedElementsIncludeShadowsAndEmbeddedWallsAndEmbeddedInserts.Count());

            Assert.AreEqual(4, hostedElementsIncludeEmbeddedWalls.Count());
            Assert.AreEqual(4, hostedElementsIncludeEmbeddedWallsAndEmbeddedInserts.Count());
            Assert.AreEqual(5, hostedElementsIncludeEmbeddedWallsAndEmbeddedInsertsAndOpenings.Count());

            Assert.AreEqual(3, hostedElementsIncludeEmbeddedInserts.Count());
            Assert.AreEqual(4, hostedElementsIncludeSEmbeddedInsertsAndOpenings.Count());
            Assert.AreEqual(4, hostedElementsIncludeEmbeddedInsertsAndOpeningsAndShadows.Count());

            //Assert all combinations has the right elements as output
            CollectionAssert.AreEqual(famNamesWithShadowsInserts, hostedElementsIncludeNothing.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithEverything, hostedElementsIncludeEverything.Select(x => x.Name).ToArray());

            CollectionAssert.AreEqual(famNamesWithOpeningsShadows, hostedElementsIncludeOpenings.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithOpeningsShadows, hostedElementsIncludeOpeningsAndShadows.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithEverything, hostedElementsIncludeOpeningsAndShadowsAndEmbeddedWalls.Select(x => x.Name).ToArray());

            CollectionAssert.AreEqual(famNamesWithShadowsInserts, hostedElementsIncludeShadows.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithShadowEmbeddedWallsInserts, hostedElementsIncludeShadowsAndEmbeddedWalls.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithShadowEmbeddedWallsInserts, hostedElementsIncludeShadowsAndEmbeddedWallsAndEmbeddedInserts.Select(x => x.Name).ToArray());

            CollectionAssert.AreEqual(famNamesWithShadowEmbeddedWallsInserts, hostedElementsIncludeEmbeddedWalls.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithShadowEmbeddedWallsInserts, hostedElementsIncludeEmbeddedWallsAndEmbeddedInserts.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithEverything, hostedElementsIncludeEmbeddedWallsAndEmbeddedInsertsAndOpenings.Select(x => x.Name).ToArray());

            CollectionAssert.AreEqual(famNamesWithShadowsInserts, hostedElementsIncludeEmbeddedInserts.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithOpeningsShadows, hostedElementsIncludeSEmbeddedInsertsAndOpenings.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(famNamesWithOpeningsShadows, hostedElementsIncludeEmbeddedInsertsAndOpeningsAndShadows.Select(x => x.Name).ToArray());
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanSuccessfullyCheckIfTwoElementsAreJoined()
        {
            var wall1 = ElementSelector.ByElementId(184176, true);
            var wall2 = ElementSelector.ByElementId(207960, true);
            var floor = ElementSelector.ByElementId(208259, true);
            var beam1 = ElementSelector.ByElementId(208422, true);
            var beam2 = ElementSelector.ByElementId(208572, true);

            // Check if different kinds of elements are joined
            // wall1 and wall2 are joined
            AssertElementsAreJoined(wall1, wall2, true);
            // wall1 and floor are joined
            AssertElementsAreJoined(wall1, floor, true);
            // wall2 and floor are not joined
            AssertElementsAreJoined(wall2, floor, false);
            // wall1 and beam1 are joined
            AssertElementsAreJoined(wall1, beam1, true);
            // wall2 and beam2 are not joined
            AssertElementsAreJoined(wall2, beam2, false);
            // beam1 and beam2 are joined
            AssertElementsAreJoined(beam1, beam2, true);
            // beam1 and floor are joined
            AssertElementsAreJoined(beam1, floor, true);
        }
        private static void AssertElementsAreJoined(Element element, Element otherElement, bool expected)
        {
            bool arejoined = element.AreJoined(otherElement);
            Assert.AreEqual(expected, arejoined);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanUnjoinListOfElements()
        {
            var wall1 = ElementSelector.ByElementId(184176, true);
            var wall2 = ElementSelector.ByElementId(207960, true);
            var floor = ElementSelector.ByElementId(208259, true);
            var doc = DocumentManager.Instance.CurrentDBDocument;

            // Are joined
            bool originalWall1AndWall2JoinedValue = JoinGeometryUtils.AreElementsJoined(doc,
                                                                                       wall1.InternalElement,
                                                                                       wall2.InternalElement);
            Assert.AreEqual(true, originalWall1AndWall2JoinedValue);
            // Are joined
            bool originalWall1AndFloorJoinedValue = JoinGeometryUtils.AreElementsJoined(doc,
                                                                                       wall1.InternalElement,
                                                                                       floor.InternalElement);
            Assert.AreEqual(true, originalWall1AndFloorJoinedValue);
            // Are not joined
            bool originalWall2AndFloorJoinedValue = JoinGeometryUtils.AreElementsJoined(doc,
                                                                                       wall2.InternalElement,
                                                                                       floor.InternalElement);
            Assert.AreEqual(false, originalWall2AndFloorJoinedValue);

            var elementList = new List<Element>() { wall1, wall2, floor };

            Element.UnjoinAllGeometry(elementList);

            bool newWall1AndWall2JoinedValue = wall1.AreJoined(wall2);
            bool newWall1AndFloorJoinedValue = wall1.AreJoined(floor);
            bool newWall2AndFloorJoinedValue = wall2.AreJoined(floor);

            // Are joined should have changed
            Assert.AreNotEqual(newWall1AndWall2JoinedValue, originalWall1AndWall2JoinedValue);
            // Are joined should have changed 
            Assert.AreNotEqual(newWall1AndFloorJoinedValue, originalWall1AndFloorJoinedValue);
            // Are joined should be the same 
            Assert.AreEqual(newWall2AndFloorJoinedValue, originalWall2AndFloorJoinedValue);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanUnjoinTwoElements()
        {
            var wall1 = ElementSelector.ByElementId(184176, true);
            var wall2 = ElementSelector.ByElementId(207960, true);
            var floor = ElementSelector.ByElementId(208259, true);
            var doc = DocumentManager.Instance.CurrentDBDocument;
            string expectedNotJoinedExceptionMessages = Revit.Properties.Resources.NotJoinedElements;

            // Are joined
            bool originalWall1AndWall2JoinedValue = JoinGeometryUtils.AreElementsJoined(doc,
                                                                                       wall1.InternalElement,
                                                                                       wall2.InternalElement);
            Assert.AreEqual(true, originalWall1AndWall2JoinedValue);

            // Are not joined
            bool originalWall2AndFloorJoinedValue = JoinGeometryUtils.AreElementsJoined(doc,
                                                                                       wall2.InternalElement,
                                                                                       floor.InternalElement);
            Assert.AreEqual(false, originalWall2AndFloorJoinedValue);

            wall1.UnjoinGeometry(wall2);
            bool newWall1AndWall2JoinedValue = wall1.AreJoined(wall2);

            // Are joined should have changed
            Assert.AreNotEqual(newWall1AndWall2JoinedValue, originalWall1AndWall2JoinedValue);
            // Should throw InvalidOperationException
            var ex = Assert.Throws<InvalidOperationException>(() => wall2.UnjoinGeometry(floor));
            Assert.AreEqual(ex.Message, expectedNotJoinedExceptionMessages);
        }

        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanSwitchOrderOfTwoJoinedElements()
        {
            int cuttingElementId = 208422;
            int cutElementId = 208572;
            int unjoinedElementId = 208259;

            List<int> unchangedOrder = new List<int>() { cuttingElementId, cutElementId };
            string invalidSwitchJoinOrderMessages = Revit.Properties.Resources.InvalidSwitchJoinOrder;

            // Joined elements
            var cuttingFraming = ElementSelector.ByElementId(cuttingElementId, true);
            var cutFraming = ElementSelector.ByElementId(cutElementId, true);

            // Not Joined element
            var unjoinedElement = ElementSelector.ByElementId(unjoinedElementId, true);

            // Elements already in the wanted join order
            IEnumerable<Element> orderedElements = Element.SwitchGeometryJoinOrder(cuttingFraming, cutFraming);
            List<int> orderedElementIds = orderedElements.Select(elem => elem.Id).ToList();
            CollectionAssert.AreEqual(unchangedOrder, orderedElementIds);

            // Elements not in wanted join order
            IEnumerable<Element> switchedElements = Element.SwitchGeometryJoinOrder(cutFraming, cuttingFraming);
            List<int> changedElementIds = switchedElements.Select(elem => elem.Id).ToList();
            unchangedOrder.Reverse();
            CollectionAssert.AreEqual(unchangedOrder, changedElementIds);

            // Element not joined
            var ex = Assert.Throws<InvalidOperationException>(() => Element.SwitchGeometryJoinOrder(cutFraming, unjoinedElement));
            Assert.AreEqual(ex.Message, invalidSwitchJoinOrderMessages);
        }
        #endregion

        [Test]
        [TestModel(@".\Element\elementIntersection.rvt")]
        public void CanGetIntersectingElementsOfSpecificCategory()
        {
            // Element to check intersections on
            int intersectionElementId = 316167;
            var intersectionElement = ElementSelector.ByElementId(intersectionElementId, true);

            // Element intersecting
            int structuralFramingId = 316318;
            var structuralFramingElement = ElementSelector.ByElementId(structuralFramingId, true);
            int floorId = 316539;
            var floorElement = ElementSelector.ByElementId(floorId, true);
            int wallId = 316246;
            var wallElement = ElementSelector.ByElementId(wallId, true);

            // Expected outcomes
            var expectedStructuralFramingIds = new List<int>() { structuralFramingId };
            var expectedFloorIds = new List<int>() { floorId };
            var expectedWallIds = new List<int>() { wallId };

            // Get intersecting elements of category
            var structuralFrameCategory = Revit.Elements.Category.ByName("StructuralFraming");
            List<int> intersectedFraming = GetIntersectingElementIds(intersectionElement, structuralFrameCategory);

            var floorCategory = Revit.Elements.Category.ByName("Floors");
            List<int> intersectedFloorId = GetIntersectingElementIds(intersectionElement, floorCategory);

            var wallCategory = Revit.Elements.Category.ByName("Walls");
            List<int> intersectedWallId = GetIntersectingElementIds(intersectionElement, wallCategory);

            // Check if method returns null if there are no intersecting elements of the specified category
            var windowCategory = Revit.Elements.Category.ByName("Windows");
            IEnumerable<Element> intersectedWindow = intersectionElement.GetIntersectingElementsOfCategory(windowCategory);

            // Assert
            CollectionAssert.AreEqual(expectedStructuralFramingIds, intersectedFraming);
            CollectionAssert.AreEqual(expectedFloorIds, intersectedFloorId);
            CollectionAssert.AreEqual(expectedWallIds, intersectedWallId);
            CollectionAssert.AreEqual(new List<Element>(), intersectedWindow);
        }

        private static List<int> GetIntersectingElementIds(Element intersectionElement, Revit.Elements.Category category)
        {
            return intersectionElement.GetIntersectingElementsOfCategory(category)
                                      .Select(elem => elem.Id)
                                      .ToList();
        }
        
        [Test]
        [TestModel(@".\Element\elementJoin.rvt")]
        public void CanSuccessfullyJoinTwoIntersectingElements()
        {
            // Arrange
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            var primaryBeam = ElementSelector.ByElementId(208422, true);
            var nonIntersectingBeam = ElementSelector.ByElementId(209681, true);
            var joinedBeam = ElementSelector.ByElementId(208572, true);
            var notJoinedWall = ElementSelector.ByElementId(207960, true);
            var notJoinedFloor = ElementSelector.ByElementId(208259, true);

            var nonIntersectingTestExpectedExceptionType = typeof(System.NullReferenceException);
            string nonIntersectingTestExpectedExceptionString = "Elements are not intersecting";

            // Act
            List<int> joinedTestExpectedOutcome = new List<int> { 208422, 208572 };
            List<int> notJoinedTestExpectedOutcome = new List<int> { 207960, 208259 };

            var alreadyJoinedOutcome = primaryBeam.JoinGeometry(joinedBeam).Select(elem => elem.Id).ToList();
            var notJoinedIntersectingOutcome = notJoinedWall.JoinGeometry(notJoinedFloor).Select(elem => elem.Id).ToList();

            // Assert
            Assert.AreEqual(joinedTestExpectedOutcome, alreadyJoinedOutcome);
            Assert.AreEqual(notJoinedTestExpectedOutcome, notJoinedIntersectingOutcome);

            // Non intersecting elements should throw a NullReferenceException
            // with the messages Elements are not intersecting
            var ex = Assert.Throws<InvalidOperationException>(() => primaryBeam.JoinGeometry(nonIntersectingBeam)); 
            Assert.AreEqual(ex.Message, nonIntersectingTestExpectedExceptionString);
        }
    }
}
