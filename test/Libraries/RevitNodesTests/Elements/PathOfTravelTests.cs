using Autodesk.DesignScript.Geometry;

using Revit.Elements;
using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;
using Revit.Elements.Views;

namespace RevitNodesTests.Elements
{
   [TestFixture]
   public class PathOfTravelTests : RevitNodeTestBase
   {
      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_ValidArgs()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            false);
       
         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(pathOfTravelOneToOne.GetLength(0), 1);
         Assert.NotNull(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(pathOfTravelManyToMany.GetLength(0), 1);
         Assert.NotNull(pathOfTravelManyToMany[0]);
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_NullView()
      {
         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               null,                                                 // null view
               new Point[] { Point.ByCoordinates(0, 0, 0) },
               new Point[] { Point.ByCoordinates(100, 100, 0) },
               false));

         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               null,                                                 // null view
               new Point[] { Point.ByCoordinates(0, 0, 0) },
               new Point[] { Point.ByCoordinates(100, 100, 0) },
               true));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_NullStartPointArray()
      {
         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               GetFloorPlan(),                                       // null start point array
               null,
               new Point[] { Point.ByCoordinates(100, 100, 0) },
               false));

         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               GetFloorPlan(),                                       // null start point array
               null,
               new Point[] { Point.ByCoordinates(100, 100, 0) },
               true));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_InvalidEndPoint()
      {
         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               GetFloorPlan(),
               new Point[] { Point.ByCoordinates(0, 0, 0) },
               null,                                                 // null end point array
               false));

         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.ByFloorPlanPoints(
               GetFloorPlan(),
               new Point[] { Point.ByCoordinates(0, 0, 0) },
               null,                                                 // null end point array
               true));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_DifferentPointCounts()
      {
         Assert.Throws(
            typeof(System.ArgumentException),
            () => PathOfTravel.ByFloorPlanPoints(
               GetFloorPlan(),
               new Point[] { Point.ByCoordinates(0, 0, 0) },
               new Point[] { Point.ByCoordinates(100, 100, 0), Point.ByCoordinates(100, 100, 100) },
               false));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_PointsIdentical()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            false);

         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(pathOfTravelOneToOne.GetLength(0), 1);
         Assert.Null(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(pathOfTravelManyToMany.GetLength(0), 1);
         Assert.Null(pathOfTravelManyToMany[0]);
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void Create_PointsTooClose()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(double.Epsilon / 2, 0, 0) },
            false);

         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(pathOfTravelOneToOne.GetLength(0), 1);
         Assert.Null(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(double.Epsilon / 2, 0, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(pathOfTravelManyToMany.GetLength(0), 1);
         Assert.Null(pathOfTravelManyToMany[0]);
      }

      FloorPlanView GetFloorPlan()
      {
         var elevation = 0;
         var level = Level.ByElevation(elevation);
         Assert.NotNull(level);

         var view = FloorPlanView.ByLevel(level);
         Assert.NotNull(view);

         return view;
      }

   }
}
