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
    internal class FunVisitor<T> : ExpressionVisitor
    {
        #region Props
        /// <summary>
        /// 表达式参数
        /// </summary>
        private DynamicParameters Param { get; set; }
        /// <summary>
        /// 表达式字符串
        /// </summary>
        private StringBuilder FunExpression = new StringBuilder();
        /// <summary>
        /// 别名
        /// </summary>
        private bool Alias { get; set; }
        /// <summary>
        /// 是否为关键字
        /// </summary>
        private bool KeyParameter { get; set; }
        /// <summary>
        /// 当前操作符
        /// </summary>
        private string Operator { get; set; }
        /// <summary>
        /// 是否是常量
        /// </summary>
        private bool IsConstant { get; set; }
        #endregion

        #region Method
        /// <summary>
        /// 构建表达式参数
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(object value)
        {
            if (KeyParameter)
            {
                FunExpression.AppendFormat("{0}", value);
                KeyParameter = false;
            }
            else if (IsConstant)
            {
                FunExpression.AppendFormat("{1}{0}{1}", value, value.GetType().IsValueType ? "" : "'");
                IsConstant = false;
            }
            else
            {
                var name = string.Format("@{0}_{1}", "param", Param.ParameterNames.Count());
                FunExpression.Append(name);
                Param.Add(name, value);
            }
        }
        /// <summary>
        /// 构建表达式字段
        /// </summary>
        /// <param name="column"></param>
        private void SetName(string column)
        {
            FunExpression.Append(column);
        }
        /// <summary>
        /// 构建函数表达式,及参数
        /// </summary>
        /// <param name="param"></param>
        /// <param name="expression"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string Build(ref DynamicParameters param, Expression expression, bool alias = true)
        {
            Param = param;
            Alias = alias;
            Visit(expression);
            return FunExpression.ToString();
        }
        #endregion

        #region Visiit
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(WhereType))
            {
                if (node.Arguments.Count == 3 && node.Method.Name.Contains("Between"))
                {
                    FunExpression.Append("(");
                    Operator = WhereType.GetOperator(node.Method.Name);
                    Visit(node.Arguments[0]);
                    FunExpression.AppendFormat(" {0} ", Operator);
                    Visit(node.Arguments[1]);
                    FunExpression.AppendFormat(" AND ");
                    Visit(node.Arguments[2]);
                    FunExpression.Append(")");
                }
                else if (node.Arguments.Count == 2)
                {
                    FunExpression.Append("(");
                    Visit(node.Arguments[0]);
                    Operator = WhereType.GetOperator(node.Method.Name);
                    FunExpression.AppendFormat(" {0} ", Operator);
                    Visit(node.Arguments[1]);
                    FunExpression.Append(")");
                }
                else if (node.Arguments.Count == 1)
                {
                    FunExpression.Append("(");
                    Visit(node.Arguments[0]);
                    Operator = WhereType.GetOperator(node.Method.Name);
                    FunExpression.AppendFormat(" {0} ", Operator);
                    FunExpression.Append(")");
                }
            }
            else if (node.Method.GetCustomAttributes(typeof(FunctionAttribute), true).Length > 0)
            {
                FunExpression.AppendFormat("{0}(", node.Method.Name.ToUpper());
                var parameters = node.Method.GetParameters();
                for (var i = 0; i < node.Arguments.Count; i++)
                {
                    if (parameters[i].GetCustomAttributes(typeof(KeyParameterAttribute), true).Length > 0)
                    {
                        KeyParameter = true;
                    }
                    Visit(node.Arguments[i]);
                    if (i + 1 < node.Arguments.Count)
                    {
                        FunExpression.Append(",");
                    }
                }
                FunExpression.Append(")");
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
            FunExpression.Append("(");
            Visit(node.Left);
            Operator = WhereType.GetOperator(node.NodeType);
            FunExpression.AppendFormat(" {0} ", Operator);
            Visit(node.Right);
            FunExpression.Append(")");
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(GetColumn(node));
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
            IsConstant = true;
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
                FunExpression.Append(WhereType.GetOperator(node.NodeType));
                Visit(node.Operand);
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
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            for (var i = 0; i < node.Bindings.Count; i++)
            {
                Visit(((MemberAssignment)node.Bindings[i]).Expression);
                if (Alias)
                {
                    FunExpression.AppendFormat(" AS {0}", node.Bindings[i].Member.Name);
                }
                if (i + 1 < node.Bindings.Count)
                {
                    FunExpression.Append(",");
                }
            }
            return node;
        }      
        protected override Expression VisitNew(NewExpression node)
        {
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                Visit(node.Arguments[i]);
                if (Alias)
                {
                    FunExpression.AppendFormat(" AS {0}", node.Members[i].Name);
                }
                if (i + 1 < node.Arguments.Count)
                {
                    FunExpression.Append(",");
                }
            }
            return node;
        }
        #endregion

        #region Utils
        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetColumn(Expression expression)
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
