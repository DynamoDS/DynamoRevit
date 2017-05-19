using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements.InternalUtilities
{
    [IsVisibleInDynamoLibrary(false)]
    public static class ElementQueries
    {
        private static readonly HashSet<Type> ClassFilterExceptions = new HashSet<Type>
        {
            typeof(Autodesk.Revit.DB.Material),
            typeof(Autodesk.Revit.DB.CurveElement),
            typeof(Autodesk.Revit.DB.ConnectorElement),
            typeof(Autodesk.Revit.DB.HostedSweep),
            typeof(Autodesk.Revit.DB.Architecture.Room),
            typeof(Autodesk.Revit.DB.Mechanical.Space),
            typeof(Autodesk.Revit.DB.Area),
            typeof(Autodesk.Revit.DB.Architecture.RoomTag),
            typeof(Autodesk.Revit.DB.Mechanical.SpaceTag),
            typeof(Autodesk.Revit.DB.AreaTag),
            typeof(Autodesk.Revit.DB.CombinableElement),
            typeof(Autodesk.Revit.DB.Mullion),
            typeof(Autodesk.Revit.DB.Panel),
            typeof(Autodesk.Revit.DB.AnnotationSymbol),
            typeof(Autodesk.Revit.DB.Structure.AreaReinforcementType),
            typeof(Autodesk.Revit.DB.Structure.PathReinforcementType),
            typeof(Autodesk.Revit.DB.AnnotationSymbolType),
            typeof(Autodesk.Revit.DB.Architecture.RoomTagType),
            typeof(Autodesk.Revit.DB.Mechanical.SpaceTagType),
            typeof(Autodesk.Revit.DB.AreaTagType),
            typeof(Autodesk.Revit.DB.Structure.TrussType)
        };

        public static IList<Element> OfFamilyType(FamilyType familyType)
        {
            if (familyType == null) return null;

            var instanceFilter = new ElementClassFilter(typeof(Autodesk.Revit.DB.FamilyInstance));
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            var familyInstances = fec.WherePasses(instanceFilter)
                .WhereElementIsNotElementType()
                .ToElements()
                .Cast<Autodesk.Revit.DB.FamilyInstance>();

            var matches = familyInstances.Where(x => x.Symbol.Id == familyType.InternalFamilySymbol.Id);

            var instances = matches
                .Select(x => ElementSelector.ByElementId(x.Id.IntegerValue)).ToList();
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
                return new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                    .OfClass(elementType.BaseType)
                    .Where(x => x.GetType() == elementType)
                    .Select(x => ElementSelector.ByElementId(x.Id.IntegerValue))
                    .ToList();
            }

            var classFilter = new ElementClassFilter(elementType);
            return new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument)
                .WherePasses(classFilter)
                .ToElementIds()
                .Select(x => ElementSelector.ByElementId(x.IntegerValue))
                .ToList();
        }

        public static IList<Element> OfCategory(Category category)
        {
            if (category == null) return null;

            var catFilter = new ElementCategoryFilter(category.InternalCategory.Id);
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            var instances = 
                fec.WherePasses(catFilter)
                    .WhereElementIsNotElementType()
                    .ToElementIds()
                    .Select(id => ElementSelector.ByElementId(id.IntegerValue))
                    .ToList();
            return instances;
        }

        public static IList<Element> AtLevel(Level arg)
        {
            if (arg == null) return null;

            var levFilter = new ElementLevelFilter(arg.InternalLevel.Id);
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            var instances =
                fec.WherePasses(levFilter)
                    .WhereElementIsNotElementType()
                    .ToElementIds()
                    .Select(id => ElementSelector.ByElementId(id.IntegerValue))
                    .ToList();
            return instances;
        }

        internal static IEnumerable<Autodesk.Revit.DB.Level> GetAllLevels()
        {
            var collector = new Autodesk.Revit.DB.FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            collector.OfClass(typeof(Autodesk.Revit.DB.Level));
            return collector.ToElements().Cast<Autodesk.Revit.DB.Level>();
        }
    }
}
