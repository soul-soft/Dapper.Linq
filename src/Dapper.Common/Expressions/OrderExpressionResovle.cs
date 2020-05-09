using System.Linq.Expressions;

namespace Dapper.Expressions
{
    public class OrderExpressionResovle : ExpressionResovle
    {
        private readonly string _asc = string.Empty;

        public OrderExpressionResovle(Expression expression, bool asc)
            : base(expression)
        {
            if (!asc)
            {
                _asc = " DESC";
            }
        }
        protected override Expression VisitNew(NewExpression node)
        {
            foreach (var item in node.Arguments)
            {
                Visit(item);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = new FunctionExpressionResovle(node).Resovle();
            _textBuilder.Append($"{result}{_asc},");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var name = GetColumnName(node.Member.DeclaringType, node.Member.Name);
            _textBuilder.Append($"{name}{_asc},");
            return node;
        }

        public override string Resovle()
        {
            return base.Resovle().Trim(',');
        }
    }
}
