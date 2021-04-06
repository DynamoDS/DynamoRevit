﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using CoreNodeModels;
using Dynamo.Interfaces;
using Dynamo.Logging;

using RevitServices.Persistence;

namespace Revit.Interactivity
{
    internal class RevitReferenceSelectionHelper : LogSourceBase, IModelSelectionHelper<Reference>
    {
        private static readonly RevitReferenceSelectionHelper instance =
            new RevitReferenceSelectionHelper();

        public static RevitReferenceSelectionHelper Instance
        {
            get { return instance; }
        }

        public IEnumerable<Reference> RequestSelectionOfType(
            string selectionMessage, SelectionType selectionType, SelectionObjectType objectType)
        {
            switch (selectionType)
            {
                case SelectionType.One:
                    return RequestReferenceSelection(selectionMessage, AsLogger(), objectType);

                case SelectionType.Many:
                    return RequestMultipleReferencesSelection(selectionMessage, AsLogger(), objectType);
            }

            return null;
        }

        #region private static methods

        private static IEnumerable<Reference> RequestReferenceSelection(
            string message, ILogger logger, SelectionObjectType selectionType)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            Reference reference = null;

            var choices = doc.Selection;

            choices.SetElementIds(new Collection<ElementId>());

            logger.Log(message);

            switch (selectionType)
            {
                case SelectionObjectType.Face:
                    reference = doc.Selection.PickObject(ObjectType.Face, message);
                    break;
                case SelectionObjectType.Edge:
                    reference = doc.Selection.PickObject(ObjectType.Edge, message);
                    break;
                case SelectionObjectType.PointOnFace:
                    reference = doc.Selection.PickObject(ObjectType.PointOnElement, message);
                    break;
                case SelectionObjectType.None:
                    reference = doc.Selection.PickObject(ObjectType.PointOnElement, new ReferenceSelectionFilter(), message);
                    break;
            }

            return reference == null ? null : new List<Reference> { reference };
        }

        private static IEnumerable<Reference> RequestMultipleReferencesSelection(
            string message, ILogger logger, SelectionObjectType selectionType)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            IList<Reference> references = null;

            var choices = doc.Selection;

            choices.SetElementIds(new Collection<ElementId>());


            logger.Log(message);

            switch (selectionType)
            {
                case SelectionObjectType.Face:
                    references = doc.Selection.PickObjects(ObjectType.Face, message);
                    break;
                case SelectionObjectType.Edge:
                    references = doc.Selection.PickObjects(ObjectType.Edge, message);
                    break;
                case SelectionObjectType.PointOnFace:
                    references = doc.Selection.PickObjects(ObjectType.PointOnElement, message);
                    break;
            }

            if (references == null || !references.Any())
                return null;

            return references;
        }

        #endregion
    }

    internal class RevitElementSelectionHelper<T> : LogSourceBase, IModelSelectionHelper<T> where T : Element
    {
        private static readonly RevitElementSelectionHelper<T> instance = new RevitElementSelectionHelper<T>();

        public static RevitElementSelectionHelper<T> Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Request an element in a selection.
        /// </summary>
        /// <typeparam name="T">The type of the Element.</typeparam>
        /// <param name="selectionMessage">The message to display.</param>
        /// <param name="selectionType">The selection type.</param>
        /// <param name="objectType">The selection object type.</param>
        /// <returns></returns>
        public IEnumerable<T> RequestSelectionOfType(
            string selectionMessage, SelectionType selectionType, SelectionObjectType objectType)
        {
            switch (selectionType)
            {
                case SelectionType.One:
                    return RequestElementSelection(selectionMessage, AsLogger());

                case SelectionType.Many:
                    return RequestMultipleElementsSelection(selectionMessage, AsLogger());
            }

            return null;
        }

        public static IEnumerable<Element> GetFamilyInstancesFromDividedSurface(DividedSurface ds)
        {
            var gn = new GridNode();

            var u = 0;
            while (u < ds.NumberOfUGridlines)
            {
                gn.UIndex = u;

                var v = 0;

                while (v < ds.NumberOfVGridlines)
                {
                    gn.VIndex = v;

                    //"Reports whether a grid node is a "seed node," a node that is associated with one or more tiles."
                    if (ds.IsSeedNode(gn))
                    {
                        var fi = ds.GetTileFamilyInstance(gn, 0);

                        if (fi != null)
                        {
                            //put the family instance into the tree
                            yield return fi;
                        }
                    }
                    v = v + 1;
                }

                u = u + 1;
            }
        }

        #region private static methods

        private static IEnumerable<T> RequestElementSelection(string selectionMessage, ILogger logger)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            Element e = null;

            var choices = doc.Selection;
            choices.SetElementIds(new Collection<ElementId>());

            logger.Log(selectionMessage);

            var elementRef = doc.Selection.PickObject(
                ObjectType.Element,
                new ElementSelectionFilter<T>(),
                selectionMessage);

            if (elementRef != null)
            {
                e = DocumentManager.Instance.CurrentDBDocument.GetElement(elementRef);
            }

            return new[] { e }.Cast<T>();
        }

        private static IEnumerable<T> RequestMultipleElementsSelection(
            string selectionMessage, ILogger logger)
        {
            var doc = DocumentManager.Instance.CurrentUIDocument;

            var choices = doc.Selection;
            choices.SetElementIds(new Collection<ElementId>());

            logger.Log(selectionMessage);

            var elements = doc.Selection.PickElementsByRectangle(
                new ElementSelectionFilter<T>(),
                selectionMessage);

            return elements.Cast<T>();
        }

        #endregion
    }

    internal class ElementSelectionFilter<T> : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is T;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    internal class ReferenceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            if (reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE 
                || reference.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_LINEAR)
                return true;
            return false;
        }
    }
}
