using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

using Rvt = Autodesk.Revit.DB;
using RvtAnalysis = Autodesk.Revit.DB.Analysis;

namespace Revit.Elements
{
   /// <summary>
   /// PathOfTravel Element.
   /// </summary>
   [DynamoServices.RegisterForTrace]
   public class PathOfTravel : Element
   {
      #region Internal properties

      /// <summary>
      /// An internal handle on the Revit PathOfTravel element.
      /// </summary>
      RvtAnalysis.PathOfTravel m_rvtPathOfTravel = null;

      /// <summary>
      /// Reference to Revit PathOfTravel Element.
      /// </summary>
      [SupressImportIntoVM]
      public override Rvt.Element InternalElement
      {
         get { return m_rvtPathOfTravel; }
      }

      /// <summary>
      /// Set Internal Element from a exsiting element.
      /// </summary>
      /// <param name="element"></param>
      internal override void SetInternalElement(Autodesk.Revit.DB.Element element)
      {
         InitPathOfTravel(element as RvtAnalysis.PathOfTravel);
      }

      #endregion

      #region Private constructors

      /// <summary>
      /// PathOfTravel from existing
      /// </summary>
      /// <param name="rvtPathOfTravel">Revit PathOfTavel element</param>
      private PathOfTravel(RvtAnalysis.PathOfTravel rvtPathOfTravel)
      {
         SafeInit(() => InitPathOfTravel(rvtPathOfTravel), true);
      }

      #endregion

      #region Public constructors

      /// <summary>
      /// Calculates the longest PathOfTravel(s) of all shortest paths from rooms in the floor plan to the specified exit points.
      /// </summary>
      /// <param name="floorPlan">Floor plan view for which rooms will be used to retrieve longest paths to the specified exit points.</param>
      /// <param name="endPtsList">List of end (exit) points.</param>
      /// <returns>List of PathOfTravel elements corresponding to the longest of shortest exit paths from rooms.</returns>
      [NodeCategory("Create")]
      [AllowRankReduction]
      public static PathOfTravel[] LongestOfShortestExitPaths(Revit.Elements.Views.FloorPlanView floorPlan, Autodesk.DesignScript.Geometry.Point[] endPtsList)
      {
         if (floorPlan == null)
            throw new ArgumentNullException("floorPlan", Properties.Resources.InvalidView);

         if (endPtsList == null)
            throw new ArgumentNullException("endPtsList", Properties.Resources.InvalidEndPointList);

         if (!endPtsList.Any())
            throw new ArgumentException(Properties.Resources.EndPointListEmpty, "endPtsList");

         if (endPtsList.Any(x => x == null))
            throw new ArgumentException(Properties.Resources.EndPointListHasNulls, "endPtsList");

         // Check that floor has some rooms
         Rvt.Document doc = DocumentManager.Instance.CurrentDBDocument;

         if(doc == null)
         {
            throw new ArgumentException(Properties.Resources.RoomsForLongestPathNotFound);
         }

         FilteredElementCollector collector = new Rvt.FilteredElementCollector(doc).OfCategory(Rvt.BuiltInCategory.OST_Rooms);

         if(collector.ToElements().Count() == 0)
         {
            throw new ArgumentException(Properties.Resources.RoomsForLongestPathNotFound);
         }

         return InternalLongestOfShortestExitPaths(
            (Rvt.View)floorPlan.InternalElement,
            endPtsList.Select(x => x.ToXyz()));
      }

      /// <summary>
      /// Constructs a list of PathOfTravel elements in a floor plan view between the specified start points and end points.
      /// </summary>
      /// <param name="floorPlan">Floor plan view to place paths of travel on</param>
      /// <param name="startPtsList">List of start points</param>
      /// <param name="endPtsList">List of end points</param>
      /// <param name="manyToMany">If true, paths are created from every point in the start point list to all points in the end point list. If false, a path is created from every point in the start point list to a corresponding point in the end point list with the same index. The two lists must have the same size when not creating many-to-many paths.</param>
      /// <returns>List of PathOfTravel elements; can contain null elements if there is no path between some points.</returns>
      [NodeCategory("Create")]
      [AllowRankReduction]
      public static PathOfTravel[] ByFloorPlanPoints(Revit.Elements.Views.FloorPlanView floorPlan, Autodesk.DesignScript.Geometry.Point[] startPtsList, Autodesk.DesignScript.Geometry.Point[] endPtsList, bool manyToMany)
      {
         if (floorPlan == null)
            throw new ArgumentNullException(Properties.Resources.InvalidView);

         if (startPtsList == null)
            throw new ArgumentNullException(Properties.Resources.InvalidStartPointList);

         if (endPtsList == null)
            throw new ArgumentNullException(Properties.Resources.InvalidEndPointList);

         if (!startPtsList.Any())
            throw new ArgumentNullException(Properties.Resources.StartPointListEmpty);

         if (!endPtsList.Any())
            throw new ArgumentNullException(Properties.Resources.EndPointListEmpty);

         if (startPtsList.Any(x => x == null))
            throw new ArgumentException(Properties.Resources.StartPointListHasNulls);

         if (endPtsList.Any(x => x == null))
            throw new ArgumentException(Properties.Resources.EndPointListHasNulls);

         if (!manyToMany && (startPtsList.Count() != endPtsList.Count()))
            throw new ArgumentException(Properties.Resources.StartEndListSizeMismatch);

         if (manyToMany)
         {
            // although there is the many-to-many version of PathOfTravel creation API (the "mapped" version), the fact that some elements
            // need to be reused  between the node runs makes it difficult to use (see UpdateReusedElements, DeleteExtraElements and CreateAdditionalElements)
            // so it's easier to flatten the lists manually and always use the same ("non-mapped") version of the API
            List<Autodesk.DesignScript.Geometry.Point> newStartPointList = new List<Autodesk.DesignScript.Geometry.Point>();
            List<Autodesk.DesignScript.Geometry.Point> newEndPointList = new List<Autodesk.DesignScript.Geometry.Point>();

            foreach(var endPt in endPtsList)
            {
               foreach(var startPt in startPtsList)
               {
                  newStartPointList.Add(startPt);
                  newEndPointList.Add(endPt);
               }
            }

            startPtsList = newStartPointList.ToArray();
            endPtsList = newEndPointList.ToArray();
         }

         return InternalByViewEndPoints(
            (Rvt.View)floorPlan.InternalElement,
            startPtsList.Select(x => x.ToXyz()),
            endPtsList.Select(x => x.ToXyz()));
      }

      #endregion

      #region Public methods

      /// <summary>
      /// Returns the WayPoints set from PathOfTravel element.
      /// </summary>
      /// <returns>List of WayPoints for the given PathOfTravel element.</returns>
      [NodeCategory("Query")]
      [AllowRankReduction]
      public IList<XYZ> GetWayPoints()
      {
         if (m_rvtPathOfTravel is null)
            throw new ArgumentException(Properties.Resources.InvalidPathOfTravel);

         return m_rvtPathOfTravel.GetWaypoints();
      }

      /// <summary>
      /// Removes WayPoint at the specified index from PathOfTravel element.
      /// </summary>
      /// <param name="index">Index of the WayPoint to be removed from the PathOfTravel element.</param>
      /// <returns>The PathOfTravel element after the WayPoint was rmnoved.</returns>
      [NodeCategory("Action")]
      [AllowRankReduction]
      public PathOfTravel RemoveWayPoint(int index)
      {
         if (m_rvtPathOfTravel is null)
            throw new ArgumentException(Properties.Resources.InvalidPathOfTravel);

         TransactionManager.Instance.EnsureInTransaction(Document);

         m_rvtPathOfTravel.RemoveWaypoint(index);

         TransactionManager.Instance.TransactionTaskDone();

         return this;
      }

      /// <summary>
      /// Inserts a WayPoint to PathOfTravel element at the specified index.
      /// </summary>
      /// <param name="wayPoint">The waypoint to insert.</param>
      /// <param name="index">The index to insert the waypoint at.</param>
      /// <returns>The PathOfTravel element after the WayPoint was inserted.</returns>
      [NodeCategory("Action")]
      [AllowRankReduction]
      public PathOfTravel InsertWayPoint(Autodesk.DesignScript.Geometry.Point wayPoint, int index)
      {
         if (wayPoint is null)
            throw new ArgumentNullException("wayPoint", Properties.Resources.PointRequired); //Please supply a point geometry.

         if (m_rvtPathOfTravel is null)
            throw new ArgumentException(Properties.Resources.InvalidPathOfTravel);

         TransactionManager.Instance.EnsureInTransaction(Document);

         m_rvtPathOfTravel.InsertWaypoint(wayPoint.ToXyz(), index);

         TransactionManager.Instance.TransactionTaskDone();

         return this;
      }

      /// <summary>
      /// Updates WayPoint at the specified index to the new specified position.
      /// </summary>
      /// <param name="newPosition">The position to which WayPoint will be set.</param>
      /// <param name="index">The index of WayPoint to update.</param>
      /// <returns>The PathOfTravel element after the WayPoint was set.</returns>
      [NodeCategory("Action")]
      [AllowRankReduction]
      public PathOfTravel SetWayPoint(Autodesk.DesignScript.Geometry.Point newPosition, int index)
      {
         if (newPosition is null)
            throw new ArgumentNullException("newPosition", Properties.Resources.PointRequired); //Please supply a point geometry.

         if (m_rvtPathOfTravel is null)
            throw new ArgumentException(Properties.Resources.InvalidPathOfTravel);

         TransactionManager.Instance.EnsureInTransaction(Document);

         m_rvtPathOfTravel.SetWaypoint(newPosition.ToXyz(), index);

         TransactionManager.Instance.TransactionTaskDone();

         return this;
      }

      /// <summary>
      /// Updates existing PathOfTravel.
      /// </summary>
      public static PathOfTravel[] Update(PathOfTravel[] elements)
      {
         List<PathOfTravel> updatedElements = new List<PathOfTravel>();

         TransactionManager.Instance.EnsureInTransaction(Document);

         IList<RvtAnalysis.PathOfTravelCalculationStatus> statuses;
         RvtAnalysis.PathOfTravel.UpdateMultiple(Document, elements.Select(x => x.InternalElementId).ToList(), out statuses);
         if(statuses != null)
         {
            int ii = 0;
            foreach(var status in statuses)
            {
               if (status == RvtAnalysis.PathOfTravelCalculationStatus.Success)
                  updatedElements.Add(elements[ii]);
               else
                  Document.Delete(elements[ii].InternalElementId);

               ++ii;
            }
         }

         TransactionManager.Instance.TransactionTaskDone();

         return updatedElements.ToArray();
      }

      #endregion

      #region Private helpers

      /// <summary>
      /// Create from existing
      /// </summary>
      /// <param name="rvtPathOfTravel">Existing Revit PathOfTravel element</param>
      /// <param name="isRevitOwned"></param>
      /// <returns></returns>
      internal static PathOfTravel FromExisting(RvtAnalysis.PathOfTravel rvtPathOfTravel, bool isRevitOwned)
      {
         return new PathOfTravel(rvtPathOfTravel)
         {
            IsRevitOwned = isRevitOwned
         };
      }

      /// <summary>
      /// [INTERNAL]: Calculates the longest PathOfTravel of all shortest paths from rooms in the floor plan to given exit points.
      /// </summary>
      /// <param name="rvtView">Floor plan view for which rooms will be used to retrieve longest paths to the given exit points.</param>
      /// <param name="endPoints">List of end (exit) points.</param>
      /// <returns>List of PathOfTravel elements corresponding to the longest of shortest exit paths from rooms.</returnsO
      /// 
      private static PathOfTravel[] InternalLongestOfShortestExitPaths(Rvt.View rvtView, IEnumerable<XYZ> endPoints)
      {
         List<PathOfTravel> pathsOfTravel = new List<PathOfTravel>();

         TransactionManager.Instance.EnsureInTransaction(Document);

         try
         {
            IList<XYZ> startsOfLongestPathsFromRooms = RvtAnalysis.PathOfTravel.FindStartsOfLongestPathsFromRooms(
               rvtView,
               endPoints.ToList());

            if (startsOfLongestPathsFromRooms.Count() != 0)
            {
               IList<XYZ> endsOfShortestPaths = RvtAnalysis.PathOfTravel.FindEndsOfShortestPaths(
                  rvtView,
                  endPoints.ToList(),
                  startsOfLongestPathsFromRooms);

               IList<RvtAnalysis.PathOfTravel> newRvtPathOfTravels = RvtAnalysis.PathOfTravel.CreateMultiple(
                  rvtView,
                  startsOfLongestPathsFromRooms.ToList(),
                  endsOfShortestPaths.ToList());

               foreach (RvtAnalysis.PathOfTravel rvtPathOfTravel in newRvtPathOfTravels)
               {
                  if (rvtPathOfTravel != null)
                  {
                     pathsOfTravel.Add(new PathOfTravel(rvtPathOfTravel));
                  }
               }

               ElementBinder.SetElementsForTrace(pathsOfTravel.Where(x => x != null).Select(x => x.InternalElement));
            } 
         }
         catch (Exception e)
         {
            //unregister the elements from the element life cycle manager and delete the elements
            var elementManager = ElementIDLifecycleManager<int>.GetInstance();
            if (pathsOfTravel != null)
            {
               foreach (var path in pathsOfTravel)
               {
                  if (path != null)
                  {
                     elementManager.UnRegisterAssociation(path.InternalElementId.IntegerValue, path);
                     Document.Delete(path.InternalElementId);
                  }
               }
            }

            throw e;
         }
         finally
         {
            TransactionManager.Instance.TransactionTaskDone();
         }

         return pathsOfTravel.ToArray();
      }

      /// <summary>
      /// Construct a new Revit PathOfTravel in a floor plan view between the specified start point and end point
      /// </summary>
      /// <param name="rvtView"></param>
      /// <param name="startPoints"></param>
      /// <param name="endPoints"></param>
      /// <returns></returns>
      private static PathOfTravel[] InternalByViewEndPoints(Rvt.View rvtView, IEnumerable<XYZ> startPoints, IEnumerable<XYZ> endPoints)
      {
         List<PathOfTravel> pathsOfTravel = new List<PathOfTravel>();

         if(TransactionManager.Instance.DisableTransactions)
         {
            IEnumerable<RvtAnalysis.PathOfTravel> persistRvtElements = ElementBinder.GetElementsFromTrace<RvtAnalysis.PathOfTravel>(Document);
            if(persistRvtElements != null)
            {
               foreach (var ele in persistRvtElements)
               {
                  var persisEle = new PathOfTravel(ele);
                  pathsOfTravel.Add(persisEle);
               }

               return pathsOfTravel.ToArray();
            }   
         }

         TransactionManager.Instance.EnsureInTransaction(Document);

         try
         {
            // get previously created elements
            IEnumerable<RvtAnalysis.PathOfTravel> existingRvtElements = ElementBinder.GetElementsFromTrace<RvtAnalysis.PathOfTravel>(Document);

            // update elements that can be reused
            IList<PathOfTravel> updatedPathsOfTravel = UpdateReusedElements(existingRvtElements, startPoints, endPoints);
            pathsOfTravel.AddRange(updatedPathsOfTravel);

            // delete elements that are not needed anymore
            DeleteExtraElements(existingRvtElements, startPoints, endPoints);

            // create additional elements
            IList<PathOfTravel> newPathsOfTravel = CreateAdditionalElements(rvtView, existingRvtElements, startPoints, endPoints);
            pathsOfTravel.AddRange(newPathsOfTravel);

            ElementBinder.SetElementsForTrace(pathsOfTravel.Where(x => x != null).Select(x => x.InternalElement));
         }
         catch (Exception e)
         {
            // unregister the elements from the element life cycle manager and delete the elements
            var elementManager = ElementIDLifecycleManager<int>.GetInstance();
            if (pathsOfTravel != null)
            {
               foreach (var path in pathsOfTravel)
               {
                  if (path != null)
                  {
                     elementManager.UnRegisterAssociation(path.InternalElementId.IntegerValue, path);
                     Document.Delete(path.InternalElementId);
                  }
               }
            }

            throw e;
         }
         finally
         {
            TransactionManager.Instance.TransactionTaskDone();
         }

         return pathsOfTravel.ToArray();
      }

      static List<PathOfTravel> UpdateReusedElements(IEnumerable<RvtAnalysis.PathOfTravel> existingRvtElements, IEnumerable<XYZ> newStartPoints, IEnumerable<XYZ> newEndPoints)
      {         
         List<PathOfTravel> updatedPathsOfTravel = new List<PathOfTravel>();

         if ((existingRvtElements == null) || (existingRvtElements.Count() == 0))
         {
            // no elements to update
            return updatedPathsOfTravel;
         }

         int existingCount = existingRvtElements.Count();
         int requestedCount = newStartPoints.Count();
         int elementsToReuse = Math.Min(existingCount, requestedCount);

         // update elements that can be reused (set new start and endpoint)
         if (elementsToReuse > 0)
         {
            for (int ii = 0; ii < elementsToReuse; ++ii)
            {
               existingRvtElements.ElementAt(ii).PathStart = newStartPoints.ElementAt(ii);
               existingRvtElements.ElementAt(ii).PathEnd = newEndPoints.ElementAt(ii);
            }

            IList<RvtAnalysis.PathOfTravelCalculationStatus> statuses = null;
            RvtAnalysis.PathOfTravel.UpdateMultiple(
               Document,
               existingRvtElements.Take(elementsToReuse).Select(x => x.Id).ToList(),
               out statuses);

            for (int ii = 0; ii < elementsToReuse; ++ii)
            {
               if (statuses[ii] == RvtAnalysis.PathOfTravelCalculationStatus.Success)
               {
                  updatedPathsOfTravel.Add(new PathOfTravel(existingRvtElements.ElementAt(ii)));
               }
               else
               {
                  Document.Delete(existingRvtElements.ElementAt(ii).Id);
                  updatedPathsOfTravel.Add(null);
               }
            }
         }

         return updatedPathsOfTravel;
      }

      static void DeleteExtraElements(IEnumerable<RvtAnalysis.PathOfTravel> existingRvtElements, IEnumerable<XYZ> newStartPoints, IEnumerable<XYZ> newEndPoints)
      {
         if ((existingRvtElements == null) || (existingRvtElements.Count() == 0))
         {
            // no elements to delete
            return;
         }

         int existingCount = existingRvtElements.Count();
         int requestedCount = newStartPoints.Count();
         int elementsToDelete = Math.Max(existingCount - requestedCount, 0);

         // delete elements that are not needed anymore (if the currently requested
         // number of paths is smaller than previously created elements)
         if (elementsToDelete > 0)
         {
            for (int ii = 0; ii < elementsToDelete; ++ii)
            {
               Document.Delete(existingRvtElements.ElementAt(existingCount - 1 - ii).Id);
            }
         }
      }
   
      static List<PathOfTravel> CreateAdditionalElements(Rvt.View rvtView, IEnumerable<RvtAnalysis.PathOfTravel> existingRvtElements, IEnumerable<XYZ> newStartPoints, IEnumerable<XYZ> newEndPoints)
      {
         List<PathOfTravel> newPathsOfTravel = new List<PathOfTravel>();

         int existingCount = existingRvtElements == null ? 0 : existingRvtElements.Count();
         int requestedCount = newStartPoints.Count();
         int elementsToCreate = Math.Max(requestedCount - existingCount, 0);

         // create additional elements (if the currently requested number of paths
         // is greater than number of previously created elements)
         if (elementsToCreate > 0)
         {
            IList<RvtAnalysis.PathOfTravel> newRvtElements = RvtAnalysis.PathOfTravel.CreateMultiple(
               rvtView,
               newStartPoints.Skip(existingCount).ToList(),
               newEndPoints.Skip(existingCount).ToList());

            foreach (RvtAnalysis.PathOfTravel rvtElement in newRvtElements)
            {
               if (rvtElement != null)
               {
                  newPathsOfTravel.Add(new PathOfTravel(rvtElement));
               }
               else
               {
                  newPathsOfTravel.Add(null);
               }
            }
         }

         return newPathsOfTravel;
      }

      /// <summary>
      /// Initialize a PathOfTravel element from existing Revit element.
      /// </summary>
      private void InitPathOfTravel(RvtAnalysis.PathOfTravel rvtPathOfTravel)
      {
         m_rvtPathOfTravel = rvtPathOfTravel;
      
         if (m_rvtPathOfTravel != null)
         {
            this.InternalElementId = m_rvtPathOfTravel.Id;
            this.InternalUniqueId = m_rvtPathOfTravel.UniqueId;
         }
      }


      #endregion
   }
}
