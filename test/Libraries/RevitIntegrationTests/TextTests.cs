using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Nodes;
using Autodesk.DesignScript.Geometry;
using CoreNodeModels.Input;
using NUnit.Framework;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;


namespace RevitSystemTests
{
    [TestFixture]
    class TextNoteTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void PlaceText()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Annotations\TextNote.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var textelement = GetPreviewValue("078437f0-b5a8-4690-9506-c3221513f1da");

            Assert.AreEqual(typeof(Revit.Elements.TextNote), textelement.GetType());

            Revit.Elements.TextNote note = (Revit.Elements.TextNote)textelement;

            Assert.AreEqual(note.Text, "Hello World\r");

        }



    }
}
