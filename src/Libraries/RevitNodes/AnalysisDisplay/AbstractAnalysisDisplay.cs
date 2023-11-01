using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using DynamoUnits;
using Newtonsoft.Json;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.AnalysisDisplay
{
    /// <summary>
    /// Hold a pair of element ID of SpatialFieldManager and primitive ID to
    /// support serialization.
    /// </summary>
    [SupressImportIntoVM]
    public class SpmPrimitiveIdPair
    {
        public long SpatialFieldManagerID { get; set; }
        public List<int> PrimitiveIds { get; set; }

        public SpmPrimitiveIdPair(long spatialFieldManagerID,  List<int> primitiveIds)
        {
            SpatialFieldManagerID = spatialFieldManagerID;
            PrimitiveIds = primitiveIds;
        }

        public SpmPrimitiveIdPair()
        {
            SpatialFieldManagerID = long.MinValue;
            PrimitiveIds = new List<int>();
        }
    }

    [SupressImportIntoVM]
    public class SpmRefPrimitiveIdListPair
    {
        public long SpatialFieldManagerID { get; set; }
        public Dictionary<Reference, int> RefIdPairs { get; set; }

        public SpmRefPrimitiveIdListPair(long spatialFieldManagerID, Dictionary<Reference, int> refIdPairs)
        {
            SpatialFieldManagerID = spatialFieldManagerID;
            RefIdPairs = refIdPairs;
        }

        public SpmRefPrimitiveIdListPair()
        {
            SpatialFieldManagerID = long.MinValue;
            RefIdPairs = new Dictionary<Reference, int>();
        }
    }

    /// <summary>
    /// Superclass for all Revit Analysis Display types
    /// 
    /// Note: We're using the user facing name from Revit (Analysis Display), rather than the same name that the Revit API
    /// uses (Spatial Field)
    /// </summary>
//    [IsVisibleInDynamoLibrary(false)]    - removing as per MAGN-3382
    [SupressImportIntoVM]
    public abstract class AbstractAnalysisDisplay : IDisposable
    {
        #region Static properties

        /// <summary>
        /// A reference to the current document
        /// </summary>
        protected static Autodesk.Revit.DB.Document Document
        {
            get
            {
                return DocumentManager.Instance.CurrentDBDocument;
            }
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// The SpatialFieldManager governing this SpatialFieldPrimitive
        /// </summary>
        protected Autodesk.Revit.DB.Analysis.SpatialFieldManager SpatialFieldManager
        {
            get;
            set;
        }

        protected List<int> PrimitiveIds { get; set; }

        #endregion

        #region Protected mutators

        /// <summary>
        /// Set the SpatialFieldManager
        /// </summary>
        /// <param name="manager"></param>
        protected void InternalSetSpatialFieldManager(SpatialFieldManager manager)
        {
            SpatialFieldManager = manager;
        }

        protected void InternalSetSpatialPrimitiveIds(List<int> primitiveIds)
        {
            PrimitiveIds = primitiveIds;
        }

        #endregion

        #region Static helper methods

        /// <summary>
        /// Get the AnalysisResultsSchemaIndex for the SpatialFieldManager
        /// </summary>
        /// <returns></returns>
        protected virtual int GetAnalysisResultSchemaIndex(string resultsSchemaName, string resultsDescription, Type unitType)
        {
            // Get the AnalysisResultSchema index - there is only one for Dynamo
            var schemaIndex = 0;
            if (!SpatialFieldManager.IsResultSchemaNameUnique(resultsSchemaName, -1))
            {
                var arses = SpatialFieldManager.GetRegisteredResults();
                schemaIndex =
                    arses.First(
                        x => SpatialFieldManager.GetResultSchema(x).Name == resultsSchemaName);
            }
            else
            {
                var ars = new AnalysisResultSchema(resultsSchemaName, resultsDescription);
                
                if (unitType != null)
                {
#pragma warning disable CS0618
               if (typeof(SIUnit).IsAssignableFrom(unitType))
                    {
                        var prop = unitType.GetProperty("Conversions");
                        var conversions = (Dictionary<string, double>)prop.GetValue(null, new object[] { });
                        if (conversions != null)
                        {
                            var unitNames = conversions.Keys.ToList();
                            var multipliers = conversions.Values.ToList();
                            ars.SetUnits(unitNames, multipliers);
                            ars.CurrentUnits = 0;
                        }
                    }
#pragma warning restore CS6018
            }

            schemaIndex = SpatialFieldManager.RegisterResult(ars);
            }

            return schemaIndex;
        }

        /// <summary>
        /// Get the SpatialFieldManager for a particular view.  This is a singleton for every view.  Note that the 
        /// number of values per analysis point is ignored if the SpatialFieldManager has already been obtained
        /// for this view.  This field cannot be mutated once the SpatialFieldManager is set for a partiular 
        /// view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="numValuesPerAnalysisPoint"></param>
        /// <returns></returns>
        protected static SpatialFieldManager GetSpatialFieldManagerFromView(Autodesk.Revit.DB.View view, uint numValuesPerAnalysisPoint = 1)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            TransactionManager.Instance.EnsureInTransaction(Document);

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view);

            if (sfm == null)
            {
                sfm = SpatialFieldManager.CreateSpatialFieldManager(view, (int)numValuesPerAnalysisPoint);
            }
            else
            {
                // If the number of values stored
                // at each location is not equal to what we are requesting,
                // then create a new SFM
                if (sfm.NumberOfMeasurements != numValuesPerAnalysisPoint)
                {
                    DocumentManager.Instance.CurrentDBDocument.Delete(sfm.Id);
                    sfm = SpatialFieldManager.CreateSpatialFieldManager(view, (int)numValuesPerAnalysisPoint);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return sfm;
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Destroy
        /// </summary>
        void IDisposable.Dispose()
        {
            if (SpatialFieldManager != null)
            {
                // no need to delete SpatialFieldManager, neither primitive id.
            }
        }

        #endregion

        #region Trace management

        /// <summary>
        /// Set the SpatialFieldManager PrimitiveId from Thread Local Storage
        /// </summary>
        /// <returns></returns>
        protected Tuple<SpatialFieldManager, List<int>> GetElementAndPrimitiveIdFromTrace()
        {
            // This is a provisional implementation until we can store both items in trace
            var traceString = ElementBinder.GetRawDataFromTrace();

            if (String.IsNullOrEmpty(traceString))
                return null;

            SpmPrimitiveIdPair idPair = null;
            try
            {
                idPair = JsonConvert.DeserializeObject<SpmPrimitiveIdPair>(traceString);
            }
            catch
            {
                //do nothing 
            }

            if (idPair == null)
                return null;

            var primitiveIds = idPair.PrimitiveIds;
            var sfmId = idPair.SpatialFieldManagerID;

            SpatialFieldManager sfm = null;

            // if we can't get the sfm, return null
            if (!Document.TryGetElement(new ElementId(sfmId), out sfm)) 
                return null;

            return new Tuple<SpatialFieldManager, List<int>>(sfm, primitiveIds);
        }

        /// <summary>
        /// Set the SpatialFieldManager and PrimitiveId in Thread Local Storage
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="primitiveIds"></param>
        protected void SetElementAndPrimitiveIdsForTrace(SpatialFieldManager manager, List<int> primitiveIds)
        {
            if (manager == null)
            {
                throw new Exception();
            }

            var idPair = new SpmPrimitiveIdPair
            {
                SpatialFieldManagerID = manager.Id.Value,
                PrimitiveIds = primitiveIds
            };

            var serializedTraceData = JsonConvert.SerializeObject(idPair);
            ElementBinder.SetRawDataForTrace(serializedTraceData);
        }

        /// <summary>
        /// Set the current SpatialFieldManager and PrimitiveId in Thread Local Storage
        /// </summary>
        protected void SetElementAndPrimitiveIdsForTrace()
        {
            SetElementAndPrimitiveIdsForTrace(SpatialFieldManager, PrimitiveIds);
        }

        #endregion
    }
}