using System;
using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;
using RTF.Framework;
using Autodesk.DesignScript.Geometry;
using RevitServices.Persistence;
using System.Linq;
using System.Collections.Generic;
using RevitServices.Transactions;
using Revit.GeometryConversion;
using RevitServices.Materials;


namespace RevitNodesTests.Elements
{
    [TestFixture]
    class DirectShapeTests : RevitNodeTestBase
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            //setup the material manager just for tests
            var mgr = MaterialsManager.Instance;
            mgr.InitializeForActiveDocumentOnIdle();
        }


        private Point BoundingBoxCentroid(DirectShape ds)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            DocumentManager.Regenerate();
            var Revitbb = ds.InternalElement.get_BoundingBox(null);
            TransactionManager.Instance.TransactionTaskDone();
            var bb = Revitbb.ToProtoType();
            var cube = bb.ToCuboid();
            var point = cube.Centroid();
            bb.Dispose();
            cube.Dispose();

            return point;
        }

        private int CheckNumTriangles(DirectShape ds)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            DocumentManager.Regenerate();
            var Revitgeo = ds.InternalElement.get_Geometry(new Autodesk.Revit.DB.Options());
            TransactionManager.Instance.TransactionTaskDone();
            return (Revitgeo.First() as Autodesk.Revit.DB.Mesh).NumTriangles;
        }

        private static DirectShape CreateDirectShapeFromQuadPoints(Point p1, Point p2, Point p3, Point p4)
        {
            var index = IndexGroup.ByIndices(0, 1, 2, 3);
            var mesh = Mesh.ByPointsFaceIndices(new List<Point>() { p1, p2, p3, p4 }, new List<IndexGroup>() { index });
            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();
            var ds = DirectShape.ByMesh(mesh, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a mesh");
            mesh.Dispose();
            return ds;
        }

        private void ByBrepNameCategoryMaterial_Cuboid()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(10.0, 10.0, 10.0);
            var p3 = Point.ByCoordinates(2.5, 2.5, 0.0);
            var p4 = Point.ByCoordinates(7.5, 7.5, 10.0);

            // Create a cube
            var c1 = Cuboid.ByCorners(p1, p2);
            // Create another smaller cube
            var c2 = Cuboid.ByCorners(p3, p4);
            // Combine the cubes
            var c3 = c1.Difference(c2);

            c1.Dispose();
            c2.Dispose();

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();
            var ds = DirectShape.ByGeometry(c3, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a cube with a square hole");
            c3.Dispose();

            Assert.NotNull(ds);

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            DocumentManager.Regenerate();
            var Revitgeo = ds.InternalElement.get_Geometry(new Autodesk.Revit.DB.Options());
            TransactionManager.Instance.TransactionTaskDone();
            var geo = Revitgeo.First() as Autodesk.Revit.DB.Solid;

            Assert.NotNull(geo);
            // Check number of faces
            Assert.AreEqual(10, geo.Faces.Size);
            // Check number of Edges
            Assert.AreEqual(24, geo.Edges.Size);
            // Check that material is set
            Assert.AreNotEqual(-1, geo.Faces.get_Item(0).MaterialElementId.IntegerValue);

            // Check bounding box to make sure we got unit conversion right
            ds.BoundingBox.MaxPoint.ShouldBeApproximately(10.0, 10.0, 10.0);
        }

        private void ByBrepNameCategoryMaterial_Surface()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(0.0, 10.0, 0.0);
            var p3 = Point.ByCoordinates(5.0, 10.0, 0.0);
            var p4 = Point.ByCoordinates(5.0, 5.0, 0.0);
            var p5 = Point.ByCoordinates(10.0, 5.0, 0.0);
            var p6 = Point.ByCoordinates(10.0, 0.0, 0.0);

            // Create a L-Shaped surface
            var s1 = Surface.ByPerimeterPoints(new List<Point>() { p1, p2, p3, p4, p5, p6 });

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();
            var ds = DirectShape.ByGeometry(s1, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a l-shaped surface");
            s1.Dispose();

            Assert.NotNull(ds);

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            DocumentManager.Regenerate();
            var Revitgeo = ds.InternalElement.get_Geometry(new Autodesk.Revit.DB.Options());
            TransactionManager.Instance.TransactionTaskDone();
            var geo = Revitgeo.First() as Autodesk.Revit.DB.Solid;

            Assert.NotNull(geo);
            // Check number of faces
            Assert.AreEqual(1, geo.Faces.Size);
            // Check number of Edges
            Assert.AreEqual(6, geo.Edges.Size);
            // Check that material is set
            Assert.AreNotEqual(-1, geo.Faces.get_Item(0).MaterialElementId.IntegerValue);

            // Check bounding box to make sure we got unit conversion right
            ds.BoundingBox.MaxPoint.ShouldBeApproximately(10.0, 10.0, 0.0);
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void BySolidNameCategoryMaterial_ValidInput()
        {

            var sphere = Sphere.ByCenterPointRadius(Point.Origin());
            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();

            var ds = DirectShape.ByGeometry(sphere, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a sphere");
            
            Assert.NotNull(ds);
            Assert.AreEqual("a sphere", ds.Name);
            Assert.AreEqual((sphere.Tags.LookupTag(ds.InternalElement.Id.ToString()) as DirectShapeState).materialId, mat.Id.IntegerValue);
            BoundingBoxCentroid(ds).DistanceTo(Point.Origin()).ShouldBeApproximately(0);
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

            var ds = DirectShape.ByGeometry(surf,Category.ByName("OST_GenericModel"), Material.ByName(mat.Name),"a polysurface");

            Assert.NotNull(ds);
            Assert.AreEqual("a polysurface", ds.Name);
            Assert.AreEqual((surf.Tags.LookupTag(ds.InternalElement.Id.ToString())as DirectShapeState).materialId, mat.Id.IntegerValue);
            BoundingBoxCentroid(ds).DistanceTo(Point.Origin()).ShouldBeApproximately(0);
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

            var ds = DirectShape.ByMesh(mesh, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a mesh");

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            Assert.AreEqual((mesh.Tags.LookupTag(ds.InternalElement.Id.ToString()) as DirectShapeState).materialId, mat.Id.IntegerValue);
            BoundingBoxCentroid(ds).DistanceTo(Point.Origin()).ShouldBeApproximately(0);
            mesh.Dispose();
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshAndBySurfaceBothLocatedSameMetric()
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            DocumentManager.Instance.CurrentDBDocument.SetUnits(new Autodesk.Revit.DB.Units(Autodesk.Revit.DB.UnitSystem.Metric));

            TransactionManager.Instance.TransactionTaskDone();

            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(1.0, 1.0, 0);
            var p3 = Point.ByCoordinates(2.0, 0, 0);

            var index1 = IndexGroup.ByIndices(0, 1, 2);

            var mesh = Mesh.ByPointsFaceIndices(new List<Point>() { p1, p2, p3 }, new List<IndexGroup>() { index1 });
            var surf = Surface.ByPerimeterPoints(new List<Point>() { p1, p2, p3 });

            var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();

            var ds = DirectShape.ByMesh(mesh,  Category.ByName("OST_GenericModel"), Material.ByName(mat.Name),"a mesh");
            var dsSurf = DirectShape.ByGeometry(surf, Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), "a surf");
            BoundingBoxCentroid(ds).DistanceTo(BoundingBoxCentroid(dsSurf)).ShouldBeApproximately(0);

            Surface.ByPerimeterPoints((ds.Geometry().First() as Mesh).VertexPositions).Area.ShouldDifferByLessThanPercentage(
                (dsSurf.Geometry().First() as Surface).Area, ApproximateAssertExtensions.Epsilon);


            mesh.Dispose();
            surf.Dispose();
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByName_NonexistentName()
        {
             var mat = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.Material>().First();
            var line = Line.ByStartPointEndPoint(Point.Origin(),Point.ByCoordinates(1.0,2.0,3.0));
            Assert.Throws(typeof(ArgumentException), () => DirectShape.ByGeometry(line,Category.ByName("OST_GenericModel"), Material.ByName(mat.Name), name:"noshape"));
        }

       
        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshNameCategoryMaterial_QuadPlanar()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(5.0, 0.0, 0.0);
            var p3 = Point.ByCoordinates(5.0, 5.0, 0.0);
            var p4 = Point.ByCoordinates(0.0, 5.0, 0.0);

            var ds =CreateDirectShapeFromQuadPoints(p1, p2, p3, p4);

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            //make sure there are two tris in Revit and a mesh comes back into Dynamo
            Assert.AreEqual(2, CheckNumTriangles(ds));
            Assert.AreEqual(1, ds.Geometry().Count());
        }


        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshNameCategoryMaterial_TriWith4PointsPlanarForms()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(2.5, 0.0, 0.0);
            var p3 = Point.ByCoordinates(5.0, 0.0, 0.0);
            var p4 = Point.ByCoordinates(0.0, 5.0, 0.0);

            var ds = CreateDirectShapeFromQuadPoints(p1, p2, p3, p4);

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            //make sure there is one tri in Revit and a mesh comes back into Dynamo
            Assert.AreEqual(1, CheckNumTriangles(ds));
            Assert.AreEqual(1, ds.Geometry().Count());
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshNameCategoryMaterial_QuadNonPlanar()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(5.0, 0.0, 0.0);
            var p3 = Point.ByCoordinates(5.0, 5.0, 0.0);
            var p4 = Point.ByCoordinates(0.0, 5.0, 3.0);

            var ds = CreateDirectShapeFromQuadPoints(p1, p2, p3, p4);

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            //make sure there are two tris in Revit and a mesh comes back into Dynamo
            Assert.AreEqual(2, CheckNumTriangles(ds));
            Assert.AreEqual(1, ds.Geometry().Count());
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByMeshNameCategoryMaterial_QuadNonPlanarTwisted()
        {
            var p1 = Point.ByCoordinates(0.0, 0.0, 0.0);
            var p2 = Point.ByCoordinates(-5.0, 0.0, 0.0);
            var p3 = Point.ByCoordinates(5.0, 5.0, 0.0);
            var p4 = Point.ByCoordinates(0.0, 5.0, 0.0);

            var ds = CreateDirectShapeFromQuadPoints(p1, p2, p3, p4);

            Assert.NotNull(ds);
            Assert.AreEqual("a mesh", ds.Name);
            //make sure there are two tris in Revit and a mesh comes back into Dynamo
            Assert.AreEqual(2, CheckNumTriangles(ds));
            Assert.AreEqual(1, ds.Geometry().Count());
        }

        [Test]
        [TestModel(@".\empty-metric.rvt")]
        public void ByBrepNameCategoryMaterial_Surface_Metric()
        {
            ByBrepNameCategoryMaterial_Surface();
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByBrepNameCategoryMaterial_Surface_Imperial()
        {
            ByBrepNameCategoryMaterial_Surface();
        }

        [Test]
        [TestModel(@".\empty-metric.rvt")]
        public void ByBrepNameCategoryMaterial_Cuboid_Metric()
        {
            ByBrepNameCategoryMaterial_Cuboid();
        }

        [Test]
        [TestModel(@".\empty.rvt")]
        public void ByBrepNameCategoryMaterial_Cuboid_Imperial()
        {
            ByBrepNameCategoryMaterial_Cuboid();
        }
    }
}

       