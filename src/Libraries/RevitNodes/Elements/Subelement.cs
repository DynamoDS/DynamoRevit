using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;
using Revit.GeometryConversion;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;
using System;
using Autodesk.Revit.DB.Architecture;

namespace Revit.Elements
{
    public class Subelement
    {
        internal Autodesk.Revit.DB.Subelement InternalSubelement { get; }

        internal Subelement(Autodesk.Revit.DB.Subelement internalSubelem)
        {
            this.InternalSubelement = internalSubelem;
        }

        #region Properties

        /// <summary>
        /// The element in which the subelement resides.
        /// </summary>
        public Element Element
        {
            get 
            { 
                var mainElem = InternalSubelement.Element;
                if (mainElem == null)
                    return null;
                return mainElem.ToDSType(true);
            }
        }

        /// <summary>
        /// The category of the subelement.
        /// </summary>
        public Category Category
        {
            get { return new Category(InternalSubelement.Category); }
        }

        #endregion

        #region Parameters
        /// <summary>
        /// Obtain all of the Parameters from an Element.
        /// </summary>
        public IList<ElementId> GetAllParameters()
        {
            return InternalSubelement.GetAllParameters();
        }

        /// <summary>
        /// Get the value of the subelement parameter.
        /// </summary>
        /// <param name="paramId">The parameter id to be retrieved.</param>
        /// <returns></returns>
        public object GetParameterValue(ElementId paramId)
        {
            return GetObjectValue( InternalSubelement.GetParameterValue(paramId) );
        }

        private object GetObjectValue(ParameterValue value)
        {
            IntegerParameterValue intValue = value as IntegerParameterValue;
            if (intValue != null)
                return intValue.Value;

            DoubleParameterValue doubleValue = value as DoubleParameterValue;
            if (doubleValue != null)
                return doubleValue.Value;

            StringParameterValue strValue = value as StringParameterValue;
            if (strValue != null)
                return strValue.Value;

            ElementIdParameterValue elemidValue = value as ElementIdParameterValue;
            if (elemidValue != null)
                return elemidValue.Value;

            return string.Empty;
        }

        /// <summary>
        /// Set the value of the specified subelement parameter. 
        /// </summary>
        /// <param name="paramId">The parameter id to be set.</param>
        /// <param name="value">The new parameter value to be set.</param>
        public void SetParameterValue(ElementId paramId, ParameterValue value)
        {
            InternalSubelement.SetParameterValue(paramId, value);
        }

        #endregion

    }
}
