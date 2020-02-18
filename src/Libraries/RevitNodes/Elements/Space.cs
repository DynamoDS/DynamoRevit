using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.Elements
{
    public class Space : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.Mechanical.Space InternalRevitElement
        {
            get; private set;
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
        /// <param name="space"></param>
        internal Space(Autodesk.Revit.DB.Mechanical.Space space)
        {
            SafeInit(() => InitElement(space));
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
            TransactionManager.Instance.EnsureInTransaction(Document);

            // Get existing space element if possible
            var spaceElement = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Mechanical.Space>(Document);

            if (spaceElement == null)
            {
                // Create new Space element
                spaceElement = Document.Create.NewSpace(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
            }
            else
            {
                if (spaceElement.LevelId.Equals(level.Id))
                {
                    // Update Location only
                    Autodesk.Revit.DB.LocationPoint point = (Autodesk.Revit.DB.LocationPoint)spaceElement.Location;
                    point.Point = location;
                }
                else
                {
                    spaceElement = Document.Create.NewSpace(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
                }
            }

            // Apply name and number if set
            if (!string.IsNullOrEmpty(name))
            {
                spaceElement.Name = name;
            }

            if (!string.IsNullOrEmpty(number))
            {
                spaceElement.Number = number;
            }

            InternalSetElement(spaceElement);

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
        /// Create a Revit Space Element by level and location.
        /// </summary>
        /// <param name="level">Level the space is hosted on</param>
        /// <param name="location">Location for the center of the space</param>
        /// <param name="name">Space name</param>
        /// <param name="number">Space number</param>
        /// <returns></returns>
        public static Space ByLevelLocation(Elements.Level level, Autodesk.DesignScript.Geometry.Point location, string name = "", string number = "")
        {
            return new Space((Autodesk.Revit.DB.Level)level.InternalElement, location.ToRevitType(true), name, number);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the space name
        /// </summary>
        public string Name
        {
            get { return this.InternalRevitElement.get_Parameter(BuiltInParameter.ROOM_NAME).AsString(); }
        }

        /// <summary>
        /// Get the space number.
        /// </summary>
        public string Number
        {
            get { return this.InternalRevitElement.Number; }
        }

        /// <summary>
        /// Get area of the space.
        /// </summary>
        public double Area
        {
            get
            {
                return this.InternalRevitElement.Area * UnitConverter.HostToDynamoFactor(UnitType.UT_Area);
            }
        }

        /// <summary>
        /// Get height of the space.
        /// </summary>
        public double Height
        {
            get
            {
                return this.InternalRevitElement.UnboundedHeight * UnitConverter.HostToDynamoFactor(UnitType.UT_Length);
            }
        }

        /// <summary>
        /// Get the volume of the space.
        /// </summary>
        public double Volume
        {
            get
            {
                return this.InternalRevitElement.Volume * UnitConverter.HostToDynamoFactor(UnitType.UT_Volume);
            }
        }

        /// <summary>
        /// Centerline boundary of the space.
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CenterBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.Center);
            }
        }

        /// <summary>
        /// Core boundary of the space.
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CoreBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.CoreBoundary);
            }
        }

        /// <summary>
        /// Finish boundary of the space.
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> FinishBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.Finish);
            }
        }

        /// <summary>
        /// Core center boundary of the space.
        /// </summary>
        public IEnumerable<IEnumerable<Autodesk.DesignScript.Geometry.Curve>> CoreCenterBoundary
        {
            get
            {
                return GetBoundaries(Autodesk.Revit.DB.SpatialElementBoundaryLocation.CoreCenter);
            }
        }

        /// <summary>
        /// Get the Location of the space.
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
        /// Check if a point is inside of a space.
        /// </summary>
        public bool IsPointInsideSpace(Autodesk.DesignScript.Geometry.Point point)
        {
            return this.InternalRevitElement.IsPointInSpace(point.ToRevitType());
        }

        /// <summary>
        /// Sets the name of the space.
        /// </summary>
        /// <param name="value">Name</param>
        public Space SetName(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.Name = value;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Sets the number of the space.
        /// </summary>
        /// <param name="value">Number</param>
        public Space SetNumber(string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            this.InternalRevitElement.Number = value;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
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
