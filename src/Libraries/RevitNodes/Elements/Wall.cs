﻿using System;

using Autodesk.Revit.DB;

using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Curve = Autodesk.Revit.DB.Curve;

namespace Revit.Elements
{
    [RegisterForTrace]
    public class Wall : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.Wall InternalWall
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalWall; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Create from an existing Revit Element
        /// </summary>
        /// <param name="wall"></param>
        private Wall(Autodesk.Revit.DB.Wall wall)
        {
            SafeInit(() => InitWall(wall));
        }

        /// <summary>
        /// Create a new instance of WallType, deleting the original
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="wallType"></param>
        /// <param name="baseLevel"></param>
        /// <param name="height"></param>
        /// <param name="offset"></param>
        /// <param name="flip"></param>
        /// <param name="isStructural"></param>
        private Wall(Curve curve, Autodesk.Revit.DB.WallType wallType, Autodesk.Revit.DB.Level baseLevel,
            double height, double offset, bool flip, bool isStructural)
        {
            SafeInit(() => InitWall(curve, wallType, baseLevel, height, offset, flip, isStructural));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Initialize a Wall element
        /// </summary>
        /// <param name="wall"></param>
        private void InitWall(Autodesk.Revit.DB.Wall wall)
        {
            InternalSetWall(wall);
        }

        /// <summary>
        /// Initialize a Wall element
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="wallType"></param>
        /// <param name="baseLevel"></param>
        /// <param name="height"></param>
        /// <param name="offset"></param>
        /// <param name="flip"></param>
        /// <param name="isStructural"></param>
        private void InitWall(Curve curve, Autodesk.Revit.DB.WallType wallType, Autodesk.Revit.DB.Level baseLevel, double height, double offset, bool flip, bool isStructural)
        {
            // This creates a new wall and deletes the old one
            TransactionManager.Instance.EnsureInTransaction(Document);

            //Phase 1 - Check to see if the object exists and should be rebound
            var wallElem =
                ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Wall>(Document);

            bool successfullyUsedExistingWall = false;
            //There was a modelcurve, try and set sketch plane
            // if you can't, rebuild 
            if (wallElem != null && wallElem.Location is Autodesk.Revit.DB.LocationCurve)
            {
                var wallLocation = wallElem.Location as Autodesk.Revit.DB.LocationCurve;
                if ((wallLocation.Curve is Autodesk.Revit.DB.Line == curve is Autodesk.Revit.DB.Line) ||
                    (wallLocation.Curve is Autodesk.Revit.DB.Arc == curve is Autodesk.Revit.DB.Arc) ||
                    (wallLocation.Curve is Autodesk.Revit.DB.Ellipse == curve is Autodesk.Revit.DB.Ellipse))
                {
                    if(!CurveUtils.CurvesAreSimilar(wallLocation.Curve, curve))
                        wallLocation.Curve = curve;

                    Autodesk.Revit.DB.Parameter baseLevelParameter =
                       wallElem.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.WALL_BASE_CONSTRAINT);
                    Autodesk.Revit.DB.Parameter topOffsetParameter =
                       wallElem.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                    Autodesk.Revit.DB.Parameter wallTypeParameter =
                       wallElem.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.ELEM_TYPE_PARAM);
                    if (baseLevelParameter.AsElementId() != baseLevel.Id)
                        baseLevelParameter.Set(baseLevel.Id);
                    if (Math.Abs(topOffsetParameter.AsDouble() - height) > 1.0e-10)
                        topOffsetParameter.Set(height);
                    if (wallTypeParameter.AsElementId() != wallType.Id)
                        wallTypeParameter.Set(wallType.Id);
                    successfullyUsedExistingWall = true;
                }
            }

            var wall = successfullyUsedExistingWall ? wallElem :
                     Autodesk.Revit.DB.Wall.Create(Document, curve, wallType.Id, baseLevel.Id, height, offset, flip, isStructural);
            InternalSetWall(wall);

            TransactionManager.Instance.TransactionTaskDone();

            // delete the element stored in trace and add this new one
            ElementBinder.CleanupAndSetElementForTrace(Document, InternalWall);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="wall"></param>
        private void InternalSetWall(Autodesk.Revit.DB.Wall wall)
        {
            InternalWall = wall;
            InternalElementId = wall.Id;
            InternalUniqueId = wall.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Wall from a guiding Curve, height, Level, and WallType
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="height"></param>
        /// <param name="level"></param>
        /// <param name="wallType"></param>
        /// <returns></returns>
        public static Wall ByCurveAndHeight(Autodesk.DesignScript.Geometry.Curve curve, double height, Level level, WallType wallType)
        {
            if (curve == null)
            {
                throw new ArgumentNullException("curve");
            }

            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (wallType == null)
            {
                throw new ArgumentNullException("wallType");
            }

            height = height * UnitConverter.DynamoToHostFactor(SpecTypeId.Length);

            if (height < 1e-6 || height > 30000)
            {
                throw new ArgumentException(string.Format(Properties.Resources.InvalidWallHeight, height));
            }

            return new Wall(curve.ToRevitType(), wallType.InternalWallType, level.InternalLevel, height, 0.0, false, false);
        }

        /// <summary>
        /// Create a Revit Wall from a guiding Curve, start Level, end Level, and WallType
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="startLevel"></param>
        /// <param name="endLevel"></param>
        /// <param name="wallType"></param>
        /// <returns></returns>
        public static Wall ByCurveAndLevels(Autodesk.DesignScript.Geometry.Curve curve, Level startLevel, Level endLevel, WallType wallType)
        {
            if (endLevel == null)
            {
                throw new ArgumentNullException("endLevel");
            }

            if (startLevel == null)
            {
                throw new ArgumentNullException("startLevel");
            }

            var height = endLevel.Elevation - startLevel.Elevation;

            return ByCurveAndHeight(curve, height, startLevel, wallType);
        }

        /// <summary>
        /// Creates a Wall following the geometry of a surface. 
        /// Walls by Faces cannot be updated, any geometry change will 
        /// create a new wall and delete the old one.
        /// </summary>
        /// <param name="locationLine"></param>
        /// <param name="wallType"></param>
        /// <param name="surface"></param>
        /// <returns></returns>
        public static Element ByFace(string locationLine, WallType wallType, Autodesk.DesignScript.Geometry.Surface surface)
        {
            WallLocationLine loc = WallLocationLine.CoreCenterline;
            if (!Enum.TryParse<WallLocationLine>(locationLine, out loc))
                loc = WallLocationLine.CoreCenterline;

            return FaceWall.ByFace(loc, wallType, surface);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Revit Wall from an existing reference
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Wall FromExisting(Autodesk.Revit.DB.Wall wall, bool isRevitOwned)
        {
            return new Wall(wall)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
