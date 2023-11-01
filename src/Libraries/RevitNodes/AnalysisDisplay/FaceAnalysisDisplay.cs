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
using Surface = Autodesk.DesignScript.Geometry.Surface;
using UV = Autodesk.Revit.DB.UV;
using View = Revit.Elements.Views.View;
using Newtonsoft.Json;

namespace Revit.AnalysisDisplay
{
    /// <summary>
    /// A Revit Point Analysis Display 
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FaceAnalysisDisplay : AbstractAnalysisDisplay
    {
        internal const string DefaultTag = "RevitFaceReference";

        #region Protected Property
        protected Dictionary<Reference, int> RefIdPairs { get; set; }

        protected void InternalSetReferencePrimitiveIdPairs(Dictionary<Reference, int> keyValuePairs)
        {
            RefIdPairs = keyValuePairs;
        }
        #endregion

        #region Private constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="data"></param>
        /// <param name="resultsName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        protected FaceAnalysisDisplay(
            Autodesk.Revit.DB.View view, ISurfaceData<Autodesk.DesignScript.Geometry.UV, double> data, string resultsName, string description,Type unitType)
        {

            SpatialFieldManager sfm;

            // create a new spatial field primitive
            TransactionManager.Instance.EnsureInTransaction(Document);

            var refPriIds = new Dictionary<Reference, int>();
            var primitiveIds = new List<int>();

            var reference = data.Surface.Tags.LookupTag(DefaultTag) as Reference;
            if (reference == null)
            {
                // Dont' throw an exception here. Handle the case of a bad tag
                // in the static constructor.
                return;
            }

            var TraceData = GetElementAndRefPrimitiveIdFromTrace();
            if (TraceData != null)
            {
                sfm = TraceData.Item1;
                refPriIds = TraceData.Item2;

                InternalSetSpatialFieldManager(sfm);
                int primitiveId;
                if (refPriIds.TryGetValue(reference, out primitiveId))
                {
                    InternalSetSpatialFieldValues(primitiveId, data, resultsName, description, unitType);

                    TransactionManager.Instance.TransactionTaskDone();

                    return;
                }
                else
                {
                    foreach(var refPriId in refPriIds)
                    {
                        sfm.RemoveSpatialFieldPrimitive(refPriId.Value);
                    }
                    refPriIds.Clear();

                    primitiveId = SpatialFieldManager.AddSpatialFieldPrimitive(reference);
                    primitiveIds.Add(primitiveId);
                    refPriIds.Add(reference, primitiveId);
                    InternalSetSpatialPrimitiveIds(primitiveIds);
                    InternalSetReferencePrimitiveIdPairs(refPriIds);

                    InternalSetSpatialFieldValues(primitiveId, data, resultsName, description, unitType);
                }
            }
            else
            {
                sfm = GetSpatialFieldManagerFromView(view);

                sfm.SetMeasurementNames(new List<string>() { Properties.Resources.Dynamo_AVF_Data_Name });

                InternalSetSpatialFieldManager(sfm);

                var primitiveId = SpatialFieldManager.AddSpatialFieldPrimitive(reference);
                primitiveIds.Add(primitiveId);
                refPriIds.Add(reference, primitiveId);
                InternalSetSpatialPrimitiveIds(primitiveIds);
                InternalSetReferencePrimitiveIdPairs(refPriIds);

                InternalSetSpatialFieldValues(primitiveId, data, resultsName, description, unitType);
            }

            TransactionManager.Instance.TransactionTaskDone();
            SetElementAndRefPrimitiveIdsForTrace();
        }

        #endregion

        #region Private mutators
        /// <summary>
        /// Set the spatial field values for the current spatial field primitive. The two 
        /// input sequences should be of the same length.
        /// </summary>
        /// <param name="primitiveId"></param>
        /// <param name="data"></param>
        /// <param name="schemaName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private void 
            InternalSetSpatialFieldValues(int primitiveId, ISurfaceData<Autodesk.DesignScript.Geometry.UV, 
            double> data, string schemaName, string description, Type unitType)
        {
            // Get the surface reference
            var reference = data.Surface.Tags.LookupTag(DefaultTag) as Reference;

            var el = DocumentManager.Instance.CurrentDBDocument.GetElement(reference.ElementId);
            var pointLocations = new List<UV>();
            if (el != null)
            {
                var face = el.GetGeometryObjectFromReference(reference) as Autodesk.Revit.DB.Face;
                if (face != null)
                {
                    foreach (var loc in data.ValueLocations)
                    {
                        var pt = data.Surface.PointAtParameter(loc.U, loc.V);
                        var faceLoc = face.Project(pt.ToXyz()).UVPoint;
                        pointLocations.Add(faceLoc);
                    }
                }
            }

            var valList = data.Values.Select(v => new ValueAtPoint(new List<double>() { v }));

            TransactionManager.Instance.EnsureInTransaction(Document);

            // Convert the analysis values to a special Revit type
            //var valList = enumerable.Select(n => new ValueAtPoint(n.ToList())).ToList();
            var sampleValues = new FieldValues(valList.ToList());

            // Convert the sample points to a special Revit Type
            var samplePts = new FieldDomainPointsByUV(pointLocations.ToList());

            // Get the analysis results schema
            var schemaIndex = GetAnalysisResultSchemaIndex(schemaName, description, unitType);

            // Update the values
            SpatialFieldManager.UpdateSpatialFieldPrimitive(primitiveId, samplePts, sampleValues, schemaIndex);

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Show a colored Face Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="surface">The surface which you want to show color.</param>
        /// <param name="sampleLocations">The locations at which you want to create analysis values.</param>
        /// <param name="samples">The analysis values at the given locations.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>A FaceAnalysisDisplay object.</returns>
        public static FaceAnalysisDisplay ByViewFacePointsAndValues(
            View view, Surface surface,
            Autodesk.DesignScript.Geometry.UV[] sampleLocations, double[] samples, string name = "", string description = "", Type unitType = null)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (surface == null)
            {
                throw new ArgumentNullException("surface");
            }

            if (sampleLocations == null)
            {
                throw new ArgumentNullException("sampleUvPoints");
            }

            if (samples == null)
            {
                throw new ArgumentNullException("samples");
            }

            if (sampleLocations.Length != samples.Length)
            {
                throw new Exception(Properties.Resources.Array_Count_Mismatch);
            }

            if (string.IsNullOrEmpty(name))
            {
                name = Properties.Resources.AnalysisResultsDefaultName;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = Properties.Resources.AnalysisResultsDefaultDescription;
            }

            var data = SurfaceData.BySurfacePointsAndValues(surface, sampleLocations, samples);

            return new FaceAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        /// <summary>
        /// Show a colored Face Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="data">A collection of SurfaceData objects.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>A FaceAnalysisDisplay object.</returns>
        [NodeObsolete("SurfaceDataObsolete", typeof(Properties.Resources))]
        public static FaceAnalysisDisplay ByViewAndFaceAnalysisData(
            View view, SurfaceData data, string name = "", string description = "", Type unitType = null)
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

            return new FaceAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        #endregion
        #region Trace management

        /// <summary>
        /// Get the SpatialFieldManager PrimitiveId from Thread Local Storage
        /// </summary>
        /// <returns></returns>
        protected Tuple<SpatialFieldManager, Dictionary<Reference, int>> GetElementAndRefPrimitiveIdFromTrace()
        {
            // This is a provisional implementation until we can store both items in trace
            var traceString = ElementBinder.GetRawDataFromTrace();

            if (String.IsNullOrEmpty(traceString))
                return null;

            SpmRefPrimitiveIdListPair idPair = null;
            try
            {
                idPair = JsonConvert.DeserializeObject<SpmRefPrimitiveIdListPair>(traceString);
            }
            catch
            {
                //do nothing 
            }

            if (idPair == null)
                return null;

            var sfmId = idPair.SpatialFieldManagerID;
            var keyValues = idPair.RefIdPairs;

            SpatialFieldManager sfm = null;

            // if we can't get the sfm, return null
            if (!Document.TryGetElement(new ElementId(sfmId), out sfm))
                return null;

            return new Tuple<SpatialFieldManager, Dictionary<Reference, int>>(sfm, keyValues);
        }

        /// <summary>
        /// Set the SpatialFieldManager and PrimitiveId in Thread Local Storage
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="keyValues"></param>
        protected void SetElementAndRefPrimitiveIdsForTrace(SpatialFieldManager manager, Dictionary<Reference, int> keyValues)
        {
            if (manager == null)
            {
                throw new Exception();
            }

            var idPair = new SpmRefPrimitiveIdListPair
            {
                SpatialFieldManagerID = manager.Id.Value,
                RefIdPairs = keyValues
            };

            var serializedTraceData = JsonConvert.SerializeObject(idPair);
            ElementBinder.SetRawDataForTrace(serializedTraceData);
        }

        protected void SetElementAndRefPrimitiveIdsForTrace()
        {
            SetElementAndRefPrimitiveIdsForTrace(SpatialFieldManager, RefIdPairs);
        }

        #endregion
    }
}
