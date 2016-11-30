using System;

using NUnit.Framework;

using Revit.Elements;

using RevitTestServices;

using RTF.Framework;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class CategoryTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryByName_ValidArgs()
        {
            var cat = Category.ByName("OST_PointClouds");
            Assert.NotNull(cat);
            Assert.AreEqual(cat.Id, cat.InternalCategory.Id.IntegerValue);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryByName_ValidArgsShort()
        {
            var cat = Category.ByName("PointClouds");
            Assert.NotNull(cat);
            Assert.AreEqual(cat.Id, cat.InternalCategory.Id.IntegerValue);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryByName_ValidArgsLong()
        {
            var cat = Category.ByName("Point Clouds");
            Assert.NotNull(cat);
            Assert.AreEqual(cat.Id, cat.InternalCategory.Id.IntegerValue);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryByName_BadArgs()
        {
            Assert.Throws<ArgumentException>(()=>Category.ByName("foo"));
            Assert.Throws<ArgumentNullException>(()=>Category.ByName(null));
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryById_ValidArgs()
        {
            var cat = Category.ById((int)Autodesk.Revit.DB.BuiltInCategory.OST_PointClouds);
            Assert.NotNull(cat);
            Assert.AreEqual(cat.Id, (int)Autodesk.Revit.DB.BuiltInCategory.OST_PointClouds);
            Assert.AreEqual(cat.Name, @"Point Clouds");
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CategoryById_BadArgs()
        {
            Assert.Throws<ArgumentException>(() => Category.ById(-1));
        }
    }
}
