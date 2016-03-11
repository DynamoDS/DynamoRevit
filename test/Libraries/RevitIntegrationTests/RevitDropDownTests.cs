using System.IO;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class RevitDropDownTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void AllCategoriesHasValidValue()
        {
            var node = new DSRevitNodesUI.Categories();
            Model.AddNodeToCurrentWorkspace(node, false);
            RunCurrentModel();
            int i = 0;
            foreach (var item in node.Items)
            {
                node.SelectedIndex = i++;
                //Since UI is not loaded, node modification will not be sent automatically.
                node.MarkNodeAsModified(); 

                RunCurrentModel();
                //Node doesn't have any warning or error.
                Assert.AreEqual(Dynamo.Graph.Nodes.ElementState.Active, node.State);
                var category = this.GetPreviewValue(node.GUID.ToString()) as Category;
                Assert.IsNotNull(category);
                Assert.AreEqual(item.Name, category.Name);
            }
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void LegacyInvalidCategory()
        {
            string samplePath = Path.Combine(workingDirectory,
                                               @".\RevitDropDown\ArcWallOpeningCategory.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            // evaluate  graph
            RunCurrentModel();

            var guid = "571c0a1e-4e26-40dc-85c5-e67cb9fa5507";
            //Node has invalid selection, it must be in warning state.
            Assert.IsTrue(IsNodeInErrorOrWarningState(guid));
            Assert.IsNull(GetRuntimeMirror(guid));

            var node = this.GetNode<DSRevitNodesUI.Categories>(guid) as DSRevitNodesUI.Categories;
            Assert.IsNotNull(node);
            Assert.AreEqual(-1, node.SelectedIndex);

            node.SelectedIndex = 0;
            node.MarkNodeAsModified();

            RunCurrentModel();
            Assert.AreEqual(Dynamo.Graph.Nodes.ElementState.Active, node.State);
            var category = this.GetPreviewValue(node.GUID.ToString()) as Category;
            Assert.IsNotNull(category);
        }
    }
}
