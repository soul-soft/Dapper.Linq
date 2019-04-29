using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.Extension
{
    /// <summary>
    ///  
    /// 摘要:
    ///     用于构建表达式工具。 若要浏览源代码，请参阅Github。
    /// </summary>
    public class ExpressionUtil : ExpressionVisitor
    {
        #region propertys
        private StringBuilder _build = new StringBuilder();
        private Type _type { get; set; }
        private Dictionary<string, object> _value { get; set; }
        private string _name = "Param";
        private string _condition { get; set; }
        #endregion

        #region override
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(node.Member.Name);
            }
            else
            {
                SetValue(node);
            }
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(ExtensionUtil))
            {
                _build.Append("(");
                if (node.Arguments.Count == 1)
                {
                    Visit(node.Arguments[0]);
                    _build.AppendFormat(" {0} ", ExtensionUtil.GetCondition(node.Method.Name));
                }
                else if (node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    _condition = ExtensionUtil.GetCondition(node.Method.Name);
                    _build.AppendFormat(" {0} ", _condition);
                    Visit(node.Arguments[1]);
                }
                else
                {
                    _condition = ExtensionUtil.GetCondition(node.Method.Name);
                    Visit(node.Arguments[0]);
                    _build.AppendFormat(" {0} ", _condition);
                    Visit(node.Arguments[1]);
                    _build.AppendFormat(" {0} ", ExtensionUtil.GetCondition(ExpressionType.AndAlso));
                    Visit(node.Arguments[2]);
                }
                _build.Append(")");
            }
            else if (node.Method.GetCustomAttributes(typeof(FunctionAttribute), true).Length > 0)
            {
                _build.AppendFormat("{0}(", node.Method.Name.ToUpper());
                var parameters = node.Method.GetParameters();
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    if (parameters[i].GetCustomAttributes(typeof(ParameterAttribute), true).Length > 0)
                    {
                        _build.Append((node.Arguments[i] as ConstantExpression).Value);
                        continue;
                    }
                    Visit(node.Arguments[i]);
                    if (i + 1 != node.Arguments.Count)
                    {
                        _build.Append(",");
                    }
                }
                _build.Append(")");
            }
            else
            {
                SetValue(node);
            }
            return node;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            _build.Append("(");
            Visit(node.Left);
            _condition = ExtensionUtil.GetCondition(node.NodeType);
            if (node.Right is ConstantExpression && (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual) && (node.Right as ConstantExpression).Value == null)
            {
                _build.AppendFormat(" {0}", node.NodeType == ExpressionType.Equal ? "IS NULL" : "IS NOT NULL");
            }
            else
            {
                _build.AppendFormat("{0}", _condition);
                Visit(node.Right);
            }
            _build.Append(")");
            return node;
        }
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            SetValue(node);
            return node;
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                _build.AppendFormat("{0}", ExtensionUtil.GetCondition(ExpressionType.Not));
            }
            Visit(node.Operand);
            return node;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            _build.AppendFormat("{0}{1}{0}", node.Value?.GetType() == typeof(string) ? "'" : "", node.Value?.ToString());
            return node;
        }
        #endregion

        #region private
        public void SetName(string name)
        {
            var column = EntityUtil.GetColumn(_type, f => f.CSharpName == name)?.ColumnName ?? name;
            _build.Append(column);
            _name = name;
        }
        public void SetValue(Expression expression)
        {
            var value = GetValue(expression);
            if (_condition == "LIKE" || _condition == "NOT LIKE")
            {
                value = string.Format("%{0}%", value);
            }
            var key = string.Format("@{0}{1}", _name, _value.Count);
            _value.Add(key, value);
            _build.Append(key);
        }

        #endregion

        #region public
        public static string BuildExpression<T>(Expression expression, Dictionary<string, object> param)
        {
            var visitor = new ExpressionUtil
            {
                _value = param,
                _type = typeof(T)
            };
            visitor.Visit(expression);
            return visitor._build.ToString();
        }
        public static Dictionary<string, string> BuildColumns<T>(Expression expression, Dictionary<string, object> param)
        {
            var columns = new Dictionary<string, string>();
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            if (expression is MemberInitExpression)
            {
                var initExpression = (expression as MemberInitExpression);
                for (int i = 0; i < initExpression.Bindings.Count; i++)
                {
                    var expr = BuildExpression<T>((initExpression.Bindings[i] as MemberAssignment).Expression, param);
                    var name = initExpression.Bindings[i].Member.Name;
                    columns.Add(name, expr);
                }
            }
            else if (expression is NewExpression)
            {
                var newExpression = (expression as NewExpression);
                for (int i = 0; i < newExpression.Arguments.Count; i++)
                {
                    var expr = BuildExpression<T>(newExpression.Arguments[i], param);
                    var name = newExpression.Members[i].Name;
                    columns.Add(name, expr);
                }
            }
            else if (expression is MemberExpression)
            {
                var name = (expression as MemberExpression).Member.Name;
                var expr = EntityUtil.GetColumn<T>(f => f.CSharpName == name)?.ColumnName ?? name;
                columns.Add(name, expr);
            }
            else
            {
                var name = string.Format("COLUMN{0}", param.Count);
                var expr = BuildExpression<T>(expression, param);
                columns.Add(name, expr);
            }
            return columns;
        }
        public static Dictionary<string, string> BuildColumn<T>(Expression expression, Dictionary<string, object> param)
        {
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            var column = new Dictionary<string, string>();
            if (expression is MemberExpression)
            {
                var name = (expression as MemberExpression).Member.Name;
                var columnName = EntityUtil.GetColumn<T>(f => f.CSharpName == name)?.ColumnName ?? name;
                column.Add(name, columnName);
                return column;
            }
            else
            {
                var name = string.Format("COLUMN");
                var build = BuildExpression<T>(expression, param);
                column.Add(name, build);
                return column;
            }
        }
        public static object GetValue(Expression expression)
        {
            var names = new Stack<string>();
            while (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;
                names.Push(memberExpression.Member.Name);
                expression = memberExpression.Expression;
                if (expression == null && memberExpression.Type == typeof(DateTime) && memberExpression.Member.Name == nameof(DateTime.Now))
                {
                    return DateTime.Now;
                }
            }
            if (expression is ConstantExpression)
            {
                var value = (expression as ConstantExpression).Value;
                foreach (var item in names)
                {
                    if (value.GetType().GetField(item) != null)
                    {
                        value = value.GetType().GetField(item).GetValue(value);
                    }
                    else
                    {
                        value = value.GetType().GetProperty(item).GetValue(value);
                    }
                }
                return value;
            }
            else
            {
                return Expression.Lambda(expression).Compile().DynamicInvoke();
            }
        }
        #endregion
    }
}
