using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class CeilingTypeTests : RevitNodeTestBase
    {
        private const double Tolerance = 0.00001;

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_ValidArgs()
        {
            var ceilingTypeName = "Generic - 4\"";
            var ceilingType = CeilingType.ByName(ceilingTypeName);
            Assert.NotNull(ceilingType);
            Assert.AreEqual(ceilingTypeName, ceilingType.Name);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => CeilingType.ByName(null));
        }
    }
}
