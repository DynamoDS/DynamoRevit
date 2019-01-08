﻿
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class RevisionTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Create_ValidArgs()
        {
            Revision rev1 = Revision.ByName("myName", "01.01.1970", "myDesc", false, "me", "to");
            Assert.NotNull(rev1);

            Assert.IsInstanceOf(typeof(Revit.Elements.Revision), rev1);

            Assert.AreEqual(rev1.InternalRevitElement.RevisionDate, "01.01.1970");
        }




    }
}
