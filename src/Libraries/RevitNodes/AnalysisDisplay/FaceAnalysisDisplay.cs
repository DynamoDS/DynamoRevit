using System;
using System.Collections.Generic;
using System.Linq;

using Analysis;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;

using UV = Autodesk.Revit.DB.UV;
using View = Revit.Elements.Views.View;

namespace Revit.AnalysisDisplay
{
    /// <summary>
    /// A Revit Point Analysis Display 
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FaceAnalysisDisplay : AbstractAnalysisDisplay
    {
        internal const string DefaultTag = "RevitFaceReference";

        #region Private constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="data"></param>
        protected FaceAnalysisDisplay(
            Autodesk.Revit.DB.View view, ISurfaceData<Autodesk.DesignScript.Geometry.UV, double> data, string resultsName, string description,Type unitType)
        {
            var sfm = GetSpatialFieldManagerFromView(view);

            // create a new spatial field primitive
            TransactionManager.Instance.EnsureInTransaction(Document);

            sfm.Clear();

            sfm.SetMeasurementNames(new List<string>(){Properties.Resources.Dynamo_AVF_Data_Name});

            InternalSetSpatialFieldManager(sfm);
            var primitiveIds = new List<int>();

            var reference = data.Surface.Tags.LookupTag(DefaultTag) as Reference;
            if (reference == null)
            {
                // Dont' throw an exception here. Handle the case of a bad tag
                // in the static constructor.
                return;
            }
                
            var primitiveId = SpatialFieldManager.AddSpatialFieldPrimitive(reference);
            primitiveIds.Add(primitiveId);
            InternalSetSpatialFieldValues(primitiveId, data, resultsName, description, unitType);

            TransactionManager.Instance.TransactionTaskDone();
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

            var reference = surface.Tags.LookupTag(DefaultTag) as Reference;
            if (reference == null)
            {
                throw new Exception(Properties.Resources.Tag_Lookup_Error);
            }

            var data = SurfaceData.BySurfacePointsAndValues(surface, sampleLocations, samples);

            return new FaceAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        /// <summary>
        /// Show a colored Face Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="data">A collection of SurfaceAnalysisData objects.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>A FaceAnalysisDisplay object.</returns>
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

    }
}
