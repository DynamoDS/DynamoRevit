using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;

using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ParseEnumTests : RevitNodeTestBase
    {
        /// <summary>
        /// Testing Enum parser
        /// </summary>
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void ParseEnum_ValidArgs()
        {
            object value1 = ParseEnum.ByString("randomValue", "noneExistingEnumType");
            Assert.IsNull(value1);

            object value2 = ParseEnum.ByString("randomValue", "Autodesk.Revit.DB.WallSide");
            Assert.IsNull(value2);

            object value3 = ParseEnum.ByString("Exterior", "Autodesk.Revit.DB.WallSide");
            Assert.AreEqual(value3, Autodesk.Revit.DB.WallSide.Exterior);
        }

    }
}
