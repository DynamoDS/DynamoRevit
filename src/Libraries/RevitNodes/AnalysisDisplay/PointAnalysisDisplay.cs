using System;
using System.Collections.Generic;
using System.Linq;
using Analysis;
using Dynamo.Graph.Nodes;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Revit.GeometryConversion;
using RevitServices.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Point = Autodesk.DesignScript.Geometry.Point;
using View = Revit.Elements.Views.View;

namespace Revit.AnalysisDisplay
{
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
        /// <param name="data"></param>
        /// <param name="resultsName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private PointAnalysisDisplay(Autodesk.Revit.DB.View view, PointData data, string resultsName, string description, Type unitType)
        {
            SpatialFieldManager sfm;
            var primitiveIds = new List<int>();

            TransactionManager.Instance.EnsureInTransaction(Document);

            var TraceData = GetElementAndPrimitiveIdFromTrace();
            if (TraceData != null)
            {
                sfm = TraceData.Item1;
                primitiveIds = TraceData.Item2;
                foreach(var idx in primitiveIds)
                {
                    sfm.RemoveSpatialFieldPrimitive(idx);
                }
                primitiveIds.Clear();
            }
            else
            {
                sfm = GetSpatialFieldManagerFromView(view);

                sfm.SetMeasurementNames(new List<string>() { Properties.Resources.Dynamo_AVF_Data_Name });
            }
            
            InternalSetSpatialFieldManager(sfm);

            InternalSetSpatialFieldValues(data, ref primitiveIds, resultsName, description, unitType);
            InternalSetSpatialPrimitiveIds(primitiveIds);
            TransactionManager.Instance.TransactionTaskDone();
            SetElementAndPrimitiveIdsForTrace(sfm, primitiveIds);
        }

        #endregion

        #region Private mutators
        /// <summary>
        /// Set the spatial field values for the current spatial field primitive.  The two 
        /// input sequences should be of the same length.
        /// </summary>
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
        [NodeObsolete("PointDataObsolete", typeof(Properties.Resources))]
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
    }

}
