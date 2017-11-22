using System;
using Autodesk.Revit.DB.Mechanical;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;
using Autodesk.Revit.DB;


namespace Revit.Elements
{
    /// <summary>
    /// Space Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Space : Element
    {
        private bool IsRevitOwned;
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.Mechanical.Space InternalRevitElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalRevitElement; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Create from existing element
        /// </summary>
        /// <param name="Space"></param>
        internal Space(Autodesk.Revit.DB.Mechanical.Space Space)
        {
            SafeInit(() => InitElement(Space));
        }

        /// <summary>
        /// Create a new Space by Level and location
        /// </summary>
        /// <param name="level"></param>
        /// <param name="location"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        private Space(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.XYZ location, string name, string number)
        {
            SafeInit(() => Init(level, location, name, number));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Set internal element
        /// </summary>
        private void InitElement(Autodesk.Revit.DB.Mechanical.Space element)
        {
            InternalSetElement(element);
        }

        private void Init(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.XYZ location, string name, string number)
        {
            // Get document and open transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // Get existing room element if possible
            var SpaceElem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Mechanical.Space>(document);

            if (SpaceElem == null)
            {
                // Create new Space element
                SpaceElem = document.Create.NewSpace(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
            }
            else
            {
                if (SpaceElem.LevelId.Equals(level.Id))
                {
                    // Update Location only
                    Autodesk.Revit.DB.LocationPoint point = (Autodesk.Revit.DB.LocationPoint)SpaceElem.Location;
                    point.Point = location;
                }
                else
                {
                    SpaceElem = document.Create.NewSpace(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
                }
            }

            // Apply name and number if set
            if (!string.IsNullOrEmpty(name))
            {
                SpaceElem.Name = name;
            }

            if (!string.IsNullOrEmpty(number))
            {
                SpaceElem.Number = number;
            }

            InternalSetElement(SpaceElem);

            // Commit transaction
            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion      

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        private void InternalSetElement(Autodesk.Revit.DB.Mechanical.Space element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private Helpers

        private IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation position)
        {
            var options = new Autodesk.Revit.DB.SpatialElementBoundaryOptions()
            {
                SpatialElementBoundaryLocation = position,
                StoreFreeBoundaryFaces = true
            };

            var boundaries = new List<List<Autodesk.DesignScript.Geometry.Curve>>();

            foreach (var segments in this.InternalRevitElement.GetBoundarySegments(options))
            {
                var boundary = new List<Autodesk.DesignScript.Geometry.Curve>();

                foreach (Autodesk.Revit.DB.BoundarySegment segment in segments)
                {
                    boundary.Add(segment.GetCurve().ToProtoType());
                }

                boundaries.Add(boundary);
            }

            return boundaries;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Space Element
        /// </summary>
        /// <param name="level">Level the room is hosted on</param>
        /// <param name="location">Location for the center of the room</param>
        /// <param name="name">Space name</param>
        /// <param name="number">Space number</param>
        /// <returns></returns>
        public static Space ByLocation(Elements.Level level, Autodesk.DesignScript.Geometry.Point location, string name = "", string number = "")
        {
            return new Space((Autodesk.Revit.DB.Level)level.InternalElement, location.ToRevitType(true), name, number);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get room name
        /// </summary>
        public string Name
        {
            get { return this.InternalRevitElement.get_Parameter(BuiltInParameter.SPACE_ZONE_NAME).AsString(); }
        }

        /// <summary>
        /// Get room number
        /// </summary>
        public string Number
        {
            get { return this.InternalRevitElement.Number; }
        }

        /// <summary>
        /// Get room area
        /// </summary>
        public double Area
        {
            get
            {
                return this.InternalRevitElement.Area * UnitConverter.HostToDynamoFactor(UnitType.UT_Area);
            }
        }

        /// <summary>
        /// Get room height
        /// </summary>
        public double Height
        {
            get
            {
                return this.InternalRevitElement.UnboundedHeight * UnitConverter.HostToDynamoFactor(UnitType.UT_Length);
            }
        }

        /// <summary>
        /// Get room volume
        /// </summary>
        public double Volume
        {
            get
            {
                return this.InternalRevitElement.Volume * UnitConverter.HostToDynamoFactor(UnitType.UT_Volume);
            }
        }

        /// <summary>
        /// Centerline boundary
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CenterBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.Center);
            }
        }

        /// <summary>
        /// Core boundary
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CoreBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.CoreBoundary);
            }
        }

        /// <summary>
        /// Finish boundary
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> FinishBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.Finish);
            }
        }

        /// <summary>
        /// Core center boundary
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CoreCenterBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.CoreCenter);
            }
        }

        /// <summary>
        /// Get Space Location
        /// </summary>
        public Autodesk.DesignScript.Geometry.Point Location
        {
            get
            {
                if (this.InternalRevitElement.Location is Autodesk.Revit.DB.LocationPoint)
                {
                    var loc = this.InternalRevitElement.Location as Autodesk.Revit.DB.LocationPoint;
                    return loc.Point.ToPoint();
                }

                return null;
            }
        }

        /// <summary>
        /// Check if a point is inside of a room
        /// </summary>
        public bool IsInsideSpace(Autodesk.DesignScript.Geometry.Point point)
        {
            return this.InternalRevitElement.IsPointInSpace(point.ToRevitType());
        }

        /// <summary>
        /// Set name
        /// </summary>
        /// <param name="value">Name</param>
        public void SetName(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.Name = value;
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Set number
        /// </summary>
        /// <param name="value">Number</param>
        public void SetNumber(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.Number = value;
            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Internal static constructors

        internal static Space FromExisting(Autodesk.Revit.DB.Mechanical.Space instance, bool isRevitOwned)
        {
            return new Space(instance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }

}