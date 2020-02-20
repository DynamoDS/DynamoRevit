using System;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using DynamoUnits;
using Revit.Elements.InternalUtilities;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Point = Autodesk.DesignScript.Geometry.Point;
using Surface = Autodesk.DesignScript.Geometry.Surface;
using Vector = Autodesk.DesignScript.Geometry.Vector;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyInstance
    /// </summary>
    [RegisterForTrace]
    public class FamilyInstance : AbstractFamilyInstance
    {

        #region Private constructors

        /// <summary>
        /// Wrap an existing FamilyInstance.
        /// </summary>
        /// <param name="instance"></param>
        protected FamilyInstance(Autodesk.Revit.DB.FamilyInstance instance)
        {
            SafeInit(() => InitFamilyInstance(instance));
        }

        /// <summary>
        /// Internal constructor for a FamilyInstance
        /// </summary>
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.XYZ pos,
            Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitFamilyInstance(fs, pos, level));
        }

        /// <summary>
        /// Internal constructor for a FamilyInstance
        /// </summary>
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.XYZ pos)
        {
            SafeInit(() => InitFamilyInstance(fs, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.Line pos)
        {
            SafeInit(() => InitFamilyInstance(fs, reference, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ location, Autodesk.Revit.DB.XYZ referenceDirection)
        {
            SafeInit(() => InitFamilyInstance(fs, reference, location, referenceDirection));
        }
        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        /// <param name="instance"></param>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilyInstance instance)
        {
            InternalSetFamilyInstance(instance);
        }

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.XYZ pos,
            Autodesk.Revit.DB.Level level)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetLevel(level);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(pos, fs, level,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(
                    pos, fs, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        /// <summary>
        /// Initialize a FamilyInstance element
        /// </summary>
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.XYZ pos)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            var fi = Document.IsFamilyDocument
                ? Document.FamilyCreate.NewFamilyInstance(pos, fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural)
                : Document.Create.NewFamilyInstance(pos, fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.Line pos)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was an existing family instance, rebind to that, and adjust its position
            if (oldFam != null && oldFam.HostFace.ElementId == reference.ElementId)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(pos);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            var fi = Document.IsFamilyDocument 
                ? Document.FamilyCreate.NewFamilyInstance(reference, pos, fs) 
                : Document.Create.NewFamilyInstance(reference, pos, fs);

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ location,
            Autodesk.Revit.DB.XYZ referenceDirection)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was an existing family instance, rebind to that, and adjust its position
            if (oldFam != null && oldFam.HostFace.ElementId == reference.ElementId)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(fs);
                InternalSetPosition(location);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //If the symbol is not active, then activate it
            if (!fs.IsActive)
                fs.Activate();

            var fi = Document.IsFamilyDocument 
                ? Document.FamilyCreate.NewFamilyInstance(reference,  location, referenceDirection, fs) 
                : Document.Create.NewFamilyInstance(reference, location, referenceDirection, fs);

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }
        #endregion

        #region Private mutators

        private void InternalSetLevel(Autodesk.Revit.DB.Level level)
        {
            if (InternalFamilyInstance.LevelId.Compare(level.Id) == 0)
                return;

            TransactionManager.Instance.EnsureInTransaction(Document);

            // http://thebuildingcoder.typepad.com/blog/2011/01/family-instance-missing-level-property.html
            InternalFamilyInstance.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level.Id);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(Autodesk.Revit.DB.XYZ fi)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as Autodesk.Revit.DB.LocationPoint;
            if (lp != null && !lp.Point.IsAlmostEqualTo(fi)) lp.Point = fi;

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(Autodesk.Revit.DB.Curve pos)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as Autodesk.Revit.DB.LocationCurve;

            if (lp != null && lp.Curve != pos) lp.Curve = pos;

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets family type of the specific family instance
        /// </summary>
        /// <search>
        /// symbol
        /// </search>
        [Obsolete("Use Element.ElementType instead.")]
        public new FamilyType Type
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Type; }
        }
        /// <summary>
        /// Gets the location of the specific family instance 
        /// </summary>
        public new Point Location
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Location; }
        }

        /// <summary>
        /// Gets the FacingOrientation of the family instance
        /// </summary>
        public Vector FacingOrientation
        {
            get
            {
                return InternalFamilyInstance.IsValidObject ? 
                    InternalFamilyInstance.FacingOrientation.ToVector() : null;
            }
        }

        /// <summary>
        /// The room in which the instance is located.
        /// </summary>
        public Element Room
        {
            get
            {
                var room = this.InternalFamilyInstance.Room;
                if (room == null)
                    return null;
                return room.ToDSType(true);
            }
        }

        /// <summary>
        /// The space in which the instance is located.
        /// </summary>
        public Element Space
        {
            get
            {
                var space = this.InternalFamilyInstance.Space;
                if (space == null)
                    return null;
                return space.ToDSType(true);
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinates in world space
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="point">Point in meters.</param>
        /// <returns></returns>
        public static FamilyInstance ByPoint(FamilyType familyType, Point point)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            } 
            
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }

            return new FamilyInstance(familyType.InternalFamilySymbol, point.ToXyz());
        }

        /// <summary>
        /// Place a Revit family instance of the given the FamilyType (also known as the FamilySymbol in the Revit API) 
        /// on a surface derived from a backing Revit face as reference and a line as reference for its position.
        /// 
        /// Note: The FamilyPlacementType must be CurveBased and the input surface must be created from a Revit Face 
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="face">Surface geometry derived from a Revit face as reference element</param>
        /// <param name="line">A line on the face defining where the symbol is to be placed</param>
        /// <returns>FamilyInstance</returns>
        public static FamilyInstance ByFace(FamilyType familyType, Surface face, Autodesk.DesignScript.Geometry.Line line)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }
            if (face == null)
            {
                throw new ArgumentNullException("face");
            }
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            var reference = ElementFaceReference.TryGetFaceReference(face);

            return new FamilyInstance(familyType.InternalFamilySymbol, reference.InternalReference, (Autodesk.Revit.DB.Line) line.ToRevitType());
        }

        /// <summary>
        /// Place a Revit family instance given the FamilyType (also known as the FamilySymbol in the Revit API) 
        /// on a surface derived from a backing Revit face as reference, a reference direction and a point location where to place the family.
        /// 
        /// Note: The FamilyType should be workplane based and the input surface must be created from a Revit Face. The reference direction defines the rotation of the instance on the reference, and thus cannot be perpendicular to the face.
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="face">Surface geometry derived from a Revit face as reference element</param>
        /// <param name="location">Point on the face where the instance is to be placed</param>
        /// <param name="referenceDirection">A vector that defines the direction of placement of the family instance</param>
        /// <returns>FamilyInstance</returns>
        public static FamilyInstance ByFace(FamilyType familyType, Surface face, Point location, 
            Vector referenceDirection)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }
            if (face == null)
            {
                throw new ArgumentNullException("face");
            }
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if (referenceDirection == null)
            {
                throw new ArgumentNullException("referenceDirection");
            }
            var reference = ElementFaceReference.TryGetFaceReference(face);

            return new FamilyInstance(familyType.InternalFamilySymbol, reference.InternalReference, 
                location.ToXyz(), referenceDirection.ToXyz());
        }

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinates in world space
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="x">X coordinate in meters</param>
        /// <param name="y">Y coordinate in meters</param>
        /// <param name="z">Z coordinate in meters</param>
        /// <returns></returns>
        public static FamilyInstance ByCoordinates(FamilyType familyType, double x = 0, double y = 0, double z = 0)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            var pt = Point.ByCoordinates(x, y, z);

            return ByPoint(familyType, pt);
        }

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API), it's coordinates in world space, and the Level
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="point">Point in meters.</param>
        /// <param name="level">Level to host Family Instance.</param>
        /// <returns></returns>
        public static FamilyInstance ByPointAndLevel(FamilyType familyType, Point point, Level level)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            return new FamilyInstance(familyType.InternalFamilySymbol, point.ToXyz(), level.InternalLevel);
        }

        /// <summary>
        /// Obtain a collection of FamilyInstances from the Revit Document and use them in the Dynamo graph
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <returns></returns>
        /// <search>
        /// byfamilysymbol,ByFamilySymbol
        /// </search>
        public static FamilyInstance[] ByFamilyType(FamilyType familyType)
        {
            if (familyType == null)
            {
                throw new ArgumentNullException("familyType");
            }

            return DocumentManager.Instance
                .ElementsOfType<Autodesk.Revit.DB.FamilyInstance>()
                .Where(x => x.Symbol.Id == familyType.InternalFamilySymbol.Id)
                .Select(x => FromExisting(x, true))
                .ToArray();
        }

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinate system.
        /// </summary>
        /// <param name="familyType">Family Type. Also called Family Symbol.</param>
        /// <param name="coordinateSystem">Coordinates system to place the new family instance in.</param>
        /// <returns>New family instance.</returns>
        public static FamilyInstance ByCoordinateSystem(FamilyType familyType, CoordinateSystem coordinateSystem)
        {
            var transform = coordinateSystem.ToTransform() as Autodesk.Revit.DB.Transform;
            double[] newRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(transform, out newRotationAngles);
            double rotation = ConvertEularToAngleDegrees(newRotationAngles.FirstOrDefault());
            Point location = transform.ToCoordinateSystem().Origin;

            FamilyInstance familyInstance = ByPoint(familyType, location);
            familyInstance.SetRotation(rotation);

            return familyInstance;
        }

        #endregion

        #region Internal static constructors 

        /// <summary>
        /// Construct a FamilyInstance from the Revit document. 
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FamilyInstance FromExisting(Autodesk.Revit.DB.FamilyInstance familyInstance, bool isRevitOwned)
        {
            return new FamilyInstance(familyInstance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.Name : "empty";
        }

        /// <summary>
        /// Gets the host of this fmaily instance (if any). Eg. returns the wall of a window or door family instance.
        /// </summary>
        public Element GetHost
        {
            get
            {
                if (this.InternalFamilyInstance.Host != null)

                    // TODO: This might need to change since its not clear if the Host element is revit owned or not.
                    // Currently there is no way of figuring this out, therefore the assumption is true (Revit owned)
                    return ElementWrapper.Wrap(this.InternalFamilyInstance.Host, true);

                throw new Exception(Properties.Resources.InvalidHost);
            }
        }

        /// <summary>
        /// Gets the family of this family instance
        /// </summary>
        public Family GetFamily 
        { 
            get 
            { 
                return Family.FromExisting(this.InternalFamilyInstance.Symbol.Family, true);
            }
        }

        /// <summary>
        /// Gets the family type of this family instance
        /// </summary>
        [Obsolete("Use Element.ElementType instead.")]
        public new FamilyType GetType
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Type; }
        }

        

        /// <summary>
        /// Set the Euler angle of the family instance around its local Z-axis.
        /// </summary>        
        /// <param name="degree">The Euler angle around Z-axis.</param>
        /// <returns>The result family instance.</returns>
        public FamilyInstance SetRotation(double degree)
        {
            if (this == null)
            {
                throw new ArgumentNullException("degree");
            }

            var oldTransform = InternalGetTransform();
            double[] oldRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(oldTransform, out oldRotationAngles);
            var newRotationAngle = degree * Math.PI / 180;

            if (!oldRotationAngles[0].AlmostEquals(newRotationAngle, 1.0e-6))
            {
                var rotateAngle = newRotationAngle - oldRotationAngles[0];
                var axis = Autodesk.Revit.DB.Line.CreateUnbound(oldTransform.Origin, oldTransform.BasisZ);
                Autodesk.Revit.DB.ElementTransformUtils.RotateElement(Document, new Autodesk.Revit.DB.ElementId(Id), axis, -rotateAngle);
            }

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        ///// <summary>
        ///// Transforms this FamilyInstance from a source CoordinateSystem to a new context CoordinateSystem.
        ///// </summary>
        ///// <param name="fromCoordinateSystem">Source CoordinatSystem</param>
        ///// <param name="contextCoordinateSystem">Context CordinateSystem</param>
        ///// <returns>Transformed Element</returns>
        public FamilyInstance Transform(CoordinateSystem fromCoordinateSystem, CoordinateSystem contextCoordinateSystem)
        {

            TransactionManager.Instance.EnsureInTransaction(Document);
            var transManager = TransactionManager.Instance.TransactionWrapper;
            var t = transManager.StartTransaction(Document);
            var elementTransform = this.InternalGetTransform();
            var contextTransform = contextCoordinateSystem.ToTransform();
            try
            {
                if (elementTransform.BasisX.IsAlmostEqualTo(contextTransform.BasisX) &&
                    elementTransform.BasisY.IsAlmostEqualTo(contextTransform.BasisY) &&
                    elementTransform.BasisZ.IsAlmostEqualTo(contextTransform.BasisZ))
                {
                    return this;
                }
                double degrees = GetRotationFromCS(fromCoordinateSystem, contextCoordinateSystem);
                this.SetRotation(degrees);
                this.SetLocationFromCS(fromCoordinateSystem, contextCoordinateSystem);
            }
            catch (Exception ex)
            {
                t.CancelTransaction();
                throw new Exception(ex.Message);
            }
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Get the transform of the internal family instance
        /// </summary>
        /// <returns>The internal transform</returns>
        private Autodesk.Revit.DB.Transform InternalGetTransform()
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Document.Regenerate();

            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.GetTransform() : null;
        }

        private double GetRotationFromCS(CoordinateSystem fromCS, CoordinateSystem contextCS)
        {
            var elementTransform = this.InternalFamilyInstance.GetTransform();
            var newTransform = contextCS.ToTransform() as Autodesk.Revit.DB.Transform;

            var oldTransform = fromCS.ToTransform() as Autodesk.Revit.DB.Transform;

            double[] oldRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(oldTransform, out oldRotationAngles);
            double[] newRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(newTransform, out newRotationAngles);
            return ConvertEularToAngleDegrees(newRotationAngles.FirstOrDefault());
        }

        private static double ConvertEularToAngleDegrees(double newRotationAngles)
        {
            return (newRotationAngles / (2 * Math.PI)) * 360;
        }

        private void SetLocationFromCS(CoordinateSystem fromCS, CoordinateSystem contextCS)
        {
            var locationGeometry = this.InternalElement.Location;
            if (locationGeometry is Autodesk.Revit.DB.LocationCurve)
            {
                var locationCurve = locationGeometry as Autodesk.Revit.DB.LocationCurve;
                var dynamoCurve = locationCurve.Curve.ToProtoType();
                var newLocation = dynamoCurve.Transform(fromCS, contextCS) as Curve;
                locationCurve.Curve = newLocation.ToRevitType(true);
                dynamoCurve.Dispose();
                newLocation.Dispose();
                return;
            }
            else if (locationGeometry is Autodesk.Revit.DB.LocationPoint)
            {
                var location = this.InternalElement.Location as Autodesk.Revit.DB.LocationPoint;
                var dynamoPoint = location.Point.ToPoint(true);
                var newLocation = dynamoPoint.Transform(fromCS, contextCS) as Autodesk.DesignScript.Geometry.Point;
                location.Point = newLocation.ToRevitType(true);
                dynamoPoint.Dispose();
                newLocation.Dispose();
                return;
            }
            throw new Exception(Properties.Resources.InvalidElementLocation);
        }

        #endregion
    }


}
