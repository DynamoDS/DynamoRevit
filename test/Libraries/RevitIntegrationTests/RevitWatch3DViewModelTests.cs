using System.IO;
using System.Linq;
using Dynamo.Applications.ViewModel;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class RevitWatch3DViewModelTests : RevitSystemTestBase
    {
        [Test, TestModel(@".\empty.rfa")]
        public void SmallCurve_Draw_DoesNotThrowException()
        {
            var vm3D = ViewModel.Watch3DViewModels.FirstOrDefault(vm => vm is RevitWatch3DViewModel);
            Assert.NotNull(vm3D);

            var samplePath = Path.Combine(workingDirectory, @".\RevitWatch3DViewModel\ShortLine.dyn");
            var testPath = Path.GetFullPath(samplePath);

            Assert.DoesNotThrow(()=>ViewModel.OpenCommand.Execute(testPath));
        }
    }
}
