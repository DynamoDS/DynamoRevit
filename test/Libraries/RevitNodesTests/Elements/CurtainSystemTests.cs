using System;
using Revit.Elements;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using Autodesk.DesignScript.Geometry;
using Revit.GeometryConversion;
using System.Linq;

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

