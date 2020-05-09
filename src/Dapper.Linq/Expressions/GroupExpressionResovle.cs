using System.Linq.Expressions;

namespace Dapper.Expressions
{
    public class GroupExpressionResovle : ExpressionResovle
    {
        public GroupExpressionResovle(Expression expression)
            : base(expression)
        {
        }

        protected override Expression VisitNew(NewExpression node)
        {
            foreach (var item in node.Arguments)
            {
                Visit(item);
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var name = GetColumnName(node.Member.DeclaringType, node.Member.Name);
            _textBuilder.Append($"{name},");
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var result = new FunctionExpressionResovle(node).Resovle();
            _textBuilder.Append($"{result},");
            return node;
        }

        public override string Resovle()
        {
            return base.Resovle().Trim(',');
        }
    }
}
