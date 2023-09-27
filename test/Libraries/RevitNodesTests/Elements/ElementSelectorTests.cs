using System.Linq;
using Autodesk.Revit.DB;
using NUnit.Framework;
using Revit.Elements;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using Family = Revit.Elements.Family;
using FamilyType = Revit.Elements.FamilyType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ElementSelectorTests : RevitNodeTestBase
    {

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void FamilyByElementId_ValidArgs()
        {
            // obtain the element id for the box family
            var name = "Box";
            var family = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Family>()
                                                      .FirstOrDefault(x => x.Name == name);
            Assert.NotNull(family);

            // use the element factory to do the same
            var famId = family.Id;
            var famFromFact = Revit.Elements.ElementSelector.ByElementId(famId.Value, true);

            Assert.NotNull(famFromFact);
            Assert.IsAssignableFrom(typeof(Family), famFromFact);
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void FamilyByUniqueId_ValidArgs()
        {
            // obtain the element id for the box family
            var name = "Box";

            // look up the loaded family
            var family = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Family>()
                                                      .FirstOrDefault(x => x.Name == name);
            Assert.NotNull(family);

            // use the element factory to do the same by unique id
            var famUniqueId = family.UniqueId;
            var famFromFact = Revit.Elements.ElementSelector.ByUniqueId(famUniqueId);

            Assert.NotNull(famFromFact);
            Assert.IsAssignableFrom(typeof(Family), famFromFact);
            Assert.AreEqual(name, (famFromFact as Family).Name);
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void FamilySymbolByElementId_ValidArgs()
        {
            // obtain the element id for the box family
            var name = "Box";

            // look up the loaded family
            var family = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Family>()
                                          .FirstOrDefault(x => x.Name == name);
            Assert.NotNull(family);

            var symbol = family.GetFamilySymbolIds().Select(x=>DocumentManager.Instance.CurrentDBDocument.GetElement(x)).
                OfType<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == name);
            Assert.NotNull(symbol);

            // use the element factory to do the same
            var famSymEleId = symbol.Id;
            var famSymFromFact = Revit.Elements.ElementSelector.ByElementId(famSymEleId.Value, true);

            Assert.NotNull(famSymFromFact);
            Assert.IsAssignableFrom(typeof(FamilyType), famSymFromFact);
            Assert.AreEqual(name, (famSymFromFact as FamilyType).Name);
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void FamilySymbolByUniqueId_ValidArgs()
        {
            // obtain the element id for the box family
            var name = "Box";

            // look up the loaded family
            var family = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Family>()
                                        .FirstOrDefault(x => x.Name == name);
            Assert.NotNull(family);

            var symbol = family.GetFamilySymbolIds().Select(x => DocumentManager.Instance.CurrentDBDocument.GetElement(x)).
                OfType<Autodesk.Revit.DB.FamilySymbol>().FirstOrDefault(x => x.Name == name);
            Assert.NotNull(symbol);

            // use the element factory to do the same
            var famSymUniqueId = symbol.UniqueId;
            var famSymFromFact = Revit.Elements.ElementSelector.ByUniqueId(famSymUniqueId);

            Assert.NotNull(famSymFromFact);
            Assert.IsAssignableFrom(typeof(FamilyType), famSymFromFact);
            Assert.AreEqual(name, (famSymFromFact as FamilyType).Name);
        }

        [Test]
        [TestModel(@".\block.rfa")]
        public void FormByType_ValidArgs()
        {
            var ele = ElementSelector.ByType<Autodesk.Revit.DB.Form>(true).FirstOrDefault();
            Assert.NotNull(ele);
        }

        [Test, TestModel(@".\UnknownElements.rvt")]
        public void UnknownElementsHaveCorrectOwnership()
        {
            // At the time of the creation of this test, we did not
            // have a wrapper for the roof type so it is "unknown". 
            // We attempt to wrap that element by id here, and verify
            // that the ownership is set correctly.

            var roof = DocumentManager.Instance.ElementsOfType<RoofBase>().FirstOrDefault();
            Assert.NotNull(roof);

            var el = ElementSelector.ByElementId(roof.Id.Value);
            Assert.NotNull(el);
            Assert.True(el.IsRevitOwned);

            // Dispose the element wrapper and ensure that the element
            // remains in the document.

            el.Dispose();

            roof = DocumentManager.Instance.ElementsOfType<RoofBase>().FirstOrDefault();
            Assert.NotNull(roof);
        }

        [Test, Ignore("Not finished - need to investigate why")]
        public void ReferencePointByElementId_ValidArgs()
        {
            Assert.Inconclusive();
        }

        [Test, Ignore("Not finished")]
        public void ReferencePointByUniqueId_ValidArgs()
        {
            Assert.Inconclusive();
        }

        [Test, Ignore("Not finished"), Category("Failure")]
        public void FamilyInstanceByElementId_ValidArgs()
        {
            Assert.Inconclusive();
        }

        [Test, Ignore("Not finished"), Category("Failure")]
        public void FamilyInstanceByUniqueId_ValidArgs()
        {
            Assert.Inconclusive();
        }

        [Test, Ignore("Not finished"), Category("Failure")]
        public void DividedPathByElementId_ValidArgs()
        {
            Assert.Inconclusive();
        }

        [Test, Ignore("Not finished"), Category("Failure")]
        public void DividedSurfaceByUniqueId_ValidArgs()
        {
            Assert.Inconclusive();
        }

    }
}
