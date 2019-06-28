using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Common
{
    public static class ExtensionUtil
    {
        #region extension
        public static bool In(this ValueType column, IEnumerable enumerable)
        {
            return true;
        }
        public static bool In(this ValueType column, params ValueType[] value)
        {
            return true;
        }
        public static bool NotIn(this ValueType column, IEnumerable enumerable)
        {
            return true;
        }
        public static bool NotIn(this ValueType column, params ValueType[] value)
        {
            return true;
        }
        public static bool In(this string column, IEnumerable enumerable)
        {
            return true;
        }
        public static bool In(this string column, params string[] value)
        {
            return true;
        }
        public static bool NotIn(this string column, IEnumerable enumerable)
        {
            return true;
        }
        public static bool NotIn(this string column, params string[] value)
        {
            return true;
        }
        public static bool Like(this string column, string text)
        {
            return true;
        }
        public static bool NotLike(this string column, string text)
        {
            return true;
        }
        public static bool Regexp(string column, string regexp)
        {
            return true;
        }
        public static bool NotRegexp(string column, string regexp)
        {
            return true;
        }
        public static bool IsNull<T>(T column)
        {
            return true;
        }
        public static bool IsNotNull<T>(T column)
        {
            return true;
        }
        public static bool Between<T>(T column, T value1, T value2)
        {
            return true;
        }
        public static bool NotBetween<T>(T column, T value1, T value2)
        {
            return true;
        }
        #endregion

        #region utils
        public static string GetOperator(string operatorType)
        {
            switch (operatorType)
            {
                case "In":
                    operatorType = "IN";
                    break;
                case "NotIn":
                    operatorType = "NOT IN";
                    break;
                case "Like":
                    operatorType = "LIKE";
                    break;
                case "NotLike":
                    operatorType = "NOT LIKE";
                    break;
                case "IsNull":
                    operatorType = "IS NULL";
                    break;
                case "IsNotNull":
                    operatorType = "IS NOT NULL";
                    break;
                case "Between":
                    operatorType = "BETWEEN";
                    break;
                case "NotBetween":
                    operatorType = "NOT BETWEEN";
                    break;
                case "Regexp":
                    operatorType = "REGEXP";
                    break;
                case "NotRegexp":
                    operatorType = "NOT REGEXP";
                    break;
            }
            return operatorType;
        }

        public static string GetOperator(ExpressionType type)
        {
            var condition = string.Empty;
            switch (type)
            {
                case ExpressionType.Add:
                    condition = "+";
                    break;
                case ExpressionType.Subtract:
                    condition = "-";
                    break;
                case ExpressionType.Multiply:
                    condition = "*";
                    break;
                case ExpressionType.Divide:
                    condition = "/";
                    break;
                case ExpressionType.Modulo:
                    condition = "%";
                    break;
                case ExpressionType.Equal:
                    condition = "=";
                    break;
                case ExpressionType.NotEqual:
                    condition = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    condition = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    condition = ">=";
                    break;
                case ExpressionType.LessThan:
                    condition = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    condition = "<=";
                    break;
                case ExpressionType.OrElse:
                    condition = "OR";
                    break;
                case ExpressionType.AndAlso:
                    condition = "AND";
                    break;
                case ExpressionType.Not:
                    condition = "NOT";
                    break;

            }
            return condition;
        }
        #endregion
    }
}
