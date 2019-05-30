using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using RevitServices.Persistence;

namespace RevitSystemTests
{
    class TestUtils
    {
        /// <summary>
        /// This function gets all the family instances in the current Revit document
        /// </summary>
        /// <param name="startNewTransaction">whether do the filtering in a new transaction</param>
        /// <returns>the family instances</returns>
        public static IList<Element> GetAllFamilyInstances()
        {
            ElementClassFilter ef = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector fec = new FilteredElementCollector(DocumentManager.Instance.CurrentUIDocument.Document);
            fec.WherePasses(ef);
            return fec.ToElements();            
        }
    }
}
