using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using DBCurve = Autodesk.Revit.DB.Curve;
using DBSketch = Autodesk.Revit.DB.Sketch;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Sketch element. Sketches hold the model curves, reference planes
    /// and dimensions that define the 2-dimensional profile of a sketch-based
    /// element such as a Floor, Roof, or PropertyLine.
    /// </summary>
    public class Sketch : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal handle on the Revit Sketch.
        /// </summary>
        internal DBSketch InternalSketch { get; private set; }

        /// <summary>
        /// Reference to the wrapped element.
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalSketch; }
        }

        #endregion

        #region Private constructors

        private Sketch(DBSketch sketch)
        {
            SafeInit(() => InitSketch(sketch), true);
        }

        #endregion

        #region Helpers for private constructors

        private void InitSketch(DBSketch sketch)
        {
            InternalSetSketch(sketch);
        }

        #endregion

        #region Private mutators

        private void InternalSetSketch(DBSketch sketch)
        {
            InternalSketch = sketch;
            InternalElementId = sketch.Id;
            InternalUniqueId = sketch.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The SketchPlane that this Sketch lies on.
        /// </summary>
        public SketchPlane SketchPlane
        {
            get
            {
                Autodesk.Revit.DB.SketchPlane revitSketchPlane = InternalSketch.SketchPlane;
                return revitSketchPlane == null
                    ? null
                    : Revit.Elements.SketchPlane.FromExisting(revitSketchPlane, true);
            }
        }

        /// <summary>
        /// The element that owns this Sketch (for example the PropertyLine, Floor,
        /// or Roof hosting it). Returns null when the Sketch is not yet attached
        /// to an owning element.
        /// </summary>
        public Element Owner
        {
            get
            {
                ElementId ownerId = InternalSketch.OwnerId;
                if (ownerId == null || ownerId == ElementId.InvalidElementId)
                {
                    return null;
                }

                Autodesk.Revit.DB.Element revitElement = Document.GetElement(ownerId);
                return revitElement?.ToDSType(true) as Element;
            }
        }

        /// <summary>
        /// All elements that belong to this Sketch (typically model curves,
        /// reference planes, and dimensions).
        /// </summary>
        public Element[] Elements
        {
            get
            {
                IList<ElementId> elementIds = InternalSketch.GetAllElements();
                if (elementIds == null || elementIds.Count == 0)
                {
                    return new Element[0];
                }

                return elementIds
                    .Select(id => Document.GetElement(id)?.ToDSType(true) as Element)
                    .Where(e => e != null)
                    .ToArray();
            }
        }

        /// <summary>
        /// The Sketch profile as one PolyCurve per closed loop.
        /// </summary>
        public PolyCurve[] Profile
        {
            get
            {
                var loops = new List<PolyCurve>();
                CurveArrArray curveArrArray = InternalSketch.Profile;
                if (curveArrArray == null)
                {
                    return loops.ToArray();
                }

                foreach (CurveArray curveArray in curveArrArray)
                {
                    if (curveArray == null || curveArray.Size == 0)
                    {
                        continue;
                    }

                    var protoCurves = new List<Curve>(curveArray.Size);
                    foreach (DBCurve revitCurve in curveArray)
                    {
                        Curve protoCurve = revitCurve?.ToProtoType(true);
                        if (protoCurve != null)
                        {
                            protoCurves.Add(protoCurve);
                        }
                    }

                    if (protoCurves.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        loops.Add(PolyCurve.ByJoinedCurves(protoCurves.ToArray(), 0.001, false));
                    }
                    finally
                    {
                        foreach (Curve c in protoCurves)
                        {
                            c.Dispose();
                        }
                    }
                }

                return loops.ToArray();
            }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Wraps a Revit-owned Sketch in the Dynamo Sketch wrapper.
        /// </summary>
        internal static Sketch FromExisting(DBSketch sketch, bool isRevitOwned)
        {
            return new Sketch(sketch)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
