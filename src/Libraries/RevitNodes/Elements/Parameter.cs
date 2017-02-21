using Autodesk.Revit.DB;
using RevitServices.Transactions;
using System.Collections.Generic;
using RevitServices.Persistence;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;

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
            get { return Revit.Elements.InternalUtilities.ElementUtils.GetParameterValue(this.InternalParameter); }
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
            var param = element.Parameters.Cast<Parameter>()
                .OrderBy(x => x.InternalParameter.Id.IntegerValue)
                .FirstOrDefault(x => x.InternalParameter.Definition.Name == name);

            if (param == null)
            {
                return null;
            }
            else
            {
                return param;
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
                var val = value as dynamic;
                Revit.Elements.InternalUtilities.ElementUtils.SetParameterValue(parameter.InternalParameter, val);
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
        /// Gets the path to the shared parameter file of this document
        /// </summary>
        /// <returns></returns>
        public static string SharedParameterFile()
        {
            Document document = Application.Document.Current.InternalDocument;
            return document.Application.SharedParametersFilename;
        }

        /// <summary>
        /// Create a new Shared Parameter in the current Revit document for all applicable categories
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        public static void CreateSharedParameterForAllCategories(string parameterName, string groupName, string type, string group, bool instance)
        {
            CreateSharedParameter(parameterName, groupName, type, group, instance, null); 
        }

        /// <summary>
        /// Create a new Shared Parameter in the current Revit document
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        /// <param name="categoryList">List of categories this parameter applies to, If no category is supplied, all possible categories are selected</param>
        public static void CreateSharedParameter(string parameterName, string groupName, string type, string group, bool instance, System.Collections.Generic.IEnumerable<Category> categoryList)
        {
            // parse parameter type
            var parameterType = Autodesk.Revit.DB.ParameterType.Text;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.ParameterType>(type, out parameterType))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

            // parse parameter group
            var parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.BuiltInParameterGroup>(group, out parameterGroup))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

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
                CategorySet categories = (categoryList == null) ? AllCategories() : ToCategorySet(categoryList);

                // Create new parameter group if it does not exist yet
                DefinitionGroup groupDef = 
                    document.Application.OpenSharedParameterFile()
                    .Groups.get_Item(groupName);

                if (groupDef == null)
                {
                    groupDef = 
                        document.Application.OpenSharedParameterFile()
                        .Groups.Create(groupName);
                }

                // If the parameter definition does not exist yet, create it
                if (groupDef.Definitions.get_Item(parameterName) == null)
                {
                    ExternalDefinition def = 
                        groupDef.Definitions.Create
                        (new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;

                    // Apply instance or type binding
                    Binding bin = (instance) ? 
                        (Binding)document.Application.Create.NewInstanceBinding(categories) : 
                        (Binding)document.Application.Create.NewTypeBinding(categories);

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
        /// Create a new Project Parameter in this current Revit document for all applicable categories
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        public static void CreateProjectParameterForAllCategories(string parameterName, string groupName, string type, string group, bool instance)
        {
            CreateProjectParameter(parameterName, groupName, type, group, instance, null);
        }

        /// <summary>
        /// Create a new Project Parameter in this current Revit document
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="type">Parameter Type</param>
        /// <param name="group">Parameter Group</param>
        /// <param name="instance">Is instance parameter, otherwise its a type parameter</param>
        /// <param name="categoryList">List of categories this parameter applies to. If no category is supplied, all possible categories are selected</param>
        public static void CreateProjectParameter(string parameterName, string groupName, string type, string group, bool instance, System.Collections.Generic.IEnumerable<Category> categoryList)
        {
            // parse parameter type
            var parameterType = Autodesk.Revit.DB.ParameterType.Text;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.ParameterType>(type, out parameterType))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

            // parse parameter group
            var parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DATA;
            if (!System.Enum.TryParse<Autodesk.Revit.DB.BuiltInParameterGroup>(group, out parameterGroup))
                throw new System.Exception(Properties.Resources.ParameterTypeNotFound);

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
                CategorySet categories = (categoryList == null) ? AllCategories() : ToCategorySet(categoryList);

                // create a new shared parameter, since the file is empty everything has to be created from scratch
                ExternalDefinition def = 
                    document.Application.OpenSharedParameterFile()
                    .Groups.Create(groupName).Definitions.Create(
                    new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;

                // Create an instance or type binding
                Binding bin = (instance) ? 
                    (Binding)document.Application.Create.NewInstanceBinding(categories) : 
                    (Binding)document.Application.Create.NewTypeBinding(categories);

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

        /// <summary>
        /// Get a CategorySet for all categories
        /// </summary>
        /// <returns></returns>
        internal static CategorySet AllCategories()
        {
            Document document = Application.Document.Current.InternalDocument;

            // Apply selected parameter categories
            CategorySet categories = document.Application.Create.NewCategorySet();


            // Walk thru all categories and add them if they allow bound parameters
            foreach (Autodesk.Revit.DB.Category cat in document.Settings.Categories)
            {
                if (cat.AllowsBoundParameters)
                {
                    categories.Insert(cat);
                }
            }


            return categories;
        }

        /// <summary>
        /// Convert a list of categories to a category set
        /// </summary>
        /// <param name="categoryList"></param>
        /// <returns></returns>
        internal static CategorySet ToCategorySet(System.Collections.Generic.IEnumerable<Category> categoryList)
        {
            Document document = Application.Document.Current.InternalDocument;

            // Apply selected parameter categories
            CategorySet categories = document.Application.Create.NewCategorySet();


            foreach (Category category in categoryList)
            {
                if (category.InternalCategory.AllowsBoundParameters)
                {
                    categories.Insert(category.InternalCategory);
                }
            }


            return categories;
        }

    }
}
