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

        internal Family OwnerFamily
        {
            get { return this.InternalFamilyDocument.OwnerFamily.ToDSType(true) as Family; }
        }

        internal Autodesk.Revit.DB.FamilyManager FamilyManager
        {
            get { return this.InternalFamilyDocument.FamilyManager; }
        }

        #region Public Properties

        /// <summary>
        /// Retrieves the Category object that represents the category or sub category in which the elements ( this family could generate ) reside.
        /// </summary>
        public Category Category
        {
            get
            {
                Autodesk.Revit.DB.Category internalCategory = OwnerFamily.InternalFamily.FamilyCategory;
                return Category.ById(internalCategory.Id.IntegerValue);
            }
        }

        /// <summary>
        /// All family parameters in this family.
        /// </summary>
        public List<FamilyParameter> Parameters
        {
            get 
            {
                var parameters = new List<FamilyParameter>();
                var enumerator = FamilyManager.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var param = (Autodesk.Revit.DB.FamilyParameter)enumerator.Current;
                    parameters.Add(new FamilyParameter(param));
                }
                return parameters;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the Category object that represents the category or sub category in which the elements ( this family could generate ) reside.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>The document family</returns>
        public FamilyDocument SetCategory(Category category)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            OwnerFamily.InternalFamily.FamilyCategory = category.InternalCategory;
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Set the formula of a family parameter.
        /// </summary>
        /// <param name="parameter">The family parameter.</param>
        /// <param name="formula">The formula string.</param>
        /// <returns>The document family</returns>
        public FamilyDocument SetFormula(string parameter, string formula)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameter);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            FamilyManager.SetFormula(familyParameter, formula);
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Gets the formula of a family parameter.
        /// </summary>
        /// <param name="parameter">The family parameter.</param>
        /// <returns>The family parameter formula.</returns>
        public string GetFormula(string parameter)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameter);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            return familyParameter.Formula;
        }

        /// <summary>
        /// Get's the value of a family parameter of the current family type.
        /// </summary>
        /// <param name="parameter">A family parameter of the current type.</param>
        /// <returns>The parameter value.</returns>
        public object GetValue(string parameter)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameter);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);
            switch (familyParameter.StorageType)
            {
                case Autodesk.Revit.DB.StorageType.Integer:
                    return FamilyManager.CurrentType.AsInteger(familyParameter) * UnitConverter.HostToDynamoFactor(familyParameter.Definition.UnitType);   

                case Autodesk.Revit.DB.StorageType.Double:
                    return FamilyManager.CurrentType.AsDouble(familyParameter) * UnitConverter.HostToDynamoFactor(familyParameter.Definition.UnitType);

                case Autodesk.Revit.DB.StorageType.String:
                    return FamilyManager.CurrentType.AsString(familyParameter);

                case Autodesk.Revit.DB.StorageType.ElementId:
                    Element element = ElementSelector.ByElementId(FamilyManager.CurrentType.AsElementId(familyParameter).IntegerValue);
                    return element;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Set the value of a family parameter of the current family type.
        /// </summary>
        /// <param name="parameter">A family parameter of the current type.</param>
        /// <param name="value">The new value for the family parameter.</param>
        /// <returns></returns>
        public FamilyDocument SetValue(string parameter, object value)
        {
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameter);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);
            switch (familyParameter.StorageType)
            {
                case Autodesk.Revit.DB.StorageType.Integer:
                    int intValue;
                    try
                    {
                        intValue = (int)value;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, familyParameter.StorageType)); ;
                    }
                    var intValueToSet = intValue * UnitConverter.DynamoToHostFactor(familyParameter.Definition.UnitType);                                         
                    FamilyManager.Set(familyParameter, intValueToSet);
                    break;
                
                case Autodesk.Revit.DB.StorageType.Double:
                    double doubleValue;
                    try
                    {
                        doubleValue = (double)value;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, familyParameter.StorageType));
                    }
                    
                    var doubleValueToSet = doubleValue * UnitConverter.DynamoToHostFactor(familyParameter.Definition.UnitType);                                                           
                    FamilyManager.Set(familyParameter, doubleValueToSet);
                    break;

                case Autodesk.Revit.DB.StorageType.String:
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
                    break;

                case Autodesk.Revit.DB.StorageType.ElementId:
                    Autodesk.Revit.DB.ElementId id;
                    try
                    {
                        id = ((Element)value).InternalElement.Id;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException(string.Format(Properties.Resources.WrongStorageType, typeof(Element)));
                    }
                    
                    FamilyManager.Set(familyParameter, id);
                    break;
                default:
                    return null;
            }
            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Add a new family parameter with a given name.
        /// </summary>
        /// <param name="parameterName">The name of the new family parameter.</param>
        /// <param name="parameterGroup">The name of the group to which the family parameter belongs.</param>
        /// <param name="parameterType">The name of the type of new family parameter.</param>
        /// <param name="instance">Indicates if the new family parameter is instance or type.</param>
        /// <returns>The new family parameter.</returns>
        public FamilyParameter AddParameter(string parameterName, string parameterGroup, string parameterType, bool instance)
        {
            // parse parameter type
            var type = Autodesk.Revit.DB.ParameterType.Text;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.ParameterType>(parameterType, out type))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

            // parse parameter group
            var group = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.BuiltInParameterGroup>(parameterGroup, out group))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            var famParameter = FamilyManager.AddParameter(parameterName, group, type, instance);
            TransactionManager.Instance.TransactionTaskDone();
            return new FamilyParameter(famParameter);
        }

        /// <summary>
        /// Remove an existing family parameter from the family.
        /// </summary>
        /// <param name="parameterName">The family parameter name.</param>
        /// <returns>The id of the deleted family parameter.</returns>
        public int DeleteParameter(string parameterName)
        {
            Autodesk.Revit.DB.FamilyParameter familyParameter = FamilyManager.get_Parameter(parameterName);
            if (familyParameter == null)
                throw new InvalidOperationException(Properties.Resources.ParameterNotFound);

            int parameterId = familyParameter.Id.IntegerValue;
            TransactionManager.Instance.EnsureInTransaction(this.InternalDocument);
            FamilyManager.RemoveParameter(familyParameter);
            TransactionManager.Instance.TransactionTaskDone();
            return parameterId;
        }

        #endregion

        #region Static Constructors

        public static FamilyDocument ByDocument(Document document)
        {
            if (!document.InternalDocument.IsFamilyDocument)
                throw new InvalidOperationException(Properties.Resources.DocumentNotFamilyDocument);
            return new FamilyDocument(document.InternalDocument);
        }

        #endregion

    }
}
