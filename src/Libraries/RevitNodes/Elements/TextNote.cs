using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;
using RVT = Autodesk.Revit.DB;

namespace Revit.Elements
{
    /// <summary>
    /// Revit Text Note Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class TextNote : TextElement
    {

        #region Private mutators

        /// <summary>
        /// TextNote
        /// </summary>
        private Autodesk.Revit.DB.TextNote InternalTextNote
        {
            get { return (Autodesk.Revit.DB.TextNote)this.InternalRevitElement; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// TextNote from existing
        /// </summary>
        /// <param name="TextNote"></param>
        internal TextNote(Autodesk.Revit.DB.TextNote TextNote)
        {
            SafeInit(() => InitElement(TextNote));
        }

        /// <summary>
        /// TextNote by Location
        /// </summary>
        /// <param name="view"></param>
        /// <param name="origin"></param>
        /// <param name="alignment"></param>
        /// <param name="text"></param>
        /// <param name="keepRotatedTextreadable"></param>
        /// <param name="rotation">in degrees</param>
        /// <param name="typeId"></param>
        private TextNote(RVT.View view, RVT.XYZ origin, RVT.HorizontalTextAlignment alignment, string text, bool keepRotatedTextreadable, double rotation, ElementId typeId)
        {
            SafeInit(() => Init(view, origin, alignment, text, keepRotatedTextreadable, rotation, typeId));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Set Text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="location"></param>
        /// <param name="alignment"></param>
        /// <param name="angle"></param>
        private void InternalSetType(string value, RVT.TextNoteType type, XYZ location, HorizontalTextAlignment alignment, double angle)
        {
            int bold = type.LookupParameter("Bold").AsInteger();
            int italic = type.LookupParameter("Italic").AsInteger();
            string font = type.LookupParameter("Text Font").AsString();
            double size = type.LookupParameter("Text Size").AsDouble();
            bool isBold = (bold == 1) ? true : false;
            bool isItalic = (italic == 1) ? true : false;

            InternalSetTextSettings(value, isBold, isItalic, size, font, location.ToPoint(true), alignment, angle);
        }

        /// <summary>
        /// Init element from existing
        /// </summary>
        /// <param name="element"></param>
        private void InitElement(Autodesk.Revit.DB.TextNote element)
        {
            // use the default coordinates for an unrotated text
            XYZ position = element.Coord;
            double rotation = 0;

            // If an element location is present, use this location instead because it
            // holds the elements rotation
            if (element.Location != null && element.Location is LocationPoint)
            {
                LocationPoint location = element.Location as LocationPoint;
                position = location.Point;
                rotation = location.Rotation.ToDegrees();
            }

            InternalSetType(element.Text, element.TextNoteType, position, element.HorizontalAlignment, rotation);
            InternalSetElement(element);
        }

        /// <summary>
        /// Init element by location
        /// </summary>
        /// <param name="view"></param>
        /// <param name="origin"></param>
        /// <param name="alignment"></param>
        /// <param name="text"></param>
        /// <param name="keepRotatedTextreadable"></param>
        /// <param name="rotation">in degrees</param>
        /// <param name="typeId"></param>
        private void Init(RVT.View view, RVT.XYZ origin, RVT.HorizontalTextAlignment alignment, string text, bool keepRotatedTextreadable, double rotation, ElementId typeId)
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            var element = ElementBinder.GetElementFromTrace<RVT.TextNote>(document);

            RVT.TextNoteOptions options = new TextNoteOptions()
            {
                HorizontalAlignment = alignment,
                KeepRotatedTextReadable = keepRotatedTextreadable,
                Rotation = rotation.ToRadians(),
                TypeId = typeId
            };

            if (element == null)
            {
                element = RVT.TextNote.Create(document, view.Id, origin, text, options);
            }
            else
            {
                if (element.HorizontalAlignment != alignment)
                {
                    element.HorizontalAlignment = alignment;
                }

                if (element.KeepRotatedTextReadable != keepRotatedTextreadable)
                {
                    element.KeepRotatedTextReadable = keepRotatedTextreadable;
                }

                if (element.Text != text)
                {
                    element.Text = text;
                }

                // if the element location is null, try to use the Coord property
                // Coord holds a point only without rotation.
                if (element.Location == null)
                {
                    if (!element.Coord.IsAlmostEqualTo(origin))
                    {
                        element.Coord = origin;
                    }
                }
                else
                {
                    // If a location point is set we can use this and extract
                    // its location and rotation
                    if (element.Location is LocationPoint)
                    {
                        LocationPoint point = (LocationPoint)element.Location;

                        if (!point.Point.IsAlmostEqualTo(origin))
                        {
                            point.Point = origin;
                        }

                        if (Math.Abs(point.Rotation - rotation.ToRadians()) <= System.Double.Epsilon)
                        {
                            point.Rotate(Line.CreateUnbound(XYZ.Zero, XYZ.BasisZ), rotation.ToRadians());
                        }
                    }
                }

                
            }

            RVT.TextNoteType type = (RVT.TextNoteType)document.GetElement(typeId);
            InternalSetType(text, type, origin, alignment, rotation);
            InternalSetElement(element);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);

        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Construct a new Revit TextNote by Location
        /// </summary>
        /// <param name="view">View to place text element on</param>
        /// <param name="location">Location in view</param>
        /// <param name="text">Text</param>
        /// <param name="alignment">Text alignment</param>
        /// <param name="keepRotatedTextReadable">Keep text horizontal</param>
        /// <param name="rotation">Rotation in degrees</param>
        /// <param name="type">Revit TextNote Type</param>
        /// <returns></returns>
        public static TextNote ByLocation(Revit.Elements.Views.View view, Autodesk.DesignScript.Geometry.Point location, string text, string alignment, [DefaultArgument("Revit.Elements.TextNoteType.Default()")]TextNoteType type, bool keepRotatedTextReadable = true, double rotation = 0)
        {
            RVT.HorizontalTextAlignment horizontalTextAlignmentStyle = HorizontalTextAlignment.Center;
            if (!Enum.TryParse<RVT.HorizontalTextAlignment>(alignment, out horizontalTextAlignmentStyle))
            {
                horizontalTextAlignmentStyle = HorizontalTextAlignment.Center;
            }

            Autodesk.Revit.DB.View revitView = (Autodesk.Revit.DB.View)view.InternalElement;

            if (!view.IsAnnotationView())
                throw new Exception(Properties.Resources.ViewDoesNotSupportAnnotations);

            return new TextNote(revitView, location.ToRevitType(true), horizontalTextAlignmentStyle, text, keepRotatedTextReadable, rotation, type.InternalElement.Id);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Get Text
        /// </summary>
        public string Text
        {
            get { return this.InternalTextNote.Text; }
        }

        /// <summary>
        /// Get geometrical curves from the text this TextNote contains.
        /// </summary>
        public new Autodesk.DesignScript.Geometry.Curve[] Curves
        {
            get { return base.Curves(); }
        }

        /// <summary>
        /// Set Text
        /// </summary>
        /// <param name="value"></param>
        public void SetText(string value)
        {
            if (value != Text)
            {
                TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                this.InternalTextNote.Text = value;
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Set Keep Rotated Text Readable
        /// </summary>
        /// <param name="value"></param>
        public void SetKeepRotatedTextReadable(bool value)
        {
            if (value != this.InternalTextNote.KeepRotatedTextReadable)
            {
                TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                this.InternalTextNote.KeepRotatedTextReadable = value;
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Set Horizontal Text Alignment
        /// </summary>
        /// <param name="value"></param>
        public void SetHorizontalAlignment(string value)
        {
            if (value != HorizontalAlignment)
            {
                RVT.HorizontalTextAlignment alignment = HorizontalTextAlignment.Left;
                Enum.TryParse<RVT.HorizontalTextAlignment>(value, out alignment);
                TransactionManager.Instance.EnsureInTransaction(Application.Document.Current.InternalDocument);
                this.InternalTextNote.HorizontalAlignment = alignment;
                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Get Horizontal Alignment
        /// </summary>
        public string HorizontalAlignment
        {
            get { return this.InternalTextNote.HorizontalAlignment.ToString(); }
        }

        /// <summary>
        /// Get Vertical Alignment
        /// </summary>
        public string VerticalAlignment
        {
            get { return this.InternalTextNote.VerticalAlignment.ToString(); }
        }


        /// <summary>
        /// Get Height
        /// </summary>
        public double Height
        {
            get { return this.InternalTextNote.Height; }
        }

        /// <summary>
        /// Get Typename
        /// </summary>
        public string Typename
        {
            get { return this.InternalTextNote.TextNoteType.Name; }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create from existing
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static TextNote FromExisting(Autodesk.Revit.DB.TextNote instance, bool isRevitOwned)
        {
            return new TextNote(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }

}
