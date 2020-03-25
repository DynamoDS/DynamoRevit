using Autodesk.DesignScript.Geometry;
using NUnit.Framework;
using Revit.Elements;
using Revit.Elements.Views;
using RevitServices.Persistence;
using RevitTestServices;
using RTF.Framework;

namespace RevitNodesTests.Elements
{
   [TestFixture]
   public class PathOfTravelTests : RevitNodeTestBase
   {
      [Test]
      [TestModel(@".\Empty.rvt")]
      public void LongestOfShortestExitPaths_NullView()
      {
         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.LongestOfShortestExitPaths(
               null,
               new Point[] { Point.ByCoordinates(0, 0, 0) }));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void LongestOfShortestExitPaths_NullExitPointsArray()
      {
         Assert.Throws(
            typeof(System.ArgumentNullException),
            () => PathOfTravel.LongestOfShortestExitPaths(
               GetFloorPlan(),
               null));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void LongestOfShortestExitPaths_EmptyExitPointsArray()
      {
         Assert.Throws(
            typeof(System.ArgumentException),
            () => PathOfTravel.LongestOfShortestExitPaths(
               GetFloorPlan(),
               new Point[] { }));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void LongestOfShortestExitPaths_NullsInExitPointsArray()
      {
         Assert.Throws(
            typeof(System.ArgumentException),
            () => PathOfTravel.LongestOfShortestExitPaths(
               GetFloorPlan(),
               new Point[] { Point.ByCoordinates(0, 0, 0), null }));
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void LongestOfShortestExitPaths_NoRoomsInFloor()
      {
         Assert.Throws(
            typeof(System.ArgumentException),
            () => PathOfTravel.LongestOfShortestExitPaths(
               GetFloorPlan(),
               new Point[] { Point.ByCoordinates(0, 0, 0) }));
      }

      [Test]
      [TestModel(@".\PathOfTravel\RoomsAndExits.rvt")]
      public void LongestOfShortestExitPaths_ValidArgOneReturn()
      {
         Autodesk.Revit.DB.ViewPlan defaultView = (Autodesk.Revit.DB.ViewPlan) GetDefaultViewPlan();

         var longestOfShortestPaths = PathOfTravel.LongestOfShortestExitPaths(
               defaultView.ToDSType(true) as FloorPlanView,
               new Point[] { Point.ByCoordinates(23.349, 2.508, 3.625),
                             Point.ByCoordinates(12.892, 7.285, 3.625) });

         Assert.NotNull(longestOfShortestPaths);
         Assert.AreEqual(1, longestOfShortestPaths.Length);
         Assert.NotNull(longestOfShortestPaths[0]);

         int boudingBoxTest = longestOfShortestPaths[0].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = 23.349, Y = -46.619, Z = 0.000), MaxPoint = Point(X = 62.349, Y = 2.508, Z = 0.000))");

         Assert.AreEqual(0, boudingBoxTest);
      }

      [Test]
      [TestModel(@".\PathOfTravel\RoomsAndExits.rvt")]
      public void LongestOfShortestExitPaths_ValidArgTwoReturns()
      {
         Autodesk.Revit.DB.ViewPlan defaultView = (Autodesk.Revit.DB.ViewPlan) GetDefaultViewPlan();

         var longestOfShortestPaths = PathOfTravel.LongestOfShortestExitPaths(
               defaultView.ToDSType(true) as FloorPlanView,
               new Point[] { Point.ByCoordinates(12.892, -7.285, 3.625),
                             Point.ByCoordinates(33.806, -7.285, 3.625) });

         Assert.NotNull(longestOfShortestPaths);
         Assert.AreEqual(2, longestOfShortestPaths.Length);
         Assert.NotNull(longestOfShortestPaths[0]);
         Assert.NotNull(longestOfShortestPaths[1]);

         int boudingBoxTest1 = longestOfShortestPaths[0].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = 33.806, Y = -7.285, Z = 0.000), MaxPoint = Point(X = 62.349, Y = 31.965, Z = 0.000))");

         int boudingBoxTest2 = longestOfShortestPaths[1].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = -16.235, Y = -7.285, Z = 0.000), MaxPoint = Point(X = 12.892, Y = 31.965, Z = 0.000))");

         Assert.AreEqual(0, boudingBoxTest1);
         Assert.AreEqual(0, boudingBoxTest2);
      }

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
         // First argument in AreEqual assert is used as expected value in the assertion failure message.
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.NotNull(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(1, pathOfTravelManyToMany.GetLength(0));
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
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.Null(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(1, pathOfTravelManyToMany.GetLength(0));
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
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.Null(pathOfTravelOneToOne[0]);

         var pathOfTravelManyToMany = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(double.Epsilon / 2, 0, 0) },
            true);

         Assert.NotNull(pathOfTravelManyToMany);
         Assert.AreEqual(1, pathOfTravelManyToMany.GetLength(0));
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
      
      Autodesk.Revit.DB.View GetDefaultViewPlan()
      {
         var doc = DocumentManager.Instance.CurrentDBDocument;

         Assert.NotNull(doc.ActiveView);
         return doc.ActiveView;
      }
   }
}
