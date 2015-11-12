using System.IO;
using System.Linq;
using DSRevitNodesUI;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class GetFamilyParametersTests : RevitSystemTestBase
    {
        [Test, TestModel(@".\Selection\DynamoSample.rvt")]
        public void CachedValueChanges_HasItemsAndSelectedIndex()
        {
            var model = ViewModel.Model;
            var samplePath = Path.Combine(workingDirectory,
                                                @".\Family\GetFamilyParameter.dyn");
            var testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            var parametersNode = model.CurrentWorkspace.Nodes.OfType<FamilyInstanceParameters>().FirstOrDefault();
            Assert.NotNull(parametersNode);

            Assert.Greater(parametersNode.Items.Count, 0);
            Assert.AreNotEqual(parametersNode.SelectedIndex, -1);
        }
    }
}
