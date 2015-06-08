using System;
using System.Collections.Generic;
using System.Linq;

using Analysis;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

using Revit.GeometryConversion;

using RevitServices.Transactions;
using View = Revit.Elements.Views.View;
using RevitServices.Persistence;
using System.Runtime.Serialization;
using RevitServices.Elements;

using Point = Autodesk.DesignScript.Geometry.Point;

namespace Revit.AnalysisDisplay
{
    [SupressImportIntoVM]
    [Serializable]
    public class SpmPrimitiveIdListPair : ISerializable
    {
        public int SpatialFieldManagerID { get; set; }
        public List<int> PrimitiveIDs { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SpatialFieldManagerID", SpatialFieldManagerID, typeof(int));
            info.AddValue("PrimitiveIDCount", PrimitiveIDs.Count, typeof(int));
            foreach (var id in PrimitiveIDs)
            {
                info.AddValue("ID", id, typeof(int));
            }
        }

        public SpmPrimitiveIdListPair()
        {
            SpatialFieldManagerID = int.MinValue;
            PrimitiveIDs = new List<int>();
        }

        public SpmPrimitiveIdListPair(SerializationInfo info, StreamingContext context)
        {
            SpatialFieldManagerID = (int)info.GetValue("SpatialFieldManagerID", typeof(int));

            int count = (int)info.GetValue("PrimitiveIDCount", typeof(int));
            PrimitiveIDs = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                var id = (int)info.GetValue("ID", typeof(int));
                PrimitiveIDs.Add(id);
            }
        }
    }

    /// <summary>
    /// A Revit Point Analysis Display 
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class PointAnalysisDisplay : AbstractAnalysisDisplay
    {
        #region Private constructors
        /// <summary>
        /// Create a Point Analysis Display in the current view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="sampleLocations"></param>
        /// <param name="samples"></param>
        /// <param name="data"></param>
        /// <param name="resultsName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private PointAnalysisDisplay(Autodesk.Revit.DB.View view, PointData data, string resultsName, string description, Type unitType)
        {
            var sfm = GetSpatialFieldManagerFromView(view);

            TransactionManager.Instance.EnsureInTransaction(Document);

            sfm.Clear();

            sfm.SetMeasurementNames(new List<string>() { Properties.Resources.Dynamo_AVF_Data_Name });

            var primitiveIds = new List<int>();

            InternalSetSpatialFieldManager(sfm);

            InternalSetSpatialFieldValues(data, ref primitiveIds, resultsName, description, unitType);

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Private mutators
        /// <summary>
        /// Set the spatial field values for the current spatial field primitive.  The two 
        /// input sequences should be of the same length.
        /// </summary>
        /// <param name="pointLocations"></param>
        /// <param name="values"></param>
        /// <param name="data"></param>
        /// <param name="primitiveIds"></param>
        /// <param name="schemaName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private void InternalSetSpatialFieldValues(IStructuredData<Point, double> data, ref List<int> primitiveIds, string schemaName, string description, Type unitType)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // We chunk here because the API has a limitation for the 
            // number of points that can be sent in one run.

            var chunkSize = 1000;

            var dataLocations = data.ValueLocations.Select(l=>l.ToXyz());
            var values = data.Values.ToList();

            while (dataLocations.Any()) 
            {
                // Compute the chunks
                var pointLocationChunk = dataLocations.Take(chunkSize);
                var valuesChunk = values.Take(chunkSize).ToList();

                // Create the ValueAtPoint objects
                var valList = valuesChunk.Select(n => new ValueAtPoint(new List<double>{n}));

                // Create the field domain points and values
                var samplePts = new FieldDomainPointsByXYZ(pointLocationChunk.ToList());
                var sampleValues = new FieldValues(valList.ToList());

                // Get the analysis results schema
                var schemaIndex = GetAnalysisResultSchemaIndex(schemaName, description, unitType);

                // Update the values
                var primitiveId = SpatialFieldManager.AddSpatialFieldPrimitive();
                primitiveIds.Add(primitiveId);
                SpatialFieldManager.UpdateSpatialFieldPrimitive(primitiveId, samplePts, sampleValues, schemaIndex);

                dataLocations = dataLocations.Skip(chunkSize);
                values = values.Skip(chunkSize).ToList();
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Show a colored Point Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="sampleLocations">The locations at which you want to create analysis values.</param>
        /// <param name="samples">The analysis values at the given locations.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>An PointAnalysisDisplay object.</returns>
        public static PointAnalysisDisplay ByViewPointsAndValues(View view,
                        Autodesk.DesignScript.Geometry.Point[] sampleLocations, double[] samples, 
            string name = "", string description = "", Type unitType = null)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (sampleLocations == null)
            {
                throw new ArgumentNullException("samplePoints");
            }

            if (samples == null)
            {
                throw new ArgumentNullException("samples");
            }

            if (sampleLocations.Length != samples.Length)
            {
                throw new Exception(Properties.Resources.SamplePointsMismatchError);
            }

            if (string.IsNullOrEmpty(name))
            {
                name = Properties.Resources.AnalysisResultsDefaultName;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = Properties.Resources.AnalysisResultsDefaultDescription;
            }

            var data = PointData.ByPointsAndValues(sampleLocations, samples);
            return new PointAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        /// <summary>
        /// Show a colored Point Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="data">A list of PointData objects.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>An PointAnalysisDisplay object.</returns>
        public static PointAnalysisDisplay ByViewAndPointAnalysisData(View view,
                        PointData data,
            string name = "", string description = "", Type unitType = null)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = Properties.Resources.AnalysisResultsDefaultName;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = Properties.Resources.AnalysisResultsDefaultDescription;
            }

            return new PointAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        #endregion

        protected Tuple<SpatialFieldManager, List<int>> GetElementAndPrimitiveIdListFromTrace()
        {
            // This is a provisional implementation until we can store both items in trace
            var id = ElementBinder.GetRawDataFromTrace();
            if (id == null)
                return null;

            var idPair = id as SpmPrimitiveIdListPair;
            if (idPair == null)
                return null;

            var primitiveIds = idPair.PrimitiveIDs;
            var sfmId = idPair.SpatialFieldManagerID;

            SpatialFieldManager sfm = null;

            // if we can't get the sfm, return null
            if (!Document.TryGetElement(new ElementId(sfmId), out sfm)) return null;

            return new Tuple<SpatialFieldManager, List<int>>(sfm, primitiveIds);
        }

        protected void SetElementAndPrimitiveIdListTrace(SpatialFieldManager manager, List<int> primitiveIds)
        {
            if (manager == null)
            {
                throw new Exception();
            }

            var idPair = new SpmPrimitiveIdListPair
            {
                SpatialFieldManagerID = manager.Id.IntegerValue,
                PrimitiveIDs = primitiveIds
            };
            ElementBinder.SetRawDataForTrace(idPair);
        }
    }

}
