using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.GeometryConversion;
using RevitTestServices;
using RTF.Framework;
using System.Linq;

namespace RevitNodesTests.GeometryConversion
{
    [TestFixture]
    public class RevitToProtoMeshTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\toposolid.rvt")]
        public void ToProtoType_ShouldReturnCorrectlyScaledMesh()
        {
            var allSolidsInDoc = ElementSelector.ByType<Autodesk.Revit.DB.Toposolid>(true)
             .Cast<Revit.Elements.Toposolid>()
             .SelectMany(x => x.InternalGeometry())
             .OfType<Autodesk.Revit.DB.Solid>()
             .Where(solid => solid != null && solid.Faces.Size > 0)
             .ToList();

            var allMeshesInDoc = allSolidsInDoc
                .SelectMany(solid => solid.Faces.OfType<Autodesk.Revit.DB.Face>())
                .Select(face => face.Triangulate())
                .Where(mesh => mesh != null)
                .ToList();

            Assert.AreEqual(2292, allMeshesInDoc.Count, "There should be 2292 meshes in toposolid.rvt.  Has the file changed?");

            var convertedMeshes = allMeshesInDoc.Select(m => m.ToProtoType()).ToList();
            Assert.IsTrue(convertedMeshes.All(m => m != null));

            var allVertices = convertedMeshes.SelectMany(m => m.VertexPositions).ToList();
            Assert.IsTrue(allVertices.Count > 0);

            var bb = BoundingBox.ByGeometry(allVertices);

            var bbvol = BoundingBoxTests.BoundingBoxVolume(bb);
            var bbmin = bb.MinPoint;
            var bbmax = bb.MaxPoint;

            bbvol.ShouldBeApproximately(76865.989621639252, 1e-1);
            bbmin.X.ShouldBeApproximately(0.0, 1e-2);
            bbmax.X.ShouldBeApproximately(99.0, 1e-2);

            convertedMeshes.ForEach(m => m.Dispose());
      }
    }
}
