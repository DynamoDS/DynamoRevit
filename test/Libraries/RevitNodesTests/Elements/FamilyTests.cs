﻿using System;
using NUnit.Framework;
using Revit.Elements;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class FamilyTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByName_ValidInput()
        {
            var fam = Family.ByName("Cone");
            Assert.NotNull(fam);
            Assert.AreEqual("Cone",fam.Name);
            Assert.AreEqual(1, fam.Types.Length);
        }

        [Test, Category("Failure")]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByName_NonexistentName()
        {
            Assert.Throws(typeof(Exception), () => Family.ByName("Turtle"));
        }

        [Test]
        [TestModel(@".\MassWithBoxAndCone.rfa")]
        public void ByName_NullInput()
        {
            Assert.Throws(typeof(ArgumentNullException), () => Family.ByName(null));
        }
    }
}
