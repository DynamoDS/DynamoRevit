﻿using Autodesk.DesignScript.Runtime;

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
        /// Create a OverrideGraphicSettings Element.
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
        /// <param name="transparency">Transparency as integer between 1-100.</param>
        /// <param name="detailLevel">Detail Level setting, ex: Coarse, Fine etc.</param>
        /// <param name="halftone">Halftone. True = halftone.</param>
        /// <returns name="overrides">Override Graphic Settings</returns>
        public static OverrideGraphicSettings ByProperties(
            [DefaultArgumentAttribute("null")]DSCore.Color cutFillColor,
            [DefaultArgumentAttribute("null")]DSCore.Color projectionFillColor,
            [DefaultArgumentAttribute("null")]DSCore.Color cutLineColor,
            [DefaultArgumentAttribute("null")]DSCore.Color projectionLineColor,
            [DefaultArgumentAttribute("null")]Revit.Elements.FillPatternElement cutFillPattern,
            [DefaultArgumentAttribute("null")]Revit.Elements.FillPatternElement projectionFillPattern,
            [DefaultArgumentAttribute("null")]Revit.Elements.LinePatternElement cutLinePattern,
            [DefaultArgumentAttribute("null")]Revit.Elements.LinePatternElement projectionLinePattern,
            int cutLineWeight = -1,
            int projectionLineWeight = -1,
            int transparency = -1,
            string detailLevel = "Undefined",
            bool halftone = false
            )
        {
            Autodesk.Revit.DB.OverrideGraphicSettings filterSettings = new Autodesk.Revit.DB.OverrideGraphicSettings();

            // Apply Colors
            if (cutFillColor != null) filterSettings.SetCutForegroundPatternColor(ToRevitColor(cutFillColor));
            if (projectionFillColor != null) filterSettings.SetSurfaceForegroundPatternColor(ToRevitColor(projectionFillColor));
            if (cutLineColor != null) filterSettings.SetCutLineColor(ToRevitColor(cutLineColor));
            if (projectionLineColor != null) filterSettings.SetProjectionLineColor(ToRevitColor(projectionLineColor));

            // Apply Lineweight
            if (cutLineWeight != -1) filterSettings.SetCutLineWeight(cutLineWeight);
            if (projectionLineWeight != -1) filterSettings.SetProjectionLineWeight(projectionLineWeight);

            // Apply Patterns          
            if (cutFillPattern != null) filterSettings.SetCutForegroundPatternId(cutFillPattern.InternalElement.Id);
            if (projectionFillPattern != null) filterSettings.SetSurfaceForegroundPatternId(projectionFillPattern.InternalElement.Id);
            if (cutLinePattern != null) filterSettings.SetCutLinePatternId(cutLinePattern.InternalElement.Id);
            if (projectionLinePattern != null) filterSettings.SetProjectionLinePatternId(projectionLinePattern.InternalElement.Id);

            // Apply transparency, detail level and halftone
            if (transparency != -1) filterSettings.SetSurfaceTransparency(transparency);
            if (halftone) filterSettings.SetHalftone(halftone);
            filterSettings.SetDetailLevel((Autodesk.Revit.DB.ViewDetailLevel)System.Enum.Parse(typeof(Autodesk.Revit.DB.ViewDetailLevel), detailLevel));

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
