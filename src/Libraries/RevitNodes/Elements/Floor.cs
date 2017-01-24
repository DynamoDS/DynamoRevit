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

            return ByOutlineTypeAndLevel(PolyCurve.ByJoinedCurves(outlineCurves), floorType, level);
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

            return new Floor(ca, floorType.InternalFloorType, level.InternalLevel );
        }

        /// <summary>
        /// Create Revit Floor by Points, Type and Level
        /// </summary>
        /// <param name="points"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Floor ByPoints(IList<Pt> points, FloorType floorType, Level level)
        { 
            CurveArray array = new CurveArray();

            for (int i = 0; i < points.Count(); i++)
            {
                XYZ p1 = points[i].ToXyz();
                XYZ p2;

                if (i < points.Count - 1)
                {
                    p2 = points[i + 1].ToXyz();
                }
                else
                {
                    p2 = points[0].ToXyz();
                }


                XYZ flatPoint1 = new XYZ(p1.X, p1.Y, 0);
                XYZ flatPoint2 = new XYZ(p2.X, p2.Y, 0);

                if (!p1.IsAlmostEqualTo(p2))
                {
                    array.Append(Autodesk.Revit.DB.Line.CreateBound(flatPoint1, flatPoint2));
                }
            }

            Floor floor = new Floor(array, floorType.InternalFloorType, level.InternalLevel);
            TransactionManager.Instance.ForceCloseTransaction();

            if (floor.InternalFloor.SlabShapeEditor != null)
            {

                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

                floor.InternalFloor.SlabShapeEditor.Enable();

                if (floor.InternalFloor.SlabShapeEditor.SlabShapeVertices.Size == 0)
                {
                    foreach (Pt point in points)
                    {
                        floor.InternalFloor.SlabShapeEditor.DrawPoint(point.ToXyz());
                    }
                }
                else
                {
                    foreach (Pt point in points)
                    {
                        foreach (SlabShapeVertex v in floor.InternalFloor.SlabShapeEditor.SlabShapeVertices)
                        {
                            XYZ ptz = point.ToXyz();
                            if (v.Position.X.EqualsTo(ptz.X) && v.Position.Y.EqualsTo(ptz.Y))
                            {
                                floor.InternalFloor.SlabShapeEditor.ModifySubElement(v, ptz.Z);
                            }
                        }
                    }                    
                }
                TransactionManager.Instance.TransactionTaskDone();


                
            }
                return floor;
            
        }


        /// <summary>
        /// Create Floor By Surface, Type and Level
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="floorType"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Floor BySurface(Autodesk.DesignScript.Geometry.Surface surface, FloorType floorType, Level level)
        {
            List<Pt> points = new List<Pt>();

            var curves = surface.PerimeterCurves();
            

            foreach (Curve curve in curves)
            {
                foreach (XYZ p in curve.ToRevitType(false).Tessellate())
                {
                    Pt point = p.ToPoint();
                    if (!points.Contains(point))
                        points.Add(point);
                }
            }


            return Floor.ByPoints(points, floorType, level);
        }


        public static Floor BySurface2(Autodesk.DesignScript.Geometry.Surface surface, FloorType floorType, Level level, bool f)
        {
            var curves = surface.PerimeterCurves();

            var ca = new CurveArray();
            if (f)
                curves.ForEach(x => ca.Append(x.PullOntoPlane(Autodesk.DesignScript.Geometry.Plane.XY()).ToRevitType()));
            else
                curves.ForEach(x => ca.Append(x.ToRevitType()));

            Floor floor = new Floor(ca, floorType.InternalFloorType, level.InternalLevel);
            TransactionManager.Instance.ForceCloseTransaction();
            

            

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            floor.InternalFloor.SlabShapeEditor.Enable();

            foreach (Curve curve in curves)
            {
                foreach (XYZ p in curve.ToRevitType(true).Tessellate())
                {
                    floor.InternalFloor.SlabShapeEditor.DrawPoint(p);
                }
            }

            TransactionManager.Instance.TransactionTaskDone();
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
        public void AddPoint(Pt point)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalFloor.SlabShapeEditor.Enable();
            this.InternalFloor.SlabShapeEditor.DrawPoint(point.ToXyz());
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Move existing point by offset
        /// </summary>
        public void MovePoint(Pt point, double offset)
        {
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
