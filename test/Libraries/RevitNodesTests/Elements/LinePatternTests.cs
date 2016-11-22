using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class LinePatternTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void GetByName_ValidArgs()
        {
            var linePattern = LinePatternElement.GetByName("Dash");
            Assert.NotNull(linePattern);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void GetByName_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => LinePatternElement.GetByName(null));
        }
    }
}
