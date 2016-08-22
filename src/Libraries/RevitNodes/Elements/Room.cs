using System;
using Autodesk.Revit.DB.Architecture;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;


namespace Revit.Elements
{
    /// <summary>
    /// Room Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Room : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.Architecture.Room InternalRevitElement
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
        /// <param name="Room"></param>
        internal Room(Autodesk.Revit.DB.Architecture.Room Room)
        {
            SafeInit(() => InitElement(Room));
        }

        /// <summary>
        /// Create a new Room by Level and location
        /// </summary>
        /// <param name="level"></param>
        /// <param name="location"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        private Room(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.XYZ location, string name, string number)
        {
            SafeInit(() => Init(level, location, name, number));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Set internal element
        /// </summary>
        private void InitElement(Autodesk.Revit.DB.Architecture.Room element)
        {
            InternalSetElement(element);
        }

        private void Init(Autodesk.Revit.DB.Level level, Autodesk.Revit.DB.XYZ location, string name, string number)
        {
            // Get document and open transaction
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // Get existing room element if possible
            var RoomElem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.Architecture.Room>(document);

            if (RoomElem == null)
            {
                // Create new Room element
                RoomElem = document.Create.NewRoom(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
            }
            else
            {
                if (RoomElem.LevelId.Equals(level.Id))
                {
                    // Update Location only
                    Autodesk.Revit.DB.LocationPoint point = (Autodesk.Revit.DB.LocationPoint)RoomElem.Location;
                    point.Point = location;
                }
                else
                {
                    RoomElem = document.Create.NewRoom(level, new Autodesk.Revit.DB.UV(location.X, location.Y));
                }
            }

            // Apply name and number if set
            if (!string.IsNullOrEmpty(name))
            { 
                RoomElem.Name = name;
            }

            if (!string.IsNullOrEmpty(number))
            { 
                RoomElem.Number = number;
            }

            InternalSetElement(RoomElem);

            // Commit transaction
            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion      

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        private void InternalSetElement(Autodesk.Revit.DB.Architecture.Room element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Room Element
        /// </summary>
        /// <param name="level">Level the room is hosted on</param>
        /// <param name="location">Location for the center of the room</param>
        /// <param name="name">Room name</param>
        /// <param name="number">Room number</param>
        /// <returns></returns>
        public static Room ByLocation(Elements.Level level, Autodesk.DesignScript.Geometry.Point location, string name = "", string number = "")
        {
            return new Room((Autodesk.Revit.DB.Level)level.InternalElement, location.ToRevitType(true), name, number);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get room name
        /// </summary>
        public string Name
        {
            get{ return this.InternalRevitElement.Name; }           
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
            get { return this.InternalRevitElement.Area; }
        }

        /// <summary>
        /// Get room height
        /// </summary>
        public double Height
        {
            get { return this.InternalRevitElement.UnboundedHeight; }
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

        internal static Room FromExisting(Autodesk.Revit.DB.Architecture.Room instance, bool isRevitOwned)
        {
            return new Room(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }

}
