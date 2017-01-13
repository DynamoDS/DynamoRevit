using System;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryConversion;
using Revit.GeometryReferences;

using RevitServices.Persistence;

using RevitTestServices;

using RTF.Framework;

using Element = Revit.Elements.Element;
using FamilyType = Revit.Elements.FamilyType;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class GlobalParametersTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void SetAndGetGlobalParameterByName()
        {
            
            var gp = Revit.Elements.GlobalParameter.ByName("MyGlobal", "Text");
            Assert.IsNotNull(gp);
            Assert.IsTrue(typeof(Revit.Elements.GlobalParameter) == gp.GetType());
            Assert.IsTrue("MyGlobal" == gp.Name);
            Assert.IsTrue("Text" == gp.ParameterType);
            Assert.IsTrue(true == gp.Visible);

            var param = Revit.Elements.GlobalParameter.FindByName("MyGlobal");
            Assert.NotNull(param);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void GetAllGlobalParameters()
        {
            var gp1 = Revit.Elements.GlobalParameter.ByName("MyGlobal1", "Text");
            Assert.IsNotNull(gp1);

            var gps = Revit.Elements.GlobalParameter.GetAllGlobalParameters();
            Assert.IsNotNull(gps);

            bool found = false;

            foreach (var gp in gps)
            {
                if (gp.Name == "MyGlobal1") found = true;
            }

            Assert.IsTrue(found);
        }


    }
}
