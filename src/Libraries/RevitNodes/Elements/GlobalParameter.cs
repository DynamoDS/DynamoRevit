using System;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;

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
        /// <param name="parameter"></param>
        private GlobalParameter(Autodesk.Revit.DB.GlobalParameter parameter)
        {
            SafeInit(() => InitGlobalParameter(parameter), true);
        }

        /// <summary>
        /// GlobalParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="forgeTypeId"></param>
        private GlobalParameter(string name, Autodesk.Revit.DB.ForgeTypeId forgeTypeId)
        {
            SafeInit(() => InitGlobalParameter(name, forgeTypeId));
        }

        #endregion


        #region Helpers for private constructors

        /// <summary>
        /// Initialize a GlobalParameter element
        /// </summary>
        /// <param name="grid"></param>
        private void InitGlobalParameter(Autodesk.Revit.DB.GlobalParameter grid)
        {
            InternalSetGlobalParameter(grid);
        }

        /// <summary>
        /// Initialize a GlobalParameter element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="forgeTypeId"></param>
        private void InitGlobalParameter(string name, Autodesk.Revit.DB.ForgeTypeId forgeTypeId)
        {
            var existingId = Autodesk.Revit.DB.GlobalParametersManager.FindByName(Document, name);

            if (existingId != null && existingId != Autodesk.Revit.DB.ElementId.InvalidElementId)
            {
                // GP already exists
                var existingParameter = Document.GetElement(existingId) as Autodesk.Revit.DB.GlobalParameter;
                InternalSetGlobalParameter(existingParameter);
            }
            else
            {
                // Create a new GP
                TransactionManager.Instance.EnsureInTransaction(Document);
                Autodesk.Revit.DB.GlobalParameter newParameter = Autodesk.Revit.DB.GlobalParameter.Create(Document, name, forgeTypeId);
                InternalSetGlobalParameter(newParameter);
                TransactionManager.Instance.TransactionTaskDone();
            }

            ElementBinder.CleanupAndSetElementForTrace(Document, InternalGlobalParameter);
        }


        #endregion

        #region Private mutators

        /// <summary>
        /// Set the internal Element, ElementId, and UniqueId
        /// </summary>
        /// <param name="grid"></param>
        private void InternalSetGlobalParameter(Autodesk.Revit.DB.GlobalParameter grid)
        {
            this.InternalGlobalParameter = grid;
            this.InternalElementId = grid.Id;
            this.InternalUniqueId = grid.UniqueId;
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
        /// Get Name
        /// </summary>
        new public string Name
        {
            get
            {
                return this.InternalGlobalParameter.GetDefinition().Name;
            }
        }

        /// <summary>
        /// Get Global Parameter Value
        /// </summary>
        public object Value
        {
            get
            {
                var valueWrapper = this.InternalGlobalParameter.GetValue();
                if (valueWrapper != null)
                {
                    if (valueWrapper.GetType() == typeof(Autodesk.Revit.DB.IntegerParameterValue))
                    {
                        var valueInt = valueWrapper as Autodesk.Revit.DB.IntegerParameterValue;
                        return valueInt.Value;
                    }
                    else if (valueWrapper.GetType() == typeof(Autodesk.Revit.DB.ElementIdParameterValue))
                    {
                        var valueElementId = valueWrapper as Autodesk.Revit.DB.ElementIdParameterValue;
                        return valueElementId.Value.Value;
                    }
                    else if (valueWrapper.GetType() == typeof(Autodesk.Revit.DB.NullParameterValue))
                    {
                        return null;
                    }
                    else if (valueWrapper.GetType() == typeof(Autodesk.Revit.DB.StringParameterValue))
                    {
                        var valueString = valueWrapper as Autodesk.Revit.DB.StringParameterValue;
                        return valueString.Value;
                    }
                    else if (valueWrapper.GetType() == typeof(Autodesk.Revit.DB.DoubleParameterValue))
                    {
                        var valueDouble = valueWrapper as Autodesk.Revit.DB.DoubleParameterValue;

                        return valueDouble.Value * Revit.GeometryConversion.UnitConverter.HostToDynamoFactor(
                            this.InternalGlobalParameter.GetDefinition().GetDataType());
                    }
                    else
                    {
                        throw new Exception(string.Format(
                            Properties.Resources.ParameterWithoutStorageType, this.InternalGlobalParameter));
                    }
                }
                else
                {
                    throw new Exception(string.Format(
                        Properties.Resources.ParameterWithoutStorageType, this.InternalGlobalParameter));
                }
            }
        }

        /// <summary>
        /// Set Global Parameter Value
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public static void SetValue(GlobalParameter parameter, object value)
        {
            if (!parameter.InternalGlobalParameter.IsReporting)
            {
                // get document and open transaction
                Autodesk.Revit.DB.Document document = Application.Document.Current.InternalDocument;
                TransactionManager.Instance.EnsureInTransaction(document);
                
                if (value == null)
                {
                    parameter.InternalGlobalParameter.SetValue(
                        new Autodesk.Revit.DB.NullParameterValue());
                }
                else if (value.GetType() == typeof(int))
                {
                    parameter.InternalGlobalParameter.SetValue(
                        new Autodesk.Revit.DB.IntegerParameterValue((int)value));
                }
                else if (value.GetType() == typeof(string))
                {
                    parameter.InternalGlobalParameter.SetValue(
                        new Autodesk.Revit.DB.StringParameterValue((string)value));
                }
                else if (value.GetType() == typeof(double) || value.GetType() == typeof(long))
                {
                    double dValue = value.GetType() == typeof(long) ? (double)(long)value : (double)value;
                    var valueToSet = dValue * UnitConverter.DynamoToHostFactor(parameter.InternalGlobalParameter.GetDefinition().GetDataType());
                    parameter.InternalGlobalParameter.SetValue(
                        new Autodesk.Revit.DB.DoubleParameterValue(valueToSet));
                }

                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Set Global Parameter Value to an Element ID from Integer
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="elementId"></param>
        public static void SetValueToElementId(GlobalParameter parameter, long elementId)
        {
            if (!parameter.InternalGlobalParameter.IsReporting)
            {
                // get document and open transaction
                Autodesk.Revit.DB.Document document = Application.Document.Current.InternalDocument;
                TransactionManager.Instance.EnsureInTransaction(document);

                Autodesk.Revit.DB.ElementId id = new Autodesk.Revit.DB.ElementId(elementId);
                parameter.InternalGlobalParameter.SetValue(new Autodesk.Revit.DB.ElementIdParameterValue(id));              

                TransactionManager.Instance.TransactionTaskDone();
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
        /// Get SpecType
        /// </summary>
        public SpecType SpecType
        {
            get
            {
                var forgeTypeId = this.InternalGlobalParameter.GetDefinition().GetDataType();

                return string.IsNullOrEmpty(forgeTypeId.TypeId) ? null : SpecType.FromExisting(forgeTypeId);
            }
        }

        /// <summary>
        /// Get Parameter Group Type
        /// </summary>
        public GroupType GroupType
        {
            get
            {
                var forgeTypeId = this.InternalGlobalParameter.GetDefinition().GetGroupTypeId();

                return string.IsNullOrEmpty(forgeTypeId.TypeId) ? null : GroupType.FromExisting(forgeTypeId);
            }
        }

        #endregion

        #region Public static constructors
        
        /// <summary>
        /// Create a new Global Parameter by Name and Type
        /// </summary>
        /// <param name="name">Name fo the parameter</param>
        /// <param name="specType">The type of new global parameter.</param>
        /// <returns></returns>
        public static GlobalParameter ByName(string name, SpecType specType)
        {
            if (!Autodesk.Revit.DB.GlobalParametersManager.AreGlobalParametersAllowed(Document))
            {
                throw new Exception(Properties.Resources.DocumentDoesNotSupportGlobalParams);
            }

            return new GlobalParameter(name, specType.InternalForgeTypeId);
        }

        #endregion

        #region Internal static constructor

        /// <summary>
        /// Wrap an existing Element in the associated DS type
        /// </summary>
        /// <param name="parameter"></param>
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
