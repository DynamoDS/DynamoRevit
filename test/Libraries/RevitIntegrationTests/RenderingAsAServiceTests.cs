using System.IO;
using System.Linq;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class RenderingAsAServiceTests : RevitSystemTestBase
    { 
        [Test]
        [TestModel(@".\empty.rfa")]
        public void CanOpenhill_climbing_simple()
        {
            //Analyze nodes do not resolve in daylight sample files.
            //http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3757
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory, @".\Samples\hill_climbing_simple.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);

            Assert.AreEqual(7, model.CurrentWorkspace.Nodes.Count());
            AssertNoDummyNodes();
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void CanOpenBruteForcingParametersandRendering()
        {
            //Analyze nodes do not resolve in daylight sample files.
            //http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3757
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory, @".\Samples\BruteForcingParametersandRendering.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);

            Assert.AreEqual(33, model.CurrentWorkspace.Nodes.Count());
            AssertNoDummyNodes();
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void hill_climbing_daylighting()
        {
            //Analyze nodes do not resolve in daylight sample files.
            //http://adsk-oss.myjetbrains.com/youtrack/issue/MAGN-3757
            var model = ViewModel.Model;
            string samplePath = Path.Combine(workingDirectory, @".\Samples\hill_climbing_daylighting.dyn");
            string testPath = Path.GetFullPath(samplePath);
            ViewModel.OpenCommand.Execute(testPath);

            Assert.AreEqual(21, model.CurrentWorkspace.Nodes.Count());
            AssertNoDummyNodes();
        }
        
    }
}
