using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;

namespace Revit.Schedules
{
    /// <summary>
    /// Revit Schedule Filter
    /// </summary>
    public class ScheduleFilter
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.ScheduleFilter InternalScheduleFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        /// <param name="internalScheduleFilter"></param>
        internal ScheduleFilter(Autodesk.Revit.DB.ScheduleFilter internalScheduleFilter)
        {
            this.InternalScheduleFilter = internalScheduleFilter;
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="filterType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ScheduleFilter ByFieldTypeAndValue(ScheduleField field, string filterType, object value)
        {
            var ft = (Autodesk.Revit.DB.ScheduleFilterType)System.Enum.Parse(typeof(Autodesk.Revit.DB.ScheduleFilterType), filterType);
            Autodesk.Revit.DB.ScheduleFilter filter;

            if (value.GetType() == typeof(int))
            {
                filter = new Autodesk.Revit.DB.ScheduleFilter(field.InternalScheduleField.FieldId, ft, (int)value);
            }
            else if (value.GetType() == typeof(double))
            {
                filter = new Autodesk.Revit.DB.ScheduleFilter(field.InternalScheduleField.FieldId, ft, (double)value);
            }
            else if (value.GetType() == typeof(string))
            {
                filter = new Autodesk.Revit.DB.ScheduleFilter(field.InternalScheduleField.FieldId, ft, (string)value);
            }
            else
            {
                filter = new Autodesk.Revit.DB.ScheduleFilter(field.InternalScheduleField.FieldId, ft, ((Revit.Elements.Element)value).InternalElement.Id);
            }

            return new ScheduleFilter(filter);
        }

        #endregion

        /// <summary>
        /// Filter Type
        /// </summary>
        public string FilterType
        {
            get
            {
                return this.InternalScheduleFilter.FilterType.ToString();
            }
        }

        /// <summary>
        /// Related Schedule Field Id
        /// </summary>
        public int FiledId
        {
            get
            {
                return this.InternalScheduleFilter.FieldId.IntegerValue;
            }
        }

        /// <summary>
        /// Value assigned to Schedule Filter
        /// </summary>
        public object Value
        {
            get
            {
                if (this.InternalScheduleFilter.IsStringValue)
                {
                    return this.InternalScheduleFilter.GetStringValue();
                }
                else if (this.InternalScheduleFilter.IsDoubleValue)
                {
                    return this.InternalScheduleFilter.GetDoubleValue();
                }
                else if (this.InternalScheduleFilter.IsIntegerValue)
                {
                    return this.InternalScheduleFilter.GetIntegerValue();
                }
                else if (this.InternalScheduleFilter.IsElementIdValue)
                {
                    return this.InternalScheduleFilter.GetElementIdValue().IntegerValue;
                }
                else
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("ScheduleFilter(FilterType: {0}, FieldId: {1}, Value: {2})", this.FilterType, this.FiledId, this.Value);
        }

        public override bool Equals(object obj)
        {
            ScheduleFilter item = obj as ScheduleFilter;

            if (item == null)
            {
                return false;
            }

            return (this.FilterType.Equals(item.FilterType) && this.FiledId.Equals(item.FiledId) && this.Value.Equals(item.Value));
        }

        public override int GetHashCode()
        {
            return this.FilterType.GetHashCode() ^ this.FiledId.GetHashCode() ^ this.Value.GetHashCode();
        }
    }

    //[IsVisibleInDynamoLibrary(false)]
    //public class ScheduleFilterComparer : IEqualityComparer<ScheduleFilter>
    //{
    //    public bool Equals(ScheduleFilter x, ScheduleFilter y)
    //    {
    //        if (x == null && y == null)
    //            return true;
    //        else if (x == null | y == null)
    //            return false;
    //        else if (x.FilterType == y.FilterType && x.FiledId == y.FiledId && x.Value == y.Value)
    //            return true;
    //        else
    //            return false;
    //    }

    //    public int GetHashCode(ScheduleFilter x)
    //    {
    //        return x.FilterType.GetHashCode() ^ x.FiledId.GetHashCode() ^ x.Value.GetHashCode();
    //    }
    //}
}
