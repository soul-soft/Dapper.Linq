using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper
{
    /// <summary>
    /// 数据库操作符
    /// </summary>
    public class Operator
    {
        /// <summary>
        /// in
        /// </summary>
        /// <typeparam name="T">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool In<T>(T column, IEnumerable<T> values) => default;
        /// <summary>
        /// in(低性能)
        /// </summary>
        /// <typeparam name="T">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool In<T>(T column, params T[] values) => default;
        /// <summary>
        /// not in
        /// </summary>
        /// <typeparam name="T">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotIn<T>(T column, IEnumerable<T> values) => default;
        /// <summary>
        /// not in(低性能)
        /// </summary>
        /// <typeparam name="T">类型推断</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotIn<T>(T column, params T[] values) => default;
        /// <summary>
        /// like %value%
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool Contains(string column, string value) => default;
        /// <summary>
        /// not like %value%
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotContains(string column, string value) => default;
        /// <summary>
        /// like value%
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool StartsWith(string column, string value) => default;
        /// <summary>
        /// not like value%
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotStartsWith(string column, string value) => default;
        /// <summary>
        /// like %value
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool EndsWith(string column, string value) => default;
        /// <summary>
        /// not like %value
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotEndsWith(string column, string value) => default;
        /// <summary>
        /// regex value
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool Regexp(string column, string value) => default;
        /// <summary>
        /// not regex value
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">参数</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "<挂起>")]
        public static bool NotRegexp(string column, string value) => default;
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string ResovleExpressionType(ExpressionType type)
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
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string ResovleExpressionType(string type)
        {
            switch (type)
            {
                case nameof(Operator.In):
                    type = "IN";
                    break;
                case nameof(Operator.NotIn):
                    type = "NOT IN";
                    break;
                case nameof(Operator.Contains):
                case nameof(Operator.StartsWith):
                case nameof(Operator.EndsWith):
                    type = "LIKE";
                    break;
                case nameof(Operator.NotContains):
                case nameof(Operator.NotStartsWith):
                case nameof(Operator.NotEndsWith):
                    type = "NOT LIKE";
                    break;
                case nameof(Operator.Regexp):
                    type = "REGEXP";
                    break;
                case nameof(Operator.NotRegexp):
                    type = "NOT REGEXP";
                    break;
            }
            return type;
        }
    }
}
