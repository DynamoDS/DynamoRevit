using System;
using System.Collections.Generic;
using System.Linq;

using Analysis;

using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

using Dynamo.Properties;

using Revit.GeometryConversion;

using RevitServices.Transactions;
using View = Revit.Elements.Views.View;

namespace Revit.AnalysisDisplay
{
    /// <summary>
    /// A Revit Vector Analysis Display 
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class VectorAnalysisDisplay : AbstractAnalysisDisplay
    {
        #region Private constructors
        /// <summary>
        /// Create a Vector Analysis Display in the current view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="data"></param>
        /// <param name="resultsName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private VectorAnalysisDisplay(Autodesk.Revit.DB.View view, VectorData data, 
            string resultsName, string description, Type unitType)
        {
            var sfm = GetSpatialFieldManagerFromView(view);

            TransactionManager.Instance.EnsureInTransaction(Document);

            sfm.Clear();

            sfm.SetMeasurementNames(new List<string>() { Properties.Resources.Dynamo_AVF_Data_Name });

            var primitiveIds = new List<int>();

            InternalSetSpatialFieldManager(sfm);

            var primitiveId = SpatialFieldManager.AddSpatialFieldPrimitive();
            InternalSetSpatialFieldValues(primitiveId, data, resultsName, description, unitType);
            primitiveIds.Add(primitiveId);

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Private mutators
        /// <summary>
        /// Set the spatial field values for the current spatial field primitive.  The two 
        /// input sequences should be of the same length.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="schemaName"></param>
        /// <param name="description"></param>
        /// <param name="unitType"></param>
        private void InternalSetSpatialFieldValues(int primitiveId, VectorData data, string schemaName, string description, Type unitType)
        {
            var valList = data.Values.Select(v => new VectorAtPoint(new List<XYZ> { v.ToXyz() }));
            TransactionManager.Instance.EnsureInTransaction(Document);

            var sampleValues = new FieldValues(valList.ToList());

            // Convert the sample points to a special Revit Type
            var samplePts = new FieldDomainPointsByXYZ(data.ValueLocations.Select(p=>p.ToXyz()).ToList());

            // Get the analysis results schema
            var schemaIndex = GetAnalysisResultSchemaIndex(schemaName, description, unitType);

            // Update the values
            SpatialFieldManager.UpdateSpatialFieldPrimitive(primitiveId, samplePts, sampleValues, schemaIndex);

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Show a Vector Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="samplePoints">The locations at which you want to create analysis values.</param>
        /// <param name="samples">The analysis values at the given locations.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>A VectorAnalysisDisplay object.</returns>
        public static VectorAnalysisDisplay ByViewPointsAndVectorValues(View view,
                        Autodesk.DesignScript.Geometry.Point[] sampleLocations, Vector[] samples,
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
                throw new Exception("The number of sample points and number of samples must be the same");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = Properties.Resources.AnalysisResultsDefaultName;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = Properties.Resources.AnalysisResultsDefaultDescription;
            }

            var data = VectorData.ByPointsAndValues(sampleLocations, samples );
            return new VectorAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        /// <summary>
        /// Show a Vector Analysis Display in the Revit view.
        /// </summary>
        /// <param name="view">The view into which you want to draw the analysis results.</param>
        /// <param name="data">A list of VectorData objects.</param>
        /// <param name="name">An optional analysis results name to show on the results legend.</param>
        /// <param name="description">An optional analysis results description to show on the results legend.</param>
        /// <param name="unitType">An optional Unit type to provide conversions in the analysis results.</param>
        /// <returns>A VectorAnalysisDisplay object.</returns>
        public static VectorAnalysisDisplay ByViewAndVectorAnalysisData(View view,
                        VectorData data,
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

            return new VectorAnalysisDisplay(view.InternalView, data, name, description, unitType);
        }

        #endregion

    }

}
