using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using Dynamo.Models;
using System.Linq;
using Revit.Elements;

namespace RevitSystemTests
{
    [TestFixture]
    class DividedCurveTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\DividedCurve\DividedCurve.rfa")]
        public void DividedCurve()
        {
            string samplePath = Path.Combine(workingDirectory, @".\DividedCurve\DividedCurve.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            AssertNoDummyNodes();
            var model = ViewModel.Model;
            Assert.AreEqual(18, model.CurrentWorkspace.Nodes.Count);
            Assert.AreEqual(14, model.CurrentWorkspace.Connectors.Count());

            //check Transaction.End
            var dividedPathID = "340aca5c-2b33-4d87-b8d0-96768cbdf1da";
            AssertPreviewCount(dividedPathID, 11);
            for (int i = 0; i < 1; i++)
            {
                var dividedPath = GetPreviewValueAtIndex(dividedPathID, i) as DividedPath;
                Assert.IsNotNull(dividedPath);
            }

            // check DividedPath.Point
            var pointsID = "c5733de3-da97-456a-88cc-f7f363afa921";
            AssertPreviewCount(pointsID, 11);
            var points = GetFlattenedPreviewValues(pointsID);
            Assert.AreEqual(points.Count(), 33);
            foreach (var element in points)
            {
                Assert.IsNotNull(element);
            }

        }
    }
}