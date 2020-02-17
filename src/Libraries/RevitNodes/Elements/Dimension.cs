using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// Dimension element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Dimension : Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.Dimension InternalRevitElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRevitElement; }
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="wall"></param>
        private void InternalSetElement(Autodesk.Revit.DB.Dimension element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Dimension from existing
        /// </summary>
        /// <param name="element"></param>
        private Dimension(Autodesk.Revit.DB.Dimension element)
        {
            SafeInit(() => InitElement(element));
        }

        /// <summary>
        /// Dimension by Elements
        /// </summary>
        /// <param name="view"></param>
        /// <param name="line"></param>
        /// <param name="references"></param>
        /// <param name="suffix"></param>
        /// <param name="prefix"></param>
        private Dimension(Autodesk.Revit.DB.View view, Autodesk.Revit.DB.Line line, ReferenceArray references, string suffix, string prefix)
        {
            SafeInit(() => Init(view, line, references, suffix, prefix));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init from existing
        /// </summary>
        /// <param name="element"></param>
        private void InitElement(Autodesk.Revit.DB.Dimension element)
        {
            InternalSetElement(element);
        }

        /// <summary>
        /// Init by Elements
        /// </summary>
        /// <param name="view"></param>
        /// <param name="line"></param>
        /// <param name="references"></param>
        /// <param name="suffix"></param>
        /// <param name="prefix"></param>
        private void Init(Autodesk.Revit.DB.View view, Autodesk.Revit.DB.Line line, ReferenceArray references, string suffix, string prefix)
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // get element from trace
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Dimension>(document);

            if (element == null)
            {
                element = document.Create.NewDimension(view, line, references);
            }
            else
            {
                LocationCurve location = element.Location as LocationCurve;
                location.Curve = line;
            }

            InternalSetElement(element);

            // apply suffix
            if (this.InternalRevitElement.NumberOfSegments == 0)
            { 
                this.InternalRevitElement.Suffix = suffix; 
            }
            else if (this.InternalRevitElement.NumberOfSegments > 1)
            {
                foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                { 
                    segment.Suffix = suffix; 
                }
            }

            // apply prefix
            if (this.InternalRevitElement.NumberOfSegments == 0)
            {
                this.InternalRevitElement.Prefix = prefix;
            }
            else if (this.InternalRevitElement.NumberOfSegments > 1)
            {
                foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                { 
                    segment.Prefix = prefix; 
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a Revit Dimension from at least two elements
        /// </summary>
        /// <param name="view">View to place dimension in</param>
        /// <param name="referenceElements">Elements to dimension</param>
        /// <param name="line">location of the dimension</param>
        /// <param name="suffix">Suffix</param>
        /// <param name="prefix">Prefix</param>
        /// <returns>Dimension</returns>
        public static Dimension ByElements(Revit.Elements.Views.View view, IEnumerable<Revit.Elements.Element> referenceElements, [DefaultArgument("null")]Autodesk.DesignScript.Geometry.Line line, string suffix = "", string prefix = "")
        {
            var elements = referenceElements.ToList();

            if (elements.Count < 2) throw new Exception(Properties.Resources.NotEnoughDataError);

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;
            Line revitLine = null;

            // if there is no location line supplied, draw a line between the midpoints
            // of the supplied elements.

            if (line == null)
            {
                BoundingBoxXYZ boundingBoxFirstElement = elements[0].InternalElement.get_BoundingBox(revitView);
                if ((boundingBoxFirstElement) == null) throw new Exception(Properties.Resources.ElementCannotBeAnnotatedError); 

                BoundingBoxXYZ boundingBoxLastElement = elements[elements.Count - 1].InternalElement.get_BoundingBox(revitView);
                if ((boundingBoxLastElement) == null) throw new Exception(Properties.Resources.ElementCannotBeAnnotatedError);

                revitLine = Line.CreateBound(GetMidpoint(boundingBoxFirstElement), GetMidpoint(boundingBoxLastElement));
            }
            else
                revitLine = (Line)line.ToRevitType(true);

            if (!view.IsAnnotationView())
            {
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);
            }

            ReferenceArray array = new ReferenceArray();

            foreach (Revit.Elements.Element element in elements)
            {
                array.Append(new Reference(element.InternalElement));
            }

            return new Dimension(revitView, revitLine, array, suffix, prefix);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get Dimension Value
        /// </summary>
        public IEnumerable<double> Value 
        {
            get
            {
                List<double> data = new List<double>();

                if (this.InternalRevitElement.NumberOfSegments == 0)
                    data.Add(this.InternalRevitElement.Value.Value);

                else if (this.InternalRevitElement.NumberOfSegments > 1)
                {
                    foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                        data.Add(segment.Value.Value);                   
                }

                return data;
            }
        }

        /// <summary>
        /// Get Prefix
        /// </summary>
        public IEnumerable<string> Prefix
        {
            get
            {
                List<string> data = new List<string>();

                if (this.InternalRevitElement.NumberOfSegments == 0)
                    data.Add(this.InternalRevitElement.Prefix);

                else if (this.InternalRevitElement.NumberOfSegments > 1)
                {
                    foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                        data.Add(segment.Prefix);
                }

                return data;
            }
        }

        /// <summary>
        /// Set Prefix
        /// </summary>
        /// <param name="values">Prefix</param>
        public void SetPrefix(IEnumerable<string> values)
        {
            var value = values.ToList();

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            if (this.InternalRevitElement.NumberOfSegments == 0)
                this.InternalRevitElement.Prefix = value[0];

            else if (this.InternalRevitElement.NumberOfSegments > 1)
            {
                int i = 0;
                foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                {
                    segment.Prefix = value[i];
                    if (value.Count - 1 > i) i++;
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get Suffix
        /// </summary>
        public IEnumerable<string> Suffix
        {
            get {
                List<string> data = new List<string>();

                if (this.InternalRevitElement.NumberOfSegments == 0)
                    data.Add(this.InternalRevitElement.Suffix);

                else if (this.InternalRevitElement.NumberOfSegments > 1)
                {
                    foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                        data.Add(segment.Suffix);
                }

                return data;
            }
        }

        /// <summary>
        /// Set Suffix
        /// </summary>
        /// <param name="values">Suffix</param>
        public void SetSuffix(IEnumerable<string> values)
        {
            var value = values.ToList();

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            if (this.InternalRevitElement.NumberOfSegments == 0)
                this.InternalRevitElement.Suffix = value[0];

            else if (this.InternalRevitElement.NumberOfSegments > 1)
            {
                int i = 0;
                foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                {
                    segment.Suffix = value[i];
                    if (value.Count - 1 > i) i++;
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Get Value override
        /// </summary>
        public IEnumerable<string> ValueOverride
        {
            get
            {
                List<string> data = new List<string>();

                if (this.InternalRevitElement.NumberOfSegments == 0)
                    data.Add(this.InternalRevitElement.ValueOverride);

                else if (this.InternalRevitElement.NumberOfSegments > 1)
                {
                    foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                        data.Add(segment.ValueOverride);
                }

                return data;
            }
        }

        /// <summary>
        /// Set Value override
        /// </summary>
        /// <param name="values">Value override</param>
        public void SetValueOverride(IEnumerable<string> values)
        {
            var value = values.ToList();

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            if (this.InternalRevitElement.NumberOfSegments == 0)
                this.InternalRevitElement.ValueOverride = value[0];

            else if (this.InternalRevitElement.NumberOfSegments > 1)
            {
                int i = 0;
                foreach (DimensionSegment segment in this.InternalRevitElement.Segments)
                {
                    segment.ValueOverride = value[i];
                    if (value.Count - 1 > i) i++;
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// The text shown above the segment's value, if the dimension has multiple segments a list of values will be returned.
        /// </summary>
        public List<string> AboveValue
        {
            get
            {
                if (this.InternalRevitElement.Segments.Size == 0)
                {
                    return new List<string>() { this.InternalRevitElement.Above };
                }

                return this.InternalRevitElement.Segments
                    .Cast<DimensionSegment>()
                    .Select(segment => segment.Above)
                    .ToList();
            } 
        }

        /// <summary>
        /// The text shown below the segment's value.
        /// </summary>
        public List<string> BelowValue
        {
            get
            {
                if (this.InternalRevitElement.Segments.Size == 0)
                {
                    return new List<string>() { this.InternalRevitElement.Below };
                }

                return this.InternalRevitElement.Segments
                    .Cast<DimensionSegment>()
                    .Select(segment => segment.Below)
                    .ToList();
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the text shown above the segment's value.
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>The dimension element</returns>
        public Element SetAboveValue(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            if (this.InternalRevitElement.Segments.Size == 0)
            {
                this.InternalRevitElement.Above = value;
            }
            else
            {
                this.InternalRevitElement.Segments
                    .Cast<DimensionSegment>()
                    .Select(segment => segment.Above = value)
                    .ToList();
            }
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Sets the text shown below the segment's value.
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>The dimension element</returns>
        public Element SetBelowValue(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            if (this.InternalRevitElement.Segments.Size == 0)
            {
                this.InternalRevitElement.Below = value;
            }
            else
            {
                this.InternalRevitElement.Segments
                    .Cast<DimensionSegment>()
                    .Select(segment => segment.Below = value)
                    .ToList();
            }
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// From existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Dimension FromExisting(Autodesk.Revit.DB.Dimension instance, bool isRevitOwned)
        {
            return new Dimension(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Helpers
        
        /// <summary>
        /// GetMidpoint from bounding box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private static Autodesk.Revit.DB.XYZ GetMidpoint(Autodesk.Revit.DB.BoundingBoxXYZ box)
        {
            return box.Min + ((box.Max - box.Min) / 2);
        }

        #endregion

    }



}
