﻿
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class GlobalParametersTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void SetAndGetGlobalParameterByName()
        {

            var gpString = Revit.Elements.GlobalParameter.ByName("MyGlobal", Autodesk.Revit.DB.ParameterType.Text.ToString());
            Assert.IsNotNull(gpString.InternalGlobalParameter);
            Revit.Elements.GlobalParameter.SetValue(gpString, "4711");
            Assert.AreEqual("4711", gpString.Value);

            var gpInt = Revit.Elements.GlobalParameter.ByName("MyGlobalInt", Autodesk.Revit.DB.ParameterType.Integer.ToString());
            Assert.IsNotNull(gpInt.InternalGlobalParameter);
            Revit.Elements.GlobalParameter.SetValue(gpInt, 4711);
            Assert.AreEqual(4711, gpInt.Value);

            var gpLen = Revit.Elements.GlobalParameter.ByName("MyGlobalDouble", Autodesk.Revit.DB.ParameterType.Length.ToString());
            Assert.IsNotNull(gpLen.InternalGlobalParameter);
            Revit.Elements.GlobalParameter.SetValue(gpLen, 47.11);
            double val = (double)gpLen.Value;
            val.ShouldBeApproximately(47.11);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void SetAndGetGlobalParameterValue()
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



    }
}
