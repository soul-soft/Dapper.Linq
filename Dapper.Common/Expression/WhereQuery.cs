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
    internal class WhereExpression
    {
        public string StringWhere { get; set; }
        public Expression LambdaWhere { get; set; }
        public ExpressionType ExpressType { get; set; }
    }
    /// <summary>
    /// 数据库表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhereQuery<T> where T : class, new()
    {
        /// <summary>
        /// 创建动态表达式对象
        /// </summary>
        public WhereQuery()
        {

        }
        /// <summary>
        /// 创建动态表达式对象
        /// </summary>
        /// <param name="expression"></param>
        public WhereQuery(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
            {
                ExpressType = ExpressionType.AndAlso,
                LambdaWhere = expression,
            });
        }
        /// <summary>
        /// 表达式列表
        /// </summary>
        internal readonly List<WhereExpression> Expressions = new List<WhereExpression>();
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
                LambdaWhere = expression,
            });
            return this;
        }
        /// <summary>
        /// And
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public WhereQuery<T> And(string expression)
        {
            Expressions.Add(new WhereExpression()
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
        public WhereQuery<T> Or(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
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
        public WhereQuery<T> Or(string expression)
        {
            Expressions.Add(new WhereExpression()
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
        /// 逻辑Or运算
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
        public WhereQuery<T> Not(Expression<Func<T, bool>> expression)
        {
            Expressions.Add(new WhereExpression()
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
        public WhereQuery<T> Not(bool condition, Expression<Func<T, bool>> expression)
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
        public WhereQuery<T> Not(string expression)
        {
            Expressions.Add(new WhereExpression()
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
        public WhereQuery<T> Not(bool condition, string expression)
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
