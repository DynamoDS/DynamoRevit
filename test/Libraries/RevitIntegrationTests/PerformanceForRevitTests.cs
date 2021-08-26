using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class PerformanceForRevitTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void CreateMutipleWalls()
        {
            //Just for run dyn file.
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\PerformanceForRevit\CreateMultipleWalls.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var values = GetPreviewValue("0e10d108811445e19056646201e3d953");
            Assert.NotNull(values);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void CreateMutipleFloors()
        {
            //Just for run dyn file.
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\PerformanceForRevit\CreateMultipleFloors.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var values = GetPreviewValue("377f6b08f1db4fb8b34836042aef4118");
            Assert.NotNull(values);
        }
    }
}
