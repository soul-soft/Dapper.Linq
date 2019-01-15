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
    public class SqlVisitor : ExpressionVisitor
    {
        #region Props
        /// <summary>
        /// 表达式字符串
        /// </summary>
        private StringBuilder SqlExpression = new StringBuilder();
        /// <summary>
        /// 表达式参数
        /// </summary>
        private DynamicParameters Args = new DynamicParameters();
        /// <summary>
        /// 类型
        /// </summary>
        private Type ClassType { get; set; }
        /// <summary>
        /// 字段栈
        /// </summary>
        private Stack<string> Names = new Stack<string>();
        /// <summary>
        /// 当前运算符
        /// </summary>
        private string CurrentOperator;
        #endregion

        #region Method
        /// <summary>
        /// 构建表达式参数
        /// </summary>
        /// <param name="value"></param>
        private void SetValue(object value)
        {
            var name = Names.Pop();
            var key = string.Format("@{0}_{1}", name, Args.ParameterNames.Count());
            if (value==null)
            {
                throw new Exception(string.Format("参数:{0}不能null", key));
            }
            if (CurrentOperator == "LIKE" || CurrentOperator == "NOT LIKE")
            {
                value = "%" + value.ToString() + "%";
            }
            SqlExpression.Append(key);
            Args.Add(key, value);
        }
        /// <summary>
        /// 构建表达式字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="memberName"></param>
        private void SetName(string columnName, string memberName)
        {
            SqlExpression.Append(columnName);
            Names.Push(memberName);
            if (CurrentOperator == "BETWEEN" || CurrentOperator == "NOT BETWEEN")
            {
                Names.Push(memberName + "_Min");
                Names.Push(memberName + "_Max");
            }
        }
        /// <summary>
        /// 构建表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressionList"></param>
        /// <returns></returns>
        public string Build<T>(ref DynamicParameters args, List<SqlExpression> expressionList)
        {
            ClassType = typeof(T);
            Args = args;
            foreach (var item in expressionList)
            {
                if (!string.IsNullOrEmpty(item.Include))
                {
                    SqlExpression.Append(item.Include);
                    continue;
                }
                if ((!item.Equals(expressionList.FindAll(f => string.IsNullOrEmpty(f.Include)).First())) && !SqlExpression.ToString().EndsWith("("))
                {
                    SqlExpression.AppendFormat(" {0} ", SqlOperator.GetOperator(item.ExpressType));
                }
                Visit(item.Expression);
            }
            return SqlExpression.ToString().Replace("()","");
        }
        #endregion

        #region Visiit
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count == 3 && SqlOperator.Methods.Contains(node.Method.Name) && node.Method.Name.Contains("Between"))
            {
                SqlExpression.Append("(");
                CurrentOperator = SqlOperator.GetOperator(node.Method.Name);
                Visit(node.Arguments[0]);
                SqlExpression.AppendFormat(" {0} ", CurrentOperator);
                Visit(node.Arguments[1]);
                SqlExpression.AppendFormat(" AND ");
                Visit(node.Arguments[2]);
                SqlExpression.Append(")");
            }
            else if (node.Arguments.Count == 2 && SqlOperator.Methods.Contains(node.Method.Name))
            {
                SqlExpression.Append("(");
                Visit(node.Arguments[0]);
                CurrentOperator = SqlOperator.GetOperator(node.Method.Name);
                SqlExpression.AppendFormat(" {0} ", CurrentOperator);
                Visit(node.Arguments[1]);
                SqlExpression.Append(")");
            }
            else if (node.Arguments.Count == 1 && SqlOperator.Methods.Contains(node.Method.Name))
            {
                SqlExpression.Append("(");
                Visit(node.Arguments[0]);
                CurrentOperator = SqlOperator.GetOperator(node.Method.Name);
                SqlExpression.AppendFormat(" {0} ", CurrentOperator);
                SqlExpression.Append(")");
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
            SqlExpression.Append("(");
            Visit(node.Left);
            CurrentOperator = SqlOperator.GetOperator(node.NodeType);
            SqlExpression.AppendFormat(" {0} ", CurrentOperator);
            Visit(node.Right);
            SqlExpression.Append(")");
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(GetColumnName(ClassType, node), GetFieldName(node));
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
                SqlExpression.Append(SqlOperator.GetOperator(node.NodeType));
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
        #endregion

        #region Utils
        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetFieldName(Expression expression)
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
            return name;
        }
        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetColumnName(Type type, Expression expression)
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
            return TypeMapper.GetColumnName(type, name);
        }
        /// <summary>
        /// 获取字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetColumnName<T>(Expression expression)
        {
            return GetColumnName(typeof(T), expression);
        }
        /// <summary>
        /// 获取字段名列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="express"></param>
        /// <returns></returns>
        public static List<string> GetColumnNames<T>(Expression<Func<T, object>> express)
        {
            var props = express.Body.Type.GetProperties().Select(s => s.Name);
            var columns = TypeMapper.GetColumnNames<T>(props.ToList());
            return columns.ToList();
        }
        #endregion

    }
   

}
