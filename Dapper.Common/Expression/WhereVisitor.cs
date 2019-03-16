using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{

    /// <summary>
    /// 数据库表达式构建
    /// </summary>
    internal class WhereVisitor<T> : ExpressionVisitor
    {
        #region Props
        /// <summary>
        /// 表达式字符串
        /// </summary>
        private StringBuilder WhereExpression = new StringBuilder();
        /// <summary>
        /// 表达式参数
        /// </summary>
        private Dictionary<string, object> Param = new Dictionary<string, object>();
        /// <summary>
        /// 字段栈
        /// </summary>
        private Stack<string> Fields = new Stack<string>();
        /// <summary>
        /// 当前运算符
        /// </summary>
        private string Operator { get; set; }
        #endregion

        #region Method
        /// <summary>
        /// 构建表达式参数
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(object value)
        {
            var field = Fields.Pop();
            var key = string.Format("@{0}_{1}", field, Param.Count);
            if (value == null)
            {
                throw new Exception(string.Format("参数:{0}不能null", key));
            }
            if (Operator == "LIKE" || Operator == "NOT LIKE")
            {
                value = "%" + value.ToString() + "%";
            }
            WhereExpression.Append(key);
            Param.Add(key, value);
        }
        /// <summary>
        /// 构建表达式字段
        /// </summary>
        /// <param name="column"></param>
        /// <param name="field"></param>
        private void SetName(string column, string field)
        {
            WhereExpression.Append(column);
            Fields.Push(field);
            if (Operator == "BETWEEN" || Operator == "NOT BETWEEN")
            {
                Fields.Push(field + "_Min");
                Fields.Push(field + "_Max");
            }
        }
        /// <summary>
        /// 构建表达式
        /// </summary>
        /// <param name="param"></param>
        /// <param name="expressionList"></param>
        /// <returns></returns>
        internal string Build(Dictionary<string, object> param, List<WhereExpression> expressionList)
        {
            Param = param;
            foreach (var item in expressionList)
            {
                if ((!item.Equals(expressionList.First())))
                {
                    WhereExpression.AppendFormat(" {0} ", WhereType.GetOperator(item.ExpressType));
                }
                if (!string.IsNullOrEmpty(item.StringWhere))
                {
                    WhereExpression.Append(item.StringWhere);
                    continue;
                }
                Visit(item.LambdaWhere);

            }
            return WhereExpression.ToString();
        }
        #endregion

        #region Visiit
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(WhereType))
            {
                if (node.Arguments.Count == 3 && node.Method.Name.Contains("Between"))
                {
                    WhereExpression.Append("(");
                    Operator = WhereType.GetOperator(node.Method.Name);
                    Visit(node.Arguments[0]);
                    WhereExpression.AppendFormat(" {0} ", Operator);
                    Visit(node.Arguments[1]);
                    WhereExpression.AppendFormat(" AND ");
                    Visit(node.Arguments[2]);
                    WhereExpression.Append(")");
                }
                else if (node.Arguments.Count == 2)
                {
                    WhereExpression.Append("(");
                    Visit(node.Arguments[0]);
                    Operator = WhereType.GetOperator(node.Method.Name);
                    WhereExpression.AppendFormat(" {0} ", Operator);
                    Visit(node.Arguments[1]);
                    WhereExpression.Append(")");
                }
                else if (node.Arguments.Count == 1)
                {
                    WhereExpression.Append("(");
                    Visit(node.Arguments[0]);
                    Operator = WhereType.GetOperator(node.Method.Name);
                    WhereExpression.AppendFormat(" {0} ", Operator);
                    WhereExpression.Append(")");
                }
            }
            else if (node.Method.GetCustomAttributes(typeof(FunctionAttribute), true).Length > 0)
            {
                WhereExpression.Append(new FunVisitor<T>().Build(Param, node, false));
                SetName("", node.Method.Name);
            }
            else
            {
                var value = Expression.Lambda(node).Compile().DynamicInvoke();
                SetValue(value);
            }
            return node;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            WhereExpression.Append("(");
            Visit(node.Left);
            Operator = WhereType.GetOperator(node.NodeType);
            WhereExpression.AppendFormat(" {0} ", Operator);
            Visit(node.Right);
            WhereExpression.Append(")");
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(GetColumn(node), node.Member.Name);
            }
            else
            {
                var value = Expression.Lambda(node).Compile().DynamicInvoke();
                SetValue(value);
            }

            return node;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            SetValue(node.Value);
            return node;
        }
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            SetValue(value);
            return node;
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                WhereExpression.Append(WhereType.GetOperator(node.NodeType));
                Visit(node.Operand);
            }
            else if (node.Operand.NodeType == ExpressionType.New)
            {
                var value = Expression.Lambda(node).Compile().DynamicInvoke();
                SetValue(value);
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            SetValue(value);
            return node;
        }
        protected override Expression VisitNew(NewExpression node)
        {
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            SetValue(value);
            return node;
        }
        #endregion

        #region Utils           
        internal static string GetColumn(Expression expression)
        {
            var name = string.Empty;
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            if (expression is MemberExpression)
            {
                name = (expression as MemberExpression).Member.Name;
            }
            else if (expression is UnaryExpression)
            {
                name = ((expression as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                throw new Exception("Not Cast MemberExpression");
            }
            return TypeMapper.GetColumnName<T>(name);
        }
        #endregion

    }


}
