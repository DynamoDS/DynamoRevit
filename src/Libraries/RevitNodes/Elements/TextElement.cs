using System;
using System.Reflection;
using Autodesk.DesignScript.Interfaces;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using Autodesk.DesignScript.Runtime;
using DS = Autodesk.DesignScript.Geometry;
using System.Windows.Media;
using System.Windows;

namespace Revit.Elements
{
    public abstract class TextElement : Element, IGraphicItem
    {
        private const byte DefR = 0;
        private const byte DefG = 0;
        private const byte DefB = 0;
        private const byte DefA = 255;    
  
        /// <summary>
        /// Text Value
        /// </summary>
        private string Value;

        /// <summary>
        /// FontFamily Name
        /// </summary>
        private string FontFamilyName;

        /// <summary>
        /// Is Bold
        /// </summary>
        private bool Bold;

        /// <summary>
        /// Is Italic
        /// </summary>
        private bool Italic;

        /// <summary>
        /// Fontsize in ft
        /// </summary>
        private double FontSize;

        /// <summary>
        /// Location
        /// </summary>
        private DS.Point Location;

        /// <summary>
        /// Constant Scale for Font Size
        /// </summary>
        const double Scale = 100;

        /// <summary>
        /// Horizontal alignment
        /// </summary>
        private HorizontalTextAlignment Alignment;

        /// <summary>
        /// Rotation
        /// </summary>
        private double Rotation = 0;

        /// <summary>
        /// Internal Revit Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRevitElement; }
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        internal Autodesk.Revit.DB.Element InternalRevitElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Set Internal Element
        /// </summary>
        /// <param name="element"></param>
        internal void InternalSetElement(Autodesk.Revit.DB.Element element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        /// <summary>
        /// Set Internal Text Display Settings
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <param name="size"></param>
        /// <param name="fontFamilyName"></param>
        /// <param name="location"></param>
        /// <param name="alignment"></param>
        /// <param name="rotation"></param>
        internal void InternalSetTextSettings(string value, bool bold, bool italic, double size, string fontFamilyName, DS.Point location, HorizontalTextAlignment alignment, double rotation)
        {
            this.Value = value;
            this.Bold = bold;
            this.Italic = italic;
            this.FontSize = size;
            this.FontFamilyName = fontFamilyName;
            this.Location = location;
            this.Alignment = alignment;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Create the outline geometry based on the formatted text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <param name="family"></param>
        /// <param name="size"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private PathGeometry CreateText(string text, bool bold, bool italic, FontFamily family, double size, System.Windows.Point location)
        {
            System.Windows.FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;

            if (bold == true) fontWeight = FontWeights.Bold;
            if (italic == true) fontStyle = FontStyles.Italic;

            FormattedText formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(family, fontStyle,fontWeight,FontStretches.Normal),
                size, System.Windows.Media.Brushes.Black 
                );

            Geometry textGeometry = formattedText.BuildGeometry(location);
            return textGeometry.GetFlattenedPathGeometry();
        }

        /// <summary>
        /// Text Tesselation
        /// </summary>
        /// <param name="package"></param>
        /// <param name="parameters"></param>
        [IsVisibleInDynamoLibrary(false)]
        public new void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {
            // Offset for text adjustments
            double horizontalOffset = 0;

            // Get Conversion factor according to the documents units
            double factor = Revit.GeometryConversion.UnitConverter.DynamoToHostFactor(UnitType.UT_Length);

            // Get Text outline path at 0,0
            PathGeometry path = CreateText(this.Value, this.Bold, this.Italic, new FontFamily(this.FontFamilyName), this.FontSize * Scale / factor, new System.Windows.Point(0, 0));

            // Apply horizontal Text offset            
            if (Alignment == HorizontalTextAlignment.Center) { horizontalOffset = path.Bounds.Width / 2; }
            if (Alignment == HorizontalTextAlignment.Right) { horizontalOffset = path.Bounds.Width; }
            
            // Walk thorugh all figures and draw lines
            foreach (PathFigure figure in path.Figures)
            {
                // Rotate the start point and apply offsets
                var startPoint = RotatePoint(new System.Windows.Point(figure.StartPoint.X - horizontalOffset, figure.StartPoint.Y * -1), this.Rotation);
                package.AddLineStripVertex(startPoint.X + Location.X , startPoint.Y + Location.Y, this.Location.Z);

                foreach (PathSegment segment in figure.Segments)
                {
                    if (segment is System.Windows.Media.PolyLineSegment)
                    {
                        System.Windows.Media.PolyLineSegment pline = segment as System.Windows.Media.PolyLineSegment;
                        for (int i = 0; i < pline.Points.Count; i++)
                        {
                            System.Windows.Point point = pline.Points[i];

                            // rotate point and apply offsets
                            var pLinePoint = RotatePoint(new System.Windows.Point(point.X - horizontalOffset, point.Y * -1), this.Rotation);
                            package.AddLineStripVertex(pLinePoint.X + Location.X , pLinePoint.Y + Location.Y, this.Location.Z);

                            // send the same point again to continue drawing
                            if (i < pline.Points.Count - 1)
                            {
                                package.AddLineStripVertex(pLinePoint.X + Location.X, pLinePoint.Y + Location.Y, this.Location.Z);
                            }
                        }
                    }
                }
            }

            if (package.LineVertexCount > 0)
            {
                package.ApplyLineVertexColors(CreateColorByteArrayOfSize(package.LineVertexCount, DefR, DefG, DefB, DefA));
            }
        }

        /// <summary>
        /// Rotate point around 0,0
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Rotated point</returns>
        static System.Windows.Point RotatePoint(System.Windows.Point point, double angle)
        {
            // Rotate around 0,0
            System.Windows.Point centerPoint = new System.Windows.Point(0, 0);
            double radians = angle * (Math.PI / 180);
            double cosTheta = Math.Cos(radians);
            double sinTheta = Math.Sin(radians);
            return new System.Windows.Point
            {
                X = (double)(cosTheta * (point.X - centerPoint.X) - sinTheta * (point.Y - centerPoint.Y) + centerPoint.X),
                Y = (double)(sinTheta * (point.X - centerPoint.X) + cosTheta * (point.Y - centerPoint.Y) + centerPoint.Y)
            };
        }


       private static byte[] CreateColorByteArrayOfSize(int size, byte red, byte green, byte blue, byte alpha)
       {
           var arr = new byte[size * 4];
           for (var i = 0; i < arr.Length; i += 4)
           {
               arr[i] = red;
               arr[i + 1] = green;
               arr[i + 2] = blue;
               arr[i + 3] = alpha;
           }
           return arr;
       }
    }
}
