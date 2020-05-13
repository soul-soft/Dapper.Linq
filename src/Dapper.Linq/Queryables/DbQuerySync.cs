using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Expressions;

namespace Dapper
{
    /// <summary>
    /// 同步linq查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class DbQuery<T>
    {
        #region sync
        public T Get(object id)
        {
            var sql = ResolveGet();
            var values = new Dictionary<string, object>
            {
                { "id", id }
            };
            return _context.Query<T>(sql, values).FirstOrDefault();
        }

        public int Count(int? commandTimeout = null)
        {
            var sql = ResovleCount();
            return _context.ExecuteScalar<int>(sql, _parameters, commandTimeout);
        }

        public int Count<TResult>(Expression<Func<T, TResult>> expression)
        {
            _countExpression = expression;
            return Count();
        }

        public int Insert(T entity)
        {
            ResovleParameter(entity);
            var sql = ResovleInsert(false);
            return _context.Execute(sql, _parameters);
        }
     
        public int InsertReturnId(T entity)
        {
            ResovleParameter(entity);
            var sql = ResovleInsert(true);
            return _context.ExecuteScalar<int>(sql, _parameters);
        }

        public int Insert(IEnumerable<T> entitys, int? commandTimeout = null)
        {
            if (entitys==null || entitys.Count()==0)
            {
                return 0;
            }
            var sql = ResovleBatchInsert(entitys);
            return _context.Execute(sql, _parameters, commandTimeout); 
        }

        public int Update(int? commandTimeout = null)
        {
            if (_setExpressions.Count > 0)
            {
                var sql = ResolveUpdate();
                return _context.Execute(sql, _parameters, commandTimeout);
            }
            return default;
        }

        public int Update(T entity)
        {
            ResovleParameter(entity);
            var sql = ResolveUpdate();
            var row = _context.Execute(sql, _parameters);
            if (GetColumnMetaInfos().Exists(a => a.IsConcurrencyCheck) && row == 0)
            {
                throw new DbUpdateConcurrencyException("更新失败：数据版本不一致");
            }
            return row;
        }

        public int Delete(int? commandTimeout = null)
        {
            var sql = ResovleDelete();
            return _context.Execute(sql, _parameters, commandTimeout);
        }

        public int Delete(Expression<Func<T, bool>> expression)
        {
            Where(expression);
            return Delete();
        }

        public bool Exists(int? commandTimeout = null)
        {
            var sql = ResovleExists();
            return _context.ExecuteScalar<bool>(sql, _parameters, commandTimeout);
        }

        public bool Exists(Expression<Func<T, bool>> expression)
        {
            Where(expression);
            return Exists();
        }

        public IDbQuery<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool condition = true)
        {
            if (true)
            {
                _setExpressions.Add(new SetExpression
                {
                    Column = column,
                    Expression = Expression.Constant(value)
                });
            }
            return this;
        }

        public IDbQuery<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (true)
            {
                _setExpressions.Add(new SetExpression
                {
                    Column = column,
                    Expression = expression
                });
            }
            return this;
        }

        public IDbQuery<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            _groupExpressions.Add(expression);
            return this;
        }

        public IDbQuery<T> Having(Expression<Func<T, bool>> expression, bool condition = true)
        {
            if (condition)
            {
                _havingExpressions.Add(expression);
            }
            return this;
        }

        public IDbQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            _orderExpressions.Add(new OrderExpression
            {
                Asc = true,
                Expression = expression
            });
            return this;
        }

        public IDbQuery<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression)
        {
            _orderExpressions.Add(new OrderExpression
            {
                Asc = false,
                Expression = expression
            });
            return this;
        }

        public IDbQuery<T> Filter<TResult>(Expression<Func<T, TResult>> column)
        {
            _filterExpression = column;
            return this;
        }

        public IDbQuery<T> Page(int index, int count, bool condition = true)
        {
            if (condition)
            {
                Skip((index - 1) * count, count);
            }
            return this;
        }

        public IDbQuery<T> With(string lockname)
        {
            _lockname = $" {lockname}";
            return this;
        }

        public IEnumerable<T> Select(int? commandTimeout = null)
        {
            var sql = ResolveSelect();
            return _context.Query<T>(sql, _parameters, commandTimeout);
        }

        public (IEnumerable<T>, int) SelectMany(int? commandTimeout = null)
        {
            var sql1 = ResolveSelect();
            var sql2 = ResovleCount();
            using (var multi = _context.QueryMultiple($"{sql1};{sql2}", _parameters, commandTimeout))
            {
                var list = multi.GetList<T>();
                var count = multi.Get<int>();
                return (list, count);
            }
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql = ResovleSum();
            return _context.ExecuteScalar<TResult>(sql, _parameters, commandTimeout);
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql = ResolveSelect();
            return _context.Query<TResult>(sql, _parameters, commandTimeout);
        }

        public (IEnumerable<TResult>, int) SelectMany<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql1 = ResolveSelect();
            var sql2 = ResovleCount();
            using (var multi = _context.QueryMultiple($"{sql1};{sql2}", _parameters, commandTimeout))
            {
                var list = multi.GetList<TResult>();
                var count = multi.Get<int>();
                return (list, count);
            }
        }

        public T Single(int? commandTimeout = null)
        {
            Take(1);
            return Select(commandTimeout).FirstOrDefault();
        }

        public TResult Single<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            Take(1);
            return Select(expression, commandTimeout).FirstOrDefault();
        }

        public IDbQuery<T> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                _page.Index = index;
                _page.Count = count;
            }
            return this;
        }

        public IDbQuery<T> Take(int count,bool condition=true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }

        public IDbQuery<T> Where(Expression<Func<T, bool>> expression, bool condition = true)
        {
            if (condition)
            {
                _whereExpressions.Add(expression);
            }
            return this;
        }

        #endregion
    }
}
