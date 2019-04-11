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
    internal class QueryableExpression
    {
        public string StringWhere { get; set; }
        public Expression LambdaWhere { get; set; }
        public ExpressionType ExpressType { get; set; }
    }
    /// <summary>
    /// 数据库表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Queryable<T> where T : class, new()
    {
        /// <summary>
        /// 创建动态表达式对象
        /// </summary>
        public Queryable()
        {

        }
        /// <summary>
        /// 创建动态表达式对象
        /// </summary>
        /// <param name="expression"></param>
        public Queryable(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                LambdaWhere = expression,
            });
        }
        /// <summary>
        /// 表达式列表
        /// </summary>
        internal readonly List<QueryableExpression> Expressions = new List<QueryableExpression>();
        /// <summary>
        /// And运算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> And(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                LambdaWhere = expression,
            });
            return this;
        }
        /// <summary>
        /// And
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> And(string expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        /// <summary>
        /// And
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> And(bool condition, string expression)
        {
            if (condition)
            {
                Expressions.Add(new QueryableExpression()
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
        public Queryable<T> And(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new QueryableExpression()
                {
                    ExpressType = ExpressionType.AndAlso,
                    LambdaWhere = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Or(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.OrElse,
                LambdaWhere = expression,
            });
            return this;
        }
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Or(string expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.OrElse,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        /// <summary>
        /// 逻辑或
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Or(bool condition, string expression)
        {
            if (condition)
            {
                Expressions.Add(new QueryableExpression()
                {
                    StringWhere = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 逻辑Or运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Or(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new QueryableExpression()
                {
                    ExpressType = ExpressionType.OrElse,
                    LambdaWhere = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Not(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.Not,
                LambdaWhere = expression,
            });
            return this;
        }
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Not(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Not(expression);
            }
            return this;
        }
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Not(string expression)
        {
            Expressions.Add(new QueryableExpression()
            {
                ExpressType = ExpressionType.Not,
                StringWhere = string.Format("({0})", expression),
            });
            return this;
        }
        /// <summary>
        /// 逻辑非
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Queryable<T> Not(bool condition, string expression)
        {
            if (condition)
            {
                Not(expression);
            }
            return this;
        } 
        /// <summary>
        /// 表达式个数
        /// </summary>
        public int Count { get { return Expressions.Count; } }
    }
   
}
