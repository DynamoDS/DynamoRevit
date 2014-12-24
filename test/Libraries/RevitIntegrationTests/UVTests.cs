using System.IO;

using NUnit.Framework;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    internal class UVTests : SystemTest
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void UVRandom()
        {
            string samplePath = Path.Combine(workingDirectory, @".\UV\UVRandom.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();
            
            //Assert.DoesNotThrow(() => ViewModel.Model.RunExpression());
        }
    }
}
