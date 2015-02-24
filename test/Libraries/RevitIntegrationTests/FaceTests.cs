using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    internal class FaceTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Face\GetSurfaceDomain.rvt")]
        public void GetSurfaceDomain()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Face\GetSurfaceDomain.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
        }
    }
}
