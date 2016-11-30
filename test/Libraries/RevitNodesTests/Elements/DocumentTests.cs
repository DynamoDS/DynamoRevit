using Revit.Application;
using NUnit.Framework;

using RevitTestServices;
using RTF.Framework;
using System.IO;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class DocumentTests : RevitNodeTestBase
    {

        [Test]
        public void Current()
        {
            var doc = Document.Current;
            Assert.NotNull(doc);
            Assert.NotNull(doc.ActiveView);
            Assert.IsTrue(doc.IsFamilyDocument);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void Document_FilePath_ValidDoc_IsValid()
        {
            var doc = Document.Current;
            var fileName = Document.Current.FilePath;
            var fileInfo = new FileInfo(fileName);
            Assert.AreEqual("empty.rvt", fileInfo.Name);
        }
    }
}
