using System;
using System.Linq;
using System.Runtime.Serialization;
using DynamoUnits;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.Elements.InternalUtilities;
using Autodesk.DesignScript.Runtime;
using UnitType = Autodesk.Revit.DB.UnitType;

namespace Revit.Elements
{
    /// <summary>
    /// This class acts as a representation of a Level constructor state, we can store it in trace
    /// it's used to keep track of what the user wanted to set the name of the level to
    /// </summary>
    [SupressImportIntoVM]
    [Serializable]
    public class LevelTraceData : SerializableId
    {
        public string InputName { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("inputName", InputName, typeof(string));
        }

        public LevelTraceData(Level lev, string inputName)
        {
            IntID = lev.InternalElement.Id.IntegerValue;
            StringID = lev.UniqueId;
            InputName = inputName;
        }

        public LevelTraceData(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            InputName = (string)info.GetValue("inputName", typeof(string));
          
        }
    }

    /// <summary>
    /// A Revit Level
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class Level : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to Revit element
        /// </summary>
        internal Autodesk.Revit.DB.Level InternalLevel
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement => InternalLevel;

        #endregion

        #region Private constructor

        /// <summary>
        /// Private constructor for Level
        /// </summary>
        /// <param name="elevation"></param>
        /// <param name="name"></param>
        private Level(double elevation, string name)
        {
            SafeInit(() => InitLevel(elevation, name));
        }

        /// <summary>
        /// Private constructor for Level
        /// </summary>
        /// <param name="level"></param>
        private Level(Autodesk.Revit.DB.Level level)
        {
            SafeInit(() => InitLevel(level));
        }

        #endregion

        #region Private constructor

        /// <summary>
        /// Initialize a Level element
        /// </summary>
        /// <param name="elevation"></param>
        /// <param name="name"></param>
        private void InitLevel(double elevation, string name)
        {
            //Phase 1 - Check to see if the object exists and should be rebound
            var oldEle = ElementBinder.GetElementAndTraceData<Autodesk.Revit.DB.Level, LevelTraceData>(Document);

            //There was an element, bind & mutate
            if (oldEle != null)
            {
                InternalSetLevel(oldEle.Item1);
                InternalSetElevation(elevation);
                InternalSetName(oldEle.Item2.InputName,name);
                return;
            }

            //There was no element, create a new one
            TransactionManager.Instance.EnsureInTransaction(Document);

            var level = Autodesk.Revit.DB.Level.Create(Document, elevation);

            InternalSetLevel(level);
            InternalSetName(string.Empty,name);
            var traceData = new LevelTraceData(this, name);
            TransactionManager.Instance.TransactionTaskDone();

            ElementBinder.SetRawDataForTrace(traceData);
        }

        private void InitLevel(Autodesk.Revit.DB.Level level)
        {
            InternalSetLevel(level);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the Element, it's Id, and it's uniqueId
        /// </summary>
        /// <param name="level"></param>
        private void InternalSetLevel(Autodesk.Revit.DB.Level level)
        {
            InternalLevel = level;
            InternalElementId = level.Id;
            InternalUniqueId = level.UniqueId;
        }

        /// <summary>
        /// Mutate the height of the level
        /// </summary>
        /// <param name="elevation"></param>
        private void InternalSetElevation(double elevation)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);
            if(!InternalLevel.Elevation.AlmostEquals(elevation, 1.0e-6))
                InternalLevel.Elevation = elevation;
            TransactionManager.Instance.TransactionTaskDone();
        }

        /// <summary>
        /// Mutate the name of the level
        /// </summary>
        /// <param name="name"> the name we want to set the level to have</param>
        /// <param name="oldname"> the oldname we tried to set this level to in the past,
        /// we retrieve this from trace</param>
        private void InternalSetName(string oldname,string name)
        {
            if (string.IsNullOrEmpty(name) ||
                string.CompareOrdinal(InternalLevel.Name, name) == 0 ||
                string.CompareOrdinal(oldname,name) == 0)
                return;
            //make a copy of the string
            var originalInputName = name;
            //Check to see whether there is an existing level with the same name
            var levels = ElementQueries.GetAllLevels().ToList();
            while (levels.Any(x => string.CompareOrdinal(x.Name, name) == 0))
            {
                //Update the level name
                ElementUtils.UpdateLevelName(ref name);
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            InternalLevel.Name = name;
            var updatedTraceData =new LevelTraceData(this, originalInputName);
            ElementBinder.SetRawDataForTrace(updatedTraceData);
            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The elevation of the level above ground level
        /// </summary>
        public double Elevation => InternalLevel.Elevation*UnitConverter.HostToDynamoFactor(UnitType.UT_Length);

        /// <summary>
        /// Elevation relative to the Project origin
        /// </summary>
        public double ProjectElevation => InternalLevel.ProjectElevation * UnitConverter.HostToDynamoFactor(UnitType.UT_Length);

        /// <summary>
        /// The name of the level
        /// </summary>
        public new string Name => InternalLevel.Name;

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Level given it's elevation and name in the project
        /// </summary>
        /// <param name="elevation">Height above project base point.</param>
        /// <param name="name">Name of the new Level.</param>
        /// <returns></returns>
        public static Level ByElevationAndName(double elevation, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Level(elevation * UnitConverter.DynamoToHostFactor(UnitType.UT_Length), name);
        }

        /// <summary>
        /// Create a Revit Level given it's elevation.  The name will be whatever
        /// Revit gives it.
        /// </summary>
        /// <param name="elevation">Height above project base point.</param>
        /// <returns></returns>
        public static Level ByElevation(double elevation)
        {
            return new Level(elevation * UnitConverter.DynamoToHostFactor(UnitType.UT_Length), null);
        }

        /// <summary>
        /// Create a Revit Level given it's length offset from an existing level
        /// </summary>
        /// <param name="level">Existing Level to offset from.</param>
        /// <param name="offset">Value of offset.</param>
        /// <returns></returns>
        public static Level ByLevelAndOffset(Level level, double offset)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            return new Level((level.Elevation + offset) * UnitConverter.DynamoToHostFactor(UnitType.UT_Length), null);
        }

        /// <summary>
        /// Create a Revit Level given a distance offset from an existing 
        /// level and a name for the new level
        /// </summary>
        /// <param name="level">Existing Level to offset from.</param>
        /// <param name="offset">Value of offset.</param>
        /// <param name="name">Name of the new Level.</param>
        /// <returns></returns>
        public static Level ByLevelOffsetAndName(Level level, double offset, string name)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Level((level.Elevation + offset) * UnitConverter.DynamoToHostFactor(UnitType.UT_Length), name);
        }

        /// <summary>
        /// Retrieves Level from project by its Name.
        /// </summary>
        /// <param name="name">Name of the Level to select.</param>
        /// <returns></returns>
        public static Level ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var doc = DocumentManager.Instance.CurrentDBDocument;
            var level = new Autodesk.Revit.DB.FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Level))
                .FirstOrDefault(x => x.Name == name);
            if (level == null)
            {
                throw new Exception(Properties.Resources.LevelByNameError);
            }

            return (Level)level.ToDSType(true);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Create a Level from a user selected Element.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static Level FromExisting(Autodesk.Revit.DB.Level level, bool isRevitOwned)
        {
            return new Level(level)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

        public override string ToString()
        {
            return $"Level(Name={Name}, Elevation={Elevation})";
        }

    }
}
