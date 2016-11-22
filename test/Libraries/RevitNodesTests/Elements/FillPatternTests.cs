using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class FillPatternTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void GetByName_ValidArgs()
        {
            var fillPattern = FillPatternElement.GetByName("Solid fill", Autodesk.Revit.DB.FillPatternTarget.Drafting.ToString());
            Assert.NotNull(fillPattern);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void GetByName_NullArgs()
        {
            Assert.Throws(typeof(ArgumentNullException), () => FillPatternElement.GetByName(null, Autodesk.Revit.DB.FillPatternTarget.Drafting.ToString()));
        }
    }
}

