using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamoServices;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Elements
{
    [RegisterForTrace]
    public class ToposolidType : ElementType
    {
        #region Internal properties

        internal Autodesk.Revit.DB.ToposolidType InternalToposolidType => InternalElementType as Autodesk.Revit.DB.ToposolidType;

        #endregion

        #region Private constructors

        /// <summary>
        /// Private constructor for DSToposolidType
        /// </summary>
        /// <param name="toposolidType"></param>
        private ToposolidType(Autodesk.Revit.DB.ToposolidType toposolidType) : base(toposolidType)
        {
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Select a toposolid type from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new ToposolidType ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.ToposolidType>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception(Properties.Resources.ToposolidTypeNotFound);
            }

            return new ToposolidType(type)
            {
                IsRevitOwned = true
            };
        }

        #endregion

        #region Internal static constructors

        /// <summary>
        /// Wrap an element in the associated DS type
        /// </summary>
        /// <param name="toposolidType">The ToposolidType</param>
        /// <param name="isRevitOwned"></param>
        /// <returns></returns>
        internal static ToposolidType FromExisting(Autodesk.Revit.DB.ToposolidType toposolidType, bool isRevitOwned)
        {
            return new ToposolidType(toposolidType)
            {
                IsRevitOwned = isRevitOwned
            };
        }

        #endregion
    }
}
