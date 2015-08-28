using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace IRevit
{
    public interface IRevitApplication
    {
    }

    public interface IRevitUIApplication
    {

    }

    public interface IRevitElementId
    {

    }

    public interface IRevitDocument
    {
        bool TryGetElement(string id, out IRevitElement element);
        IRevitElement GetElement(IRevitElementId id);
    }

    public interface IRevitUIDocument
    {

    }

    public interface IRevitView
    {

    }

    public interface IRevitElement
    {

    }

    public interface IRevitMaterial
    {

    }

    public interface IRevitFilteredElementCollector : IEnumerable<IRevitElement>, IDisposable
    {

    }

    public interface IRevitElementQuerys
    {
        Dictionary<string, T> ElementsOfTypeByName<T>();
    }

    public interface IRevitUpdaterData
    {
        IEnumerable<IRevitElementId> GetAddedElementIds();
        IEnumerable<IRevitElementId> GetModifiedElementIds();
        IEnumerable<IRevitElementId> GetDeletedElementIds();
    }

    public class DynamoRevitElementQueries
    {
        public DynamoRevitElementQueries(IRevitDocument doc)
        {

        }
    }
}
