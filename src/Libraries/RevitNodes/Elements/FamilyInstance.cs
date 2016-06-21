using System;
using System.Linq;

using Autodesk.Revit.DB;

using DynamoServices;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;
using DynamoUnits;
using Revit.Elements.InternalUtilities;

using Point = Autodesk.DesignScript.Geometry.Point;
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
            lp.Point = fi;

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
                return GeometryPrimitiveConverter.ToVector(InternalFamilyInstance.FacingOrientation);
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
            return InternalFamilyInstance.Name;
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

            return InternalFamilyInstance.GetTransform();
        }

        #endregion
    }
}
