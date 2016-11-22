using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// Fill Pattern Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class FillPatternElement : Revit.Elements.Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FillPatternElement InternalRevitElement
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

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="e"></param>
        private void InternalSetElement(Autodesk.Revit.DB.FillPatternElement e)
        {
            InternalRevitElement = e;
            InternalElementId = e.Id;
            InternalUniqueId = e.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// FillPatternElement from existing
        /// </summary>
        /// <param name="e"></param>
        private FillPatternElement(Autodesk.Revit.DB.FillPatternElement e)
        {
            SafeInit(() => InitElement(e));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init FillPatternElement from existing
        /// </summary>
        /// <param name="e"></param>
        private void InitElement(Autodesk.Revit.DB.FillPatternElement e)
        {
            InternalSetElement(e);
        }

        #endregion

        /// <summary>
        /// Fill Pattern Element by Name and Target Type.
        /// </summary>
        /// <param name="name">Name of the Fill Pattern.</param>
        /// <param name="fillPatternTarget">Target Fill pattern. Fill Patterns can be either "Drafting" or "Modeling". Default is "Drafting".</param>
        /// <returns name="FillPattern">Filled Pattern Element.</returns>
        public static FillPatternElement GetByName(string name, string fillPatternTarget = "Drafting")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException("name");
            }
            else
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                Autodesk.Revit.DB.FillPatternTarget t = (Autodesk.Revit.DB.FillPatternTarget)System.Enum.Parse(typeof(Autodesk.Revit.DB.FillPatternTarget), fillPatternTarget);
                return new FillPatternElement(Autodesk.Revit.DB.FillPatternElement.GetFillPatternElementByName(doc, t, name));
            }
        }

        #region Internal static constructors

        /// <summary>
        /// From existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static FillPatternElement FromExisting(Autodesk.Revit.DB.FillPatternElement instance, bool isRevitOwned)
        {
            return new FillPatternElement(instance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}