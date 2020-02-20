using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Elements
{
    public class Area : Element
    {
        #region Internal properties

        internal Autodesk.Revit.DB.Area InternalArea
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalArea; }
        }

        #endregion

        #region Private constructors

        private Area(Autodesk.Revit.DB.Area area)
        {
            SafeInit(() => InitArea(area));
        }

        #endregion

        #region Helper for private constructors

        private void InitArea(Autodesk.Revit.DB.Area area)
        {
            InternalSetArea(area);
        }

        #endregion

        #region private mutators

        /// <summary>
        /// Set the internal Revit representation and update the ElementId and UniqueId
        /// </summary>
        /// <param name="area"></param>
        private void InternalSetArea(Autodesk.Revit.DB.Area area)
        {
            this.InternalArea = area;
            this.InternalElementId = area.Id;
            this.InternalUniqueId = area.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Returns the boundary segments of the area.
        /// </summary>
        public List<List<Autodesk.DesignScript.Geometry.Curve>> Boundaries
        {
            get
            {
                var area = this.InternalArea;
                var boundaryCurves = new List<List<Autodesk.DesignScript.Geometry.Curve>>();
                var boundarySegments = area.GetBoundarySegments(new SpatialElementBoundaryOptions());
                for (int i = 0; i < boundarySegments.Count; i++)
                {
                    var segment = boundarySegments[i];
                    boundaryCurves.Add(segment.Select(seg => RevitToProtoCurve.ToProtoType(seg.GetCurve())).ToList());
                }
                return boundaryCurves;
            }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Construct an Element from an existing Element in the Document
        /// </summary>
        /// <param name="area"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Area FromExisting(Autodesk.Revit.DB.Area area, bool isRevitOwned)
        {
            return new Area(area)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
