using System.IO;
using System.Linq;

using Dynamo.Nodes;

using NUnit.Framework;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class ModelCurveTests : SystemTest
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void ModelCurve()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\ModelCurve\ModelCurve.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            AssertNoDummyNodes();

            RunCurrentModel();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.OfClass(typeof(CurveElement));

            //verify five model curves created
            int count = fec.ToElements().Count;
            Assert.IsInstanceOf(typeof(ModelCurve), fec.ToElements().First());
            Assert.AreEqual(5, count);

            //update any number node and verify 
            //that the element gets updated not recreated
            var doubleNodes = ViewModel.Model.CurrentWorkspace.Nodes.Where(x => x is DoubleInput);
            var node = doubleNodes.First() as DoubleInput;

            Assert.IsNotNull(node);

            node.Value = node.Value + .1;
            RunCurrentModel();
            Assert.AreEqual(5, fec.ToElements().Count);
        }
    }
}
