using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;

using NUnit.Framework;

using Revit.Elements;
using Revit.GeometryReferences;

using RevitServices.Persistence;
using RevitServices.Transactions;
using RevitTestServices;
using Dynamo.Graph.Nodes;

using RTF.Framework;

using Element = Revit.Elements.Element;
using FamilyType = Revit.Elements.FamilyType;
using Wall = Autodesk.Revit.DB.Wall;
using Revit.GeometryConversion;
using Revit.Elements.Views;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class RevitLinkTests : RevitNodeTestBase
    {
        #region LinkInstance Tests

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkInstanceById()
        {
            // Unique Id of Revit Link Instance
            const string linkUniqueId = "f3758e77-7d7e-4854-a96d-d33f21866d7e-00000abb";

            // Link instance id to check 
            long linkInstanceId = 2747;
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true);

            // check selection by Unique Id
            Assert.IsNotNull(linkInstance);
            Assert.AreEqual(linkUniqueId, linkInstance.UniqueId);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkInstanceByName()
        {
            // Link Instance Name
            string linkInstanceName = "LinkA";

            // Id of Revit Link Instance
            long linkInstanceId = 2747;

            List<Revit.Elements.RevitLinkInstance> linkInstances = Revit.Elements.RevitLinkInstance.ByName(linkInstanceName);

            Assert.IsNotNull(linkInstances);
            Assert.AreEqual(1, linkInstances.Count);
            Assert.AreEqual(linkInstanceId, linkInstances.First().Id);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkInstanceByTitle()
        {
            // Revit link instance Title
            string title = "linkFile";
            const long linkInstanceId = 2747;

            List<Revit.Elements.RevitLinkInstance> linkInstancesByTitle = Revit.Elements.RevitLinkInstance.ByTitle(title);

            Assert.IsNotNull(linkInstancesByTitle);
            Assert.AreEqual(1, linkInstancesByTitle.Count);
            Assert.AreEqual(linkInstancesByTitle.First().Id, linkInstanceId);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementsByCategory()
        {
            // Link instance id
            long linkInstanceId = 2747;
            // Column element id
            long columnId = 3339;

            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            // find link elements by category
            var structuralColumnsCategory = Revit.Elements.Category.ByName("StructuralColumns");
            var roofsCategory = Revit.Elements.Category.ByName("Roofs");

            var columnsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, structuralColumnsCategory);
            var roofsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, roofsCategory);

            // Check result
            Assert.NotNull(columnsInLink);
            Assert.AreEqual(1, columnsInLink.Count);
            Assert.AreEqual(0, roofsInLink.Count);
            Assert.AreEqual(columnsInLink.First().Id, columnId);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanCreateSolidFromCropRegion()
        {
            long floorPlanViewId = 3295;
            double solidVolume = 598.0551;


            // select element by Id
            Revit.Elements.Views.View plan = ElementSelector.ByElementId(floorPlanViewId, true) as Revit.Elements.Views.View;
            var internalView = plan.InternalView;

            var currentDocument = Revit.Application.Document.Current.InternalDocument;
            var solidFromCropRegion = Revit.Elements.RevitLinkInstance.CreateSolidFromCropRegion(currentDocument, internalView as ViewPlan);

            Assert.NotNull(solidFromCropRegion);
            solidFromCropRegion.Volume.ShouldBeApproximately(solidVolume, 1e-4);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetElementsOfCategoryInView()
        {
            long linkInstanceId = 2747;
            long sectionViewId = 3284;
            long floorPlanViewId = 3295;

            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;
            Revit.Elements.Views.View section = ElementSelector.ByElementId(sectionViewId, true) as Revit.Elements.Views.View;
            Revit.Elements.Views.View plan = ElementSelector.ByElementId(floorPlanViewId, true) as Revit.Elements.Views.View;

            var structuralColumnsCategory = Revit.Elements.Category.ByName("StructuralColumns");
            var columnsInSection = Revit.Elements.RevitLinkInstance.AllElementsOfCategoryInView(linkInstance, structuralColumnsCategory, section);
            var columnsInPlan = Revit.Elements.RevitLinkInstance.AllElementsOfCategoryInView(linkInstance, structuralColumnsCategory, plan);

            Assert.NotNull(columnsInSection);
            Assert.NotNull(columnsInPlan);
            Assert.AreEqual(columnsInSection.Count, 0);
            Assert.AreEqual(columnsInPlan.Count, 1);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementsAtLevel()
        {
            // Link instance id
            long linkInstanceId = 2747;

            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            // select level
            var levelsCategory = Revit.Elements.Category.ByName("Levels");
            var levelsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, levelsCategory);

            List<Element> elementsAtLevel = Revit.Elements.RevitLinkInstance.AllElementsAtLevel(linkInstance, levelsInLink.First() as Revit.Elements.Level);
            
            Assert.NotNull(elementsAtLevel);
            Assert.AreEqual(elementsAtLevel.Count, 2);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementsByClass()
        {
            // Link instance id
            long linkInstanceId = 2747;

            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;
            Type elementClass = typeof(Wall);
            List<Element> elementsOfClass = Revit.Elements.RevitLinkInstance.AllElementsOfClass(linkInstance, elementClass);

            Assert.NotNull(elementsOfClass);
            Assert.AreEqual(elementsOfClass.Count, 1);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementById()
        {
            // Link instance id
            long linkInstanceId = 2747;
            // Column element id
            long columnId = 3339;
            string columnUniqueId = "182832a9-4aa6-4ab6-9aeb-7bcedea59ee9-00000d0b";

            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            var linkElementById = Revit.Elements.RevitLinkInstance.ElementById(columnId, linkInstance);

            Assert.NotNull(linkElementById);
            Assert.AreEqual(linkElementById.UniqueId, columnUniqueId);
            Assert.AreEqual(linkElementById.GetType(), typeof(Revit.Elements.StructuralFraming));

        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementDocument()
        {
            // Link instance id
            long linkInstanceId = 2747;
            string linkDocumentTitle = "linkFile";
            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            // Get Document
            Revit.Application.Document linkDocument = Revit.Elements.RevitLinkInstance.Document(linkInstance);

            // Check result by comparing Document titles
            Assert.NotNull(linkDocument);
            Assert.AreEqual(linkDocument.InternalDocument.Title, linkDocumentTitle);
        }


        #endregion

        #region LinkElement Tests
        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementLocation()
        {
            // Link instance id
            long linkInstanceId = 2747;
            XYZ columnLocationPoint = new XYZ(5016.27451, 2975.75163, 0);
            XYZ wallLocationEndPoint = new XYZ(3416.27451, 1675.75163, 0);
            XYZ wallLocationStartPoint = new XYZ(2569.79731, 4134.09993, 0);

            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId,true) as Revit.Elements.RevitLinkInstance;

            // get structural column
            var structuralColumnsCategory = Revit.Elements.Category.ByName("StructuralColumns");
            var wallsCategory = Revit.Elements.Category.ByName("Walls"); 
            var columnsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, structuralColumnsCategory);
            var wallsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, wallsCategory);

            // get location
            var columnLocation = Revit.Elements.LinkElement.GetLocation(columnsInLink.First()) as Autodesk.DesignScript.Geometry.Point;
            var wallLocationCurve = LinkElement.GetLocation(wallsInLink.First());
            var wallStartPoint = (wallLocationCurve as Autodesk.DesignScript.Geometry.Curve).StartPoint;
            var wallEndPoint = (wallLocationCurve as Autodesk.DesignScript.Geometry.Curve).EndPoint;

            Assert.NotNull(columnLocation);
            Assert.NotNull(wallLocationCurve);
            columnLocation.ShouldBeApproximately(columnLocationPoint, 1e-2);
            wallStartPoint.ShouldBeApproximately(wallLocationStartPoint, 1e-2);
            wallEndPoint.ShouldBeApproximately(wallLocationEndPoint, 1e-2);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void GetLinkInstanceTransform()
        {
            // Link instance id
            long linkInstanceId = 2747;
            
            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            // origin to check. The units are Dynamo units (mm for this test file)
            double transformOriginX = 1480;
            double transformOriginY = 880;
            double transformOriginZ = 0;

            // get link element 
            // find link elements by category
            var structuralColumnsCategory = Revit.Elements.Category.ByName("StructuralColumns");
            var columnsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, structuralColumnsCategory);

            var linkTransform = LinkElement.LinkTransform(columnsInLink.First());

            Assert.NotNull(linkTransform);
            Assert.AreEqual(linkTransform.Origin.X, transformOriginX, 1e-2);
            Assert.AreEqual(linkTransform.Origin.Y, transformOriginY, 1e-2);
            Assert.AreEqual(linkTransform.Origin.Z, transformOriginZ, 1e-2);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void GetLinksContainingLinkElement()
        {
            // Link instance id
            long linkInstanceId = 2747;
            
            // select element by Id
            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            var structuralColumnsCategory = Revit.Elements.Category.ByName("StructuralColumns");
            var columnsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, structuralColumnsCategory);

            // get link instances 
            List<Autodesk.Revit.DB.RevitLinkInstance> revitLinkInstances = LinkElement.GetLinkInstancesContainingLinkElement(columnsInLink.First());
            
            // check links containing element
            Assert.NotNull(revitLinkInstances);
            Assert.AreEqual(revitLinkInstances.First().Id.Value, linkInstanceId);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void GetLinkElementGeometry()
        {
            long linkInstanceId = 2747;
            double solidVolume = 2080000000; 

            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;
            
            var wallsCategory = Revit.Elements.Category.ByName("Walls");
            var wallsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, wallsCategory);

            List<object> wallGeometry = LinkElement.GetGeometry(wallsInLink.First(), "Medium") as List<object>;

            Assert.NotNull(wallGeometry);

            Autodesk.DesignScript.Geometry.Solid solid = wallGeometry.First() as Autodesk.DesignScript.Geometry.Solid; 
     
            Assert.AreEqual(solid.Volume, solidVolume, 1e-1);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementBoundingBox()
        {
            long linkInstanceId = 2747;
            XYZ BBmin = new XYZ(3321.723, 1643.195, 0);
            XYZ BBmax = new XYZ(2664.349, 4166.657, 4000.0);

            var linkInstance = ElementSelector.ByElementId(linkInstanceId, true) as Revit.Elements.RevitLinkInstance;

            var wallsCategory = Revit.Elements.Category.ByName("Walls");
            var wallsInLink = Revit.Elements.RevitLinkInstance.AllElementsOfCategory(linkInstance, wallsCategory);

            var wallBoundingBox = LinkElement.BoundingBox(wallsInLink.First());
            wallBoundingBox.MinPoint.ShouldBeApproximately(BBmin, 1e-2);
            wallBoundingBox.MaxPoint.ShouldBeApproximately(BBmax, 1e-2);
        }

        [Test]
        [TestModel(@".\Element\hostFile.rvt")]
        public void CanGetLinkElementByRaybounce()
        {
            long linkWallId = 2796;
            long threeDviewId = 3651;
            Revit.Elements.Views.View view = ElementSelector.ByElementId(threeDviewId, true) as Revit.Elements.Views.View;
            Autodesk.DesignScript.Geometry.Point origin = Autodesk.DesignScript.Geometry.Point.ByCoordinates(3935, 2873, 300);
            Vector direction = Vector.ByCoordinates(-894, 0, 0);

            Dictionary<string, object> linkElementsDictionary = LinkElement.ByRayBounce(origin, direction, 5, view as Revit.Elements.Views.View3D);
            List<Element> linkElements = linkElementsDictionary["linkedElements"] as List<Element>;

            Assert.AreEqual(linkElements.Count, 1);
            Assert.AreEqual(linkElements.First().Id, linkWallId);
        }

        #endregion
    }
}
