using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;

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
            SafeInit(() => InitElement(tag));
        }

        /// <summary>
        /// Tag by Element
        /// </summary>
        /// <param name="view"></param>
        /// <param name="host"></param>
        /// <param name="orientation"></param>
        /// <param name="mode"></param>
        /// <param name="addLeader"></param>
        /// <param name="point"></param>
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

            // create a new tag if the existing tag is null or its host view is not the selected one
            // or the host is neither set a host or linked element.
            if (tagElem == null || view.Id != tagElem.OwnerViewId ||
                (tagElem.TaggedElementId.HostElementId != host.Id && tagElem.TaggedElementId.LinkedElementId != host.Id))
            {
                tagElem = document.Create.NewTag(view, host, addLeader, mode, orientation, vector);
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

        #region Public static constructors

        /// <summary>
        /// Create a Revit Tag for a Revit Element
        /// </summary>
        /// <param name="view">View to Tag</param>
        /// <param name="element">Element to tag</param>
        /// <param name="horizontal">Horizontal alignment</param>
        /// <param name="addLeader">Add a leader</param>
        /// <param name="offset">Offset Vector or Tag Location</param>
        /// <param name="isOffset">Specifies if the point is being used as an offset vector or if it specifies the tags location</param>
        /// <param name="horizontalAlignment">Horizontal Alignment</param>
        /// <param name="verticalAlignment">Vertical Alignment</param>
        /// <returns></returns>
        public static Tag ByElement(Revit.Elements.Views.View view, Element element, bool horizontal, bool addLeader, Autodesk.Revit.DB.HorizontalAlignmentStyle horizontalAlignment, Autodesk.Revit.DB.VerticalAlignmentStyle verticalAlignment, [DefaultArgument("Autodesk.DesignScript.Geometry.Vector.ByCoordinates(0,0,0)")]Autodesk.DesignScript.Geometry.Vector offset, bool isOffset = true)
        {   

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;
            Autodesk.Revit.DB.XYZ point = offset.ToRevitType(true);
            Autodesk.Revit.DB.TagMode tagMode = TagMode.TM_ADDBY_CATEGORY;
            Autodesk.Revit.DB.TagOrientation orientation = (horizontal) ? TagOrientation.Horizontal : TagOrientation.Vertical;

            if (!view.IsAnnotationView())
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);

            if (isOffset)
            {
                BoundingBoxXYZ box = element.InternalElement.get_BoundingBox(revitView);
                if (box == null) box = element.InternalElement.get_BoundingBox(null);
                if (box != null)
                {
                    double Y, X = 0;

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

                    point = new XYZ(X + point.X, Y + point.Y, 0 + point.Z);
                }
                else
                {
                    if (element.InternalElement.Location != null)
                    {
                        if (element.InternalElement.Location.GetType() == typeof(LocationCurve))
                        {
                            LocationCurve lc = (LocationCurve)element.InternalElement.Location;
                            XYZ midpoint = lc.Curve.Evaluate(0.5, true);
                            point = new XYZ(midpoint.X + point.X, midpoint.Y + point.Y, 0 + point.Z);
                        }
                        else if (element.InternalElement.Location.GetType() == typeof(LocationPoint))
                        {
                            LocationPoint lp = (LocationPoint)element.InternalElement.Location;
                            point = new XYZ(lp.Point.X + point.X, lp.Point.Y + point.Y, 0 + point.Z);
                        }
                        else
                            throw new Exception(Properties.Resources.InvalidElementLocation);
                    }
                    else
                        throw new Exception(Properties.Resources.InvalidElementLocation);
                }
            }



            return new Tag(revitView, element.InternalElement, orientation, tagMode, addLeader, point);
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
                return (Revit.Elements.Element)ElementWrapper.Wrap(this.InternalTextNote.GetTaggedLocalElement(), true);                
            }
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
