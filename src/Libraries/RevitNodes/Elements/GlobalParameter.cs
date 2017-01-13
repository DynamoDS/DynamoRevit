using System;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using System.Collections.Generic;
using Revit.GeometryConversion;
using Revit.GeometryReferences;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    /// <summary>
    /// Revit Global Parameters
    /// </summary>
    [RegisterForTrace]
    public class GlobalParameter : Element
    {
        #region Internal properties

        /// <summary>
        /// Internal reference to Element
        /// </summary>
        internal Autodesk.Revit.DB.GlobalParameter InternalGlobalParameter
        {
            get; private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        [SupressImportIntoVM]
        public override Autodesk.Revit.DB.Element InternalElement
        {
            get { return InternalGlobalParameter; }
        }

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for wrapping an existing Element
        /// </summary>
        /// <param name="grid"></param>
        private GlobalParameter(Autodesk.Revit.DB.GlobalParameter parameter)
        {
            SafeInit(() => InitGlobalParameter(parameter));
        }

        /// <summary>
        /// GlobalParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private GlobalParameter(string name, Autodesk.Revit.DB.ParameterType type)
        {
            SafeInit(() => InitGlobalParameter(name, type));
        }

        #endregion


        #region Helpers for private constructors

        /// <summary>
        /// Initialize a GlobalParameter element
        /// </summary>
        /// <param name="grid"></param>
        private void InitGlobalParameter(Autodesk.Revit.DB.GlobalParameter g)
        {
            InternalSetGlobalParameter(g);
        }

        /// <summary>
        /// Initialize a GlobalParameter element
        /// </summary>
        /// <param name="line"></param>
        private void InitGlobalParameter(string name, Autodesk.Revit.DB.ParameterType type)
        {
            if (Autodesk.Revit.DB.GlobalParametersManager.IsUniqueName(Document, name))
            {
                TransactionManager.Instance.EnsureInTransaction(Document);

                Autodesk.Revit.DB.GlobalParameter g = Autodesk.Revit.DB.GlobalParameter.Create(Document, name, type);

                InternalSetGlobalParameter(g);

                TransactionManager.Instance.TransactionTaskDone();

                ElementBinder.SetElementForTrace(this.InternalElement);
            }
            else
                throw new Exception(Properties.Resources.NameAlreadyInUse);
        }


        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="grid"></param>
        private void InternalSetGlobalParameter(Autodesk.Revit.DB.GlobalParameter g)
        {
            this.InternalGlobalParameter = g;
            this.InternalElementId = g.Id;
            this.InternalUniqueId = g.UniqueId;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Find Global Parameter by Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GlobalParameter FindByName(string name)
        {
            if (!Autodesk.Revit.DB.GlobalParametersManager.AreGlobalParametersAllowed(Document))
            {
                throw new Exception(Properties.Resources.DocumentDoesNotSupportGlobalParams);
            }

            var id = Autodesk.Revit.DB.GlobalParametersManager.FindByName(Document, name);

            if (id != null && id != Autodesk.Revit.DB.ElementId.InvalidElementId)
            {
                if (Autodesk.Revit.DB.GlobalParametersManager.IsValidGlobalParameter(Document, id))
                {
                    var global = Document.GetElement(id) as Autodesk.Revit.DB.GlobalParameter;
                    return GlobalParameter.FromExisting(global, true);
                }
            }
            
            return null;
        }

        /// <summary>
        /// Get all Global Parameters
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GlobalParameter> GetAllGlobalParameters()
        {
            if (!Autodesk.Revit.DB.GlobalParametersManager.AreGlobalParametersAllowed(Document))
            {
                throw new Exception(Properties.Resources.DocumentDoesNotSupportGlobalParams);
            }

            var ids = Autodesk.Revit.DB.GlobalParametersManager.GetAllGlobalParameters(Document);

            List<GlobalParameter> parameters = new List<GlobalParameter>();

            foreach (Autodesk.Revit.DB.ElementId id in ids)
            {
                var global = Document.GetElement(id) as Autodesk.Revit.DB.GlobalParameter;
                parameters.Add(GlobalParameter.FromExisting(global, true));
            }

            return parameters;
        }


        /// <summary>
        /// Get Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.InternalGlobalParameter.GetDefinition().Name;
            }
        }

        /// <summary>
        /// Get Parameter Group
        /// </summary>
        public string ParameterGroup
        {
            get
            {
                return this.InternalGlobalParameter.GetDefinition().ParameterGroup.ToString();
            }
        }

        /// <summary>
        /// Get Parameter Visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.InternalGlobalParameter.GetDefinition().Visible;
            }
        }

        /// <summary>
        /// Get Parameter Type
        /// </summary>
        public string ParameterType
        {
            get
            {
                return this.InternalGlobalParameter.GetDefinition().ParameterType.ToString();
            }
        }


        #endregion

        #region Public static constructors

        /// <summary>
        /// Create a new Global Parameter by Name and Type
        /// </summary>
        /// <param name="name">Name fo the parameter</param>
        /// <param name="parameterType">Parameter type</param>
        /// <returns></returns>
        public static GlobalParameter ByName(string name, string parameterType)
        {
            Autodesk.Revit.DB.ParameterType ptype = Autodesk.Revit.DB.ParameterType.Text;
            if (!Enum.TryParse<Autodesk.Revit.DB.ParameterType>(parameterType, out ptype))
                ptype = Autodesk.Revit.DB.ParameterType.Text;

            if (!Autodesk.Revit.DB.GlobalParametersManager.AreGlobalParametersAllowed(Document))
            {
                throw new Exception(Properties.Resources.DocumentDoesNotSupportGlobalParams);
            }

            return new GlobalParameter(name, ptype);
        }

        #endregion

        #region Internal static constructor

        /// <summary>
        /// Wrap an existing Element in the associated DS type
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static GlobalParameter FromExisting(Autodesk.Revit.DB.GlobalParameter parameter, bool isRevitOwned)
        {
            return new GlobalParameter(parameter)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion

    }
}
