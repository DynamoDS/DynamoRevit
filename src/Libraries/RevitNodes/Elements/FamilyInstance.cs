using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;
using DynamoUnits;
using Revit.Elements.InternalUtilities;
using Revit.GeometryReferences;
using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;
using Surface = Autodesk.DesignScript.Geometry.Surface;

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
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos,
            Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitFamilyInstance(fs, pos, level));
        }

        /// <summary>
        /// Internal constructor for a FamilyInstance
        /// </summary>
        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos)
        {
            SafeInit(() => InitFamilyInstance(fs, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Reference reference, Line pos)
        {
            SafeInit(() => InitFamilyInstance(fs, reference, pos));
        }

        internal FamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, Reference reference, XYZ location, XYZ referenceDirection)
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
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos,
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
        private void InitFamilyInstance(Autodesk.Revit.DB.FamilySymbol fs, XYZ pos)
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

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(pos, fs,
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(
                    pos, fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(FamilySymbol fs, Reference reference, Line pos)
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

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(reference, pos, fs);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(reference, pos, fs);
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(InternalElement);
        }

        private void InitFamilyInstance(FamilySymbol fs, Reference reference, XYZ location,
            XYZ referenceDirection)
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

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                fi = Document.FamilyCreate.NewFamilyInstance(reference,  location, referenceDirection, fs);
            }
            else
            {
                fi = Document.Create.NewFamilyInstance(reference, location, referenceDirection, fs);
            }

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
            InternalFamilyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level.Id);

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(XYZ fi)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as LocationPoint;
            if (lp != null && !lp.Point.IsAlmostEqualTo(fi)) lp.Point = fi;

            TransactionManager.Instance.TransactionTaskDone();
        }

        private void InternalSetPosition(Curve pos)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            var lp = InternalFamilyInstance.Location as LocationCurve;

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

        #endregion

        #region Public static constructors

        /// <summary>
        /// Place a Revit FamilyInstance given the FamilyType (also known as the FamilySymbol in the Revit API) and its coordinates in world space
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="point"></param>
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
        /// <param name="familyType"></param>
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

            return new FamilyInstance(familyType.InternalFamilySymbol, reference.InternalReference, (Line) line.ToRevitType());
        }

        /// <summary>
        /// Place a Revit family instance given the FamilyType (also known as the FamilySymbol in the Revit API) 
        /// on a surface derived from a backing Revit face as reference, a reference direction and a point location where to place the family.
        /// 
        /// Note: The FamilyType should be workplane based and the input surface must be created from a Revit Face
        /// </summary>
        /// <param name="familyType"></param>
        /// <param name="face">Surface geometry derived from a Revit face as reference element</param>
        /// <param name="location">Point on the face where the instance is to be placed</param>
        /// <param name="referenceDirection">A vector that defines the direction of placement of the family instance. 
        /// Note that this direction defines the rotation of the instance on the reference, and thus cannot be perpendicular to the face</param>
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
        /// <param name="familyType"></param>
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
        /// <param name="familyType"></param>
        /// <param name="point">Point in meters</param>
        /// <param name="level"></param>
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
        /// <param name="familyType"></param>
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

        public override string ToString()
        {
            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.Name : "empty";
        }

        #region Public methods

        /// <summary>
        /// Set the Euler angle of the family instance around its local Z-axis.
        /// </summary>        
        /// <param name="degree">The Euler angle around Z-axis.</param>
        /// <returns>The result family instance.</returns>
        public FamilyInstance SetRotation(double degree)
        {
            if (this == null)
            {
                throw new ArgumentNullException("familyInstance");
            }

            var oldTransform = InternalGetTransform();
            double[] oldRotationAngles;
            TransformUtils.ExtractEularAnglesFromTransform(oldTransform, out oldRotationAngles);
            double newRotationAngle = degree * Math.PI / 180;

            if (!oldRotationAngles[0].AlmostEquals(newRotationAngle, 1.0e-6))
            {
                double rotateAngle = newRotationAngle - oldRotationAngles[0];
                var axis = Line.CreateUnbound(oldTransform.Origin, oldTransform.BasisZ);
                ElementTransformUtils.RotateElement(Document, new ElementId(Id), axis, -rotateAngle);
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
        private Transform InternalGetTransform()
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            Document.Regenerate();

            return InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.GetTransform() : null;
        }

        #endregion
    }
}
