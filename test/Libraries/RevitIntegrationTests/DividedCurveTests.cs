using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

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
            
        }
    }
}
