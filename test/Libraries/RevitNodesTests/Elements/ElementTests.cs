using System.Linq;
using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ElementTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\materials.rvt")]
        public void SetParameterByName_Element_CanSuccessfullySetMaterialByElement()
        {
            var mat = Material.ByName("Glass");
            var ele = ElementSelector.ByType<Autodesk.Revit.DB.FamilyInstance>(true).First();
            const string paramName = "Body Material";
            var elemId0 = ele.GetParameterValueByName(paramName);
            Assert.AreNotEqual( mat.Id, elemId0 );

            ele.SetParameterByName(paramName, mat);
            DocumentManager.Regenerate();

            var elemId1 = (Element)ele.GetParameterValueByName(paramName);
            Assert.AreEqual(mat.InternalElement.Id, elemId1.InternalElement.Id);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySetAndGetElement()
        {
            var wall = ElementSelector.ByElementId(184176, true);
            var famSym = FamilyType.ByName("18\" x 18\"");
            const string name = "Column";
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
            units.SetFormatOptions(Autodesk.Revit.DB.UnitType.UT_Length, new Autodesk.Revit.DB.FormatOptions(Autodesk.Revit.DB.DisplayUnitType.DUT_METERS));
            DocumentManager.Instance.CurrentDBDocument.SetUnits(units);

            SetExpectedWallHeight(wall, 45.5);
            GetExpectedWallHeight(wall, 45.5);
        }

        private static void SetExpectedWallHeight(Element wall, double value)
        {
            const string name = "Unconnected Height";
            wall.SetParameterByName(name, value);
        }

        private static void GetExpectedWallHeight(Element wall, double value)
        {
            const string name = "Unconnected Height";
            var height = (double)wall.GetParameterValueByName(name);

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

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullyPinElement()
        {
            var wall = ElementSelector.ByElementId(184176, true);
            wall.PinUnpin();
            Assert.IsTrue(wall.InternalElement.Pinned);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySelectById()
        {
            var selected = Element.ById(184176);
            Assert.IsNotNull(selected);
        }

        [Test]
        [TestModel(@".\element.rvt")]
        public void CanSuccessfullySelectByUniqueId()
        {
            var wall = ElementSelector.ByElementId(184176, true);
            var uniqueId = wall.InternalElement.UniqueId;
            var selected = Element.ByUniqueId(uniqueId);
            Assert.IsNotNull(selected);
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

            var solids = objects.OfType<Solid>().ToList();
            Assert.AreEqual(1, solids.Count);

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
            Assert.AreEqual(4, crvs.OfType<Line>().Count());

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
            Assert.AreEqual(4, crvs.OfType<Line>().Count());

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
        
    }
}
