using System.Linq;

using NUnit.Framework;

using Revit.Elements;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.GeometryConversion
{
    [TestFixture]
    internal class ReviSolidToProtoTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\importInstance.rfa")]
        public void ToProtoType_Boolean_ShouldConvertSolidInDocument()
        {
            var allGeometries = ElementSelector.ByType<Autodesk.Revit.DB.ImportInstance>(true)
                .SelectMany(x => x.Geometry())
                .ToList();

            Assert.AreEqual(5, allGeometries.Count);
        }
    }
}
