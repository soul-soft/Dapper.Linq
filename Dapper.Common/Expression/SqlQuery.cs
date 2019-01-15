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
    public class SqlExpression
    {
        public string Include { get; set; }
        public Expression Expression { get; set; }
        public ExpressionType ExpressType { get; set; }
    }
    /// <summary>
    /// 数据库表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlQuery<T> where T : class, new()
    {
        public SqlQuery()
        {

        }
        public SqlQuery(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new SqlExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                Expression = expression,
            });
        }
        /// <summary>
        /// 表达式列表
        /// </summary>
        public List<SqlExpression> Expressions = new List<SqlExpression>();
        /// <summary>
        /// And运算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlQuery<T> And(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new SqlExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                Expression = expression,
            });
            return this;
        }
        /// <summary>
        /// Or运算
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlQuery<T> Or(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new SqlExpression()
            {
                ExpressType = ExpressionType.OrElse,
                Expression = expression,
            });
            return this;
        }
        /// <summary>
        /// 条件And运算
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlQuery<T> And(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new SqlExpression()
                {
                    ExpressType = ExpressionType.AndAlso,
                    Expression = expression,
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
        public SqlQuery<T> Or(bool condition, Expression<Func<T, bool>> expression)
        {
            if (condition)
            {
                Expressions.Add(new SqlExpression()
                {
                    ExpressType = ExpressionType.OrElse,
                    Expression = expression,
                });
            }
            return this;
        }
        /// <summary>
        /// 左括号
        /// </summary>
        /// <returns></returns>
        public SqlQuery<T> AndLeft()
        {
            Expressions.Add(new SqlExpression()
            {
                Include = " AND ("
            });
            return this;
        }
        public SqlQuery<T> OrLeft()
        {
            Expressions.Add(new SqlExpression()
            {
                Include = " OR ("
            });
            return this;
        }
        public SqlQuery<T> NotLeft()
        {
            Expressions.Add(new SqlExpression()
            {
                Include = " NOT ("
            });
            return this;
        }
        /// <summary>
        /// 右括号
        /// </summary>
        /// <returns></returns>
        public SqlQuery<T> Right()
        {
            Expressions.Add(new SqlExpression()
            {
                Include = ")"
            });
            return this;
        }
        public int Count { get { return Expressions.Count; } }
    }
}
