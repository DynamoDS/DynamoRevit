using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;


namespace RevitSystemTests
{
    [TestFixture]
    class FilterRuleTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void FilterRule()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\FilterRule.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("7e48a071-e368-4ba5-981b-1188bcab4d44");
            Assert.AreEqual(type.GetType(), typeof(Revit.Filter.FilterRule));

            var element = GetPreviewValue("7237e863-824a-4a59-b241-a8f6568925b2");
            Assert.AreEqual(element.GetType(), typeof(Revit.Filter.ParameterFilterElement));


        }
    }
}