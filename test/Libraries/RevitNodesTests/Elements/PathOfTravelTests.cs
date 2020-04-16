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
               new Point[] { Point.ByCoordinates(23.340, 2.494, 3.625),
                             Point.ByCoordinates(12.883, -7.299, 3.625) });

         Assert.NotNull(longestOfShortestPaths);
         Assert.AreEqual(1, longestOfShortestPaths.Length);
         Assert.NotNull(longestOfShortestPaths[0]);

         int boudingBoxTest = longestOfShortestPaths[0].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = 12.883, Y = -46.633, Z = 0.000), MaxPoint = Point(X = 62.340, Y = -7.299, Z = 0.000))");

         Assert.AreEqual(0, boudingBoxTest);
      }

      [Test]
      [TestModel(@".\PathOfTravel\RoomsAndExits.rvt")]
      public void LongestOfShortestExitPaths_ValidArgTwoReturns()
      {
         Autodesk.Revit.DB.ViewPlan defaultView = (Autodesk.Revit.DB.ViewPlan) GetDefaultViewPlan();

         var longestOfShortestPaths = PathOfTravel.LongestOfShortestExitPaths(
               defaultView.ToDSType(true) as FloorPlanView,
               new Point[] { Point.ByCoordinates(12.883, -7.299, 3.625),
                             Point.ByCoordinates(33.796, -7.299, 3.625) });

         Assert.NotNull(longestOfShortestPaths);
         Assert.AreEqual(2, longestOfShortestPaths.Length);
         Assert.NotNull(longestOfShortestPaths[0]);
         Assert.NotNull(longestOfShortestPaths[1]);

         int boudingBoxTest1 = longestOfShortestPaths[0].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = 33.796, Y = -7.299, Z = 0.000), MaxPoint = Point(X = 62.340, Y = 31.951, Z = 0.000))");

         int boudingBoxTest2 = longestOfShortestPaths[1].BoundingBox.ToString().CompareTo(
            "BoundingBox(MinPoint = Point(X = -16.244, Y = -7.299, Z = 0.000), MaxPoint = Point(X = 12.883, Y = 31.951, Z = 0.000))");

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

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void WayPointInsertGet_ValidArgs()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            false);

         // Assert created Path of Travel is valid.
         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.NotNull(pathOfTravelOneToOne[0]);

         // Current Path of Travel.
         var pathOfTravel = pathOfTravelOneToOne[0];

         // Assert there is no way points on current Path of Travel.
         var listWayPointsEmpty = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsEmpty);
         Assert.AreEqual(0, listWayPointsEmpty.Count);

         // Assert only 0 index can be used to insert a way point.
         Assert.Throws(
            typeof(Autodesk.Revit.Exceptions.InvalidOperationException),
            () => pathOfTravel.InsertWayPoint( Point.ByCoordinates(50, 50, 0), -1));
         Assert.Throws(
            typeof(Autodesk.Revit.Exceptions.InvalidOperationException),
            () => pathOfTravel.InsertWayPoint( Point.ByCoordinates(50, 50, 0), 1));

         // Assert we can insert a way point at valid index
         pathOfTravel.InsertWayPoint(Point.ByCoordinates(50, 50, 0), 0);
         var listWayPointsOneElem = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsOneElem);
         Assert.AreEqual(1, listWayPointsOneElem.Count);
         Assert.NotNull(listWayPointsOneElem[0]);
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsOneElem[0].ToString());

         // Assert valid indices to insert way points are 0 and 1.
         Assert.Throws(
            typeof(Autodesk.Revit.Exceptions.InvalidOperationException),
            () => pathOfTravel.InsertWayPoint( Point.ByCoordinates(25, 25, 0), -1));
         Assert.Throws(
            typeof(Autodesk.Revit.Exceptions.InvalidOperationException),
            () => pathOfTravel.InsertWayPoint( Point.ByCoordinates(25, 25, 0), 2));

         // Assert way point is inserted at index 0.
         // inserting at index 1 is tested in WayPointSetGet_ValidArgs.
         pathOfTravel.InsertWayPoint( Point.ByCoordinates(25, 25, 0), 0);
         var listWayPointsTwoElems = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsTwoElems);
         Assert.AreEqual(2, listWayPointsTwoElems.Count);
         Assert.NotNull(listWayPointsTwoElems[0]);
         Assert.NotNull(listWayPointsTwoElems[1]);
         Assert.AreEqual("(25.000000000, 25.000000000, 0.000000000)", listWayPointsTwoElems[0].ToString());
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsTwoElems[1].ToString());
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void WayPointSetGet_ValidArgs()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            false);

         // Assert created Path of Travel is valid.
         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.NotNull(pathOfTravelOneToOne[0]);

         // Current Path of Travel.
         var pathOfTravel = pathOfTravelOneToOne[0];

         // Assert we can insert a way point at valid index
         pathOfTravel.InsertWayPoint(Point.ByCoordinates(50, 50, 0), 0);
         var listWayPointsOneElem = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsOneElem);
         Assert.AreEqual(1, listWayPointsOneElem.Count);
         Assert.NotNull(listWayPointsOneElem[0]);
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsOneElem[0].ToString());

         // Assert way point is inserted at index 1
         // inserting at index 0 is tested in WayPointInsertGet_ValidArgs.
         pathOfTravel.InsertWayPoint(Point.ByCoordinates(75, 75, 0), 1);
         var listWayPointsTwoElems = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsTwoElems);
         Assert.AreEqual(2, listWayPointsTwoElems.Count);
         Assert.NotNull(listWayPointsTwoElems[0]);
         Assert.NotNull(listWayPointsTwoElems[1]);
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsTwoElems[0].ToString());
         Assert.AreEqual("(75.000000000, 75.000000000, 0.000000000)", listWayPointsTwoElems[1].ToString());

         //Assert we can modify a way point
         pathOfTravel.SetWayPoint(Point.ByCoordinates(90, 90, 0), 1);
         var listWayPointsTwoElemsAfterSet = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsTwoElemsAfterSet);
         Assert.AreEqual(2, listWayPointsTwoElemsAfterSet.Count);
         Assert.NotNull(listWayPointsTwoElemsAfterSet[0]);
         Assert.NotNull(listWayPointsTwoElemsAfterSet[1]);
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsTwoElemsAfterSet[0].ToString());
         Assert.AreEqual("(90.000000000, 90.000000000, 0.000000000)", listWayPointsTwoElemsAfterSet[1].ToString());
      }

      [Test]
      [TestModel(@".\Empty.rvt")]
      public void WayPointRemoveGet_ValidArgs()
      {
         var pathOfTravelOneToOne = PathOfTravel.ByFloorPlanPoints(
            GetFloorPlan(),
            new Point[] { Point.ByCoordinates(0, 0, 0) },
            new Point[] { Point.ByCoordinates(100, 100, 0) },
            false);

         // Assert created Path of Travel is valid.
         Assert.NotNull(pathOfTravelOneToOne);
         Assert.AreEqual(1, pathOfTravelOneToOne.GetLength(0));
         Assert.NotNull(pathOfTravelOneToOne[0]);

         // Current Path of Travel.
         var pathOfTravel = pathOfTravelOneToOne[0];

         // Assert we can insert a way point at valid index
         pathOfTravel.InsertWayPoint(Point.ByCoordinates(50, 50, 0), 0);
         var listWayPointsOneElem = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsOneElem);
         Assert.AreEqual(1, listWayPointsOneElem.Count);
         Assert.NotNull(listWayPointsOneElem[0]);
         Assert.AreEqual("(50.000000000, 50.000000000, 0.000000000)", listWayPointsOneElem[0].ToString());

         // Assert way point is removed
         pathOfTravel.RemoveWayPoint(0);
         var listWayPointsEmpty = pathOfTravel.GetWayPoints();
         Assert.NotNull(listWayPointsEmpty);
         Assert.AreEqual(0, listWayPointsEmpty.Count);
      }

      FloorPlanView GetFloorPlan()
      {
         var level = Level.ByElevation(0);
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
