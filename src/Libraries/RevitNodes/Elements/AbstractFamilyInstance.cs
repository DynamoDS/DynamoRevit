using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

using Revit.GeometryConversion;

using RevitServices.Persistence;
using RevitServices.Transactions;
using Point = Autodesk.DesignScript.Geometry.Point;

namespace Revit.Elements
{
    /// <summary>
    /// An abstract Revit FamilyInstance - implementors include FamilyInstance, AdaptiveComponent, StructuralFraming
    /// </summary>
    [DynamoServices.RegisterForTrace]
    [IsVisibleInDynamoLibrary(false)]
    public abstract class AbstractFamilyInstance : Element
    {

        #region Internal properties

        /// <summary>
        /// Reference to the Element
        /// </summary>
        internal Autodesk.Revit.DB.FamilyInstance InternalFamilyInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalFamilyInstance; }
        }

        #endregion

        #region Protected mutators

        protected void InternalSetFamilyInstance(Autodesk.Revit.DB.FamilyInstance fi)
        {
            this.InternalFamilyInstance = fi;
            this.InternalElementId = fi.Id;
            this.InternalUniqueId = fi.UniqueId;
        }

        protected void InternalSetFamilySymbol(Autodesk.Revit.DB.FamilySymbol fs)
        {
            TransactionManager.Instance.EnsureInTransaction(Document);

            // Don't attempt to set the symbol if it is the same.
            // Doing so will raise a document modification event which
            // will, in turn, be responded to by this node causing an infinite loop.
            if (InternalFamilyInstance.Symbol.UniqueId != fs.UniqueId)
            {
                InternalFamilyInstance.Symbol = fs;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion

        #region Public properties

        public FamilyType Type
        {
            get
            {
                return FamilyType.FromExisting(this.InternalFamilyInstance.Symbol, true);
            }
        }

        public Point Location
        {
            get
            {
                TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
                DocumentManager.Regenerate();
                var pos = InternalFamilyInstance.Location as LocationPoint;
                TransactionManager.Instance.TransactionTaskDone();
                return pos.Point.ToPoint();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Family={0}, Type={1}", 
                InternalFamilyInstance != null && InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.Symbol.Name : "empty", 
                InternalFamilyInstance != null && InternalFamilyInstance.IsValidObject ? InternalFamilyInstance.Name : "empty");
        }
    }
}
