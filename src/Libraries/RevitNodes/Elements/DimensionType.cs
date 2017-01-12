using System;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// Dimension Type element.
    /// </summary>
    [RegisterForTrace]
    public class DimensionType : Element
    {
        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.DimensionType InternalRevitElement
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
        /// <param name="element"></param>
        private void InternalSetElement(Autodesk.Revit.DB.DimensionType element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// DimensionType from existing
        /// </summary>
        /// <param name="element"></param>
        private DimensionType(Autodesk.Revit.DB.DimensionType element)
        {
            SafeInit(() => InitElement(element));
        }

        /// <summary>
        /// DimensionType by duplicating existing.
        /// </summary>
        /// <param name="dimType"></param>
        /// <param name="name"></param>
        private DimensionType(Autodesk.Revit.DB.DimensionType dimType, string name)
        {
            SafeInit(() => Init(dimType, name));
        }

        #endregion

        #region Helpers for private constructors

        private void InitElement(Autodesk.Revit.DB.DimensionType element)
        {
            InternalSetElement(element);
        }

        private void Init(Autodesk.Revit.DB.DimensionType dimType, string name)
        {
            var document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            // get element from trace
            var element = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.DimensionType>(document);

            if (element == null)
            {
                element = dimType.Duplicate(name) as Autodesk.Revit.DB.DimensionType;
            }

            InternalSetElement(element);
            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Creates new Dimension Type by duplicating an existing.
        /// </summary>
        /// <param name="dimensionType">Dimension Type to duplicate.</param>
        /// <param name="name">New name that will be assigned to Dimension Type.</param>
        /// <returns></returns>
        public static DimensionType FromExisting(Revit.Elements.Element dimensionType, string name)
        {
            var dimType = dimensionType.InternalElement as Autodesk.Revit.DB.DimensionType;
            return new DimensionType(dimType, name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns Dimension Style for a givent type.
        /// </summary>
        public string StyleType
        {
            get
            {
                return this.InternalRevitElement.StyleType.ToString();
            }
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// From existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static DimensionType FromExisting(Autodesk.Revit.DB.DimensionType instance, bool isRevitOwned)
        {
            return new DimensionType(instance)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}

