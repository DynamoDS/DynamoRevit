using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.GeometryReferences
{
    /// <summary>
    /// A base class for revit Reference objects
    /// </summary>
    //[SupressImportIntoVM]
    [IsVisibleInDynamoLibrary(false)]
    public abstract class ElementGeometryReference
    {
        internal static Document Document
        {
            get { return DocumentManager.Instance.CurrentDBDocument; }
        }

        /// <summary>
        /// A stable reference to a Revit Element's geometry
        /// </summary>
        internal Autodesk.Revit.DB.Reference InternalReference
        {
            get;
            set;
        }
    }
}
