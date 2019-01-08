using System.Linq;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{

    [TestFixture]
    public class CurtainSystemTests : RevitNodeTestBase
    {

        [Test]
        [TestModel(@".\InPlaceMass.rvt")]
        public void Create_Valid()
        {
            CurtainSystemType type = CurtainSystemType.ByName("5' x 10'");
            Assert.NotNull(type);

            var mass =  Revit.Elements.ElementSelector.ByElementId(205302);

            Assert.NotNull(mass);
            
            var system = CurtainSystem.ByFace(mass.Faces.ElementAt(0),type);
            Assert.NotNull(system);

            Assert.NotNull(system.Faces);
            Assert.NotNull(system.InternalCurtainSystem);
        }




    }
}

