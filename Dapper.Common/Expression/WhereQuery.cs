using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// 表达式模型
    /// </summary>
    public class WhereExpression
    {
        public string StringWhere { get; set; }
        public Expression Expression { get; set; }
        public ExpressionType? ExpressType { get; set; }
    }
    /// <summary>
    /// 数据库表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhereQuery<T> where T : class, new()
    {
        public WhereQuery()
        {

        }
        public WhereQuery(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                Expression = expression,
            });
        }
        /// <summary>
        /// 表达式列表
        /// </summary>
        public readonly List<WhereExpression> Expressions = new List<WhereExpression>();
        /// <summary>
        /// And运算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WhereQuery<T> And(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                Expression = expression,
            });
            return this;
        }
        public WhereQuery<T> And(string expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        public WhereQuery<T> And(bool condition, string expression)
        {
            if (condition)
            {
                Expressions.Add(new WhereExpression()
                {
                    ExpressType = ExpressionType.AndAlso,
                    StringWhere = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 条件And运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WhereQuery<T> And(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new WhereExpression()
                {
                    ExpressType = ExpressionType.AndAlso,
                    Expression = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// Or运算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WhereQuery<T> Or(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.OrElse,
                Expression = expression,
            });
            return this;
        }
        public WhereQuery<T> Or(string expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.OrElse,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        public WhereQuery<T> Or(bool condition, string expression)
        {
            if (condition)
            {
                Expressions.Add(new WhereExpression()
                {
                    StringWhere = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 条件Or运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WhereQuery<T> Or(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new WhereExpression()
                {
                    ExpressType = ExpressionType.OrElse,
                    Expression = expression,
                });
            }
            return this;
        }
        public WhereQuery<T> Not(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.Not,
                Expression = expression,
            });
            return this;
        }
        public WhereQuery<T> Not(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Not(expression);
            }
            return this;
        }
        public WhereQuery<T> Not(string expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.Not,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        public WhereQuery<T> Not(bool condition, string expression)
        {
            if (condition)
            {
                Not(expression);
            }
            return this;
        }       
        public int Count { get { return Expressions.Count; } }
    }
   
}
