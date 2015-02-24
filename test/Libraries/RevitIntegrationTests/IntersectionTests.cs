using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class IntersectionTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Intersect\CurveCurveIntersection.rfa")]
        public void CurveCurveIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\CurveCurveIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
        }

        [Test]
        [TestModel(@".\Intersect\CurveFaceIntersection.rfa")]
        public void CurveFaceIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\CurveFaceIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
        }

        [Test]
        [TestModel(@".\Intersect\FaceFaceIntersection.rfa")]
        public void FaceFaceIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\FaceFaceIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
        }

        [Test]
        [TestModel(@".\Intersect\EdgePlaneIntersection.rfa")]
        public void EdgePlaneIntersection()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Intersect\EdgePlaneIntersection.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
        }
    }
}
