using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Expressions
{
    public class FunctionExpressionResovle : ExpressionResovle
    {
        public FunctionExpressionResovle(Expression expression)
           : base(expression)
        {
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _textBuilder.Append(node.Method.Name.ToUpper());
            _textBuilder.Append("(");
            for (var i = 0; i < node.Arguments.Count; i++)
            {
                var item = node.Arguments[i];
                Visit(item);
            }
            if (_textBuilder[_textBuilder.Length - 1] == ',')
            {
                _textBuilder.Remove(_textBuilder.Length - 1, 1);
            }
            _textBuilder.Append(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = VisitConstantValue(node);
            if (value == null)
            {
                value = "NULL";
            }
            else if (value is string)
            {
                value = $"'{value}'";
            }
            else if (value is bool)
            {
                value = Convert.ToBoolean(value) ? 1 : 0;
            }
            _textBuilder.Append($"{value},");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var name = GetColumnName(node.Member.DeclaringType, node.Member.Name);
            _textBuilder.Append($"{name},");
            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            foreach (var item in node.Expressions)
            {
                Visit(item);
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.Operand != null)
            {
                Visit(node.Operand);
            }
            return node;
        }

    }
}
