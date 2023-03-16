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
        /// Return the Document of the given Link
        /// </summary>
        /// <returns name="linkDocument">linkDocument</returns>
        public static Revit.Application.Document LinkElementDocument(RevitLinkInstance linkInstance)
        {
            var linkDocument = linkInstance.GetLinkDocument();

            return new Revit.Application.Document(linkDocument); 
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
            Autodesk.Revit.DB.ViewPlan revitView = view.InternalView as Autodesk.Revit.DB.ViewPlan;
            if(revitView != null)
            {
                var solidFromCropBox = CreateSolidFromCropBox(currentDocument, linkInstance, revitView);
                var solidIntersectionFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(solidFromCropBox);


                BuiltInCategory bic = (BuiltInCategory)System.Enum.Parse(typeof(BuiltInCategory),
                                                                                     category.InternalCategory.Id.ToString()); 

                var linkedElementsInView = new FilteredElementCollector(linkInstance.GetLinkDocument())
                                .OfCategory(bic)
                                .WhereElementIsNotElementType()
                                .WherePasses(solidIntersectionFilter)
                                .ToElements()
                                .Select(el => el.ToDSType(true))
                                .ToList();
                return linkedElementsInView;
            }
            return null;
        }

        private static Solid CreateSolidFromCropBox(Document doc,RevitLinkInstance link, Autodesk.Revit.DB.ViewPlan viewPlan)
        {

            PlanViewRange planViewRange = viewPlan.GetViewRange();
            ElementId levelId = planViewRange.GetLevelId(PlanViewPlane.CutPlane);
            Autodesk.Revit.DB.Level level = doc.GetElement(levelId) as Autodesk.Revit.DB.Level;
            double levelElevation = level.Elevation;
            double cutPlaneOffset = planViewRange.GetOffset(PlanViewPlane.CutPlane);
            double viewDepth = planViewRange.GetOffset(PlanViewPlane.ViewDepthPlane);
            // view depth needs to be inverted
            double solidBottomZ = levelElevation + viewDepth;
            double solidTopZ = levelElevation + cutPlaneOffset;
            double solidHeight = solidTopZ-solidBottomZ;

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
                          
            List<CurveLoop> correctedCropLoopList = new List<CurveLoop>();
            correctedCropLoopList.Add(correctedCropLoop);

            XYZ direction = XYZ.BasisZ;
            Solid cropViewSolid = GeometryCreationUtilities.CreateExtrusionGeometry(correctedCropLoopList, direction, solidHeight);
            Transform invertedLinkTransform = link.GetTotalTransform().Inverse;
            Solid transformedSolid = SolidUtils.CreateTransformed(cropViewSolid, invertedLinkTransform);
            return transformedSolid;
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
            ElementId levelId = level.InternalLevel.LevelId;
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
            var currentDocument = Application.Document.Current.InternalDocument;
            var linkInstancesByName = new FilteredElementCollector(currentDocument)
                .OfCategory(BuiltInCategory.OST_RvtLinks)
                .WhereElementIsNotElementType()
                .Cast<RevitLinkInstance>()
                .ToList();
            return linkInstancesByName;
        }
    }
}
