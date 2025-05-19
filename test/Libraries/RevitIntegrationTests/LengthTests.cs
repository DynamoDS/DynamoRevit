using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.Linq;
using System.Collections.Generic;


namespace RevitSystemTests
{
    [TestFixture]
    class LengthTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void Length()
        {
            const double Tolerance = 0.00001;
            string samplePath = Path.Combine(workingDirectory, @".\Length\Length.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            AssertNoDummyNodes();
            Assert.AreEqual(10, ViewModel.Model.CurrentWorkspace.Nodes.Count());
            Assert.AreEqual(5, ViewModel.Model.CurrentWorkspace.Connectors.Count());

            //check length of String
            var length = GetPreviewValue("c0ecff8e98204341a73b04de6e99c90e");
            var lengthValue = (length is KeyValuePair<string, object> kvp) ? kvp.Value : length;
            var firstValue = ((IEnumerable<object>)((dynamic)lengthValue).Values).Cast<object>().First();
            Assert.AreEqual((double)firstValue, -27.341145833333332d, Tolerance);
        }
    }
}