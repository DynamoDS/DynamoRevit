using System.Linq;

using Autodesk.DesignScript.Geometry;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using Point = Autodesk.DesignScript.Geometry.Point;

namespace RevitNodesTests.Elements
{
    [TestFixture]
    public class ImportInstanceTests : RevitNodeTestBase
    {
        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByGeometry_ProvidesExpectedConversion()
        {
            // construct the cuboid in meters
            var c0 = Point.ByCoordinates(-1, -1, -1);
            var c1 = Point.ByCoordinates(1, 1, 1);
            var cuboid = Cuboid.ByCorners(c0, c1);

            // import
            var import = Revit.Elements.ImportInstance.ByGeometry(cuboid);

            // extract geometry
            var importGeometry = import.Geometry().First();

            Assert.IsAssignableFrom(typeof(Autodesk.DesignScript.Geometry.Solid), importGeometry);

            var solidImport = (Autodesk.DesignScript.Geometry.Solid)importGeometry;

            cuboid.Volume.ShouldBeApproximately(solidImport.Volume);
            cuboid.BoundingBox.MinPoint.ShouldBeApproximately(solidImport.BoundingBox.MinPoint);
            cuboid.BoundingBox.MaxPoint.ShouldBeApproximately(solidImport.BoundingBox.MaxPoint);
        }

        [Test]
        [TestModel(@".\empty.rfa")]
        public void ByGeometryAndView_ProviedesExpectedConversion()
        {
            // construct the cuboid in meters
            var c0 = Point.ByCoordinates(-1, -1, -1);
            var c1 = Point.ByCoordinates(1, 1, 1);
            var cuboid = Cuboid.ByCorners(c0, c1);

            // set the view into which the ImportInstance will be imported
            var elevation = 100;
            var level = Revit.Elements.Level.ByElevation(elevation);
            Assert.NotNull(level);
            var view = Revit.Elements.Views.FloorPlanView.ByLevel(level);
            
            // import
            var import = Revit.Elements.ImportInstance.ByGeometryAndView(cuboid,view);
            // extract geometry
            var importGeometry = import.Geometry().First();

            Assert.IsAssignableFrom(typeof(Autodesk.DesignScript.Geometry.Solid), importGeometry);

            var solidImport = (Autodesk.DesignScript.Geometry.Solid)importGeometry;

            var c2 = Point.ByCoordinates(-1, -1, -1 + elevation);
            var c3 = Point.ByCoordinates(1, 1, 1 + elevation);
            var cuboid2 = Cuboid.ByCorners(c2, c3);

            cuboid2.Volume.ShouldBeApproximately(solidImport.Volume);
            cuboid2.BoundingBox.MinPoint.ShouldBeApproximately(solidImport.BoundingBox.MinPoint);
            cuboid2.BoundingBox.MaxPoint.ShouldBeApproximately(solidImport.BoundingBox.MaxPoint);
      }
    }
}
