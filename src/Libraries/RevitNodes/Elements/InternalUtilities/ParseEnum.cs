using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;

namespace Revit.Elements
{
    /// <summary>
    /// Parse Revit Enum
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class ParseEnum
    {
        /// <summary>
        /// Parse any Revit Enum by String
        /// </summary>
        /// <param name="value">enum string value</param>
        /// <param name="typeName">full type name</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static object ByString(string value, string typeName)
        {
            // Get Revit Assembly
            var revitAssembly = System.Reflection.Assembly.GetAssembly(typeof(Autodesk.Revit.DB.Document));

            // Get Type from Assembly by string
            Type type = revitAssembly.GetType(typeName);

            if (type != null)
            {
                return Enum.Parse(type, value);
            }
            else
            {
                return null;
            }
        }
    }
}
