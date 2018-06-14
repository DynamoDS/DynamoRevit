using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;

namespace Revit.Elements
{
    /// <summary>
    /// Line Pattern Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class LinePatternElement : Revit.Elements.Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.LinePatternElement InternalRevitElement
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
        private void InternalSetElement(Autodesk.Revit.DB.LinePatternElement e)
        {
            InternalRevitElement = e;
            InternalElementId = e.Id;
            InternalUniqueId = e.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// LinePatternElement from existing
        /// </summary>
        /// <param name="e"></param>
        private LinePatternElement(Autodesk.Revit.DB.LinePatternElement e)
        {
            SafeInit(() => InitElement(e));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init LinePatternElement from existing
        /// </summary>
        /// <param name="e"></param>
        private void InitElement(Autodesk.Revit.DB.LinePatternElement e)
        {
            InternalSetElement(e);
        }

        #endregion

        /// <summary>
        /// Line Pattern Element by name.
        /// </summary>
        /// <param name="name">Name of the line pattern.</param>
        /// <returns name="LinePattern">Filled Pattern Element.</returns>
        public static LinePatternElement GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException("name");
            }
            else
            {
                var doc = DocumentManager.Instance.CurrentDBDocument;
                return new LinePatternElement(Autodesk.Revit.DB.LinePatternElement.GetLinePatternElementByName(doc, name));
            }
        }

        #region Internal static constructors

        /// <summary>
        /// From existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static LinePatternElement FromExisting(Autodesk.Revit.DB.LinePatternElement instance, bool isRevitOwned)
        {
            return new LinePatternElement(instance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}