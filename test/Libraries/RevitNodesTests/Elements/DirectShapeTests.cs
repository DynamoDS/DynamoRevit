using System;
using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;
using RTF.Framework;
using Autodesk.DesignScript.Geometry;
using RevitServices.Persistence;
using System.Linq;
using System.Collections.Generic;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    class DirectShapeTests : RevitNodeTestBase
    {
        //this test fails because the materialsManager is not initialzed without DynamoRevitModel
        [Test,Category("Failure")]
        [TestModel(@".\empty.rfa")]
        public void ByGeoNameCategory_ValidInput()
        {
            var sphere = Sphere.ByCenterPointRadius(Point.Origin());
            
            var ds = DirectShape.ByGeometryNameCategory(sphere,"a sphere", Category.ByName("OST_GenericModel"));
            sphere.Dispose();
            Assert.NotNull(ds);
            Assert.AreEqual("a sphere", ds.Name);
            ds.Centroid.DistanceTo(Point.Origin()).ShouldBeApproximately(0);
           
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void BySolidNameCategoryMaterial_ValidInput()
        {

            var sphere = Sphere.ByCenterPointRadius(Point.Origin());
            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();
            
            var ds = DirectShape.ByGeometryNameCategoryMaterial(sphere, "a sphere", Category.ByName("OST_GenericModel"),Material.ByName(mat.Name));
            
            Assert.NotNull(ds);
            Assert.AreEqual("a sphere", ds.Name);
            Assert.AreEqual(sphere.Tags.LookupTag(ds.InternalElement.Id.ToString()) as Autodesk.Revit.DB.ElementId,mat.Id);
            ds.Centroid.DistanceTo(Point.Origin()).ShouldBeApproximately(0);
            sphere.Dispose();
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void BySurfNameCategoryMaterial_ValidInput()
        {

            var cube = Cuboid.ByLengths();
            var faces = cube.Faces;
            var surfs = faces.Select(x=>x.SurfaceGeometry()).ToList();
            var surf = PolySurface.ByJoinedSurfaces(surfs);

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();

            var ds = DirectShape.ByGeometryNameCategoryMaterial(surf, "a polysurface", Category.ByName("OST_GenericModel"), Material.ByName(mat.Name));

            Assert.NotNull(ds);
            Assert.AreEqual("a polysurface", ds.Name);
            Assert.AreEqual(surf.Tags.LookupTag(ds.InternalElement.Id.ToString()) as Autodesk.Revit.DB.ElementId, mat.Id);
            ds.Centroid.DistanceTo(Point.Origin()).ShouldBeApproximately(0);
            surf.Dispose();
            surfs.ForEach(x => x.Dispose());
            faces.ForEach(x => x.Dispose());
            cube.Dispose();
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshNameCategoryMaterial_ValidInput()
        {

           var p1 = Point.ByCoordinates(0.0,0.0,0.0);
            var p2 = Point.ByCoordinates(1.0,1.0,0);
            var p3 = Point.ByCoordinates(2.0,0,0);

            var index1 = IndexGroup.ByIndices(0,1,2);

           var mesh= Mesh.ByPointsFaceIndices(new List<Point>() { p1, p2, p3 }, new List<IndexGroup>() { index1 });
            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();

            var ds = DirectShape.ByMeshNameCategoryMaterial(mesh, "a mesh", Category.ByName("OST_GenericModel"), Material.ByName(mat.Name));

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            Assert.AreEqual(mesh.Tags.LookupTag(ds.InternalElement.Id.ToString()) as Autodesk.Revit.DB.ElementId, mat.Id);
            ds.Centroid.DistanceTo(Point.Origin()).ShouldBeApproximately(0);
            mesh.Dispose();
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshAndBySurfaceBothLocatedSameMetric()
        {

            DocumentManager.Instance.CurrentDBDocument.SetUnits(new Autodesk.Revit.DB.Units(Autodesk.Revit.DB.UnitSystem.Metric));

            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(1.0, 1.0, 0);
            var p3 = Point.ByCoordinates(2.0, 0, 0);

            var index1 = IndexGroup.ByIndices(0, 1, 2);

            var mesh = Mesh.ByPointsFaceIndices(new List<Point>() { p1, p2, p3 }, new List<IndexGroup>() { index1 });
            var surf = Surface.ByPerimeterPoints(new List<Point>() { p1, p2, p3 });

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();

            var ds = DirectShape.ByMeshNameCategoryMaterial(mesh, "a mesh", Category.ByName("OST_GenericModel"), Material.ByName(mat.Name));
            var dsSurf = DirectShape.ByGeometryNameCategoryMaterial(surf, "a surf", Category.ByName("OST_GenericModel"), Material.ByName(mat.Name));
            ds.Centroid.DistanceTo(dsSurf.Centroid).ShouldBeApproximately(0);

            Surface.ByPerimeterPoints((ds.Geometry().First() as Mesh).VertexPositions).Area.ShouldDifferByLessThanPercentage((dsSurf.Geometry().First() as Surface).Area, ApproximateAssertExtensions.Epsilon);

            mesh.Dispose();
            surf.Dispose();
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NonexistentName()
        {
            var line = Line.ByStartPointEndPoint(Point.Origin(),Point.ByCoordinates(1.0,2.0,3.0));
            Assert.Throws(typeof(ArgumentException), () => DirectShape.ByGeometryNameCategory(line,"noshape",Category.ByName("OST_GenericModel")));
        }
    }
}
       