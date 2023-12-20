using System;
using System.Linq;

using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using System.Collections.Generic;

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
        public long Id
        {
            get
            {
                return InternalCategory.Id.Value;
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
            Autodesk.Revit.DB.Category category = null;
            category = GetCategory(name);

            if (category == null)
            {
                throw new ArgumentException(Properties.Resources.InvalidCategory);
            }

            return new Category(category);
        }

        /// <summary>
        /// Gets Revit Built-in category from current document based on category Id
        /// </summary>
        /// <param name="id">Category Id as Integer value</param>
        /// <returns>Category if present in current document.</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static Category ById(long id)
        {
            try
            {
                var document = DocumentManager.Instance.CurrentDBDocument;
                BuiltInCategory categoryId = (BuiltInCategory)id;
                Autodesk.Revit.DB.Category category = Autodesk.Revit.DB.Category.GetCategory(document, categoryId);
                if(null == category)
                    throw new ArgumentException(Properties.Resources.InvalidCategory);

                return new Category(category);
            }
            catch
            {
                throw new ArgumentException(Properties.Resources.InvalidCategory);
            }
        }

        #endregion

        public override string ToString()
        {
            return internalCategory != null ? Name : string.Empty;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Category;

            if (item == null)
            {
                return false;
            }
            else
            {
                return this.Id.Equals(item.Id);
            }
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        private static Autodesk.Revit.DB.Category GetCategory(String name)
        {
            Autodesk.Revit.DB.Category category = null;
            
            var splits = name.Split('-');

            // search by ui name first, that can be "{parent name} - {name}" or only the "{name}"
            category = FindCategory((Autodesk.Revit.DB.Category cat) => { return name.Equals(new Revit.Elements.Category(cat).Name); });
            if(category != null)
            {
                return category;
            }
            else if (splits.Count() > 1)
            {
                var indexs = FindAllChars(name, '-');
                foreach(var index in indexs)
                {
                    var parentName = name.Substring(0, index).TrimEnd(' ');
                    var subName = name.Substring(index + 1).TrimStart(' ');
                    Autodesk.Revit.DB.Category parentCategory = FindCategory((Autodesk.Revit.DB.Category cat) => { return parentName.Equals(cat.Name); });
                    if(parentCategory != null)
                    {
                        if(parentCategory.SubCategories.Contains(subName))
                        {
                            category = parentCategory.SubCategories.get_Item(subName);
                            break;
                        }
                    }
                }                
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
                    category = Autodesk.Revit.DB.Category.GetCategory(DocumentManager.Instance.CurrentDBDocument, builtInCat);
                }
            }

            return category;
        }

        private static List<int> FindAllChars(String source, char specifiedChar)
        {
            List<int> CharIndex = new List<int>();

            var splits = source.Split(specifiedChar);
            int index = -1;
            for (int i = 0; i < splits.Count() - 1; i++)
            {
               index = index + splits[i].Length + 1;
               CharIndex.Add(index);
            }

            return CharIndex;
        }

        private static Autodesk.Revit.DB.Category FindCategory(Predicate<Autodesk.Revit.DB.Category> pred)
        {
            var document = DocumentManager.Instance.CurrentDBDocument;

            foreach (BuiltInCategory categoryId in Enum.GetValues(typeof(BuiltInCategory)))
            {
                Autodesk.Revit.DB.Category tempCategory;

                try
                {
                    tempCategory = Autodesk.Revit.DB.Category.GetCategory(document, categoryId);
                }
                catch
                {
                    // We get here for internal/deprecated categories
                    continue;
                }

                if(tempCategory != null && pred(tempCategory))
                {
                    return tempCategory;
                }
            }

            return null;
        }
    }
}
