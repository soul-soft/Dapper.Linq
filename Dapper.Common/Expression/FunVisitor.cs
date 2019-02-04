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
    public class FunVisitor<T> : ExpressionVisitor
    {
        #region Props
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
        /// 是否为创建表达式
        /// </summary>
        private bool NewExpression { get; set; }
        #endregion

        #region Method
        /// <summary>
        /// 构建表达式参数
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(object value)
        {
            var key = string.Format("{1}{0}{1}", value==null?"NULL":value.ToString(),KeyParameter||value is ValueType?"":"'");
            KeyParameter = false;
            FunExpression.Append(key);            
        }
        /// <summary>
        /// 构建表达式字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="memberName"></param>
        private void SetName(string columnName)
        {
            FunExpression.Append(columnName);
        }
        /// <summary>
        /// 构建表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressionList"></param>
        /// <returns></returns>
        public string Build(Expression<Func<T, object>> expression, bool alias=true)
        {
            NewExpression = true;
            Alias = alias;
            Visit(expression.Body);
            return FunExpression.ToString();
        }
        public string Build(Expression expression)
        {
            NewExpression = false;
            Alias = false;
            Visit(expression);
            return FunExpression.ToString();
        }
        #endregion

        #region Visiit
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            FunExpression.AppendFormat("{0}(", node.Method.Name.ToUpper());
            var parameters = node.Method.GetParameters();
            for (var i=0; i<node.Arguments.Count;i++)
            {
                if (parameters[i].GetCustomAttributes(typeof(KeyParameterAttribute),true).Length>0)
                {
                    KeyParameter = true;
                }
                Visit(node.Arguments[i]);
                if (i+1<node.Arguments.Count)
                {
                    FunExpression.Append(",");
                }
            }
            FunExpression.Append(")");
            return node;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            FunExpression.Append("(");
            Visit(node.Left);
            Visit(node.Right);
            FunExpression.Append(")");
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(GetColumnName(node));
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
        protected override Expression VisitNew(NewExpression node)
        {
            if (NewExpression)
            {
                NewExpression = false;
                for (var i = 0; i < node.Arguments.Count; i++)
                {
                    Visit(node.Arguments[i]);
                    if (Alias)
                    {
                        FunExpression.AppendFormat(" AS {0}", node.Members[i].Name);
                    }
                    if (i+1<node.Arguments.Count)
                    {
                        FunExpression.Append(",");
                    }
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
        public static string GetColumnName(Expression expression)
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
