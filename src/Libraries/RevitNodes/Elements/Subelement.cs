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
using System.Windows.Media.Animation;

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
        /// Obtain all parameter ids of the subelement.
        /// </summary>
        public IList<ElementId> GetAllParameters()
        {
            return InternalSubelement.GetAllParameters();
        }

        /// <summary>
        /// Get the value of the subelement parameter.
        /// </summary>
        /// <param name="paramId">The parameter id to be retrieved, as ElementId or Int.</param>
        /// <returns></returns>
        public object GetParameterValue(object paramId)
        {
            ElementId idParam = paramId as ElementId;
            if(idParam == null)
            {
                Int64 longValue = (Int64)paramId;
                idParam = new ElementId(longValue);
                if (idParam == null)
                    return string.Empty;
            }
            return GetObjectValue( InternalSubelement.GetParameterValue(idParam) );
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
        /// <param name="paramId">The parameter id to be set, as ElementId</param>
        /// <param name="value">The new parameter value to be set.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public Subelement SetParameterValue(ElementId paramId, ParameterValue value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            InternalSubelement.SetParameterValue(paramId, value);

            TransactionManager.Instance.TransactionTaskDone();
            return this;
        }

        /// <summary>
        /// Set the value of the specified subelement parameter. 
        /// </summary>
        /// <param name="paramId">The parameter id to be set.</param>
        /// <param name="value">The new parameter value to be set.</param>
        /// <returns></returns>
        public Subelement SetDoubleParameterValue(Int64 paramId, double value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            ElementId idParam = new ElementId(paramId);
            DoubleParameterValue paramValue = new DoubleParameterValue(value);
            InternalSubelement.SetParameterValue(idParam, paramValue);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Set the value of the specified subelement parameter. 
        /// </summary>
        /// <param name="paramId">The parameter id to be set.</param>
        /// <param name="value">The new parameter value to be set.</param>
        /// <returns></returns>
        public Subelement SetIntegerParameterValue(Int64 paramId, int value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            ElementId idParam = new ElementId(paramId);
            IntegerParameterValue paramValue = new IntegerParameterValue(value);
            InternalSubelement.SetParameterValue(idParam, paramValue);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Set the value of the specified subelement parameter. 
        /// </summary>
        /// <param name="paramId">The parameter id to be set.</param>
        /// <param name="value">The new parameter value to be set.</param>
        /// <returns></returns>
        public Subelement SetElementIdParameterValue(Int64 paramId, Int64 value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            ElementId idParam = new ElementId(paramId);
            ElementIdParameterValue paramValue = new ElementIdParameterValue(new ElementId(value));
            InternalSubelement.SetParameterValue(idParam, paramValue);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        /// <summary>
        /// Set the value of the specified subelement parameter. 
        /// </summary>
        /// <param name="paramId">The parameter id to be set.</param>
        /// <param name="value">The new parameter value to be set.</param>
        /// <returns></returns>
        public Subelement SetStringParameterValue(Int64 paramId, string value)
        {
            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);

            ElementId idParam = new ElementId(paramId);
            StringParameterValue paramValue = new StringParameterValue(value);
            InternalSubelement.SetParameterValue(idParam, paramValue);

            TransactionManager.Instance.TransactionTaskDone();

            return this;
        }

        #endregion

    }
}
