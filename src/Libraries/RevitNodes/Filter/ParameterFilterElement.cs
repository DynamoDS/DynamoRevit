﻿using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Filter
{
    /// <summary>
    /// Parameter Filter Element
    /// </summary>
    [DynamoServices.RegisterForTrace]
    public class ParameterFilterElement : Revit.Elements.Element
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.ParameterFilterElement InternalRevitElement
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
        private void InternalSetElement(Autodesk.Revit.DB.ParameterFilterElement element)
        {
            InternalRevitElement = element;
            InternalElementId = element.Id;
            InternalUniqueId = element.UniqueId;
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// ParameterFilterElement from existing
        /// </summary>
        /// <param name="elem"></param>
        private ParameterFilterElement(Autodesk.Revit.DB.ParameterFilterElement elem)
        {
            SafeInit(() => InitElement(elem),true);
        }

        /// <summary>
        /// ParameterFilterElement by Rules
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ids"></param>
        /// <param name="rules"></param>
        private ParameterFilterElement(string name, IEnumerable<ElementId> ids, IEnumerable<Autodesk.Revit.DB.FilterRule> rules)
        {
            SafeInit(() => Init(name, ids, rules));
        }

        #endregion

        #region Helpers for private constructors

        /// <summary>
        /// Init ParameterFilterElement from existing
        /// </summary>
        /// <param name="element"></param>
        private void InitElement(Autodesk.Revit.DB.ParameterFilterElement element)
        {
            InternalSetElement(element);
        }

        /// <summary>
        /// Init ParameterFilterElement by rules
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ids"></param>
        /// <param name="rules"></param>
        private void Init(string name, IEnumerable<ElementId> ids, IEnumerable<Autodesk.Revit.DB.FilterRule> rules)
        {
            Autodesk.Revit.DB.Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            var elemFilters = new List<ElementFilter>();
            foreach(var rule in rules)
            {
               var elemParamFilter = new ElementParameterFilter(rule);
               elemFilters.Add(elemParamFilter);
            }
            Autodesk.Revit.DB.ElementFilter eleFilter = new LogicalOrFilter(elemFilters);

            var elem = ElementBinder.GetElementFromTrace<Autodesk.Revit.DB.ParameterFilterElement>(document);
         

            if (elem == null)
            {
               // ParameterFilterElement..::..Create Method (Document, String, ICollection<ElementId>, IList<FilterRule>) is deprecated in Revit 2019 and will be removed in the next version of Revit. 
               //We suggest you instead use a Create method that takes an ElementFilter as input.
               elem = Autodesk.Revit.DB.ParameterFilterElement.Create(document, name, ids.ToList(), eleFilter);
            }
            else
            {
                elem.Name = name;
                elem.SetCategories(ids.ToList());
                //ParameterFilterElement..::..SetRules Method is deprecated in Revit 2019 and will be removed in the next version of Revit. 
                //We suggest you instead use SetElementFilter instead.
                elem.SetElementFilter(eleFilter);
            }

            InternalSetElement(elem);

            TransactionManager.Instance.TransactionTaskDone();
            ElementBinder.SetElementForTrace(this.InternalElement);
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a Revit Parameter Filter Element
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="categories">Categories the filter applies to</param>
        /// <param name="rules">Filter rules</param>
        /// <returns></returns>
        public static ParameterFilterElement ByRules(string name, IEnumerable<Revit.Elements.Category> categories, IEnumerable<FilterRule> rules)
        {
            List<Autodesk.Revit.DB.FilterRule> ruleSet = new List<Autodesk.Revit.DB.FilterRule>();
            foreach (FilterRule rule in rules)
            { 
                ruleSet.Add(rule.InternalFilterRule);
            }

            List<ElementId> catIds = new List<ElementId>();
            foreach (Revit.Elements.Category category in categories)
            {
                catIds.Add(new ElementId(category.Id));
            }

            return new ParameterFilterElement(name, catIds, ruleSet);
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// From existing element
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ParameterFilterElement FromExisting(Autodesk.Revit.DB.ParameterFilterElement instance, bool isRevitOwned)
        {
            return new ParameterFilterElement(instance)
            { 
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }

}
