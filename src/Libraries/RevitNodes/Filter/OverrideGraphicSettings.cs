using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;

namespace Revit.Filter
{
    /// <summary>
    /// Override Graphic Settings
    /// </summary>
    public class OverrideGraphicSettings
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.OverrideGraphicSettings InternalOverrideGraphicSettings
        { 
            get; set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        internal OverrideGraphicSettings(Autodesk.Revit.DB.OverrideGraphicSettings internalOverrideGraphicSettings)
        {
            this.InternalOverrideGraphicSettings = internalOverrideGraphicSettings;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a OverrideGraphicSettings element
        /// </summary>
        /// <param name="cutFillColor">Fill color</param>
        /// <param name="projectionFillColor">Projection color</param>
        /// <param name="cutLineColor">Cut line color</param>
        /// <param name="projectionLineColor">Projection line color</param>
        /// <param name="cutLineWeight">Cut line weight</param>
        /// <param name="projectionLineWeight">Projection line weight</param>
        /// <param name="cutFillPattern">Cut fill pattern</param>
        /// <param name="projectionFillPattern">Projection fill pattern</param>
        /// <param name="cutLinePattern">Cut line pattern</param>
        /// <param name="projectionLinePattern">Projection line pattern</param>
        /// <returns>OverrideGraphicSettings</returns>
        public static OverrideGraphicSettings ByProperties(
            DSCore.Color cutFillColor = null,
            DSCore.Color projectionFillColor = null,
            DSCore.Color cutLineColor = null,
            DSCore.Color projectionLineColor = null,
            int cutLineWeight = -1,
            int projectionLineWeight = -1,
            FillPatternElement cutFillPattern = null,
            FillPatternElement projectionFillPattern = null,
            LinePatternElement cutLinePattern = null,
            LinePatternElement projectionLinePattern = null
            )
        {
            Autodesk.Revit.DB.OverrideGraphicSettings filterSettings = new Autodesk.Revit.DB.OverrideGraphicSettings();

            // Apply Colors
            if (cutFillColor != null) filterSettings.SetCutFillColor(ToRevitColor(cutFillColor));
            if (projectionFillColor != null) filterSettings.SetProjectionFillColor(ToRevitColor(projectionFillColor));
            if (cutLineColor != null) filterSettings.SetCutLineColor(ToRevitColor(cutLineColor));
            if (projectionLineColor != null) filterSettings.SetProjectionLineColor(ToRevitColor(projectionLineColor));

            // Apply Lineweight
            if (cutLineWeight != -1) filterSettings.SetCutLineWeight(cutLineWeight);
            if (projectionLineWeight != -1) filterSettings.SetProjectionLineWeight(projectionLineWeight);

            // Apply Patterns          
            if (cutFillPattern != null) filterSettings.SetCutFillPatternId(cutFillPattern.Id);
            if (projectionFillPattern != null) filterSettings.SetProjectionFillPatternId(projectionFillPattern.Id);
            if (cutLinePattern != null) filterSettings.SetCutLinePatternId(cutLinePattern.Id);
            if (projectionLinePattern != null) filterSettings.SetProjectionLinePatternId(projectionLinePattern.Id);

            return new OverrideGraphicSettings(filterSettings);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Revit Color to DS Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Autodesk.Revit.DB.Color ToRevitColor(DSCore.Color color)
        {
            return new Autodesk.Revit.DB.Color(color.Red, color.Green, color.Blue);
        }

        #endregion

    }



}
