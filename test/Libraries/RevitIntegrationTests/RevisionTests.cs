using System.IO;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;
using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using System.Collections.Generic;


namespace RevitSystemTests
{
    [TestFixture]
    class RevisionTests : RevitSystemTestBase
    {

        private const double Tolerance = 0.001;

        [Test]
        [TestModel(@".\emptyAnnotativeView.rvt")]
        public void Revision()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Annotations\Revisions.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();


            var type = GetPreviewValue("98da559a-b823-4379-be25-36eddce3dd65");
            Assert.AreEqual(type.GetType(), typeof(Revit.Elements.RevisionCloud));



            var rev = GetPreviewValue("4f40afa4-82b8-4860-940e-96909aa4ab40");
            Assert.AreEqual(rev.GetType(), typeof(Revit.Elements.Revision));

        }


        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void RevisionNodes()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RevisionNodes.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var revisionCloudCurvesNode = GetFlattenedPreviewValues("4e147d0d704441778852198b45cd1eb0");
            var revisionCloudCurves = revisionCloudCurvesNode[0] as Autodesk.DesignScript.Geometry.Curve;
            Assert.AreEqual(4, revisionCloudCurvesNode.Count());
            Assert.AreEqual(-12327.314, revisionCloudCurves.StartPoint.X, Tolerance);
            Assert.AreEqual(-3725.279, revisionCloudCurves.EndPoint.X, Tolerance);
            foreach (var curve in revisionCloudCurvesNode)
            {
                Assert.IsNotNull(curve);
            }


            //Sheet Issues/Revisions cannot be exported from Revit, so we will read the data from the code
            //to ensure that the changes were made
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var revisions = new FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Revision))
                .Cast<Autodesk.Revit.DB.Revision>()
                .ToList();

            foreach (var rev in revisions)
            {
                string date = rev.RevisionDate;
                string description = rev.Description;
                string issued = rev.Issued.ToString();
                string issuedTo = rev.IssuedTo;
                string issuedBy = rev.IssuedBy;

                List<string> revData = new List<string>
                {
                    "18/09/2025",
                    "Revision 1",
                    "True",
                    "Amalia",
                    "Alice"
                };

                foreach (var data in revData)
                {
                    Assert.IsTrue(date == data || description == data || issued == data || issuedTo == data || issuedBy == data);
                }
            }
        }

        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void SelectRevision()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\SelectRevision.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var revisionByName = GetPreviewValue("1fd8b877bd4f4fd0ab2a18a0f0b6d1c3");
            Assert.AreEqual(revisionByName.GetType(), typeof(Revit.Elements.Revision));

            var numberType = GetPreviewValue("94abb64e177b4892a104c121bc6873d0");
            Assert.AreEqual("Alphanumeric", numberType.ToString());

            //Sheet Issues/Revisions cannot be exported from Revit, so we will read the data from the code
            //to ensure that the changes were made
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var revisions = new FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Revision))
                .Cast<Autodesk.Revit.DB.Revision>()
                .ToList();

            if (revisions.Count > 2)
            {
                var rev = revisions[2];
                string date = rev.RevisionDate;
                string description = rev.Description;
                string issued = rev.Issued.ToString();
                string visibility = rev.Visibility.ToString();

                List<string> revData = new List<string>
                {
                    "30/09/2025",
                    "Test Revision",
                    "False",
                    "Tag"
                };

                foreach (var data in revData)
                {
                    Assert.IsTrue(date == data || description == data || issued == data || visibility == data);
                }
            }
        }

        [Test]
        [TestModel(@".\Revision2025.rvt")]
        public void RevisionNumbering()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Script\RevisionNumbering.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var revisionByName = GetPreviewValue("63c45df9ebd74a7c83ae37bcafcdab18");
            Assert.AreEqual(revisionByName.GetType(), typeof(Revit.Elements.Revision));

            var revNumbering = GetPreviewValue("7ad6f002a88b491db430f5ccd0c9a3ab");
            Assert.AreEqual("PerProject", revNumbering.ToString());

            //Sheet Issues/Revisions cannot be exported from Revit, so we will read the data from the code
            //to ensure that the changes were made
            var doc = DocumentManager.Instance.CurrentDBDocument;
            var revisions = new FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Revision))
                .Cast<Autodesk.Revit.DB.Revision>()
                .ToList();

            if (revisions.Count > 2)
            {
                var rev = revisions[2];
                string date = rev.RevisionDate;
                string description = rev.Description;
                string issued = rev.Issued.ToString();
                string issuedTo = rev.IssuedTo;
                string issuedBy = rev.IssuedBy;
                string visibility = rev.Visibility.ToString();

                List<string> revData = new List<string>
                {
                    "10/10/2025",
                    "Revision PerProject",
                    "False",
                    "Alice",
                    "None"
                };

                foreach (var data in revData)
                {
                    Assert.IsTrue(date == data || description == data || issued == data || issuedTo == data 
                        || issuedBy == data || visibility == data);
                }
            }
        }

    }
}