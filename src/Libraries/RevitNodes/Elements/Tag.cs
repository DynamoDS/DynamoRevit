using System;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace Revit.Elements
{
    /// <summary>
    /// Revit Tag Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Tag : TextElement
    {

        #region Private mutators

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        private Autodesk.Revit.DB.IndependentTag InternalTextNote
        {
            get { return (Autodesk.Revit.DB.IndependentTag)this.InternalRevitElement; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Tag
        /// </summary>
        /// <param name="tag"></param>
        private Tag(Autodesk.Revit.DB.IndependentTag tag)
        {
            SafeInit(() => InitElement(tag), true);
        }

        /// <summary>
        /// Tag by Element
        /// </summary>
        /// <param name="view"></param>
        /// <param name="host"></param>
        /// <param name="orientation"></param>
        /// <param name="mode"></param>
        /// <param name="addLeader"></param>
        /// <param name="vector"></param>
        private Tag(
            Autodesk.Revit.DB.View view,
            Autodesk.Revit.DB.Element host,
            Autodesk.Revit.DB.TagOrientation orientation,
            Autodesk.Revit.DB.TagMode mode,
            bool addLeader,
            Autodesk.Revit.DB.XYZ vector)
        {
            SafeInit(() => Init(view, host, orientation, mode, addLeader, vector));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Set Text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <param name="angle"></param>
        private void InternalSetType(string value, XYZ location, double angle)
        {
            string font = "Arial";
            double size = 0.0104;
            InternalSetTextSettings(value, false, false, size, font, location.ToPoint(true), HorizontalTextAlignment.Center, angle);
        }

        /// <summary>
        /// Init element from existing
        /// </summary>
        /// <param name="element"></param>
        private void InitElement(Autodesk.Revit.DB.IndependentTag element)
        {
            double rotation = (element.TagOrientation == TagOrientation.Horizontal) ? 0 : 90;
            InternalSetType(element.TagText, element.TagHeadPosition, rotation);
            InternalSetElement(element);
        }

        /// <summary>
        /// Init Tag element
        /// </summary>
        /// <param name="view"></param>
        /// <param name="host"></param>
        /// <param name="orientation"></param>
        /// <param name="mode"></param>
        /// <param name="addLeader"></param>
        /// <param name="vector"></param>
        private void Init(
            Autodesk.Revit.DB.View view,
            Autodesk.Revit.DB.Element host,
            Autodesk.Revit.DB.TagOrientation orientation,
            Autodesk.Revit.DB.TagMode mode,
            bool addLeader,
            Autodesk.Revit.DB.XYZ vector)
        {

            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            var tagElem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.IndependentTag>(document);

            var hostElementIds = new List<ElementId>();
            var linkedElementIds = new List<ElementId>();

            if(tagElem != null)
            {
                hostElementIds.AddRange(tagElem.GetTaggedElementIds().Select(x => x.HostElementId));
                linkedElementIds.AddRange(tagElem.GetTaggedElementIds().Select(x => x.LinkedElementId));
            }

            // create a new tag if the existing tag is null or its host view is not the selected one
            // or the host is neither set a host or linked element.
            if (tagElem == null || view.Id != tagElem.OwnerViewId ||
                (!hostElementIds.Contains(host.Id) && !linkedElementIds.Contains(host.Id)))
            {
                tagElem = IndependentTag.Create(Document, view.Id, new Autodesk.Revit.DB.Reference(host), addLeader, mode, orientation, vector);
            }
            else
            {
                // apply properties
                if (tagElem.TagOrientation != orientation)
                {
                    tagElem.TagOrientation = orientation;
                }

                if (tagElem.HasLeader != addLeader)
                {
                    tagElem.HasLeader = addLeader;
                }

                if (!tagElem.TagHeadPosition.Equals(vector))
                {
                    tagElem.TagHeadPosition = vector;
                }
            }

            double rotation = (orientation == TagOrientation.Horizontal) ? 0 : 90;
            InternalSetType(tagElem.TagText, tagElem.TagHeadPosition, rotation);
            InternalSetElement(tagElem);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Helpers for public static constructors

        /// <summary>
        /// Gets element location from locationPoint or Curve
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static XYZ GetLocationPoint(Element element)
        {
            if (element.InternalElement.Location != null)
            {
                if (element.InternalElement.Location.GetType() == typeof(LocationCurve))
                {
                    LocationCurve lc = (LocationCurve)element.InternalElement.Location;
                    XYZ midpoint = lc.Curve.Evaluate(0.5, true);
                    return new XYZ(midpoint.X, midpoint.Y, 0);
                }
                else if (element.InternalElement.Location.GetType() == typeof(LocationPoint))
                {
                    LocationPoint lp = (LocationPoint)element.InternalElement.Location;
                    return new XYZ(lp.Point.X, lp.Point.Y, 0);
                }
                else
                    throw new Exception(Properties.Resources.InvalidElementLocation);
            }
            else
                throw new Exception(Properties.Resources.InvalidElementLocation);
        }

        /// <summary>
        /// Get extents of an element by view
        /// </summary>
        /// <param name="element"></param>
        /// <param name="view"></param>
        private static BoundingBoxXYZ GetElementExtentsByView(Element element, View view)
        {
            var box = element.InternalElement.get_BoundingBox(view);
            if (box == null) box = element.InternalElement.get_BoundingBox(null);
            return box;
        }

        /// <summary>
        /// Get element extents by view and apply offset vector
        /// </summary>
        /// <param name="element"></param>
        /// <param name="view"></param>
        /// <param name="offset"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        /// <returns></returns>
        private static XYZ GetExtentsWithOffset(Element element, View view, XYZ offset, VerticalAlignmentStyle verticalAlignment, HorizontalAlignmentStyle horizontalAlignment)
        {
            var box = GetElementExtentsByView(element, view);
            if (box != null)
            {
                double X, Y, Z = 0;

                switch (verticalAlignment)
                {
                    case VerticalAlignmentStyle.Bottom: Y = box.Min.Y; break;
                    case VerticalAlignmentStyle.Top: Y = box.Max.Y; break;
                    default: Y = box.Min.Y + ((box.Max.Y - box.Min.Y) / 2); break;
                }

                switch (horizontalAlignment)
                {
                    case HorizontalAlignmentStyle.Left: X = box.Min.X; break;
                    case HorizontalAlignmentStyle.Right: X = box.Max.X; break;
                    default: X = box.Min.X + ((box.Max.X - box.Min.X) / 2); break;
                }

                return new XYZ(X + offset.X, Y + offset.Y, Z + offset.Z);
            }
            else
            {
                XYZ location = GetLocationPoint(element);
                return new XYZ(location.X + offset.X, location.Y + offset.Y, location.Z + offset.Z);
            }
        }


        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Tag for a Revit Element
        /// </summary>
        /// <param name="view">View to Tag in</param>
        /// <param name="element">Element to tag</param>
        /// <param name="horizontal">Place tag horizontal</param>
        /// <param name="addLeader">Add a leader</param>
        /// <param name="offset">Optional: Offset Vector or Tag Location, defaults to 0,0,0</param>
        /// <param name="isOffset">Optional: Specifies if the point is being used as an offset vector or if it specifies the tags location, defaults to true</param>
        /// <param name="horizontalAlignment">Horizontal Alignment within the element's extents</param>
        /// <param name="verticalAlignment">Vertical Alignment within the element's extents</param>
        /// <returns></returns>
        /// <search>
        /// tagelement,annotate,documentation
        /// </search>
        public static Tag ByElement(Revit.Elements.Views.View view, Element element, bool horizontal, bool addLeader, string horizontalAlignment, string verticalAlignment, [DefaultArgument("Autodesk.DesignScript.Geometry.Vector.ByCoordinates(0,0,0)")]Autodesk.DesignScript.Geometry.Vector offset, bool isOffset = true)
        {
            Autodesk.Revit.DB.HorizontalAlignmentStyle horizontalAlignmentStyle = HorizontalAlignmentStyle.Center;
            if (!Enum.TryParse<Autodesk.Revit.DB.HorizontalAlignmentStyle>(horizontalAlignment, out horizontalAlignmentStyle))
            {
                horizontalAlignmentStyle = HorizontalAlignmentStyle.Center;
            }

            Autodesk.Revit.DB.VerticalAlignmentStyle verticalAlignmentStyle = VerticalAlignmentStyle.Middle;
            if (!Enum.TryParse<Autodesk.Revit.DB.VerticalAlignmentStyle>(verticalAlignment, out verticalAlignmentStyle))
            {
                verticalAlignmentStyle = VerticalAlignmentStyle.Middle;
            }

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;
            Autodesk.Revit.DB.TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            Autodesk.Revit.DB.TagOrientation orientation = (horizontal) ? TagOrientation.Horizontal : TagOrientation.Vertical;

            if (!view.IsAnnotationView())
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);

            XYZ location = GetExtentsWithOffset(element, revitView, offset.ToRevitType(true), verticalAlignmentStyle, horizontalAlignmentStyle);

            return new Tag(revitView, element.InternalElement, orientation, tagMode, addLeader, location);
        }

        /// <summary>
        /// Create a Revit Tag for a Revit Element at a specified location point
        /// </summary>
        /// <param name="view">View to tag in</param>
        /// <param name="element">Element to tag</param>
        /// <param name="location">Location point</param>
        /// <param name="horizontal">Optional: Place tag horizontal, defaults to true</param>
        /// <param name="addLeader">Optional: Add a leader, defaults to false</param>
        /// <returns></returns>
        /// <search>
        /// tagelement,annotate,documentation,taglocation
        /// </search>
        public static Tag ByElementAndLocation(Revit.Elements.Views.View view, Element element, Autodesk.DesignScript.Geometry.Point location, bool horizontal = true, bool addLeader = false)
        {
            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;
            Autodesk.Revit.DB.XYZ point = location.ToRevitType(true);
            Autodesk.Revit.DB.TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            Autodesk.Revit.DB.TagOrientation orientation = (horizontal) ? TagOrientation.Horizontal : TagOrientation.Vertical;

            if (!view.IsAnnotationView())
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);

            return new Tag(revitView, element.InternalElement, orientation, tagMode, addLeader, point);
        }

        /// <summary>
        /// Create a Revit Tag for a Revit Element at an offset location 
        /// from the element's view extents
        /// </summary>
        /// <param name="view">View to tag in</param>
        /// <param name="element">Element to tag</param>
        /// <param name="horizontal">Optional: Place tag horizontal, 
        /// defaults to true</param>
        /// <param name="addLeader">Optional: Add a leader, defaults to false</param>
        /// <param name="offset">Optional: Offset Vector, defaults to 0,0,0</param>
        /// <param name="horizontalAlignment">Optional: Horizontal Alignment 
        /// within the element's extents, defaults to Center</param>
        /// <param name="verticalAlignment">Optional: Vertical Alignment 
        /// within the element's extents, defaults to Middle</param>
        /// <returns></returns>
        /// <search>
        /// tagelement,annotate,documentation,tagoffset,movetag
        /// </search>
        public static Tag ByElementAndOffset(Revit.Elements.Views.View view, Element element, [DefaultArgument("Autodesk.DesignScript.Geometry.Vector.ByCoordinates(0,0,0)")]Autodesk.DesignScript.Geometry.Vector offset, string horizontalAlignment = "Center", string verticalAlignment = "Middle", bool horizontal = true, bool addLeader = false)
        {
            Autodesk.Revit.DB.HorizontalAlignmentStyle horizontalAlignmentStyle = HorizontalAlignmentStyle.Center;
            if (!Enum.TryParse<Autodesk.Revit.DB.HorizontalAlignmentStyle>(horizontalAlignment, out horizontalAlignmentStyle))
            {
                horizontalAlignmentStyle = HorizontalAlignmentStyle.Center;
            }

            Autodesk.Revit.DB.VerticalAlignmentStyle verticalAlignmentStyle = VerticalAlignmentStyle.Middle;
            if (!Enum.TryParse<Autodesk.Revit.DB.VerticalAlignmentStyle>(verticalAlignment, out verticalAlignmentStyle))
            {
                verticalAlignmentStyle = VerticalAlignmentStyle.Middle;
            }

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;
            
            // Tagging elements by element category
            Autodesk.Revit.DB.TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;

            Autodesk.Revit.DB.TagOrientation orientation = (horizontal) ? TagOrientation.Horizontal : TagOrientation.Vertical;

            if (!view.IsAnnotationView())
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);

            XYZ location = GetExtentsWithOffset(element, revitView, offset.ToRevitType(true), verticalAlignmentStyle, horizontalAlignmentStyle);

            return new Tag(revitView, element.InternalElement, orientation, tagMode, addLeader, location);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get Tag's Text
        /// </summary>
        public string TagText
        {
            get
            {
                return this.InternalTextNote.TagText;
            }
        }

        /// <summary>
        /// Get Tagged Element
        /// </summary>
        public Revit.Elements.Element TaggedElement
        {
            get {
                var eles = this.InternalTextNote.GetTaggedLocalElements();
                if (eles.Count == 1)
                    return (Revit.Elements.Element)ElementWrapper.Wrap(eles.First(), true);
                else
                    throw new Exception(Properties.Resources.GetTaggedLocalElements);
            }
        }

        /// <summary>
        /// The position of the head of tag in model coordinates.
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point HeadLocation
        {
            get
            {
                return (this.InternalTextNote.TagHeadPosition is null) ? null : this.InternalTextNote.TagHeadPosition.ToPoint(true);
            }
        }

        /// <summary>
        /// Set the position of the head of tag.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Tag SetHeadLocation(Autodesk.DesignScript.Geometry.Point location)
        {
            Autodesk.Revit.DB.XYZ point = location.ToRevitType(true);
            Autodesk.Revit.DB.IndependentTag tagElem = this.InternalTextNote;
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

            TransactionManager.Instance.EnsureInTransaction(document);

            tagElem.TagHeadPosition = point;
            
            double rotation = (tagElem.TagOrientation == TagOrientation.Horizontal) ? 0 : 90;
            InternalSetType(tagElem.TagText, tagElem.TagHeadPosition, rotation);

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// The position of the elbow of the tag's leader. 
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point LeaderElbow
        {
            get
            {
                if (this.InternalTextNote.HasLeader)
                {
                    var refs = InternalTextNote.GetTaggedReferences();
                    if (refs.Count == 1)
                    {
                        if (this.InternalTextNote.HasLeaderElbow(refs.First()))
                            return this.InternalTextNote.GetLeaderElbow(refs.First()).ToPoint(true);
                    }
                    else
                        throw new Exception(Properties.Resources.GetTaggedReferences);
                }

                return null;
            }
        }

        /// <summary>
        /// Set the position of the elbow of the tag's leader.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Tag SetLeaderElbow(Autodesk.DesignScript.Geometry.Point location)
        {
            Autodesk.Revit.DB.XYZ point = location.ToRevitType(true);
            Autodesk.Revit.DB.IndependentTag tagElem = this.InternalTextNote;
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

            TransactionManager.Instance.EnsureInTransaction(document);

            var refs = InternalTextNote.GetTaggedReferences();
            if (refs.Count == 1)
                tagElem.SetLeaderElbow(refs.First(), point);
            else
                throw new Exception(Properties.Resources.GetTaggedReferences);

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// The position of the leader end for a tag using free end leader behavior. 
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point LeaderEnd
        {
            get
            {
                if (this.InternalTextNote.HasLeader)
                    if(this.InternalTextNote.LeaderEndCondition.Equals(LeaderEndCondition.Free))
                    {
                        var refs = InternalTextNote.GetTaggedReferences();
                        if (refs.Count == 1)
                            return this.InternalTextNote.GetLeaderEnd(refs.First()).ToPoint(true);
                        else
                            throw new Exception(Properties.Resources.GetTaggedReferences);
                    }
                        
                return null;
            }
        }

        /// <summary>
        /// Set the position of the leader end for a tag using free end leader behavior.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Tag SetLeaderEnd(Autodesk.DesignScript.Geometry.Point location)
        {
            Autodesk.Revit.DB.XYZ point = location.ToRevitType(true);
            Autodesk.Revit.DB.IndependentTag tagElem = this.InternalTextNote;
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;

            TransactionManager.Instance.EnsureInTransaction(document);

            var refs = InternalTextNote.GetTaggedReferences();
            if (refs.Count == 1)
                tagElem.SetLeaderEnd(refs.First(), point);
            else
                throw new Exception(Properties.Resources.GetTaggedReferences);

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Get Tag's leaderEnd condition.
        /// </summary>
        public string GetLeaderEndCondition
        {
            get
            {
                if (this.InternalTextNote.HasLeader)
                    return this.InternalTextNote.LeaderEndCondition.ToString();
                return null;
            }
        }

        /// <summary>
        /// Set Tag's LeaderEnd condition.
        /// </summary>
        /// <param name="leaderEndCondition"></param>
        /// <returns></returns>
        public Tag SetLeaderEndCondition(string leaderEndCondition)
        {
            LeaderEndCondition endCondition = (LeaderEndCondition)Enum.Parse(typeof(LeaderEndCondition), leaderEndCondition);
            RevitServices.Transactions.TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
            InternalTextNote.LeaderEndCondition = endCondition;
            RevitServices.Transactions.TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create form existing
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Tag FromExisting(Autodesk.Revit.DB.IndependentTag instance, bool isRevitOwned)
        {
            return new Tag(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }

}
