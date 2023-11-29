using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using DynamoUnits;
using Newtonsoft.Json;
using Revit.Elements.InternalUtilities;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{

    /// <summary>
    /// This class acts as a representation of a Level constructor state, we can store it in trace
    /// it's used to keep track of what the user wanted to set the name of the level to
    /// </summary>
    [SupressImportIntoVM]
    public class LevelTraceData : SerializableId
    {
        public string InputName { get; set; }

        [JsonConstructor]
        public LevelTraceData(int intID, string stringID,  string inputName)
        {
            IntID = intID;
            StringID = stringID;
            InputName = inputName;  
        }

        public LevelTraceData(Level lev, string inputName) :
            base()
        {
            this.IntID = lev.InternalElement.Id.Value;
            this.StringID = lev.UniqueId;
            this.InputName = inputName;
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
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalLevel; }
        }

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
            SafeInit(() => InitLevel(level), true);
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

            Autodesk.Revit.DB.Level level;

            if (Document.IsFamilyDocument)
            {
                level = Autodesk.Revit.DB.Level.Create(Document, elevation);
            }
            else
            {
                level = Autodesk.Revit.DB.Level.Create(Document, elevation);
            }

            InternalSetLevel(level);
            InternalSetName(string.Empty,name);
            var traceData = new LevelTraceData(this, name);
            TransactionManager.Instance.TransactionTaskDone();

            var serializedTraceData = JsonConvert.SerializeObject(traceData);

            ElementBinder.SetRawDataForTrace(serializedTraceData);

        }

        private void InitLevel(Autodesk.Revit.DB.Level level)
        {
            this.InternalSetLevel(level);
        }

        #endregion

        #region Private mutators

        /// <summary>
        /// Set the Element, it's Id, and it's uniqueId
        /// </summary>
        /// <param name="level"></param>
        private void InternalSetLevel(Autodesk.Revit.DB.Level level)
        {
            this.InternalLevel = level;
            this.InternalElementId = level.Id;
            this.InternalUniqueId = level.UniqueId;
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
            if (String.IsNullOrEmpty(name) ||
                string.CompareOrdinal(InternalLevel.Name, name) == 0 ||
                string.CompareOrdinal(oldname,name) == 0)
                return;
            //make a copy of the string
            var originalInputName = name;
            //Check to see whether there is an existing level with the same name
            var levels = ElementQueries.GetAllLevels();
            while (levels.Any(x => string.CompareOrdinal(x.Name, name) == 0))
            {
                //Update the level name
                Revit.Elements.InternalUtilities.ElementUtils.UpdateLevelName(ref name);
            }

            TransactionManager.Instance.EnsureInTransaction(Document);
            this.InternalLevel.Name = name;
            var updatedTraceData =new LevelTraceData(this, originalInputName);
            var serializedTraceData = JsonConvert.SerializeObject(updatedTraceData);
            ElementBinder.SetRawDataForTrace(serializedTraceData);
            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The elevation of the level above ground level
        /// </summary>
        public double Elevation
        {
            get { return InternalLevel.Elevation*UnitConverter.HostToDynamoFactor(SpecTypeId.Length); }
        }

        /// <summary>
        /// Elevation relative to the Project origin
        /// </summary>
        public double ProjectElevation
        {
            get
            {
                return InternalLevel.ProjectElevation * UnitConverter.HostToDynamoFactor(SpecTypeId.Length);
            }
        }

        /// <summary>
        /// The name of the level
        /// </summary>
        public new string Name
        {
            get
            {
                return InternalLevel.Name;
            }
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Level given it's elevation and name in the project
        /// </summary>
        /// <param name="elevation"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Level ByElevationAndName(double elevation, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new Level(elevation * UnitConverter.DynamoToHostFactor(SpecTypeId.Length), name);
        }

        /// <summary>
        /// Create a Revit Level given it's elevation.  The name will be whatever
        /// Revit gives it.
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public static Level ByElevation(double elevation)
        {
            return new Level(elevation * UnitConverter.DynamoToHostFactor(SpecTypeId.Length), null);
        }

        /// <summary>
        /// Create a Revit Level given it's length offset from an existing level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Level ByLevelAndOffset(Level level, double offset)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            return new Level((level.Elevation + offset) * UnitConverter.DynamoToHostFactor(SpecTypeId.Length), null);
        }

        /// <summary>
        /// Create a Revit Level given a distance offset from an existing 
        /// level and a name for the new level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="offset"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Level ByLevelOffsetAndName(Level level, double offset, string name)
        {
            if (level == null)
            {
                throw new ArgumentNullException("level");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return new Level((level.Elevation + offset) * UnitConverter.DynamoToHostFactor(SpecTypeId.Length), name);
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
            return InternalLevel.IsValidObject ? string.Format("Level(Name={0}, Elevation={1})", Name, Elevation) : "empty";
        }

    }
}
