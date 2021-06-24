﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.Creation;
using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// A Revit FamilyInstance
    /// </summary>
    [RegisterForTrace]
    public class StructuralFraming : AbstractFamilyInstance
    {

        #region Private constructors

        /// <summary>
        /// Wrap an existing FamilyInstance. 
        /// </summary>
        /// <param name="instance"></param>
        private StructuralFraming(Autodesk.Revit.DB.FamilyInstance instance)
        {
            SafeInit(() => InitStructuralFraming(instance), true);
        }

        /// <summary>
        /// Internal constructor - creates a single StructuralFraming instance
        /// </summary>
        internal StructuralFraming(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.XYZ upVector, 
            Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.Structure.StructuralType structuralType, 
            Autodesk.Revit.DB.FamilySymbol symbol)
        {
            SafeInit(() => InitStructuralFraming(curve, upVector, level, structuralType, symbol));
        }

        /// <summary>
        /// Internal constructor - creates a single StructuralFraming instance
        /// </summary>
        internal StructuralFraming(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.Level level,
            Autodesk.Revit.DB.Structure.StructuralType structuralType, Autodesk.Revit.DB.FamilySymbol symbol)
        {
            SafeInit(() => InitStructuralFraming(curve, level, structuralType, symbol));
        }
        
        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a StructuralFraming element
        /// </summary>
        /// <param name="instance"></param>
        private void InitStructuralFraming(Autodesk.Revit.DB.FamilyInstance instance)
        {
            InternalSetFamilyInstance(instance);
        }

        /// <summary>
        /// Initialize a StructuralFraming element
        /// </summary>
        private void InitStructuralFraming(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.XYZ upVector, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.Structure.StructuralType structuralType, Autodesk.Revit.DB.FamilySymbol symbol)
        {

            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There was a point, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(symbol);
                InternalSetCurve(curve);
                return;
            }

            //Phase 2- There was no existing point, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            var creationData = GetCreationData(curve, upVector, level, structuralType, symbol);

            Autodesk.Revit.DB.FamilyInstance fi;
            if (Document.IsFamilyDocument)
            {
                var elementIds = Document.FamilyCreate.NewFamilyInstances2(new List<FamilyInstanceCreationData>() { creationData });

                if (elementIds.Count == 0)
                {
                    throw new Exception(Properties.Resources.FamilyInstanceCreationFailure);
                }

                fi = (Autodesk.Revit.DB.FamilyInstance)Document.GetElement(elementIds.First());
            }
            else
            {
                var elementIds = Document.Create.NewFamilyInstances2(new List<FamilyInstanceCreationData>() { creationData });

                if (elementIds.Count == 0)
                {
                    throw new Exception(Properties.Resources.FamilyInstanceCreationFailure);
                }

                fi = (Autodesk.Revit.DB.FamilyInstance)Document.GetElement(elementIds.First());
            }

            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        /// <summary>
        /// Initialize a StructuralFraming element
        /// </summary>
        private void InitStructuralFraming(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.Structure.StructuralType structuralType, Autodesk.Revit.DB.FamilySymbol symbol)
        {

            //Phase 1 - Check to see if the object exists and should be rebound
            var oldFam =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.FamilyInstance>(Document);

            //There's an existing structural framing element, rebind to that, and adjust its position
            if (oldFam != null)
            {
                InternalSetFamilyInstance(oldFam);
                InternalSetFamilySymbol(symbol);
                InternalSetCurve(curve);
                return;
            }

            //Phase 2- There was no existing structural framing element, create one
            TransactionManager.Instance.EnsureInTransaction(Document);

            var creationData = GetCreationData(curve, level, structuralType, symbol);

            Autodesk.Revit.DB.FamilyInstance fi;

            if (Document.IsFamilyDocument)
            {
                var elementIds =
                    Document.FamilyCreate.NewFamilyInstances2(
                        new List<FamilyInstanceCreationData>() { creationData });

                if (elementIds.Count == 0)
                {
                    throw new Exception(Properties.Resources.FamilyInstanceCreationFailure);
                }

                fi = (Autodesk.Revit.DB.FamilyInstance)Document.GetElement(elementIds.First());
            }
            else
            {
                var elementIds =
                    Document.Create.NewFamilyInstances2(
                        new List<FamilyInstanceCreationData>() { creationData });

                if (elementIds.Count == 0)
                {
                    throw new Exception(Properties.Resources.FamilyInstanceCreationFailure);
                }

                fi = (Autodesk.Revit.DB.FamilyInstance)Document.GetElement(elementIds.First());
            }


            InternalSetFamilyInstance(fi);

            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Private helper methods

        private static FamilyInstanceCreationData GetCreationData(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.XYZ upVector, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.Structure.StructuralType structuralType, Autodesk.Revit.DB.FamilySymbol symbol)
        {

            //calculate the desired rotation
            //we do this by finding the angle between the z axis
            //and vector between the start of the beam and the target point
            //both projected onto the start plane of the beam.
            var zAxis = new Autodesk.Revit.DB.XYZ(0, 0, 1);
            var yAxis = new Autodesk.Revit.DB.XYZ(0, 1, 0);

            //flatten the beam line onto the XZ plane
            //using the start's z coordinate
            var start = curve.GetEndPoint(0);
            var end = curve.GetEndPoint(1);
            var newEnd = new Autodesk.Revit.DB.XYZ(end.X, end.Y, start.Z); //drop end point to plane

            //catch the case where the end is directly above
            //the start, creating a normal with zero length
            //in that case, use the Z axis
            var planeNormal = newEnd.IsAlmostEqualTo(start) ? zAxis : (newEnd - start).Normalize();

            var gamma = upVector.AngleOnPlaneTo(zAxis.IsAlmostEqualTo(planeNormal) ? yAxis : zAxis, planeNormal);

            return new FamilyInstanceCreationData(curve, symbol, level, structuralType)
            {
                RotateAngle = gamma
            };

        }

        private static FamilyInstanceCreationData GetCreationData(Autodesk.Revit.DB.Curve curve, Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.Structure.StructuralType structuralType, Autodesk.Revit.DB.FamilySymbol symbol)
        {
            return new FamilyInstanceCreationData(curve, symbol, level, structuralType);
        }

        #endregion

        #region Private mutators

        private void InternalSetCurve(Autodesk.Revit.DB.Curve crv)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // Updating the curve will cause a document modification event
            // which will be handled by this node and could cause an 
            // infinite loop. Check the framing's curve for similarity to the 
            // provided curve and update only if the two are different.

            var locCurve = InternalFamilyInstance.Location as Autodesk.Revit.DB.LocationCurve;
            if (locCurve != null && !CurveUtils.CurvesAreSimilar(locCurve.Curve, crv))
            {
                locCurve.Curve = crv;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties
        /// <summary>
        /// Gets curve geometry from location of the specified structural element
        /// </summary>
        public new Autodesk.DesignScript.Geometry.Curve Location
        {
            get
            {
                var location = this.InternalFamilyInstance.Location;
                var crv = location as Autodesk.Revit.DB.LocationCurve;
                if (null != crv && null != crv.Curve)
                    return crv.Curve.ToProtoType();
                throw new Exception(Properties.Resources.InvalidElementLocation);
            }
        }
        /// <summary>
        /// Gets family type from the specified structural element
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

        // (Konrad) We had to add this call here because if we feed Structural Framing to
        // FamilyInstance.GetType it tries to call StructuralFraming.GetType and that throws 
        // an exception. These methods should not exist on either class, but rather on base Element.
        [Obsolete("Use Element.ElementType instead.")]
        [IsVisibleInDynamoLibrary(false)]
        public new FamilyType GetType
        {
            // NOTE: Because AbstractFamilyInstance is not visible in the library
            //       we redefine this method on FamilyInstance
            get { return base.Type; }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Structural Member - a special FamilyInstance
        /// </summary>
        /// <param name="curve">The curve path for the structural member</param>
        /// <param name="upVector">The up vector for the element - this is required to determine the orientation of the element</param>
        /// <param name="level">The level on which the member should appear</param>
        /// <param name="structuralType">The type of the structural element - a beam, column, etc</param>
        /// <param name="structuralFramingType">The structural framing type representing the structural type</param>
        /// <returns></returns>
        [Obsolete("Use StructuralFraming.BeamByCurve, StructuralFraming.BraceByCurve, or StructuralFraming.ColumnByCurve instead.")]
        public static StructuralFraming ByCurveLevelUpVectorAndType(Autodesk.DesignScript.Geometry.Curve curve, Level level, 
            Autodesk.DesignScript.Geometry.Vector upVector, StructuralType structuralType, FamilyType structuralFramingType)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (upVector == null)
            {
                throw new ArgumentNullException("upVector");
            }

            if (structuralFramingType == null)
            {
                throw new ArgumentNullException("structuralFramingType");
            }            

            return new StructuralFraming(curve.ToRevitType(), upVector.ToXyz(), level.InternalLevel,
                structuralType.ToRevitType(), structuralFramingType.InternalFamilySymbol);
        }

        /// <summary>
        /// Create a beam.
        /// </summary>
        /// <param name="curve">The curve which defines the center line of the beam.</param>
        /// <param name="level">The level with which you'd like the beam to be associated.</param>
        /// <param name="structuralFramingType">The structural framing type representing the beam.</param>
        /// <returns></returns>
        public static StructuralFraming BeamByCurve(Autodesk.DesignScript.Geometry.Curve curve, Level level, FamilyType structuralFramingType)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (structuralFramingType == null)
            {
                throw new ArgumentNullException("structuralFramingType");
            }

            return new StructuralFraming(curve.ToRevitType(), level.InternalLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam, structuralFramingType.InternalFamilySymbol);
        }

        /// <summary>
        /// Create a brace.
        /// </summary>
        /// <param name="curve">The cruve which defines the center line of the brace.</param>
        /// <param name="level">The level with which you'd like the brace to be associated.</param>
        /// <param name="structuralFramingType">The structural framing type representing the brace.</param>
        /// <returns></returns>
        public static StructuralFraming BraceByCurve(Autodesk.DesignScript.Geometry.Curve curve, Level level, FamilyType structuralFramingType)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (structuralFramingType == null)
            {
                throw new ArgumentNullException("structuralFramingType");
            }

            return new StructuralFraming(curve.ToRevitType(), level.InternalLevel, Autodesk.Revit.DB.Structure.StructuralType.Brace, structuralFramingType.InternalFamilySymbol);
        }

        /// <summary>
        /// Create a column.
        /// </summary>
        /// <param name="curve">The curve which defines the center line of the column.</param>
        /// <param name="level">The level with which you'd like the column to be associated.</param>
        /// <param name="structuralColumnType">The structural column type representing the column.</param>
        /// <returns></returns>
        public static StructuralFraming ColumnByCurve(
            Autodesk.DesignScript.Geometry.Curve curve, Level level, FamilyType structuralColumnType)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (structuralColumnType == null)
            {
                throw new ArgumentNullException("structuralColumnType");
            }

            var start = curve.PointAtParameter(0);
            var end = curve.PointAtParameter(1);

            // Revit will throw an exception if you attempt to create a column whose 
            // base is above its top. 
            if (end.Z <= start.Z)
            {
                throw new Exception(Properties.Resources.InvalidColumnBaseLocation);
            }

            return new StructuralFraming(curve.ToRevitType(), level.InternalLevel, Autodesk.Revit.DB.Structure.StructuralType.Column, structuralColumnType.InternalFamilySymbol);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Construct a FamilyInstance from the Revit document. 
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static StructuralFraming FromExisting(Autodesk.Revit.DB.FamilyInstance familyInstance, bool isRevitOwned)
        {
            return new StructuralFraming(familyInstance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
