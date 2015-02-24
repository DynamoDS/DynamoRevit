using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class TopographyTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void TopographyFromPoints()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Topography\TopographyFromPoints.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);


            RunCurrentModel();
            
        }

        [Test]
        [TestModel(@".\Topography\topography.rvt")]
        public void PointsFromTopography()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Topography\PointsFromTopography.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);


            RunCurrentModel();
            
        }
    }
}
