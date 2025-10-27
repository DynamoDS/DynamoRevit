using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using DSRevitNodesUI;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;
using FamilyType = Revit.Elements.FamilyType;
using IntegerSlider = CoreNodeModels.Input.IntegerSlider;

namespace RevitSystemTests
{
    [TestFixture]
    public class StructuralFramingTests : RevitSystemTestBase
    {
        private const double Tolerance = 0.001;

        [Test, TestModel(@".\StructuralFraming\StructuralFraming.rvt")]
        public void StructuralFraming_Beam()
        {
            OpenAndRunDynamoDefinition(@".\StructuralFraming\Beam.dyn");

            CompareStructuralTypeAgainstElements();
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralFraming,10);
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralFraming, 5);
        }

        [Test, TestModel(@".\StructuralFraming\StructuralFraming.rvt")]
        public void StructuralFraming_Brace()
        {
            OpenAndRunDynamoDefinition(@".\StructuralFraming\Brace.dyn");

            CompareStructuralTypeAgainstElements();
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralFraming, 10);
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralFraming, 5);
        }

        [Test, TestModel(@".\StructuralFraming\StructuralFraming.rvt")]
        public void StructuralFraming_Column()
        {
            OpenAndRunDynamoDefinition(@".\StructuralFraming\Column.dyn");

            CompareStructuralTypeAgainstElements();
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralColumns, 10);
            CompareSliderCountAndMemberCount(BuiltInCategory.OST_StructuralColumns, 5);
        }

        [Test]
        [TestModel(@".\LOC model_2022_automatic test.rvt")]
        public void LocationStructFram_StrucPlanView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\LocationStructFram_StrucPlanView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var structFramLoc = GetPreviewValue("b16048797c57445f90ae241ff895eb2f") as Autodesk.DesignScript.Geometry.Line;
            Assert.AreEqual(-19127.681, structFramLoc.StartPoint.X, Tolerance);
            Assert.AreEqual(13470.534, structFramLoc.StartPoint.Y, Tolerance);
            Assert.AreEqual(5994.798, structFramLoc.StartPoint.Z, Tolerance);
            Assert.AreEqual(-15127.681, structFramLoc.EndPoint.X, Tolerance);
            Assert.AreEqual(13470.534, structFramLoc.EndPoint.Y, Tolerance);
            Assert.AreEqual(5994.798, structFramLoc.EndPoint.Z, Tolerance);
            Assert.AreEqual(4000.000, structFramLoc.Direction.Length, Tolerance);

            var structFramPlanView = GetPreviewValue("064eb4a3ad6944a9b316d5020ad31145");
            Assert.IsNotNull(structFramPlanView);
        }

        private void CompareStructuralTypeAgainstElements()
        {
            AssertTypeAndCountWhenSelectingFromDropDown(0);
            AssertTypeAndCountWhenSelectingFromDropDown(1);
        }

        private void AssertTypeAndCountWhenSelectingFromDropDown(int selectedIndex)
        {
            var slider = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is IntegerSlider) as IntegerSlider;

            var typeSelector = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is AllElementsInBuiltInCategory) as RevitDropDownBase;
            typeSelector.SelectedIndex = selectedIndex;

            RunCurrentModel();
            
            var dynamoSymbol = typeSelector.GetValue(0, ViewModel.Model.EngineController).Data as FamilyType;
            var revitSymbol = dynamoSymbol.InternalElement;

            Console.WriteLine("Family type is now set to {0}", revitSymbol);

            var symbolFilter = new FamilyInstanceFilter(
                DocumentManager.Instance.CurrentDBDocument,
                revitSymbol.Id);
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.WherePasses(symbolFilter);

            Assert.AreEqual(fec.ToElements().Count, slider.Value);
        }

        private void CompareSliderCountAndMemberCount(BuiltInCategory cat, int sliderCount)
        {
            var slider = ViewModel.Model.CurrentWorkspace.Nodes.FirstOrDefault(x => x is IntegerSlider) as IntegerSlider;
            slider.Value = sliderCount;

            RunCurrentModel();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(FamilyInstance)).OfCategory(cat);

            Assert.AreEqual(slider.Value, fec.ToElements().Count);
        }
    }
}
