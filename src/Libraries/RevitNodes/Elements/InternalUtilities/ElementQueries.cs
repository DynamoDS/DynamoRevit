using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.Elements.Views;
using RevitServices.Persistence;

namespace Revit.Elements.InternalUtilities
{
    [IsVisibleInDynamoLibrary(false)]
    public static class ElementQueries
    {
        internal static readonly HashSet<Type> ClassFilterExceptions = new HashSet<Type>
        {
            typeof(Autodesk.Revit.DB.HostedSweep),
            typeof(Autodesk.Revit.DB.Architecture.Room),
            typeof(Autodesk.Revit.DB.Mechanical.Space),
            typeof(Autodesk.Revit.DB.Area),
            typeof(Autodesk.Revit.DB.Architecture.RoomTag),
            typeof(Autodesk.Revit.DB.Mechanical.SpaceTag),
            typeof(Autodesk.Revit.DB.AreaTag),
            typeof(Autodesk.Revit.DB.Mullion),
            typeof(Autodesk.Revit.DB.Panel),
            typeof(Autodesk.Revit.DB.AnnotationSymbol),
            typeof(Autodesk.Revit.DB.Structure.AreaReinforcementType),
            typeof(Autodesk.Revit.DB.Structure.PathReinforcementType),
            typeof(Autodesk.Revit.DB.AnnotationSymbolType),
            typeof(Autodesk.Revit.DB.Architecture.RoomTagType),
            typeof(Autodesk.Revit.DB.Mechanical.SpaceTagType),
            typeof(Autodesk.Revit.DB.AreaTagType),
            typeof(Autodesk.Revit.DB.Structure.TrussType),
            typeof(Autodesk.Revit.DB.Structure.AreaReinforcementCurve),
            typeof(Autodesk.Revit.DB.CurveByPoints),
            typeof(Autodesk.Revit.DB.DetailArc),
            typeof(Autodesk.Revit.DB.DetailCurve),
            typeof(Autodesk.Revit.DB.DetailEllipse),
            typeof(Autodesk.Revit.DB.DetailLine),
            typeof(Autodesk.Revit.DB.DetailNurbSpline),
            typeof(Autodesk.Revit.DB.Architecture.Fascia),
            typeof(Autodesk.Revit.DB.Architecture.Gutter),
            typeof(Autodesk.Revit.DB.ModelArc),
            typeof(Autodesk.Revit.DB.ModelCurve),
            typeof(Autodesk.Revit.DB.ModelEllipse),
            typeof(Autodesk.Revit.DB.ModelHermiteSpline),
            typeof(Autodesk.Revit.DB.ModelLine),
            typeof(Autodesk.Revit.DB.ModelNurbSpline),
            typeof(Autodesk.Revit.DB.SlabEdge),
            typeof(Autodesk.Revit.DB.SymbolicCurve)
        };

        internal static Type GetClassFilterExceptionsValidType(Type elementType)
        {
            if(ClassFilterExceptions.Contains(elementType.BaseType))
            {
                return GetClassFilterExceptionsValidType(elementType.BaseType);
            }
            else
            {
                return elementType.BaseType;
            }
        }

        public static IList<Element> OfFamilyType(FamilyType familyType)
        {
            if (familyType == null) return null;

            var instanceFilter = new Autodesk.Revit.DB.ElementClassFilter(typeof(Autodesk.Revit.DB.FamilyInstance));
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            var familyInstances = fec.WherePasses(instanceFilter)
                .WhereElementIsNotElementType()
                .ToElements()
                .Cast<Autodesk.Revit.DB.FamilyInstance>();

            var matches = familyInstances.Where(x => x.Symbol.Id == familyType.InternalFamilySymbol.Id);

            var instances = matches
                .Select(x => ElementSelector.ByElementId(x.Id.Value)).ToList();
            return instances;
        }

        public static IList<Element> OfElementType(Type elementType)
        {
            if (elementType == null) return null;

            /*
            (Konrad) According to RevitAPI documentation the quick filter
            ElementClassFilter() has certain limitations that prevent it
            from working on certain derived classes. In that case we need
            to collect elements from base class and then perform additional
            filtering to get our intended element set.
            */

            if (ClassFilterExceptions.Contains(elementType))
            {
                var type = GetClassFilterExceptionsValidType(elementType);
                return new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                    .OfClass(type)
                    .Where(x => x.GetType() == elementType)
                    .Select(x => ElementSelector.ByElementId(x.Id.Value))
                    .ToList();
            }

            var classFilter = new Autodesk.Revit.DB.ElementClassFilter(elementType);
            return new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .WherePasses(classFilter)
                .ToElementIds()
                .Select(x => ElementSelector.ByElementId(x.Value))
                .ToList();
        }

        public static IList<Element> OfCategory(Category category, Revit.Elements.Views.View view = null)
        {
            if (category == null) return null;

            var catFilter = new Autodesk.Revit.DB.ElementCategoryFilter(category.InternalCategory.Id);
            var fec = (view == null) ? 
                new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument) :
                new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument, view.InternalView.Id);
            var instances = 
                fec.WherePasses(catFilter)
                    .WhereElementIsNotElementType()
                    .ToElementIds()
                    .Select(id => ElementSelector.ByElementId(id.Value))
                    .ToList();
            return instances;
        }

        public static IList<Element> AtLevel(Level arg)
        {
            if (arg == null) return null;

            var levFilter = new Autodesk.Revit.DB.ElementLevelFilter(arg.InternalLevel.Id);
            var fec = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            var instances =
                fec.WherePasses(levFilter)
                    .WhereElementIsNotElementType()
                    .ToElementIds()
                    .Select(id => ElementSelector.ByElementId(id.Value))
                    .ToList();
            return instances;
        }

        public static Element ById(object id)
        {
            if (id == null)
                return null;

            // handle ElementId types first
            if (id.GetType() == typeof(Autodesk.Revit.DB.ElementId))
                return ElementSelector.ByElementId(((Autodesk.Revit.DB.ElementId)id).Value);

            var idType = Type.GetTypeCode(id.GetType());
            Element element;

            switch (idType)
            {
                case TypeCode.Int64:
                    element = ElementSelector.ByElementId((long)id);
                    break;

                case TypeCode.Int32:
                    element = ElementSelector.ByElementId((int)id);
                    break;

                case TypeCode.String:
                    string idString = (string)id;
                    long intId;
                    if (Int64.TryParse(idString, out intId))
                    {
                        element = ElementSelector.ByElementId(intId);
                        break;
                    }

                    element = ElementSelector.ByUniqueId(idString);
                    break;
                    
                default:
                    throw new InvalidOperationException(Revit.Properties.Resources.InvalidElementId);
            }
                
            return element;
        }

        internal static IEnumerable<Autodesk.Revit.DB.Level> GetAllLevels()
        {
            var collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            collector.OfClass(typeof(Autodesk.Revit.DB.Level));
            return collector.ToElements().Cast<Autodesk.Revit.DB.Level>();
        }

        public static List<List<Element>> RoomsByStatus()
        {
            var roomsByStatus = new List<List<Element>>();
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            var allRooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Autodesk.Revit.DB.Architecture.Room>();
            
            var placedRooms = new List<Revit.Elements.Element>();
            var unplacedRooms = new List<Revit.Elements.Element>();
            var notEnclosedRooms = new List<Revit.Elements.Element>();
            var redundantRooms = new List<Revit.Elements.Element>();

            SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();

            foreach (Autodesk.Revit.DB.Architecture.Room room in allRooms)
            {
                var dsRoom = room.ToDSType(true);
                if (room.Area > 0)
                {
                    placedRooms.Add(dsRoom);
                    continue;
                }

                if (room.Location == null)
                {
                    unplacedRooms.Add(dsRoom);
                    continue;
                }
                if (room.GetBoundarySegments(opt) == null || (room.GetBoundarySegments(opt)).Count == 0)
                {
                    notEnclosedRooms.Add(dsRoom);
                    continue;
                }

                redundantRooms.Add(dsRoom);
            }

            roomsByStatus.Add(placedRooms);
            roomsByStatus.Add(unplacedRooms);
            roomsByStatus.Add(notEnclosedRooms);
            roomsByStatus.Add(redundantRooms);

            return roomsByStatus;
        }
    }
}
