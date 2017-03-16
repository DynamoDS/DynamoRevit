using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.Elements.Views;
using View3D = Revit.Elements.Views.View3D;

namespace Revit.Elements
{
    /// <summary>
    /// Element wrapper supplies tools for wrapping Autodesk.Revit.DB.Element types
    /// in their associated Revit.Elements.Element wrapper
    /// </summary>
    [SupressImportIntoVM]
    public static class ElementWrapper
    {
        /// <summary>
        /// If possible, wrap the element in a DS type
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="isRevitOwned">Whether the returned object should be revit owned or not</param>
        /// <returns></returns>
        public static Element ToDSType(this Autodesk.Revit.DB.Element ele, bool isRevitOwned)
        {
            // cast to dynamic to dispatch to the appropriate wrapping method
            dynamic dynamicElement = ele;
            return ElementWrapper.Wrap(dynamicElement, isRevitOwned);

        }

        #region Wrap methods

        public static UnknownElement Wrap(Autodesk.Revit.DB.Element element, bool isRevitOwned)
        {
            return UnknownElement.FromExisting(element, isRevitOwned);
        }

        public static AbstractFamilyInstance Wrap(Autodesk.Revit.DB.FamilyInstance ele, bool isRevitOwned)
        {
            if (AdaptiveComponentInstanceUtils.HasAdaptiveFamilySymbol(ele))
            {
                return AdaptiveComponent.FromExisting(ele, isRevitOwned);
            }

            if (ele.StructuralType != Autodesk.Revit.DB.Structure.StructuralType.NonStructural &&
                ele.StructuralType != Autodesk.Revit.DB.Structure.StructuralType.Footing)
            {
                return StructuralFraming.FromExisting(ele, isRevitOwned);
            }

            return FamilyInstance.FromExisting(ele, isRevitOwned);
        }

        public static DirectShape Wrap(Autodesk.Revit.DB.DirectShape ele, bool isRevitOwned)
        {
            return DirectShape.FromExisting(ele, isRevitOwned);
        }

        public static DividedPath Wrap(Autodesk.Revit.DB.DividedPath ele, bool isRevitOwned)
        {
            return DividedPath.FromExisting(ele, isRevitOwned);
        }

        public static DividedSurface Wrap(Autodesk.Revit.DB.DividedSurface ele, bool isRevitOwned)
        {
            return DividedSurface.FromExisting(ele, isRevitOwned);
        }

        public static Family Wrap(Autodesk.Revit.DB.Family ele, bool isRevitOwned)
        {
            return Family.FromExisting(ele, isRevitOwned);
        }

        public static FamilyType Wrap(Autodesk.Revit.DB.FamilySymbol ele, bool isRevitOwned)
        {
            return FamilyType.FromExisting(ele, isRevitOwned);
        }

        public static Floor Wrap(Autodesk.Revit.DB.Floor ele, bool isRevitOwned)
        {
            return Floor.FromExisting(ele, isRevitOwned);
        }

        public static FloorType Wrap(Autodesk.Revit.DB.FloorType ele, bool isRevitOwned)
        {
            return FloorType.FromExisting(ele, isRevitOwned);
        }

        public static Form Wrap(Autodesk.Revit.DB.Form ele, bool isRevitOwned)
        {
            return Form.FromExisting(ele, isRevitOwned);
        }

        [SupressImportIntoVM]
        public static FreeForm Wrap(Autodesk.Revit.DB.FreeFormElement ele, bool isRevitOwned)
        {
            return FreeForm.FromExisting(ele, isRevitOwned);
        }

        public static Grid Wrap(Autodesk.Revit.DB.Grid ele, bool isRevitOwned)
        {
            return Grid.FromExisting(ele, isRevitOwned);
        }

        public static Level Wrap(Autodesk.Revit.DB.Level ele, bool isRevitOwned)
        {
            return Level.FromExisting(ele, isRevitOwned);
        }

        public static ModelCurve Wrap(Autodesk.Revit.DB.ModelCurve ele, bool isRevitOwned)
        {
            return ModelCurve.FromExisting(ele, isRevitOwned);
        }

        public static CurveByPoints Wrap(Autodesk.Revit.DB.CurveByPoints ele, bool isRevitOwned)
        {
            return CurveByPoints.FromExisting(ele, isRevitOwned);
        }

        public static ModelText Wrap(Autodesk.Revit.DB.ModelText ele, bool isRevitOwned)
        {
            return ModelText.FromExisting(ele, isRevitOwned);
        }

        public static ModelTextType Wrap(Autodesk.Revit.DB.ModelTextType ele, bool isRevitOwned)
        {
            return ModelTextType.FromExisting(ele, isRevitOwned);
        }

        public static ReferencePlane Wrap(Autodesk.Revit.DB.ReferencePlane ele, bool isRevitOwned)
        {
            return ReferencePlane.FromExisting(ele, isRevitOwned);
        }

        public static ReferencePoint Wrap(Autodesk.Revit.DB.ReferencePoint ele, bool isRevitOwned)
        {
            return ReferencePoint.FromExisting(ele, isRevitOwned);
        }

        public static SketchPlane Wrap(Autodesk.Revit.DB.SketchPlane ele, bool isRevitOwned)
        {
            return SketchPlane.FromExisting(ele, isRevitOwned);
        }

        public static Wall Wrap(Autodesk.Revit.DB.Wall ele, bool isRevitOwned)
        {
            return Wall.FromExisting(ele, isRevitOwned);
        }

        public static WallType Wrap(Autodesk.Revit.DB.WallType ele, bool isRevitOwned)
        {
            return WallType.FromExisting(ele, isRevitOwned);
        }

        public static View3D Wrap(Autodesk.Revit.DB.View3D view, bool isRevitOwned)
        {
            if (!view.IsTemplate)
            {
                if (view.IsPerspective)
                    return PerspectiveView.FromExisting(view, isRevitOwned);
                else
                    return AxonometricView.FromExisting(view, isRevitOwned);
            }
            return null;
        }

        public static Element Wrap(Autodesk.Revit.DB.ViewPlan view, bool isRevitOwned)
        {
            switch (view.ViewType)
            {
                case ViewType.CeilingPlan:
                    return CeilingPlanView.FromExisting(view, isRevitOwned);
                case ViewType.FloorPlan:
                    return FloorPlanView.FromExisting(view, isRevitOwned);
                case ViewType.EngineeringPlan:
                    return StructuralPlanView.FromExisting(view, isRevitOwned);
                default:
                    return UnknownElement.FromExisting(view, true);
            }
        }

        public static SectionView Wrap(Autodesk.Revit.DB.ViewSection view, bool isRevitOwned)
        {
            return SectionView.FromExisting(view, isRevitOwned);
        }

        public static ScheduleView Wrap(Autodesk.Revit.DB.ViewSchedule view, bool isRevitOwned)
        {
            return ScheduleView.FromExisting(view, isRevitOwned);
        }

        public static Sheet Wrap(Autodesk.Revit.DB.ViewSheet view, bool isRevitOwned)
        {
            return Sheet.FromExisting(view, isRevitOwned);
        }

        public static DraftingView Wrap(Autodesk.Revit.DB.ViewDrafting view, bool isRevitOwned)
        {
            return DraftingView.FromExisting(view, isRevitOwned);
        }

        public static Topography Wrap(Autodesk.Revit.DB.Architecture.TopographySurface topoSurface, bool isRevitOwned)
        {
            return Topography.FromExisting(topoSurface, isRevitOwned);
        }

        public static object Wrap(Autodesk.Revit.DB.Panel ele, bool isRevitOwned)
        {
            if (AdaptiveComponentInstanceUtils.IsAdaptiveFamilySymbol(ele.Symbol))
            {
                return AdaptiveComponent.FromExisting(ele, isRevitOwned);
            }

           return CurtainPanel.FromExisting(ele, isRevitOwned);
        }

        public static Mullion Wrap(Autodesk.Revit.DB.Mullion ele, bool isRevitOwned)
        {
           return Mullion.FromExisting(ele, isRevitOwned);
        }

        public static Dimension Wrap(Autodesk.Revit.DB.Dimension ele, bool isRevitOwned)
        {
            return Dimension.FromExisting(ele, isRevitOwned);
        }

        public static DimensionType Wrap(Autodesk.Revit.DB.DimensionType ele, bool isRevitOwned)
        {
            return DimensionType.FromExisting(ele, isRevitOwned);
        }

        public static FilledRegionType Wrap(Autodesk.Revit.DB.FilledRegionType ele, bool isRevitOwned)
        {
            return FilledRegionType.FromExisting(ele, isRevitOwned);
        }

        public static FilledRegion Wrap(Autodesk.Revit.DB.FilledRegion ele, bool isRevitOwned)
        {
            return FilledRegion.FromExisting(ele, isRevitOwned);
	    }

        public static FillPatternElement Wrap(Autodesk.Revit.DB.FillPatternElement ele, bool isRevitOwned)
        {
            return FillPatternElement.FromExisting(ele, isRevitOwned);
        }

        public static LinePatternElement Wrap(Autodesk.Revit.DB.LinePatternElement ele, bool isRevitOwned)
        {
            return LinePatternElement.FromExisting(ele, isRevitOwned);
        }

        public static TextNote Wrap(Autodesk.Revit.DB.TextNote ele, bool isRevitOwned)
        {
            return TextNote.FromExisting(ele, isRevitOwned);
        }

        public static Tag Wrap(Autodesk.Revit.DB.IndependentTag ele, bool isRevitOwned)
        {
            return Tag.FromExisting(ele, isRevitOwned);
        }

        public static TextNoteType Wrap(Autodesk.Revit.DB.TextNoteType ele, bool isRevitOwned)
        {
            return TextNoteType.FromExisting(ele, isRevitOwned);
		}

        public static Revision Wrap(Autodesk.Revit.DB.Revision ele, bool isRevitOwned)
        {
            return Revision.FromExisting(ele, isRevitOwned);
        }

        public static RevisionCloud Wrap(Autodesk.Revit.DB.RevisionCloud ele, bool isRevitOwned)
        {
            return RevisionCloud.FromExisting(ele, isRevitOwned);
		}

        public static Revit.Filter.ParameterFilterElement Wrap(Autodesk.Revit.DB.ParameterFilterElement ele, bool isRevitOwned)
        {
            return Revit.Filter.ParameterFilterElement.FromExisting(ele, isRevitOwned);
    	}

        public static Room Wrap(Autodesk.Revit.DB.Architecture.Room ele, bool isRevitOwned)
        {
            return Room.FromExisting(ele, isRevitOwned);
    	}

        public static DetailCurve Wrap(Autodesk.Revit.DB.DetailCurve ele, bool isRevitOwned)
        {
            return DetailCurve.FromExisting(ele, isRevitOwned);

        }

        public static FaceWall Wrap(Autodesk.Revit.DB.FaceWall ele, bool isRevitOwned)
        {
            return FaceWall.FromExisting(ele, isRevitOwned);
        }

        public static GlobalParameter Wrap(Autodesk.Revit.DB.GlobalParameter ele, bool isRevitOwned)
        {
            return GlobalParameter.FromExisting(ele, isRevitOwned);

        }

        public static CurtainSystem Wrap(Autodesk.Revit.DB.CurtainSystem ele, bool isRevitOwned)
        {
            return CurtainSystem.FromExisting(ele, isRevitOwned);
        }
        #endregion

    }

}
