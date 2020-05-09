using System.Linq.Expressions;

namespace Dapper.Expressions
{
    public class SelectExpressionResovle : ExpressionResovle
    {
        public SelectExpressionResovle(Expression expression)
         : base(expression)
        {

        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            for (int i = 0; i < node.Bindings.Count; i++)
            {
                var item = node.Bindings[i] as MemberAssignment;

                if (item.Expression is MemberExpression member)
                {
                    var name = GetColumnName(member.Member.DeclaringType, member.Member.Name);
                    _textBuilder.Append($"{name} AS {item.Member.Name}");
                }
                else if (item.Expression is MethodCallExpression)
                {
                    var expression = new FunctionExpressionResovle(item.Expression).Resovle();
                    _textBuilder.Append($"{expression} AS {item.Member.Name}");
                }
                if (i != node.Bindings.Count - 1)
                {
                    _textBuilder.Append(",");
                }
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var item = node.Arguments[i];
                var column = node.Members[i].Name;
                if (item is MemberExpression member)
                {
                    var name = GetColumnName(member.Member.DeclaringType, member.Member.Name);
                    if (name != column)
                    {
                        _textBuilder.Append($"{name} AS {column}");
                    }
                    else
                    {
                        _textBuilder.Append($"{name}");
                    }
                }
                else if (item is MethodCallExpression)
                {
                    var expression = new FunctionExpressionResovle(item).Resovle();
                    _textBuilder.Append($"{expression} AS {column}");
                }
                if (i != node.Arguments.Count - 1)
                {
                    _textBuilder.Append(",");
                }
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var name = GetColumnName(node.Member.DeclaringType, node.Member.Name);
            _textBuilder.Append($"{name}");
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = new FunctionExpressionResovle(node).Resovle();
            _textBuilder.Append($"{result} AS expr");
            return node;
        }

    }
}
