using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    public static class WhereType
    {

        #region In
        /// <summary>
        /// In查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool In(this ValueType param, IEnumerable array)
        {
            return true;
        }
        /// <summary>
        /// Not In查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool NotIn(this ValueType param, IEnumerable array)
        {
            return true;
        }
        /// <summary>
        /// In查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool In(this string param, IEnumerable array)
        {
            return true;
        }
        /// <summary>
        /// Not In查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool NotIn(this string param, IEnumerable array)
        {
            return true;
        }
        #endregion

        #region Like
        /// <summary>
        /// Like查询
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Like(this string param1, string param2)
        {
            return true;
        }
        /// <summary>
        /// Not Like查询
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool NotLike(this string param1, string param2)
        {
            return true;
        }
        #endregion

        #region <,>,<=,>=,<>,=
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Gt(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 大于等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Ge(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 小于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Lt(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Le(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Ne(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Ne(this string param1, string param2)
        {
            return true;
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Eq(this ValueType param1, ValueType param2)
        {
            return true;
        }
        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        public static bool Eq(this string param1, string param2)
        {
            return true;
        }
        #endregion

        #region IsNull
        /// <summary>
        /// Is Null查询
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(this object value)
        {
            return true;
        }
        /// <summary>
        /// Is Not Null查询
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object value)
        {
            return true;
        }
        #endregion

        #region Between
        /// <summary>
        /// Between查询
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool Between(this ValueType value, ValueType min, ValueType max)
        {
            return true;
        }
        /// <summary>
        /// Not Between
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool NotBetween(this ValueType value, ValueType min, ValueType max)
        {
            return true;
        }
        #endregion

        #region Regexp
        /// <summary>
        /// 正则匹配
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regexp"></param>
        /// <returns></returns>
        public static bool Regexp(this string text, string regexp)
        {
            return true;
        }
        /// <summary>
        /// 正则不匹配
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regexp"></param>
        /// <returns></returns>
        public static bool NotRegexp(this string text, string regexp)
        {
            return true;
        }
        #endregion

        #region Merthod
        public static string GetOperator(string method)
        {
            var name = string.Empty;
            switch (method)
            {
                case "In":
                    name = "IN";
                    break;
                case "NotIn":
                    name = "NOT IN";
                    break;
                case "Like":
                    name = "LIKE";
                    break;
                case "NotLike":
                    name = "NOT LIKE";
                    break;              
                case "Gt":
                    name = ">";
                    break;
                case "Ge":
                    name = ">=";
                    break;
                case "Lt":
                    name = "<";
                    break;
                case "Le":
                    name = "<=";
                    break;
                case "Ne":
                    name = "<>";
                    break;
                case "Eq":
                    name = "=";
                    break;
                case "IsNull":
                    name = "IS NULL";
                    break;
                case "IsNotNull":
                    name = "IS NOT NULL";
                    break;
                case "Between":
                    name = "BETWEEN";
                    break;
                case "NotBetween":
                    name = "NOT BETWEEN";
                    break;
                case "Regexp":
                    name = "REGEXP";
                    break;
                case "NotRegexp":
                    name = "NOT REGEXP";
                    break;
            }
            return name;
        }
        public static string GetOperator(ExpressionType node)
        {
            var name = string.Empty;
            switch (node)
            {
                case ExpressionType.AndAlso:
                    name = "AND";
                    break;
                case ExpressionType.Equal:
                    name = "=";
                    break;
                case ExpressionType.GreaterThan:
                    name = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    name = ">=";
                    break;
                case ExpressionType.LessThan:
                    name = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    name = "<=";
                    break;
                case ExpressionType.Not:
                    name = "NOT";
                    break;
                case ExpressionType.NotEqual:
                    name = "<>";
                    break;
                case ExpressionType.OrElse:
                    name = "OR";
                    break;
                case ExpressionType.Add:
                    name = "+";
                    break;
                case ExpressionType.SubtractChecked:
                    name = "-";
                    break;
                case ExpressionType.Multiply:
                    name = "*";
                    break;
                case ExpressionType.Divide:
                    name = "/";
                    break;
                case ExpressionType.Default:
                    name = string.Empty;
                    break;
            }
            return name;
        }
        #endregion

    }
}
