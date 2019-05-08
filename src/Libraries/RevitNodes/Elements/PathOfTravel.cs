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
   /// Revit Path of Travel Element
   /// </summary>
   [DynamoServices.RegisterForTrace]
   public class PathOfTravel : Element
   {
      #region Internal properties

      /// <summary>
      /// An internal handle on the Revit path of travel
      /// </summary>
      RvtAnalysis.PathOfTravel m_rvtPathOfTravel = null;

      /// <summary>
      /// Reference to the Element
      /// </summary>
      [SupressImportIntoVM]
      public override Rvt.Element InternalElement
      {
         get { return m_rvtPathOfTravel; }
      }

      #endregion

      #region Private constructors

      /// <summary>
      /// PathOfTravel from existing
      /// </summary>
      /// <param name="rvtPathOfTravel">Revit PathOfTavel element</param>
      private PathOfTravel(RvtAnalysis.PathOfTravel rvtPathOfTravel)
      {
         SafeInit(() => InitPathOfTravel(rvtPathOfTravel));
      }

      #endregion

      #region Public constructors

      /// <summary>
      /// Construct a list of Path of Travel elements in a floor plan view between the specified start points and end points
      /// </summary>
      /// <param name="floorPlan">Floor plan view to place paths of travel on</param>
      /// <param name="startPtList">List of start points</param>
      /// <param name="endPtList">List of end points</param>
      /// <param name="manyToMany">If true, paths are created from every point in the start point list to all points in the end point list. If false, a path is created from every point in the start point list to a corresponding point in the end point list with the same index. The two lists must have the same size when not creating many-to-many paths.</param>
      /// <returns>List of Path of Travel elements; can contain null elements if there is no path between some points.</returns>
      [NodeCategory("Create")]
      [AllowRankReduction]
      public static PathOfTravel[] ByFloorPlanPoints(Revit.Elements.Views.FloorPlanView floorPlan, Autodesk.DesignScript.Geometry.Point[] startPtList, Autodesk.DesignScript.Geometry.Point[] endPtList, bool manyToMany)
      {
         if (floorPlan == null)
            throw new ArgumentNullException("Invalid view");

         if (startPtList == null)
            throw new ArgumentNullException("Invalid start points list");

         if (endPtList == null)
            throw new ArgumentNullException("Invalid end point list");

         if (!startPtList.Any())
            throw new ArgumentNullException("Start points list is empty");

         if (!endPtList.Any())
            throw new ArgumentNullException("End point list is empty");

         if (startPtList.Any(x => x == null))
            throw new ArgumentException("Start point list contains null elements");

         if (endPtList.Any(x => x == null))
            throw new ArgumentException("End point list contains null elements");

         if (!manyToMany && (startPtList.Count() != endPtList.Count()))
            throw new ArgumentException("Size of start point array doesn't match size of end point array");

         if (manyToMany)
         {
            // although there is the many-to-many version of PathOfTravel creation API (the "mapped" version), the fact that some elements
            // need to be reused  between the node runs makes it difficult to use (see UpdateReusedElements, DeleteExtraElements and CreateAdditionalElements)
            // so it's easier to flatten the lists manually and always use the same ("non-mapped") version of the API
            List<Autodesk.DesignScript.Geometry.Point> newStartPointList = new List<Autodesk.DesignScript.Geometry.Point>();
            List<Autodesk.DesignScript.Geometry.Point> newEndPointList = new List<Autodesk.DesignScript.Geometry.Point>();

            foreach(var endPt in endPtList)
            {
               foreach(var startPt in startPtList)
               {
                  newStartPointList.Add(startPt);
                  newEndPointList.Add(endPt);
               }
            }

            startPtList = newStartPointList.ToArray();
            endPtList = newEndPointList.ToArray();
         }

         return InternalByViewEndPoints(
            (Rvt.View)floorPlan.InternalElement,
            startPtList.Select(x => x.ToXyz()),
            endPtList.Select(x => x.ToXyz()));
      }

      #endregion

      #region Public methods    
      
      /// <summary>
      /// Updates existing PathOfTravel
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
      /// Construct a new Revit PathOfTravel in a floor plan view between the specified start point and end point
      /// </summary>
      /// <param name="rvtView"></param>
      /// <param name="startPoints"></param>
      /// <param name="endPoints"></param>
      /// <returns></returns>
      private static PathOfTravel[] InternalByViewEndPoints(Rvt.View rvtView, IEnumerable<XYZ> startPoints, IEnumerable<XYZ> endPoints)
      {
         List<PathOfTravel> pathsOfTravel = new List<PathOfTravel>();

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
      /// Initialize a Path of Travel element from existing Revit element
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
