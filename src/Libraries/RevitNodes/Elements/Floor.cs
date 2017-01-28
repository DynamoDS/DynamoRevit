using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.DesignScript.Geometry.Curve;
using Pt = Autodesk.DesignScript.Geometry.Point;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit Floor
    /// </summary>
    public class Floor : Element
    {
        #region Internal properties

        /// <summary>
        /// An internal handle on the Revit floor
        /// </summary>
        internal Autodesk.Revit.DB.Floor InternalFloor
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalFloor; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Floor(Autodesk.Revit.DB.Floor floor)
        {
            SafeInit(() => InitFloor(floor));
        }
      
        /// <summary>
        /// Private constructor
        /// </summary>
        private Floor(CurveArray curveArray, Autodesk.Revit.DB.FloorType floorType, Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitFloor(curveArray, floorType, level));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a floor element
        /// </summary>
        private void InitFloor(Autodesk.Revit.DB.Floor floor)
        {
            InternalSetFloor(floor);
        }

        /// <summary>
        /// Initialize a floor element
        /// </summary>
        private void InitFloor(CurveArray curveArray, Autodesk.Revit.DB.FloorType floorType, Autodesk.Revit.DB.Level level)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Autodesk.Revit.DB.Floor floor = null;
            if (floorType.IsFoundationSlab)
            {
                floor = Document.Create.NewFoundationSlab(curveArray, floorType, level, false, XYZ.BasisZ);
            }
            else
            {
                // we assume the floor is not structural here, this may be a bad assumption
                floor = Document.Create.NewFloor(curveArray, floorType, level, false);
            }

            InternalSetFloor(floor);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalFloor);
        }

        #endregion

        #region Private mutators


        /// <summary>
        /// Set the InternalFloor property and the associated element id and unique id
        /// </summary>
        /// <param name="floor"></param>
        private void InternalSetFloor(Autodesk.Revit.DB.Floor floor)
        {
            InternalFloor = floor;
            InternalElementId = floor.Id;
            InternalUniqueId = floor.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Floor given it's curve outline and Level
        /// </summary>
        /// <param name="outlineCurves"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <returns>The floor</returns>
        public static Floor ByOutlineTypeAndLevel(Curve[] outlineCurves, FloorType floorType, Level level)
        {
            if (outlineCurves == null)
            {
                throw new ArgumentNullException("outlineCurves");
            }

            var floor = ByOutlineTypeAndLevel(PolyCurve.ByJoinedCurves(outlineCurves), floorType, level);
            DocumentManager.Regenerate();
            return floor;
        }

        /// <summary>
        /// Create a Revit Floor given it's curve outline and Level
        /// </summary>
        /// <param name="outline"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <returns>The floor</returns>
        public static Floor ByOutlineTypeAndLevel(PolyCurve outline, FloorType floorType, Level level)
        {
            if (outline == null)
            {
                throw new ArgumentNullException("outline");
            }

            if (floorType == null)
            {
                throw new ArgumentNullException("floorType");
            }

            if ( level == null )
            {
                throw new ArgumentNullException("level");
            }

            if (!outline.IsClosed)
            {
                throw new ArgumentException(Properties.Resources.OpenInputPolyCurveError);
            }

            var ca = new CurveArray();
            outline.Curves().ForEach(x => ca.Append(x.ToRevitType())); 

            var floor = new Floor(ca, floorType.InternalFloorType, level.InternalLevel);
            DocumentManager.Regenerate();
            return floor;
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Get Slab Shape Points
        /// </summary>
        public IEnumerable<Pt> Points
        {
            get 
            {
                if (this.InternalFloor.SlabShapeEditor == null)
                {
                    throw new Exception(Properties.Resources.InvalidShapeEditor);
                }

                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                this.InternalFloor.SlabShapeEditor.Enable();
                TransactionManager.Instance.TransactionTaskDone();

                List<Pt> points = new List<Pt>();              
                foreach (SlabShapeVertex v in this.InternalFloor.SlabShapeEditor.SlabShapeVertices)
                {
                    points.Add(v.Position.ToPoint());
                }
                return points;
            }
        }

        /// <summary>
        /// Add Point to Slab Shape
        /// </summary>
        public Floor AddPoint(Pt point)
        {
            if (this.InternalFloor.SlabShapeEditor == null)
            {
                throw new Exception(Properties.Resources.InvalidShapeEditor);
            }

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalFloor.SlabShapeEditor.Enable();
            this.InternalFloor.SlabShapeEditor.DrawPoint(point.ToXyz());
            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Move an existing point in the slab shape editor by an offset.
        /// Behaves as moving a point manually in the slab shape editor.
        /// </summary>
        public Floor MovePoint(Pt point, double offset)
        {
            if (this.InternalFloor.SlabShapeEditor == null)
            {
                throw new Exception(Properties.Resources.InvalidShapeEditor);
            }

            SlabShapeVertex vertex = null;

            foreach (SlabShapeVertex v in this.InternalFloor.SlabShapeEditor.SlabShapeVertices)
            {
                if (point.IsAlmostEqualTo(v.Position.ToPoint()))
                {
                    vertex = v;
                }
            }

            if (vertex != null && offset != 0)
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                this.InternalFloor.SlabShapeEditor.Enable();
                this.InternalFloor.SlabShapeEditor.ModifySubElement(vertex, offset);
                TransactionManager.Instance.TransactionTaskDone();
            }

            return this;
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Floor from a user selected Element.
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Floor FromExisting(Autodesk.Revit.DB.Floor floor, bool isRevitOwned)
        {
            return new Floor(floor)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
