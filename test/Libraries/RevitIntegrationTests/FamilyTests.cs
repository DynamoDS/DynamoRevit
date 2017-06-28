using System;
using System.IO;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes;
using Dynamo.Wpf.ViewModels;
using NUnit.Framework;
using RevitServices.Elements;
using RevitTestServices;

using RTF.Framework;
using FamilyInstance = Revit.Elements.FamilyInstance;

namespace RevitSystemTests
{
    [TestFixture]
    class FamilyTests : RevitSystemTestBase
    {
        public const double Epsilon = 1e-6;

        [Test]
        [TestModel(@".\Family\GetFamilyInstancesByType.rvt")]
        public void GetFamilyInstancesByType()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Family\GetFamilyInstancesByType.dyn");
            string testPath = Path.GetFullPath(samplePath);


            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            Assert.AreEqual(100, GetPreviewValue("5eac6ab9-e736-49a9-a90a-8b6d93676813"));
        }

        [Test]
        [TestModel(@".\Family\GetFamilyInstanceLocation.rvt")]
        public void GetFamilyInstanceLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\GetFamilyInstanceLocation.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            AssertPreviewCount("4274fd18-23b8-4c5c-9006-14d927fa3ff3", 100);

        }

        [Test]
        [TestModel(@".\Family\AC_locationStandAlone.rfa")]
        public void CanLocateAdaptiveComponent()
        {
            var model = ViewModel.Model;

            string samplePath = Path.Combine(workingDirectory, @".\Family\AC_locationStandAlone.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var pnt = GetPreviewValue("79dde258-ddce-49b7-9700-da21b2d5a9ae") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            Assert.IsTrue(IsFuzzyEqual(pnt.X, -31.453185696, Epsilon));
            Assert.IsTrue(IsFuzzyEqual(pnt.Y, -115.515869423, Epsilon));
            Assert.IsTrue(IsFuzzyEqual(pnt.Z, 26.806669948, Epsilon));
        }

        [Test]
        [TestModel(@".\Family\AC_locationInDividedSurface.rfa")]
        public void CanLocateAdaptiveComponentInDividedSurface()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\AC_locationInDividedSurface.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            

            AssertPreviewCount("76076507-d16e-4480-802c-14ba87d88f81", 25);
        }

        [Test]
        [TestModel(@".\Family\SetFamilyInstanceRotation.rvt")]
        public void SetFamilyInstanceRotation()
        {
           string samplePath = Path.Combine(workingDirectory, @".\Family\SetFamilyInstanceRotation.dyn");
           string testPath = Path.GetFullPath(samplePath);

           ViewModel.OpenCommand.Execute(testPath);

           RunCurrentModel();
           var elId = new Autodesk.Revit.DB.ElementId(187030);
           var famInst = RevitServices.Persistence.DocumentManager.Instance.CurrentDBDocument.GetElement(elId) as Autodesk.Revit.DB.FamilyInstance;
           var transform = famInst.GetTransform();
           double[] rotationAngles;
           Revit.Elements.InternalUtilities.TransformUtils.ExtractEularAnglesFromTransform(transform, out rotationAngles);
           Assert.AreEqual(30.0, rotationAngles[0] * 180 / System.Math.PI, Epsilon);
        }

        [Test]
        [TestModel(@".\Family\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\FamilyInstancePlacementByFace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var famInst = GetPreviewValue("e8dbd9fa-c0fd-4a5c-9cd8-2616f98285c8") as FamilyInstance;
            Assert.IsNotNull(famInst);

            // PointAtParameter on surface
            var pnt = GetPreviewValue("31171a34-edd6-4c81-b821-2ad0bae36aab") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            var pos = famInst.Location;

            // Verify that location of family instance matches with the placement point on surface
            Assert.AreEqual(pnt.X, pos.X, Epsilon, "X property does not match");
            Assert.AreEqual(pnt.Y, pos.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(pnt.Z, pos.Z, Epsilon, "Z property does not match");

            // Element ID of Revit face selected in DYN file
            var elId = new Autodesk.Revit.DB.ElementId(250730);
            var internalFamInst = famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Assert.IsNotNull(internalFamInst);

            // Verify that the Revit face selected is the hosting face of the family instance
            Assert.AreEqual(elId, internalFamInst.HostFace.ElementId);


        }

        [Test]
        [TestModel(@".\Family\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_UpdateLocation_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\FamilyInstancePlacementByFace.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var initialNumber = GetAllFamilyInstances(false).Count;
            var famInst1 = GetPreviewValue("e8dbd9fa-c0fd-4a5c-9cd8-2616f98285c8") as FamilyInstance;
            Assert.IsNotNull(famInst1);

            // Update placement location of family instance
            var cbn = GetNode<CodeBlockNodeModel>("31b09d10-931b-4ac1-b2cc-7e04faf334dc");

            var command = new Dynamo.Models.DynamoModel.UpdateModelValueCommand(
                Guid.Empty, cbn.GUID, "Code", "0.75");
            this.Model.ExecuteCommand(command);

            RunCurrentModel();

            var finalNumber = GetAllFamilyInstances(false).Count;
            var famInst2 = GetPreviewValue("e8dbd9fa-c0fd-4a5c-9cd8-2616f98285c8") as FamilyInstance;
            Assert.IsNotNull(famInst2);

            // PointAtParameter on surface
            var pnt = GetPreviewValue("31171a34-edd6-4c81-b821-2ad0bae36aab") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(pnt);

            var pos = famInst2.Location;

            // Verify that location of family instance matches with the placement point on surface
            Assert.AreEqual(pnt.X, pos.X, Epsilon, "X property does not match");
            Assert.AreEqual(pnt.Y, pos.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(pnt.Z, pos.Z, Epsilon, "Z property does not match");

            // Element ID of Revit face selected in DYN file
            var elId = new Autodesk.Revit.DB.ElementId(250730);
            var internalFamInst = famInst2.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Assert.IsNotNull(internalFamInst);

            // Verify that the Revit face selected is the hosting face of the family instance
            Assert.AreEqual(elId, internalFamInst.HostFace.ElementId);

            // Assert that there is no change in the total number of family instances in the document
            // as the original should have been updated and no new one should be created
            Assert.AreEqual(initialNumber, finalNumber);
        }

        [Test]
        [TestModel(@".\Family\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_LineReference_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\FamilyInstancePlacementByLine.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            // FamilyInstance.ByFace (with Line overload) node
            var famInst = GetPreviewValue("f8a4485d-6bfa-413c-a547-60a8df5022cf") as FamilyInstance;
            Assert.IsNotNull(famInst);

            // Start point and end point of input line
            var startPnt = GetPreviewValue("8a558ab3-4662-4cdb-b3c8-97833777d101") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(startPnt);

            var endPnt = GetPreviewValue("53c60050-5a39-4e51-be36-a2b9e8f07069") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(endPnt);

            // Start point and end point of family instance line location
            var pos = famInst.InternalElement.Location as LocationCurve;
            Assert.IsNotNull(pos);

            var sp = pos.Curve.GetEndPoint(0);
            var ep = pos.Curve.GetEndPoint(1);

            // Verify that line location of family instance matches with the input line
            Assert.AreEqual(startPnt.X, sp.X, Epsilon, "X property does not match");
            Assert.AreEqual(startPnt.Y, sp.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(startPnt.Z, sp.Z, Epsilon, "Z property does not match");

            Assert.AreEqual(endPnt.X, ep.X, Epsilon, "X property does not match");
            Assert.AreEqual(endPnt.Y, ep.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(endPnt.Z, ep.Z, Epsilon, "Z property does not match");

            // Element ID of Revit face selected in DYN file
            var elId = new Autodesk.Revit.DB.ElementId(250730);
            var internalFamInst = famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Assert.IsNotNull(internalFamInst);

            // Verify that the Revit face selected is the hosting face of the family instance
            Assert.AreEqual(elId, internalFamInst.HostFace.ElementId);
        }

        [Test]
        [TestModel(@".\Family\FamilyInstancePlacementByFace.rvt")]
        public void ByFace_UpdateLineLocation_ProducesValidFamilyInstanceWithCorrectLocation()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\FamilyInstancePlacementByLine.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var initialNumber = GetAllFamilyInstances(false).Count;

            // Update input line used to create family instance
            // connect Surface.PointAtParameter node to endPoint of Line.StartPointEndPoint node
            var pointAtParamGuid = "434acfa6-e253-449e-9493-8a96c53725e1";
            var lineGuid = "2b3ab51b-1117-42c2-a747-03dc5d8b3195";

            Model.ExecuteCommand(new Dynamo.Models.DynamoModel.MakeConnectionCommand(pointAtParamGuid, 0, PortType.Output,
                Dynamo.Models.DynamoModel.MakeConnectionCommand.Mode.Begin));
            Model.ExecuteCommand(new Dynamo.Models.DynamoModel.MakeConnectionCommand(lineGuid, 1, PortType.Input,
                Dynamo.Models.DynamoModel.MakeConnectionCommand.Mode.End));

            RunCurrentModel();

            var finalNumber = GetAllFamilyInstances(false).Count;

            // FamilyInstance.ByFace (with Line overload) node
            var famInst = GetPreviewValue("f8a4485d-6bfa-413c-a547-60a8df5022cf") as FamilyInstance;
            Assert.IsNotNull(famInst);

            // Start point and end point of input line
            var startPnt = GetPreviewValue("8a558ab3-4662-4cdb-b3c8-97833777d101") as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(startPnt);

            var endPnt = GetPreviewValue(pointAtParamGuid) as Autodesk.DesignScript.Geometry.Point;
            Assert.IsNotNull(endPnt);

            // Start point and end point of family instance line location
            var pos = famInst.InternalElement.Location as LocationCurve;
            Assert.IsNotNull(pos);

            var sp = pos.Curve.GetEndPoint(0);
            var ep = pos.Curve.GetEndPoint(1);

            // Verify that line location of family instance matches with the input line
            Assert.AreEqual(startPnt.X, sp.X, Epsilon, "X property does not match");
            Assert.AreEqual(startPnt.Y, sp.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(startPnt.Z, sp.Z, Epsilon, "Z property does not match");

            Assert.AreEqual(endPnt.X, ep.X, Epsilon, "X property does not match");
            Assert.AreEqual(endPnt.Y, ep.Y, Epsilon, "Y property does not match");
            Assert.AreEqual(endPnt.Z, ep.Z, Epsilon, "Z property does not match");

            // Element ID of Revit face selected in DYN file
            var elId = new Autodesk.Revit.DB.ElementId(250730);
            var internalFamInst = famInst.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Assert.IsNotNull(internalFamInst);

            // Verify that the Revit face selected is the hosting face of the family instance
            Assert.AreEqual(elId, internalFamInst.HostFace.ElementId);

            // Assert that there is no change in the total number of family instances in the document
            // as the original should have been updated and no new one should be created
            Assert.AreEqual(initialNumber, finalNumber);
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void CreateFamilyTypeByGeometry()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Family\CreateFamilyTypeByGeometry.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();

            var famInst = GetPreviewValue("df1bc5da-b9de-4fb2-8c9b-9aa7c8997834");
            Assert.IsNotNull(famInst);
            Assert.IsTrue(typeof(Revit.Elements.FamilyType) == famInst.GetType());
            
        }
    }
}
