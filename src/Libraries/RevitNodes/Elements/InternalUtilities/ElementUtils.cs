using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;


namespace Revit.Elements.InternalUtilities
{
    [IsVisibleInDynamoLibrary(false)]
    public class ElementUtils
    {
        /// <summary>
        /// This function checks if the name ends with "(num)". Here num is a integer.
        /// If yes, it will replace "(num)" with "(num+1)". Here num+1 is the form of the
        /// evaluated integer. Otherwise, it will append "(1)" at the end of the name.
        /// For example:
        /// This function will change the name from "abc(2)" to "abc(3)",
        /// from "abc" to "abc(1)".
        /// </summary>
        /// <param name="name"></param>
        [IsVisibleInDynamoLibrary(false)]
        public static void UpdateLevelName(ref string name)
        {
            if (name.EndsWith(")"))
            {
                int index = name.LastIndexOf("(");
                if (index < 0)
                {
                    name += "(1)";
                }
                else
                {
                    int length = name.Length - index - 2;
                    string strNumber = name.Substring(index + 1, length);
                    int number = 0;
                    if (int.TryParse(strNumber, out number))
                    {
                        number = number + 1;
                        name = name.Substring(0, index) + "(" + number.ToString() + ")";
                    }
                    else
                    {
                        name += "(1)";
                    }
                }
            }
            else
            {
                name += "(1)";
            }
        }


        /// <summary>
        /// Get a revit parameters value
        /// </summary>
        /// <param name="param">Revit parameter</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static object GetParameterValue(Autodesk.Revit.DB.Parameter param)
        {
            object result;

            switch (param.StorageType)
            {
                case StorageType.ElementId:
                    int valueId = param.AsElementId().IntegerValue;
                    if (valueId > 0)
                    {
                        // When the element is obtained here, to convert it to our element wrapper, it
                        // need to be figured out whether this element is created by us. Here the existing
                        // element wrappers will be checked. If there is one, its property to specify
                        // whether it is created by us will be followed. If there is none, it means the
                        // element is not created by us.
                        var elem = ElementIDLifecycleManager<int>.GetInstance().GetFirstWrapper(valueId) as Element;
                        result = ElementSelector.ByElementId(valueId, elem == null ? true : elem.IsRevitOwned);
                    }
                    else
                    {
                        int paramId = param.Id.IntegerValue;
                        if (paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM || paramId == (int)BuiltInParameter.ELEM_CATEGORY_PARAM_MT)
                        {
                            var categories = DocumentManager.Instance.CurrentDBDocument.Settings.Categories;
                            result = new Category(categories.get_Item((BuiltInCategory)valueId));
                        }
                        else
                        {
                            // For other cases, return a localized string
                            result = param.AsValueString();
                        }
                    }
                    break;
                case StorageType.String:
                    result = param.AsString();
                    break;
                case StorageType.Integer:
                    result = param.AsInteger();
                    break;
                case StorageType.Double:
                    var paramType = param.Definition.ParameterType;
                    if (Element.IsConvertableParameterType(paramType))
                        result = param.AsDouble() * Revit.GeometryConversion.UnitConverter.HostToDynamoFactor(
                            Element.ParameterTypeToUnitType(paramType));
                    else
                        result = param.AsDouble();
                    break;
                default:
                    throw new Exception(string.Format(Properties.Resources.ParameterWithoutStorageType, param));
            }

            return result;
        }
    }
}
