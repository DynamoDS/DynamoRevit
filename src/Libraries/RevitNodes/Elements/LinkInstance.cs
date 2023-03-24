using Autodesk.Revit.DB;
using System;
using Revit.Elements;
using System.Collections.Generic;
using System.Linq;
using DynamoUnits;
using View = Revit.Elements.Views.View;
using System.Text;
using System.Threading.Tasks;
using Level = Revit.Elements.Level;
using RevitServices.Persistence;
using Autodesk.Revit.UI;

namespace Revit.Elements
{ 
    /// <summary>
    /// A Revit Link Instance
    /// </summary>
    public static class LinkInstance
    {

        /// <summary>
        /// Return the Document of the given Link Instance
        /// </summary>
        /// <returns name="linkDocument">linkDocument</returns>
        public static Revit.Application.Document Document(RevitLinkInstance revitLinkInstance)
        {
           
            var elementDocument = revitLinkInstance.GetLinkDocument();

            return new Revit.Application.Document(elementDocument);
        }



        /// <summary> Returns a list of elements in the given Revit Link Instance by a given Category </summary>>
        /// <param name="linkInstance">Linked Element Instance </param>>
        /// <param name="category">Element Category to look up in the linked file </param>>
        /// <returns name="linkedElements">Linked Elements of Category</returns>
        public static List<Revit.Elements.Element> AllElementsOfCategory(RevitLinkInstance linkInstance, Revit.Elements.Category category)
        {
            BuiltInCategory bic = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                 category.InternalCategory.Id.ToString());
            var linkedElements = new FilteredElementCollector(linkInstance.GetLinkDocument())
                .OfCategory(bic)
                .WhereElementIsNotElementType()
                .ToElements()
                .Select(el => el.ToDSType(true))
                .ToList();

            return linkedElements;
        }
               

        /// <summary>
        /// Returns a list of elements in the given Revit Link Instance by a given Category and in a given View
        /// </summary>>
        /// <param name="linkInstance">Linked Element Instance </param>>
        /// <param name="category">Linked Element Category</param>>
        /// <param name="view">Active Document View </param>>
        /// <returns name="linkedElementsInView"> Link Elements of Category in Active Document View</returns>
        public static List<Revit.Elements.Element> AllElementsOfCategoryInView(RevitLinkInstance linkInstance, Elements.Category category, Elements.Views.View view)
        {
            
            var currentDocument = Application.Document.Current.InternalDocument;
            Autodesk.Revit.DB.View revitView = view.InternalView as Autodesk.Revit.DB.View;
            
            if(revitView != null)
            {
                Solid solidForFilter;

                // When the View is a Section or Elevation, use the Crop Region
                if (revitView is ViewSection)
                {
                    solidForFilter = CreateSolidFromSectionCropRegion(revitView as ViewSection);
                }
                // When the View is a 3D View, use the Section Box or return null is no section box is enabled
       
                else if (revitView is View3D)
                {
                    View3D threeD = (View3D)revitView;
                    
                    if (threeD.IsSectionBoxActive)
                    {
                        solidForFilter = CreateSolidFromSectionBox(revitView as View3D);
                    }
                    else
                    {
                        solidForFilter = null;
                    }
                }
                else
                {
                    solidForFilter = CreateSolidFromCropRegion(currentDocument,revitView as ViewPlan);
                }

                BuiltInCategory bic = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                         category.InternalCategory.Id.ToString());

                if (solidForFilter != null)
                {
                    Solid transformedSolidInverted = SolidUtils.CreateTransformed(solidForFilter, linkInstance.GetTotalTransform().Inverse);
                    var solidIntersectionFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(transformedSolidInverted);
                                                           
                    var linkedElementsInView = new FilteredElementCollector(linkInstance.GetLinkDocument())
                                    .OfCategory(bic)
                                    .WhereElementIsNotElementType()
                                    .WherePasses(solidIntersectionFilter)
                                    .ToElements()
                                    .Select(el => el.ToDSType(true))
                                    .ToList();
                    return linkedElementsInView;
                }
                else
                {
                    // for 3D views with no section box enabled - the collector will return all 
                    var linkedElements = new FilteredElementCollector(linkInstance.GetLinkDocument())
                                   .OfCategory(bic)
                                   .WhereElementIsNotElementType()                                  
                                   .ToElements()
                                   .Select(el => el.ToDSType(true))
                                   .ToList();
                    return linkedElements;
                }
                
                
            }
            return null;
        }

        private static Solid CreateSolidFromCropRegion(Document doc, Autodesk.Revit.DB.ViewPlan viewPlan)
        {

            PlanViewRange planViewRange = viewPlan.GetViewRange();
            ElementId cutPlaneLevelId = planViewRange.GetLevelId(PlanViewPlane.CutPlane);
            ElementId viewDepthLevelId = planViewRange.GetLevelId(PlanViewPlane.ViewDepthPlane);
            Autodesk.Revit.DB.Level cutPlaneLevel = doc.GetElement(cutPlaneLevelId) as Autodesk.Revit.DB.Level;
            double cutPlaneLevelElevation = cutPlaneLevel.Elevation;
            Autodesk.Revit.DB.Level viewDepthPlaneLevel = doc.GetElement(viewDepthLevelId) as Autodesk.Revit.DB.Level;
            double viewDepthPlaneLevelElevation = viewDepthPlaneLevel.Elevation;
            double cutPlaneOffset = planViewRange.GetOffset(PlanViewPlane.CutPlane);
            double viewDepthOffset = planViewRange.GetOffset(PlanViewPlane.ViewDepthPlane);
            ViewType viewType = viewPlan.ViewType;

            double solidBottomZ;
            double solidTopZ;

            if (viewType == ViewType.CeilingPlan)
            {
                solidBottomZ = cutPlaneLevelElevation + cutPlaneOffset;
                solidTopZ = viewDepthPlaneLevelElevation + viewDepthOffset;
            }
            else
            { 
                solidBottomZ = viewDepthPlaneLevelElevation + viewDepthOffset;
                solidTopZ = cutPlaneLevelElevation + cutPlaneOffset;
            }

            double solidHeight = solidTopZ - solidBottomZ;

            // using crop region to generate the solid for intersection
            ViewCropRegionShapeManager crsm = viewPlan.GetCropRegionShapeManager();
            IList<CurveLoop> cropLoopList = crsm.GetCropShape();

            CurveLoop correctedCropLoop = new CurveLoop();
            
            foreach (CurveLoop cropLoop in cropLoopList)
            {
                foreach (Curve curve in cropLoop)
                {
                    XYZ startPoint = curve.GetEndPoint(0);
                    XYZ endPoint = curve.GetEndPoint(1);
                    XYZ correctedStartPoint = new XYZ(startPoint.X, startPoint.Y, solidBottomZ);
                    XYZ correctedEndPoint = new XYZ(endPoint.X, endPoint.Y, solidBottomZ);
                    Line line = Line.CreateBound(correctedStartPoint, correctedEndPoint);
                    correctedCropLoop.Append(line);
                }
            }

            List<CurveLoop> correctedCropLoopList = new List<CurveLoop>
            {
                correctedCropLoop
            };

            XYZ direction = XYZ.BasisZ;
            Solid cropViewSolid = GeometryCreationUtilities.CreateExtrusionGeometry(correctedCropLoopList, direction, solidHeight);
            
            return cropViewSolid;
        }


        private static Solid CreateSolidFromSectionCropRegion(ViewSection viewSection)
        {
            XYZ sectionDirection = viewSection.ViewDirection.Negate();
            BoundingBoxXYZ sectionBBox = viewSection.get_BoundingBox(null);
            double extrusionDistance = sectionBBox.Max.Z - sectionBBox.Min.Z;

            ViewCropRegionShapeManager crsm = viewSection.GetCropRegionShapeManager();
            IList<CurveLoop> cropLoopList = crsm.GetCropShape();

            Solid cropSolid = GeometryCreationUtilities.CreateExtrusionGeometry(cropLoopList, sectionDirection, extrusionDistance);
            
            return cropSolid;
        }


        private static Solid CreateSolidFromSectionBox(Autodesk.Revit.DB.View3D view3D)
        {
            BoundingBoxXYZ bBox = view3D.GetSectionBox();
            Transform bBoxTransform = bBox.Transform;

            // recontstruct solid by Bounding Box corners 
            XYZ pt0 = new XYZ(bBox.Min.X, bBox.Min.Y, bBox.Min.Z);
            XYZ pt1 = new XYZ(bBox.Max.X, bBox.Min.Y, bBox.Min.Z);
            XYZ pt2 = new XYZ(bBox.Max.X, bBox.Max.Y, bBox.Min.Z);
            XYZ pt3 = new XYZ(bBox.Min.X, bBox.Max.Y, bBox.Min.Z);
                     
            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);

            List<Curve> edges = new List<Curve>
            {
                edge0,
                edge1,
                edge2,
                edge3
            };

            double height = bBox.Max.Z - bBox.Min.Z;
            CurveLoop curveLoop = CurveLoop.Create(edges);
            List<CurveLoop> curveLoopList = new List<CurveLoop>
            {
                curveLoop
            };
            
            XYZ direction = XYZ.BasisZ;
            // transform for positioning the solid within the current doc
            Solid preTransformSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoopList, direction, height);
            Solid sectionBoxSolid = SolidUtils.CreateTransformed(preTransformSolid, bBoxTransform);         
            return sectionBoxSolid;
        }


        /// <summary>
        /// Returns a list of elements in the given Revit Link Instance by a given Category and in a given View
        /// </summary>>
        /// <param name="linkInstance">Linked Element Instance </param>>
        /// <param name="elementClass">Linked Element Class </param>>
        /// <returns>LinkInstanceElementsOfCategoryInView</returns>
        public static List<Revit.Elements.Element> AllElementsOfClass(RevitLinkInstance linkInstance, System.Type elementClass)
        {

            var linkedOfClass = new FilteredElementCollector(linkInstance.GetLinkDocument())
                            .OfClass(elementClass)
                            .WhereElementIsNotElementType()
                            .ToElements()
                            .Select(el => el.ToDSType(true))
                            .ToList();
            return linkedOfClass;
        }


        public static List<Revit.Elements.Element> AllElementsAtLevel(RevitLinkInstance linkInstance, Level level) 
        {
            Autodesk.Revit.DB.Level revitLevel = level.InternalLevel;
            ElementId levelId = revitLevel.Id;
            ElementLevelFilter levelFilter = new ElementLevelFilter(levelId);
            var linkedAtLevel = new FilteredElementCollector(linkInstance.GetLinkDocument())
                .WhereElementIsNotElementType()
                .WherePasses(levelFilter)
                .Select(el => el.ToDSType(true))
                .ToList();
            return linkedAtLevel;
        }


        public static List<RevitLinkInstance> ByName (string name)
        {
            ElementId paramId =  new ElementId(BuiltInParameter.RVT_LINK_INSTANCE_NAME);
            ParameterValueProvider valueProvider = new ParameterValueProvider(paramId);
            FilterStringEquals evaluator = new FilterStringEquals();
            FilterStringRule filterStringRule = new FilterStringRule(valueProvider, evaluator, name);
            ElementParameterFilter paramterFilter = new ElementParameterFilter(filterStringRule);
            var currentDocument = Application.Document.Current.InternalDocument;
            var linkInstancesByName = new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType()
                .WherePasses(paramterFilter)
                .Cast<RevitLinkInstance>()
                .ToList();
            return linkInstancesByName;
        }

        public static List<RevitLinkInstance> ByTitle(string title)
        {
            var currentDocument = Application.Document.Current.InternalDocument;
            var linkInstances = new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType()            
                .ToList();
            List<RevitLinkInstance> linkInstancesByTitle = new List<RevitLinkInstance>();
            foreach (RevitLinkInstance linkInstance in linkInstances)
                {
                Document linkDoc = linkInstance.GetLinkDocument();
                if (linkDoc.Title == title)
                    { 
                        linkInstancesByTitle.Add(linkInstance);
                    }
                }
            return linkInstancesByTitle;
        }


        public static ElementId Id (RevitLinkInstance revitLinkInstance)
        {
            ElementId id = revitLinkInstance.Id;
            return id;
        }


        public static string UniqueId(RevitLinkInstance revitLinkInstance)
        {
            string uniqueId = revitLinkInstance.UniqueId;
            return uniqueId;
        }


        public static Element ElementById(string idAsString, RevitLinkInstance revitLinkInstance)
        {
            int idAsInt = int.Parse(idAsString);
            Document linkDocument = revitLinkInstance.GetLinkDocument();
            ElementId elementId = new ElementId(idAsInt);
            Autodesk.Revit.DB.Element linkElementById = linkDocument.GetElement((elementId));
            return linkElementById.ToDSType(true);
        }


    }
}
