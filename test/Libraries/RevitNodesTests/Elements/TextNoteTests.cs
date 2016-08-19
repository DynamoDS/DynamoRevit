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
    public class TextNoteTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Create_ValidArgs()
        {
            var note = TextNote.ByLocation(Revit.Application.Document.Current.ActiveView, Point.ByCoordinates(0, 0, 0), "Hello World", Autodesk.Revit.DB.HorizontalTextAlignment.Center, TextNoteType.Default());
            Assert.NotNull(note);

            Assert.AreEqual(note.Text, "Hello World");          
        }
        




    }
}
