using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;
using Revit.GeometryConversion;
using System;

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
        /// Get the parameter's element Id
        /// </summary>
        public long Id
        {
            get { return InternalParameter.Id.Value; }
        }

        /// <summary>
        /// Get the parameter's SpecType
        /// </summary>
        public SpecType SpecType
        {
            get
            {
                var forgeTypeId = InternalParameter.Definition.GetDataType();

                return string.IsNullOrEmpty(forgeTypeId.TypeId)? null : SpecType.FromExisting(forgeTypeId);
            }
        }

        /// <summary>
        /// Get the Parameter's Group Type
        /// </summary>
        public GroupType GroupType
        {
            get
            {
                var forgeTypeId = InternalParameter.Definition.GetGroupTypeId();

                return string.IsNullOrEmpty(forgeTypeId.TypeId) ? null : GroupType.FromExisting(forgeTypeId);
            }
        }

        /// <summary>
        /// Get the Parameter's Unit Type
        /// </summary>
        public DynamoUnits.Unit Unit
        {
            get
            {
                var unitTypeId = InternalParameter.GetUnitTypeId();
                var cleanUnitTypeId = UnitConverter.ConvertRevitInternalUnitTypeIdToSupportedUnitTypeId(unitTypeId);

                return DynamoUnits.Unit.ByTypeID(cleanUnitTypeId.TypeId);
            }
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
                .OrderBy(x => x.InternalParameter.Id.Value)
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
        /// <param name="specType">The type of new parameter.</param>
        /// <param name="groupType">The type of the group to which the parameter belongs.</param>
        /// <param name="instance">True if it's an instance parameter, otherwise it's a type parameter</param>
        public static void CreateSharedParameterForAllCategories(string parameterName, string groupName, SpecType specType, GroupType groupType, bool instance)
        {
            CreateSharedParameter(parameterName, groupName, specType, groupType, instance, null);
        }

        /// <summary>
        /// Create a new Shared Parameter in the current Revit document
        /// </summary>
        /// <param name="parameterName">Name</param>
        /// <param name="groupName">Group of the parameter for shared parameters</param>
        /// <param name="specType">The type of new parameter.</param>
        /// <param name="groupType">The type of the group to which the parameter belongs.</param>
        /// <param name="instance">True if it's an instance parameter, otherwise it's a type parameter</param>
        /// <param name="categoryList">A list of categories this parameter applies to. If no category is supplied, all possible categories are selected</param>
        public static void CreateSharedParameter(string parameterName, string groupName, SpecType specType, GroupType groupType, bool instance, System.Collections.Generic.IEnumerable<Category> categoryList)
        {
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
                        (new ExternalDefinitionCreationOptions(parameterName, specType.InternalForgeTypeId)) as ExternalDefinition;

                    // Apply instance or type binding
                    Binding bin = (instance) ?
                        (Binding)document.Application.Create.NewInstanceBinding(categories) :
                        (Binding)document.Application.Create.NewTypeBinding(categories);

                    document.ParameterBindings.Insert(def, bin, groupType.InternalForgeTypeId);
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
