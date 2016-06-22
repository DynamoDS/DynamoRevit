using Autodesk.Revit.DB;
using RevitServices.Transactions;
using System.Collections.Generic;
using RevitServices.Persistence;
using System.Linq;

namespace Revit.Elements
{
    public class Parameter
    {
        internal Autodesk.Revit.DB.Parameter InternalParameter
        {
            get;
            set;
        }

        internal Parameter(Autodesk.Revit.DB.Parameter internalParameter)
        {
            this.InternalParameter = internalParameter;
        }

        #region Properties

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name
        {
            get
            {
                return InternalParameter.Definition.Name;
            }
        }

        /// <summary>
        /// Get the value of the parameter
        /// </summary>
        public object Value
        {
            get
            {
                switch (this.InternalParameter.StorageType)
                {
                    case Autodesk.Revit.DB.StorageType.ElementId:
                        int valueId = this.InternalParameter.AsElementId().IntegerValue;
                        if (valueId > 0)
                        {
                            // When the element is obtained here, to convert it to our element wrapper, it
                            // need to be figured out whether this element is created by us. Here the existing
                            // element wrappers will be checked. If there is one, its property to specify
                            // whether it is created by us will be followed. If there is none, it means the
                            // element is not created by us.
                            var elem = ElementIDLifecycleManager<int>.GetInstance().GetFirstWrapper(valueId) as Element;
                            return ElementSelector.ByElementId(valueId, elem == null ? true : elem.IsRevitOwned);
                        }
                        else
                        {
                            int paramId = this.InternalParameter.Id.IntegerValue;
                            if (paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM || paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
                            {
                                var categories = DocumentManager.Instance.CurrentDBDocument.Settings.Categories;
                                return new Category(categories.get_Item((BuiltInCategory)valueId));
                            }
                            else
                            {
                                // For other cases, return a localized string
                                return this.InternalParameter.AsValueString();
                            }
                        }

                    case Autodesk.Revit.DB.StorageType.String:
                        return this.InternalParameter.AsString();

                    case Autodesk.Revit.DB.StorageType.Integer:
                        return this.InternalParameter.AsInteger();

                    case Autodesk.Revit.DB.StorageType.Double:
                        var paramType = this.InternalParameter.Definition.ParameterType;
                        if (Element.IsConvertableParameterType(paramType))
                            return this.InternalParameter.AsDouble() * Revit.GeometryConversion.UnitConverter.HostToDynamoFactor(Element.ParameterTypeToUnitType(paramType));
                        else 
                            return this.InternalParameter.AsDouble();

                    default:
                        throw new System.Exception(string.Format(Properties.Resources.ParameterWithoutStorageType, this.InternalParameter));
                }

            }
        }

        /// <summary>
        /// Check if the Parameter has a value
        /// </summary>
        public bool HasValue
        {
            get { return InternalParameter.HasValue; }
        }

        /// <summary>
        /// Check if the Parameter is read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return InternalParameter.IsReadOnly; }
        }

        /// <summary>
        /// Check if the Parameter is shared
        /// </summary>
        public bool IsShared
        {
            get { return InternalParameter.IsShared; }
        }

        /// <summary>
        /// Get the parameter's group
        /// </summary>
        public string Group
        {
            get { return InternalParameter.Definition.ParameterGroup.ToString(); }
        }

        /// <summary>
        /// Get the parameter type
        /// </summary>
        public string ParameterType
        {
            get { return InternalParameter.Definition.ParameterType.ToString(); }
        }

        /// <summary>
        /// Get the parameter's element Id
        /// </summary>
        public int Id
        {
            get { return InternalParameter.Id.IntegerValue; }
        }

        /// <summary>
        /// Get the parameter's unit type
        /// </summary>
        public string UnitType
        {
            get { return InternalParameter.Definition.UnitType.ToString(); }
        }


        /// <summary>
        /// Get Element's Parameter by Name
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="name">Parameter Name</param>
        /// <returns>Parameter</returns>
        public static Elements.Parameter ParameterByName(Elements.Element element, string name)
        {
            var param = element.Parameters.Cast<Autodesk.Revit.DB.Parameter>()
                .OrderBy(x => x.Id.IntegerValue)
                .FirstOrDefault(x => x.Definition.Name == name);

            if (param == null)
            {
                return null;
            }
            else
            {
                return new Parameter(param);
            }
        }

        /// <summary>
        /// Set the value of the parameter
        /// </summary>
        public static void SetValue(Parameter parameter, object value)
        {
            if (!parameter.IsReadOnly)
            {
                // get document and open transaction
                Document document = Application.Document.Current.InternalDocument;
                TransactionManager.Instance.EnsureInTransaction(document);

                // apply value according to the parameters storage type
                switch (parameter.InternalParameter.StorageType)
                {
                    case Autodesk.Revit.DB.StorageType.Double:
                        if (value is double || value is int)
                            parameter.InternalParameter.Set((double)value);
                        else if (value is string)
                        {
                            double val = 0;
                            if (double.TryParse((string)value, out val))
                                parameter.InternalParameter.Set(val);
                        }
                        break;

                    case Autodesk.Revit.DB.StorageType.Integer:
                        if (value is double || value is int)
                            parameter.InternalParameter.Set((int)value);
                        else if (value is string)
                        {
                            int val = 0;
                            if (int.TryParse((string)value, out val))
                                parameter.InternalParameter.Set(val);
                        }
                        break;

                    case Autodesk.Revit.DB.StorageType.ElementId:
                        if (value is double || value is int)
                            parameter.InternalParameter.Set(new ElementId((int)value));
                        else if (value is string)
                        {
                            int val = 0;
                            if (int.TryParse((string)value, out val))
                                parameter.InternalParameter.Set(new ElementId((int)val));
                        }
                        else if (value is ElementId)
                            parameter.InternalParameter.Set((ElementId)value);
                        break;

                    case Autodesk.Revit.DB.StorageType.String:
                        parameter.InternalParameter.Set(value.ToString());
                        break;
                }

                TransactionManager.Instance.TransactionTaskDone();
            }
        }

        /// <summary>
        /// Get Parameter Storage Type
        /// </summary>
        public string StorageType
        {
            get
            {
                return InternalParameter.StorageType.ToString();
            }
        }

        public override string ToString()
        {
            string value = string.Empty;
            switch (InternalParameter.StorageType)
            {
                case Autodesk.Revit.DB.StorageType.String:
                    value = InternalParameter.AsString();
                    break;
                default:
                    var ops = new FormatOptions() { };
                    value = InternalParameter.AsValueString(ops);
                    break;
            }
            return string.Format("{0} : {1}", Name, value);
        }

        #endregion

        #region Shared Parameters

        /// <summary>
        /// Get shared parameter file path
        /// </summary>
        /// <returns></returns>
        public static string SharedParameterFile()
        {
            Document document = Application.Document.Current.InternalDocument;
            return document.Application.SharedParametersFilename;
        }

        /// <summary>
        /// Create Shared Parameter
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        /// <param name="categoryList">List of categories this parameter applies to</param>
        public static void CreateSharedParameter(string parameterName, string groupName, string type, string group, bool instance, System.Collections.Generic.List<Category> categoryList = null)
        {
            // parse parameter type
            Autodesk.Revit.DB.ParameterType parameterType = Autodesk.Revit.DB.ParameterType.Text;
            System.Enum.TryParse<Autodesk.Revit.DB.ParameterType>(type, out parameterType);

            // parse parameter group
            Autodesk.Revit.DB.BuiltInParameterGroup parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA;
            System.Enum.TryParse<Autodesk.Revit.DB.BuiltInParameterGroup>(group, out parameterGroup);

            // get document and open transaction
            Document document = Application.Document.Current.InternalDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            try
            {
                // get current shared parameter file
                string sharedParameterFile = document.Application.SharedParametersFilename;

                // if the file does not exist, throw an error
                if (sharedParameterFile == null || !System.IO.File.Exists(sharedParameterFile))
                    throw new System.Exception(Properties.Resources.NoSharedParameterFileFound);

                // Apply selected parameter categories
                CategorySet categories = document.Application.Create.NewCategorySet();
                if (categories == null)
                {
                    // Walk thru all categories and add them if they allow bound parameters
                    foreach (Autodesk.Revit.DB.Category cat in document.Settings.Categories)
                    {
                        if (cat.AllowsBoundParameters)
                        {
                            categories.Insert(cat);
                        }
                    }
                }
                else
                {
                    foreach (Category category in categoryList)
                    {
                        if (category.InternalCategory.AllowsBoundParameters)
                        {
                            categories.Insert(category.InternalCategory);
                        }
                    }
                }

                // Create new parameter group if it does not exist yet
                DefinitionGroup groupDef = document.Application.OpenSharedParameterFile().Groups.get_Item(groupName);
                if (groupDef == null)
                {
                    groupDef = document.Application.OpenSharedParameterFile().Groups.Create(groupName);
                }

                // If the parameter definition does not exist yet, create it
                if (groupDef.Definitions.get_Item(parameterName) == null)
                {
                    ExternalDefinition def = groupDef.Definitions.Create(new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;

                    // Apply instance or type binding
                    Binding bin = (instance) ? (Binding)document.Application.Create.NewInstanceBinding(categories) : (Binding)document.Application.Create.NewTypeBinding(categories);
                    document.ParameterBindings.Insert(def, bin, parameterGroup);
                }

            }
            catch (System.Exception e)
            {
                TransactionManager.Instance.ForceCloseTransaction();
                throw e;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }


        #endregion

        #region Project Parameters

        /// <summary>
        /// Create Project Parameter
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        /// <param name="categoryList">List of categories this parameter applies to</param>
        public static void CreateProjectParameter(string parameterName, string groupName, string type, string group, bool instance, System.Collections.Generic.List<Category> categoryList = null)
        {
            // parse parameter type
            Autodesk.Revit.DB.ParameterType parameterType = Autodesk.Revit.DB.ParameterType.Text;
            System.Enum.TryParse<Autodesk.Revit.DB.ParameterType>(type, out parameterType);

            // parse parameter group
            Autodesk.Revit.DB.BuiltInParameterGroup parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA;
            System.Enum.TryParse<Autodesk.Revit.DB.BuiltInParameterGroup>(group, out parameterGroup);

            // get document and open transaction
            Document document = Application.Document.Current.InternalDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            try
            {
                // buffer the current shared parameter file name and apply a new empty parameter file instead
                string sharedParameterFile = document.Application.SharedParametersFilename;
                string tempSharedParameterFile = System.IO.Path.GetTempFileName() + ".txt";
                using (System.IO.File.Create(tempSharedParameterFile)) { }
                document.Application.SharedParametersFilename = tempSharedParameterFile;

                // Apply selected parameter categories
                CategorySet categories = document.Application.Create.NewCategorySet();
                if (categories == null)
                {
                    // Walk thru all categories and add them if they allow bound parameters
                    foreach (Autodesk.Revit.DB.Category cat in document.Settings.Categories)
                    {
                        if (cat.AllowsBoundParameters)
                        {
                            categories.Insert(cat);
                        }
                    }
                }
                else
                {
                    foreach (Category category in categoryList)
                    {
                        if (category.InternalCategory.AllowsBoundParameters)
                        {
                            categories.Insert(category.InternalCategory);
                        }
                    }
                }

                // create a new shared parameter, since the file is empty everything has to be created from scratch
                ExternalDefinition def = document.Application.OpenSharedParameterFile().Groups.Create(groupName).Definitions.Create(new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;

                // Create an instance or type binding
                Binding bin = (instance) ? (Binding)document.Application.Create.NewInstanceBinding(categories) : (Binding)document.Application.Create.NewTypeBinding(categories);

                // Apply parameter bindings
                document.ParameterBindings.Insert(def, bin, parameterGroup);

                // apply old shared parameter file
                document.Application.SharedParametersFilename = sharedParameterFile;

                // delete temp shared parameter file
                System.IO.File.Delete(tempSharedParameterFile);


            }
            catch (System.Exception e)
            {
                TransactionManager.Instance.ForceCloseTransaction();
                throw e;
            }

            TransactionManager.Instance.TransactionTaskDone();
        }

        #endregion



    }
}
