using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{

    [TestFixture]
    public class ToposolidTypeTests : RevitNodeTestBase
    {

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_ValidArgs()
        {
            var toposolidTypeName = "Grassland - 16\"";
            var toposolidType = ToposolidType.ByName(toposolidTypeName);
            Assert.NotNull(toposolidType);
            Assert.AreEqual(toposolidTypeName, toposolidType.Name);
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NullArgument()
        {
            Assert.Throws(typeof(ArgumentNullException), () => ToposolidType.ByName(null));
        }
    }
}

