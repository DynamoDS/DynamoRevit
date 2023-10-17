using Autodesk.Revit.DB;
using Revit.Elements;
using Revit.Elements.InternalUtilities;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Nodes;

namespace Revit.Application
{
    public class FamilyDocument : Document
    {
        /// <summary>
        /// Internal reference to the Document
        /// </summary>
        internal Autodesk.Revit.DB.Document InternalFamilyDocument => InternalDocument;

        internal FamilyDocument(Autodesk.Revit.DB.Document familyDocument) : base(familyDocument)
        {
        }

        internal Elements.Family OwnerFamily
        {
            get { return this.InternalFamilyDocument.OwnerFamily.ToDSType(true) as Elements.Family; }
        }

        internal Autodesk.Revit.DB.FamilyManager FamilyManager
        {
            get { return this.InternalFamilyDocument.FamilyManager; }
        }

        #region Public Properties

        /// <summary>
        /// Retrieves the Category object that represents the category or sub category in which the elements ( this family could generate ) reside.
        /// </summary>
        public Elements.Category Category
        {
            get
            {
                Autodesk.Revit.DB.Category internalCategory = OwnerFamily.InternalFamily.FamilyCategory;
                return Elements.Category.ById(internalCategory.Id.Value);
            }
        }

        /// <summary>
        /// All family parameters in this family.
        /// </summary>
        public List<Elements.FamilyParameter> Parameters
        {
            get 
            {
                return FamilyManager.Parameters
                    .Cast<Autodesk.Revit.DB.FamilyParameter>()
                    .Select(p => new Elements.FamilyParameter(p))
                    .ToList();
            }
        }

        /// <summary>
        /// All family type names in the family document.
        /// </summary>
        public List<string> Types
        {
            get 
            {
                return this.FamilyManager
                    .Types
                    .Cast<Autodesk.Revit.DB.FamilyType>()
                    .Select(f => f.Name)
                    .ToList();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the Category of the Family.
        /// </summary>
        /// <param name="category">Category to set.</param>
        /// <returns>The document family</returns>
        public FamilyDocument SetCategory(Elements.Category category)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            OwnerFamily.InternalFamily.FamilyCategory = category.InternalCategory;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Set the formula of a family parameter (syntax is exactly as Revit, whatever works in Revit's formulas works here).
        /// </summary>
        /// <param name="parameterName">The family parameter.</param>
        /// <param name="formula">The formula string.</param>
        /// <returns>The document family</returns>
        public FamilyDocument SetFormula(string parameterName, string formula)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameterName);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            FamilyManager.SetFormula(familyParameter, formula);
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Gets the formula of a family parameter.
        /// </summary>
        /// <param name="parameterName">The family parameter.</param>
        /// <returns>The family parameter formula.</returns>
        public string GetFormula(string parameterName)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameterName);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            return familyParameter.Formula;
        }

        /// <summary>
        /// Gets the value of a family parameter of the current family type, this applies to all family parameters (instance and type).
        /// </summary>
        /// <param name="parameterName">A family parameter of the current type.</param>
        /// <param name="familyTypeName">The name of the family type.</param>
        /// <returns>The parameter value.</returns>
        public object GetParameterValueByName(string familyTypeName, string parameterName)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameterName);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            TransactionManager.Instance.EnsureInTransaction(this.InternalFamilyDocument);
            SetFamilyDocumentCurrentType(familyTypeName);
            TransactionManager.Instance.TransactionTaskDone();

            switch (familyParameter.StorageType)
            {
                case Autodesk.Revit.DB.StorageType.Integer:
                    return FamilyManager.CurrentType.AsInteger(familyParameter) * UnitConverter.HostToDynamoFactor(familyParameter.Definition.GetDataType());

                case Autodesk.Revit.DB.StorageType.Double:
                    return FamilyManager.CurrentType.AsDouble(familyParameter) * UnitConverter.HostToDynamoFactor(familyParameter.Definition.GetDataType());

                case Autodesk.Revit.DB.StorageType.String:
                    return FamilyManager.CurrentType.AsString(familyParameter);

                case Autodesk.Revit.DB.StorageType.ElementId:
                    Elements.Element element = ElementSelector.ByElementId(FamilyManager.CurrentType.AsElementId(familyParameter).Value);
                    return element;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Set the value of a family parameter of the current family type, this applies to all family parameters (instance and type).
        /// </summary>
        /// <param name="familyTypeName">The name of Family Type</param>
        /// <param name="parameter">A family parameter of the current type.</param>
        /// <param name="value">The new value for the family parameter.</param>
        /// <returns>The family document with an updated value for the specified parameter.</returns>
        public FamilyDocument SetParameterValueByName(string familyTypeName, string parameter, object value)
        {
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameter);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            SetFamilyDocumentCurrentType(familyTypeName);

            switch (familyParameter.StorageType)
            {
                case Autodesk.Revit.DB.StorageType.Integer:
                    SetIntValue(value, familyParameter);
                    break;

                case Autodesk.Revit.DB.StorageType.Double:
                    SetDoubleValue(value, familyParameter);
                    break;

                case Autodesk.Revit.DB.StorageType.String:
                    SetStringValue(value, familyParameter);
                    break;

                case Autodesk.Revit.DB.StorageType.ElementId:
                    SetElementIdValue(value, familyParameter);
                    break;

                default:
                    break;
            }
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Add a new family parameter with a given name.
        /// </summary>
        /// <param name="parameterName">The name of the new family parameter.</param>
        /// <param name="groupType">The type of the group to which the family parameter belongs.</param>
        /// <param name="specType">The type of new family parameter.</param>
        /// <param name="isInstance">Indicates if the new family parameter is instance or type (true if parameter should be instance).</param>
        /// <returns></returns>
        public Elements.FamilyParameter AddParameter(string parameterName, Revit.Elements.GroupType groupType, SpecType specType, bool isInstance)
        {
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            var famParameter = FamilyManager.AddParameter(parameterName, groupType.InternalForgeTypeId, specType.InternalForgeTypeId, isInstance);
            TransactionManager.Instance.TransactionTaskDone();
            return new Elements.FamilyParameter(famParameter);
        }

        /// <summary>
        /// Remove an existing family parameter from the family.
        /// </summary>
        /// <param name="parameterName">The family parameter name.</param>
        /// <returns>The id of the deleted family parameter.</returns>
        public long DeleteParameter(string parameterName)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameterName);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            long parameterId = familyParameter.Id.Value;
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            FamilyManager.RemoveParameter(familyParameter);
            TransactionManager.Instance.TransactionTaskDone();
            return parameterId;
        }

        #endregion

        #region Private Helpers

        private void SetFamilyDocumentCurrentType(string familyTypeName)
        {
            List<Autodesk.Revit.DB.FamilyType> familyType = this.InternalFamilyDocument.FamilyManager.Types
                .Cast<Autodesk.Revit.DB.FamilyType>()
                .Where(x => x.Name == familyTypeName)
                .Select(x => x)
                .ToList();

            if (familyType == null)
                throw new InvalidOperationException(Properties.Resources.FamilyTypeDoesNotExist);

            this.FamilyManager.CurrentType = familyType.FirstOrDefault();
        }

        private void SetElementIdValue(object value, Autodesk.Revit.DB.FamilyParameter familyParameter)
        {
            Autodesk.Revit.DB.ElementId id;
            try
            {
                id = ((Elements.Element)value).InternalElement.Id;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, typeof(Elements.Element)));
            }

            FamilyManager.Set(familyParameter, id);
        }

        private void SetStringValue(object value, Autodesk.Revit.DB.FamilyParameter familyParameter)
        {
            string strValue;
            try
            {
                strValue = (string)value;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, familyParameter.StorageType));
            }

            FamilyManager.Set(familyParameter, strValue);
        }

        private void SetDoubleValue(object value, Autodesk.Revit.DB.FamilyParameter familyParameter)
        {
            double doubleValue;
            try
            {
                if (Type.GetTypeCode(value.GetType()) == TypeCode.Int64 || Type.GetTypeCode(value.GetType()) == TypeCode.Int32)
                    doubleValue = Convert.ToDouble(value);
                else
                    doubleValue = (double)value;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, familyParameter.StorageType));
            }

            double doubleValueToSet = doubleValue * UnitConverter.DynamoToHostFactor(familyParameter.Definition.GetDataType());
            FamilyManager.Set(familyParameter, doubleValueToSet);
        }

        private void SetIntValue(object value, Autodesk.Revit.DB.FamilyParameter familyParameter)
        {
            long intValue;
            try
            {
                // support int64 conversion
                if(value.GetType() == typeof(long))
                    intValue = (long)value;
                else
                    intValue = (int)value;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, familyParameter.StorageType)); ;
            }
            var intValueToSet = intValue * UnitConverter.DynamoToHostFactor(familyParameter.Definition.GetDataType());
            FamilyManager.Set(familyParameter, intValueToSet);
        }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Get FamilyDocument from a Document 
        /// if this document is a Family Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static FamilyDocument ByDocument(Document document)
        {
            if (!document.InternalDocument.IsFamilyDocument)
                throw new InvalidOperationException(Properties.Resources.DocumentNotFamilyDocument);
            return new FamilyDocument(document.InternalDocument);
        }

        #endregion

    }
}
