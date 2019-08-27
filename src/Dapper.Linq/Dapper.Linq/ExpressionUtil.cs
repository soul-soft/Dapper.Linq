using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Linq.Util
{
    public class ExpressionUtil : ExpressionVisitor
    {
        #region propertys
        private StringBuilder _build = new StringBuilder();
        private Dictionary<string, object> _param { get; set; }
        private string _paramName = "Name";
        private string _prefix { get; set; }
        private string _operatorMethod { get; set; }
        private string _operator { get; set; }
        private bool _singleTable { get; set; }
        #endregion

        #region override
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                SetName(node);
            }
            else
            {
                SetValue(node);
            }
            return node;
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Operator))
            {
                var notInclude =
                    node.Method.Name == nameof(Operator.All)
                    || node.Method.Name == nameof(Operator.Any)
                    || node.Method.Name == nameof(Operator.Exists)
                    || node.Method.Name == nameof(Operator.NotExists);
                _build.AppendFormat("{0}", notInclude && node.Arguments.Count == 1 ? "" : "(");
                if (node.Arguments.Count == 1)
                {
                    if (notInclude)
                    {
                        _build.AppendFormat("{0}", Operator.GetOperator(node.Method.Name));
                        Visit(node.Arguments[0]);
                    }
                    else
                    {
                        Visit(node.Arguments[0]);
                        _build.AppendFormat(" {0} ", Operator.GetOperator(node.Method.Name));
                    }
                }
                else if (node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    _operator = Operator.GetOperator(node.Method.Name);
                    _operatorMethod = node.Method.Name;
                    _build.AppendFormat(" {0} ", _operator);
                    Visit(node.Arguments[1]);
                }
                else
                {
                    _operator = Operator.GetOperator(node.Method.Name);
                    Visit(node.Arguments[0]);
                    _build.AppendFormat(" {0} ", _operator);
                    Visit(node.Arguments[1]);
                    _build.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                    Visit(node.Arguments[2]);
                }
                _build.AppendFormat("{0}", notInclude && node.Arguments.Count == 1 ? "" : ")");
            }
            else if (node.Method.GetCustomAttributes(typeof(FunctionAttribute), true).Length > 0)
            {
                _build.AppendFormat("{0}(", node.Method.Name.ToUpper());
                var parameters = node.Method.GetParameters();
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    if (node.Arguments[i] is NewArrayExpression newArrayExpression)
                    {
                        for (int j = 0; j < newArrayExpression.Expressions.Count; j++)
                        {
                            Visit(newArrayExpression.Expressions[j]);
                            if (j + 1 != newArrayExpression.Expressions.Count)
                            {
                                _build.Append(",");
                            }
                        }
                    }
                    else
                    {
                        Visit(node.Arguments[i]);
                    }
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
            if (node.Right is ConstantExpression && (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual) && (node.Right as ConstantExpression).Value == null)
            {
                _operator = node.NodeType == ExpressionType.Equal ? Operator.GetOperator(nameof(Operator.IsNull)) : Operator.GetOperator(nameof(Operator.IsNotNull));
                _build.AppendFormat(" {0}", _operator);
            }
            else
            {
                _operator = Operator.GetOperator(node.NodeType);
                _build.AppendFormat(" {0} ", _operator);
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
        protected override Expression VisitNew(NewExpression node)
        {
            SetValue(node);
            return node;
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                _build.AppendFormat("{0} ", Operator.GetOperator(ExpressionType.Not));
                Visit(node.Operand);
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
            {
                _build.AppendFormat("NULL");
            }
            else if (_operator == "LIKE" || _operator == "NOT LIKE")
            {
                if (_operatorMethod == nameof(Operator.EndsWith) || _operatorMethod == nameof(Operator.NotEndsWith))
                {
                    _build.AppendFormat("'%{0}'", node.Value);
                }
                else if (_operatorMethod == nameof(Operator.StartsWith) || _operatorMethod == nameof(Operator.NotStartsWith))
                {
                    _build.AppendFormat("'{0}%'", node.Value);
                }
                else
                {
                    _build.AppendFormat("'%{0}%'", node.Value);
                }
            }
            else if (node.Value is string)
            {
                _build.AppendFormat("'{0}'", node.Value);
            }
            else if (node.Value is Enum)
            {
                _build.AppendFormat("{0}", Convert.ToInt32(node.Value));
            }
            else if (node.Value is bool)
            {
                _build.AppendFormat("{0}", Convert.ToBoolean(node.Value) ? 1 : 0);
            }
            else
            {
                _build.AppendFormat("{0}", node.Value);
            }
            return node;
        }
        #endregion

        #region private
        private static string GetColumnName(Type type, string name, bool singleTable)
        {
            var columnName = EntityUtil.GetColumn(type, f => f.CSharpName == name)?.ColumnName ?? name;
            if (!singleTable)
            {
                var tableName = EntityUtil.GetTable(type).TableName;
                columnName = string.Format("{0}.{1}", tableName, columnName);
            }
            return columnName;
        }
        public void SetName(MemberExpression expression)
        {
            var name = expression.Member.Name;
            var columnName = GetColumnName(expression.Expression.Type, name, _singleTable);
            _build.Append(columnName);
            _paramName = name;
        }
        public void SetValue(Expression expression)
        {
            var value = GetValue(expression);
            if (value is ISqlBuilder sqlBuilder)
            {
                _build.Append(sqlBuilder.Build(_param, _prefix));
            }
            else
            {
                if (_operator == "LIKE" || _operator == "NOT LIKE")
                {
                    if (_operatorMethod == nameof(Operator.EndsWith) || _operatorMethod == nameof(Operator.NotEndsWith))
                    {
                        value = string.Format("%{0}", value);
                    }
                    else if (_operatorMethod == nameof(Operator.StartsWith) || _operatorMethod == nameof(Operator.NotStartsWith))
                    {
                        value = string.Format("{0}%", value);
                    }
                    else
                    {
                        value = string.Format("%{0}%", value);
                    }
                }
                else if (value is Enum)
                {
                    value = Convert.ToInt32(value);
                }
                else if (value is bool)
                {
                    value = Convert.ToBoolean(value) ? 1 : 0;
                }
                var key = string.Format("{0}{1}{2}", _prefix, _paramName, _param.Count);
                _param.Add(key, value);
                _build.Append(key);
            }
        }
        #endregion

        #region public
        public static string BuildExpression(Expression expression, Dictionary<string, object> param, string prefix = "@", bool singleTable = true)
        {
            var visitor = new ExpressionUtil
            {
                _param = param ?? new Dictionary<string, object>(),
                _singleTable = singleTable,
                _prefix = prefix,
            };
            visitor.Visit(expression);
            return visitor._build.ToString();
        }
        public static Dictionary<string, string> BuildColumns(Expression expression, Dictionary<string, object> param, string prefix, bool singleTable = true)
        {
            var columns = new Dictionary<string, string>();
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            if (expression is MemberExpression memberExpression0 && memberExpression0.Expression != null && memberExpression0.Expression.NodeType == ExpressionType.Parameter)
            {
                var memberName = memberExpression0.Member.Name;
                var columnName = GetColumnName(memberExpression0.Expression.Type, memberExpression0.Member.Name, singleTable);
                columns.Add(memberName, columnName);
            }
            else if (expression is MemberInitExpression initExpression)
            {
                for (int i = 0; i < initExpression.Bindings.Count; i++)
                {
                    var columnName = string.Empty;
                    var memberName = initExpression.Bindings[i].Member.Name;
                    var argument = (initExpression.Bindings[i] as MemberAssignment).Expression;
                    if (argument is UnaryExpression)
                    {
                        argument = (argument as UnaryExpression).Operand;
                    }
                    if (argument is MemberExpression memberExpression1 && memberExpression1.Expression != null && memberExpression1.Expression.NodeType == ExpressionType.Parameter)
                    {
                        columnName = GetColumnName(memberExpression1.Expression.Type, memberExpression1.Member.Name, singleTable);
                    }
                    else if (argument is MemberExpression memberExpression2 && memberExpression2.Expression.NodeType == ExpressionType.Constant)
                    {
                        var value = GetValue(argument);
                        if (value is ISqlBuilder sqlBuilder)
                        {
                            columnName = sqlBuilder.Build(param, prefix);
                        }
                        else
                        {
                            columnName = value.ToString();
                        }
                    }
                    else if (argument is MethodCallExpression && (argument as MethodCallExpression).Method.DeclaringType == typeof(Convert))
                    {
                        var value = GetValue((argument as MethodCallExpression).Arguments[0]);
                        if (value is ISqlBuilder sqlBuilder)
                        {
                            columnName = sqlBuilder.Build(param, prefix);
                        }
                        else
                        {
                            columnName = value.ToString();
                        }
                    }
                    else if (argument is ConstantExpression)
                    {
                        columnName = GetValue(argument).ToString();
                    }
                    else
                    {
                        columnName = BuildExpression(argument, param, prefix, singleTable);
                    }

                    columns.Add(memberName, columnName);
                }
            }
            else if (expression is NewExpression newExpression)
            {
                for (int i = 0; i < newExpression.Arguments.Count; i++)
                {
                    var columnName = string.Empty;
                    var memberName = newExpression.Members[i].Name;
                    var argument = newExpression.Arguments[i];
                    if (argument is UnaryExpression)
                    {
                        argument = (argument as UnaryExpression).Operand;
                    }
                    if (argument is MemberExpression memberExpression1 && memberExpression1.Expression != null && memberExpression1.Expression.NodeType == ExpressionType.Parameter)
                    {
                        columnName = GetColumnName(memberExpression1.Expression.Type, memberExpression1.Member.Name, singleTable);
                    }
                    else if (argument is MemberExpression memberExpression2 && memberExpression2.Expression != null && memberExpression2.Expression.NodeType == ExpressionType.Constant)
                    {
                        var value = GetValue(memberExpression2);
                        if (value is ISqlBuilder sqlBuilder)
                        {
                            columnName = sqlBuilder.Build(param, prefix);
                        }
                        else
                        {
                            columnName = value.ToString();
                        }
                    }
                    else if (argument is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof(Convert))
                    {
                        var value = GetValue(methodCallExpression.Arguments[0]);
                        if (value is ISqlBuilder sqlBuilder)
                        {
                            columnName = sqlBuilder.Build(param, prefix);
                        }
                        else
                        {
                            columnName = value.ToString();
                        }
                    }
                    else if (argument is ConstantExpression)
                    {
                        columnName = GetValue(argument).ToString();
                    }
                    else
                    {
                        columnName = BuildExpression(argument, param, prefix, singleTable);
                    }
                    columns.Add(memberName, columnName);
                }
            }
            else
            {
                var name = string.Format("COLUMN0");
                var columnName = BuildExpression(expression, param, prefix, singleTable);
                columns.Add(name, columnName);
            }
            return columns;
        }

        public static Dictionary<string, string> BuildColumn(Expression expression, Dictionary<string, object> param, string prefix, bool singleTable = true)
        {
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            var column = new Dictionary<string, string>();
            if (expression is MemberExpression memberExpression && memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
            {
                var memberName = memberExpression.Member.Name;
                var columnName = GetColumnName(memberExpression.Expression.Type, memberName, singleTable);
                column.Add(memberName, columnName);
                return column;
            }
            else
            {
                var name = string.Format("COLUMN0");
                var build = BuildExpression(expression, param, prefix, singleTable);
                column.Add(name, build);
                return column;
            }
        }
        public static Dictionary<string, string> BuildColumnAndValues(Expression expression, Dictionary<string, object> param, string prefix)
        {
            var columns = new Dictionary<string, string>();
            expression = (expression as LambdaExpression).Body;
            var initExpression = (expression as MemberInitExpression);
            var type = initExpression.Type;
            for (int i = 0; i < initExpression.Bindings.Count; i++)
            {
                Expression argument = (initExpression.Bindings[i] as MemberAssignment).Expression;
                var name = initExpression.Bindings[i].Member.Name;
                var columnName = EntityUtil.GetColumn(type, f => f.CSharpName == name).ColumnName;
                var value = GetValue(argument);
                if (value is Enum)
                {
                    value = Convert.ToInt32(value);
                }
                else if (value is bool)
                {
                    value = Convert.ToBoolean(value) ? 1 : 0;
                }
                var key = string.Format("{0}{1}", prefix, name);
                columns.Add(columnName, key);
                param.Add(key, value);
            }
            return columns;
        }
        public static object GetValue(Expression expression)
        {
            if ((expression is UnaryExpression unaryExpression) && unaryExpression.NodeType == ExpressionType.Convert)
            {
                expression = unaryExpression.Operand;
            }
            var names = new Stack<string>();
            var exps = new Stack<Expression>();
            var mifs = new Stack<System.Reflection.MemberInfo>();
            var tempExpression = expression;
            while (tempExpression is MemberExpression member)
            {
                names.Push(member.Member.Name);
                exps.Push(member.Expression);
                mifs.Push(member.Member);
                tempExpression = member.Expression;
            }
            if (names.Count > 0)
            {
                object value = null;
                foreach (var name in names)
                {
                    var exp = exps.Pop();
                    var mif = mifs.Pop();
                    if (exp is ConstantExpression cex)
                    {
                        value = cex.Value;
                    }
                    if ((mif is System.Reflection.PropertyInfo pif))
                    {
                        value = pif.GetValue(value);
                    }
                    else if ((mif is System.Reflection.FieldInfo fif))
                    {
                        value = fif.GetValue(value);
                    }
                }
                return value;
            }
            else if (expression is ConstantExpression constant)
            {
                return constant.Value;
            }
            else
            {
                return Expression.Lambda(expression).Compile().DynamicInvoke();
            }
        }
        #endregion
    }
}
