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
    }
}
