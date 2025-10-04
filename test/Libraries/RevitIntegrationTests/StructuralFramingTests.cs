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

        [Test]
        [TestModel(@".\LOC model_2022_automatic test.rvt")]
        public void LocationStructFram_StrucPlanView()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\LocationStructFram_StrucPlanView.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var structFramLoc = GetPreviewValue("b16048797c57445f90ae241ff895eb2f");
            var expectedLocation = "Line(StartPoint = Point(X = -19127.681, Y = 13470.534, Z = 5994.798), " +
                "EndPoint = Point(X = -15127.681, Y = 13470.534, Z = 5994.798), " +
                "Direction = Vector(X = 4000.000, Y = -0.000, Z = 0.000, Length = 4000.000))";
            Assert.AreEqual(expectedLocation, structFramLoc.ToString());

            var structFramPlanView = GetPreviewValue("064eb4a3ad6944a9b316d5020ad31145");
            var expectedPlanView = "StructuralPlanView(Name = Level 1(1) )";
            Assert.AreEqual(expectedPlanView, structFramPlanView.ToString());
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
