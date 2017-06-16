using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;
using XYZ = Autodesk.Revit.DB.XYZ;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit ReferencePlane
    /// </summary>
    [RegisterForTrace]
    public class ReferencePlane : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal handle for the Revit object
        /// </summary>
        internal Autodesk.Revit.DB.ReferencePlane InternalReferencePlane
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement => InternalReferencePlane;

        #endregion

        #region Private constructors

        /// <summary>
        /// Internal reference plane
        /// </summary>
        /// <param name="referencePlane"></param>
        private ReferencePlane( Autodesk.Revit.DB.ReferencePlane referencePlane)
        {
            SafeInit(() => InitReferencePlane(referencePlane));
        }

        /// <summary>
        /// Constructor used internally by public static constructors
        /// </summary>
        /// <param name="bubbleEnd"></param>
        /// <param name="freeEnd"></param>
        /// <param name="normal"></param>
        /// <param name="view"></param>
        private ReferencePlane(XYZ bubbleEnd, XYZ freeEnd, XYZ normal, Autodesk.Revit.DB.View view )
        {
            SafeInit(() => InitReferencePlane(bubbleEnd, freeEnd, normal, view));
        }

        /// <summary>
        /// Constructor used internally by public static constructors
        /// </summary>
        /// <param name="bubbleEnd"></param>
        /// <param name="freeEnd"></param>
        /// <param name="thirdPoint"></param>
        /// <param name="view"></param>
        /// <param name="second"></param>
        private ReferencePlane(XYZ bubbleEnd, XYZ freeEnd, XYZ thirdPoint, Autodesk.Revit.DB.View view, bool second = true)
        {
            SafeInit(() => InitReferencePlane2(bubbleEnd, freeEnd, thirdPoint, view, second));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a ReferencePlane element
        /// </summary>
        /// <param name="referencePlane"></param>
        private void InitReferencePlane(Autodesk.Revit.DB.ReferencePlane referencePlane)
        {
            InternalSetReferencePlane(referencePlane);
        }

        /// <summary>
        /// Initialize a ReferencePlane element
        /// </summary>
        /// <param name="bubbleEnd"></param>
        /// <param name="freeEnd"></param>
        /// <param name="normal"></param>
        /// <param name="view"></param>
        private void InitReferencePlane(XYZ bubbleEnd, XYZ freeEnd, XYZ normal, Autodesk.Revit.DB.View view)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldEle =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ReferencePlane>(Document);

            //There was an element, bind & mutate
            if (oldEle != null)
            {
                InternalSetReferencePlane(oldEle);
                if (InternalSetEndpoints(bubbleEnd, freeEnd)) return;

                // delete the old element, we couldn't update it for some reason
                DocumentManager.Instance.DeleteElement(new ElementUUID(oldEle.UniqueId));
            }

            //Phase 2- There was no existing element, create new
            TransactionManager.Instance.EnsureInTransaction(Document);

            var refPlane = Document.IsFamilyDocument 
                ? Document.FamilyCreate.NewReferencePlane( bubbleEnd, freeEnd, normal, view) 
                : Document.Create.NewReferencePlane(bubbleEnd,freeEnd,normal,view);

            InternalSetReferencePlane(refPlane);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalReferencePlane);
        }

        /// <summary>
        /// Initialize a ReferencePlane element
        /// </summary>
        /// <param name="bubbleEnd"></param>
        /// <param name="freeEnd"></param>
        /// <param name="thirdPoint"></param>
        /// <param name="view"></param>
        /// <param name="second"></param>
        private void InitReferencePlane2(XYZ bubbleEnd, XYZ freeEnd, XYZ thirdPoint, Autodesk.Revit.DB.View view, bool second = false)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldEle = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ReferencePlane>(Document);

            //There was an element, bind & mutate
            if (oldEle != null)
            {
                InternalSetReferencePlane(oldEle);
                if (InternalSetEndpoints(bubbleEnd, freeEnd)) return;

                // delete the old element, we couldn't update it for some reason
                DocumentManager.Instance.DeleteElement(new ElementUUID(oldEle.UniqueId));
            }

            //Phase 2- There was no existing element, create new
            TransactionManager.Instance.EnsureInTransaction(Document);

            var refPlane = Document.IsFamilyDocument
                ? Document.FamilyCreate.NewReferencePlane2(bubbleEnd, freeEnd, thirdPoint, view)
                : Document.Create.NewReferencePlane2(bubbleEnd, freeEnd, thirdPoint, view);

            InternalSetReferencePlane(refPlane);
            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(InternalReferencePlane);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the InternalReferencePlane and update the element id and unique id
        /// </summary>
        /// <param name="rp"></param>
        private void InternalSetReferencePlane(Autodesk.Revit.DB.ReferencePlane rp)
        {
            InternalReferencePlane = rp;
            InternalElementId = rp.Id;
            InternalUniqueId = rp.UniqueId;
        }

        /// <summary>
        /// Mutate the two end points of the ReferencePlane 
        /// </summary>
        /// <param name="bubbleEnd"></param>
        /// <param name="freeEnd"></param>
        /// <returns>False if the operation failed</returns>
        private bool InternalSetEndpoints(XYZ bubbleEnd, XYZ freeEnd)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var refPlane = InternalReferencePlane;

            var oldBubbleEnd = refPlane.BubbleEnd;
            var oldFreeEnd = refPlane.FreeEnd;

            var success = true;

            if (!refPlane.FreeEnd.IsAlmostEqualTo(oldFreeEnd) ||
                !refPlane.BubbleEnd.IsAlmostEqualTo(oldBubbleEnd))
            {
                var midPointOld = 0.5 * (oldBubbleEnd + oldFreeEnd);

                var midPoint = 0.5 * (bubbleEnd + freeEnd);
                var moveVec = XYZ.BasisZ.DotProduct(midPoint - midPointOld) * XYZ.BasisZ;

                // (sic) From Dynamo Legacy
                try
                {
                    Autodesk.Revit.DB.ElementTransformUtils.MoveElement(Document, refPlane.Id, moveVec);
                    refPlane.BubbleEnd = bubbleEnd;
                    refPlane.FreeEnd = freeEnd;
                }
                catch
                {
                    success = false;
                }
            }

            TransactionManager.Instance.TransactionTaskDone();

            return success;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Get the internal Geometric Plane
        /// </summary>
        public Plane Plane => InternalReferencePlane.GetPlane().ToPlane();

        /// <summary>
        /// Get a reference to this plane for downstream Elements requiring it
        /// </summary>
        public ElementPlaneReference ElementPlaneReference => new ElementPlaneReference(InternalReferencePlane.GetReference());

        #endregion

        #region Public static constructors

        /// <summary>
        /// Form a ReferencePlane from a line in the Active view.  The cut vector is the Z Axis.
        /// </summary>
        /// <param name="line">The line where the bubble wil be located at the start</param>
        /// <returns></returns>
        public static ReferencePlane ByLine( Line line )
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            var start = line.StartPoint.ToXyz();
            var end = line.EndPoint.ToXyz();
            var norm = (end - start).GetPerpendicular();

            return new ReferencePlane( start, end,norm, Document.ActiveView );
        }

        /// <summary>
        /// Form a Refernece plane from two end points in the Active view.  The cut vector is the Z Axis.
        /// </summary>
        /// <param name="start">The location where the bubble will be located</param>
        /// <param name="end">The other end</param>
        /// <returns></returns>
        public static ReferencePlane ByStartPointEndPoint( Point start, Point end )
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));

            return new ReferencePlane(  start.ToXyz(), end.ToXyz(),(end.ToXyz() - start.ToXyz()).GetPerpendicular(), Document.ActiveView);
        }

        /// <summary>
        /// Retrieve Reference Plane by its Name.
        /// </summary>
        /// <param name="name">Name of the Reference Plane.</param>
        /// <returns></returns>
        public static ReferencePlane ByName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var plane = new Autodesk.Revit.DB.FilteredElementCollector(Document)
                .OfClass(typeof(Autodesk.Revit.DB.ReferencePlane))
                .Cast<Autodesk.Revit.DB.ReferencePlane>()
                .FirstOrDefault(x => x.Name == name);

            if (plane == null) throw new Exception(Properties.Resources.ReferencePlaneInvalidName);

            return new ReferencePlane(plane);
        }

        /// <summary>
        /// Form a Refernece Plane from two end points and a normal vector in the Active view.
        /// </summary>
        /// <param name="start">The location where the bubble will be located</param>
        /// <param name="end">The other end.</param>
        /// <param name="normal">Reference Plane normal direction.</param>
        /// <returns></returns>
        public static ReferencePlane ByStartPointEndPointNormal(Point start, Point end, Vector normal)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            if (normal == null) throw new ArgumentNullException(nameof(normal));

            return new ReferencePlane(start.ToXyz(), end.ToXyz(), normal.ToXyz(), Document.ActiveView);
        }

        /// <summary>
        /// Form a Refernece Plane from three points in the Active view.
        /// </summary>
        /// <param name="pt1">The location where the bubble will be located</param>
        /// <param name="pt2">The other end.</param>
        /// <param name="pt3">Third point.</param>
        /// <returns></returns>
        public static ReferencePlane ByPoints(Point pt1, Point pt2, Point pt3)
        {
            if (pt1 == null) throw new ArgumentNullException(nameof(pt1));
            if (pt2 == null) throw new ArgumentNullException(nameof(pt2));
            if (pt3 == null) throw new ArgumentNullException(nameof(pt3));

            return new ReferencePlane(pt1.ToXyz(), pt2.ToXyz(), pt3.ToXyz(), Document.ActiveView, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sketchPlane"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static ReferencePlane BySketchPlaneAndView(SketchPlane sketchPlane, Views.View view)
        {
            if (sketchPlane == null) throw new ArgumentNullException(nameof(sketchPlane));
            if (view == null) throw new ArgumentNullException(nameof(view));

            var plane = sketchPlane.Plane.ToPlane();
            var start = plane.Origin + plane.XVec;
            var end = plane.Origin + plane.YVec;
            var normal = plane.Normal;
            return new ReferencePlane(start, end, normal, Document.ActiveView);
        }

        #endregion

        #region Internal static constructors

        internal static ReferencePlane FromExisting(Autodesk.Revit.DB.ReferencePlane ele, bool isRevitOwned)
        {
            return new ReferencePlane(ele)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
