using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using RTF.Framework;
using RevitTestServices;

namespace RevitSystemTests
{
    [TestFixture]
    class RoofTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByOutlineExtrusionTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Roof\ByOutLineExtrusionTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("9daa9b583bdf4659901779b540fdeecc");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.Roof));
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByOutlineTypeAndLevel()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Roof\ByOutlineTypeAndLevel.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("c6d16ef7c551467a8d46cd307b2a465b");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.Roof));
        }
    }
}
