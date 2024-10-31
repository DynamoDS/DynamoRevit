using System;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;

namespace Revit.Filter
{
    /// <summary>
    /// Revit Filter Rule
    /// </summary>
    public class FilterRule
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FilterRule InternalFilterRule
        { 
            get; set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        /// <param name="internalFilterRule"></param>
        internal FilterRule(Autodesk.Revit.DB.FilterRule internalFilterRule)
        {
            this.InternalFilterRule = internalFilterRule;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// RuleType Enumeration
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum RuleType
        {
            BeginsWith,
            Contains,
            EndsWith,
            Equals,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual,
            NotBeginsWith,
            NotContains,
            NotEndsWith,
            NotEquals,
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Prefix for reflection call of Create Rule
        /// </summary>
        const string CreatePrefix = "Create";

        /// <summary>
        /// Suffix for reflection call of Create Rule 
        /// </summary>
        const string RuleSuffix = "Rule";

        /// <summary>
        /// Create a new Filter Rule
        /// </summary>
        /// <param name="type">Filter Rule Type</param>
        /// <param name="value">Value to check</param>
        /// <param name="parameter">Parameter to filter</param>
        /// <returns></returns>
        public static FilterRule ByRuleType(string type, object value, Elements.Parameter parameter)
        {
            RuleType ruletype = RuleType.Equals;
            if (!Enum.TryParse<RuleType>(type, out ruletype))
            {
                ruletype = RuleType.Equals;
            }


            ElementId parameterId = parameter.InternalParameter.Id;

            object convertedValue = null;
            try
            {
                Type parameterType = Revit.Elements.InternalUtilities.ElementUtils.GetParameterType(parameter);
                convertedValue = Convert.ChangeType(value, parameterType);
            }
            catch (Exception)
            {
                throw new ApplicationException(Properties.Resources.InputValueParameterValueTypeMismatch);
            }        

            // assemble the method name to construct a new filter using the FilterType
            string methodname = string.Format("{0}{1}{2}", new object[] { CreatePrefix , type, RuleSuffix});
    
            // all of the following FilterType constructors are handled the same
            if (
                ruletype == RuleType.Equals ||
                ruletype == RuleType.NotEquals ||
                ruletype == RuleType.Greater ||
                ruletype == RuleType.Less ||
                ruletype == RuleType.GreaterOrEqual ||
                ruletype == RuleType.LessOrEqual
                )
            {
                if (convertedValue.GetType() == typeof(int))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(int) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (int)convertedValue }));
                    }
                }
                else if (convertedValue.GetType() == typeof(double))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(double), typeof(double) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (double)convertedValue, 0 }));
                    }
                }
                else if (convertedValue.GetType() == typeof(string))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(string) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (string)convertedValue }));
                    }
                }
            }
            else
            {
                if (convertedValue.GetType() == typeof(string))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(string), typeof(bool) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (string)convertedValue, true }));
                    }
                }
            }


            return null;
        }

        #endregion

    }



}
