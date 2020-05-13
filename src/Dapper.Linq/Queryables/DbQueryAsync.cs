using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper.Expressions;

namespace Dapper
{
    /// <summary>
    /// 异步linq查询
    /// </summary>
    public partial class DbQuery<T>
    {

        #region async
        public async Task<T> GetAsync(object id)
        {
            var sql = ResolveGet();
            var values = new Dictionary<string, object>
            {
                { "id", id }
            };
            return (await _context.QueryAsync<T>(sql, values)).FirstOrDefault();
        }
        public Task<int> CountAsync(int? commandTimeout = null)
        {
            var sql = ResovleCount();
            return _context.ExecuteScalarAsync<int>(sql, _parameters, commandTimeout);
        }

        public Task<int> CountAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            _countExpression = expression;
            return CountAsync();
        }

        public Task<int> DeleteAsync(int? commandTimeout = null)
        {
            var sql = ResovleDelete();
            return _context.ExecuteAsync(sql, _parameters, commandTimeout);
        }

        public Task<int> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            Where(expression);
            return DeleteAsync();
        }

        public Task<bool> ExistsAsync(int? commandTimeout = null)
        {
            var sql = ResovleExists();
            return _context.ExecuteScalarAsync<bool>(sql, _parameters, commandTimeout);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            Where(expression);
            return ExistsAsync();
        }

        public Task<int> UpdateAsync(int? commandTimeout = null)
        {
            if (_setExpressions.Count > 0)
            {
                var sql = ResolveUpdate();
                return _context.ExecuteAsync(sql, _parameters, commandTimeout);
            }
            return default;
        }

        public async Task<int> UpdateAsync(T entity)
        {
            ResovleParameter(entity);
            var sql = ResolveUpdate();
            var row = await _context.ExecuteAsync(sql, _parameters);
            if (GetColumnMetaInfos().Exists(a => a.IsConcurrencyCheck) && row == 0)
            {
                throw new DbUpdateConcurrencyException("更新失败：数据版本不一致");
            }
            return row;
        }

        public Task<int> InsertAsync(T entity)
        {
            ResovleParameter(entity);
            var sql = ResovleInsert(false);
            return _context.ExecuteAsync(sql, _parameters);
        }

        public async Task<int> InsertAsync(IEnumerable<T> entitys, int? commandTimeout = null)
        {
            if (entitys == null || entitys.Count() == 0)
            {
                return 0;
            }
            var sql = ResovleBatchInsert(entitys);
            return await _context.ExecuteAsync(sql, _parameters, commandTimeout);
        }

        public Task<int> InsertReturnIdAsync(T entity)
        {
            ResovleParameter(entity);
            var sql = ResovleInsert(true);
            return _context.ExecuteScalarAsync<int>(sql, _parameters);
        }
      
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql = ResovleSum();
            return await _context.ExecuteScalarAsync<TResult>(sql, _parameters, commandTimeout);
        }
       
        public Task<IEnumerable<T>> SelectAsync(int? commandTimeout = null)
        {
            var sql = ResolveSelect();
            return _context.QueryAsync<T>(sql, _parameters, commandTimeout);
        }

        public async Task<(IEnumerable<T>, int)> SelectManyAsync(int? commandTimeout = null)
        {
            var sql1 = ResolveSelect();
            var sql2 = ResovleCount();
            using (var multi = _context.QueryMultiple($"{sql1};{sql2}", _parameters, commandTimeout))
            {
                var list = await multi.GetListAsync<T>();
                var count = await multi.GetAsync<int>();
                return (list, count);
            }
        }

        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql = ResolveSelect();
            return _context.QueryAsync<TResult>(sql, _parameters, commandTimeout);
        }

        public async Task<(IEnumerable<TResult>, int)> SelectManyAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            _selectExpression = expression;
            var sql1 = ResolveSelect();
            var sql2 = ResovleCount();
            using (var multi = _context.QueryMultiple($"{sql1};{sql2}", _parameters, commandTimeout))
            {
                var list = await multi.GetListAsync<TResult>();
                var count = await multi.GetAsync<int>();
                return (list, count);
            }
        }

        public async Task<T> SingleAsync(int? commandTimeout = null)
        {
            Take(1);
            return (await SelectAsync(commandTimeout)).FirstOrDefault();
        }

        public async Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> expression, int? commandTimeout = null)
        {
            Take(1);
            return (await SelectAsync(expression, commandTimeout)).FirstOrDefault();
        }
        #endregion
    }
}
