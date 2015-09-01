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

            (ds.Geometry().First() as Surface).Area.ShouldDifferByLessThanPercentage((dsSurf.Geometry().First() as Surface).Area, ApproximateAssertExtensions.Epsilon);

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
    }
}
       