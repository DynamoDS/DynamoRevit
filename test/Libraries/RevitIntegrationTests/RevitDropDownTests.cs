using System.IO;
using System.Linq;
using NUnit.Framework;
using Revit.Elements;
using RevitServices.Persistence;
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

        [Test]
        [TestModel(@".\empty.rvt")]
        public void AllMaterialsHaveValidValue()
        {
            var node = new DSRevitNodesUI.Materials();
            Model.AddNodeToCurrentWorkspace(node, false);
            RunCurrentModel();

            var documentMaterials = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .OfClass(typeof(Autodesk.Revit.DB.Material))
                .ToElements();
            Assert.IsNotEmpty(documentMaterials, "The test model is expected to contain materials.");

            // Every material in the document is listed exactly once, sorted by name
            var expectedNames = documentMaterials.Select(m => m.Name).OrderBy(name => name).ToList();
            CollectionAssert.AreEqual(expectedNames, node.Items.Select(item => item.Name).ToList());

            for (int i = 0; i < node.Items.Count; i++)
            {
                var item = node.Items[i];
                node.SelectedIndex = i;
                //Since UI is not loaded, node modification will not be sent automatically.
                node.MarkNodeAsModified();

                RunCurrentModel();
                //Node doesn't have any warning or error.
                Assert.AreEqual(Dynamo.Graph.Nodes.ElementState.Active, node.State);
                var material = this.GetPreviewValue(node.GUID.ToString()) as Material;
                Assert.IsNotNull(material);
                Assert.AreEqual(item.Name, material.Name);
                Assert.AreEqual(((Autodesk.Revit.DB.Material)item.Item).Id.Value, material.Id);
            }
        }
    }
}
