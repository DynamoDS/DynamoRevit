using System;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace Revit.Elements
{
    public class Category
    {
        #region internal constructors

        internal Category(Autodesk.Revit.DB.Category category)
        {
            internalCategory = category;
        }

        #endregion

        #region private members

        private readonly Autodesk.Revit.DB.Category internalCategory;

        #endregion

        #region public properties

        /// <summary>
        /// The name of the Category.   
        /// </summary>
        public string Name
        {
            get
            {
                var parent = internalCategory.Parent;
                if(parent == null)
                {
                    return internalCategory.Name;
                }
                else
                {
                    return internalCategory.Parent.Name + " - " + internalCategory.Name;
                }
            }
        }

        /// <summary>
        /// The Id of the category.
        /// </summary>
        public int Id
        {
            get
            {
                return InternalCategory.Id.IntegerValue;
            }
        }

        #endregion

        internal Autodesk.Revit.DB.Category InternalCategory
        {
            get { return internalCategory; }
        }

        #region public static constructors

        /// <summary>
        /// Gets a Revit category by the built-in category name.
        /// </summary>
        /// <param name="name">The built in category name.</param>
        /// <returns></returns>
        public static Category ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            // Find category using localized name
            Settings documentSettings = DocumentManager.Instance.CurrentDBDocument.Settings;
            var groups = documentSettings.Categories;

            Autodesk.Revit.DB.Category category = null;
            var splits = name.Split('-');
            if(splits.Count() > 1)
            {
                var parentName = splits[0].TrimEnd(' ');
                if(groups.Contains(parentName))
                {
                    var parentCategory = groups.get_Item(parentName);
                    if(parentCategory != null)
                    {
                        var subName = splits[1].TrimStart(' ');
                        if(parentCategory.SubCategories.Contains(subName))
                        {
                            category = parentCategory.SubCategories.get_Item(subName);
                        }
                    }
                }
            }
            else if (groups.Contains(name))
            {
                category = groups.get_Item(name);
            }
            else
            {
                // Fall back
                // Use category enum name with or without OST_ prefix
                var fullName = name.Length > 3 && name.Substring(0, 4) == "OST_" ? name : "OST_" + name;
                var names = Enum.GetNames(typeof(BuiltInCategory));
                if(System.Array.Exists(names, entry => entry == fullName))
                {
                    var builtInCat = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), fullName);
                    category = groups.get_Item(builtInCat);
                }
            }

            if (category == null)
            {
                throw new Exception(Properties.Resources.InvalidCategory);
            }

            return new Category(category);
        }

        #endregion

        public override string ToString()
        {
            return internalCategory != null ? Name : string.Empty;
        }
    }
}
