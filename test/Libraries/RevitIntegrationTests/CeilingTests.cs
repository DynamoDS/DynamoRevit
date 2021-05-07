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
    class CeilingTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByOutlineTypeAndLevel()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Ceiling\ByOutlineTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var ceiling = GetPreviewValue("ced77f97812442b88ccc7e1a2f8fb25a");
            Assert.AreEqual(ceiling.GetType(), typeof(Revit.Elements.Ceiling));
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByPolyCurveTypeAndLevel()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Ceiling\ByPolyCurveTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var ceiling = GetPreviewValue("6456c1d2e0e044168cf209f8ea0ad869");
            Assert.AreEqual(ceiling.GetType(), typeof(Revit.Elements.Ceiling));
        }
    }
}
