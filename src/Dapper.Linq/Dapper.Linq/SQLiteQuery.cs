using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Dapper.Linq.Util;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    public class SQLiteQuery<T> : IQueryable<T> where T : class
    {
        #region constructor
        public IDbContext _dbcontext { get; }
        public string _prefix { get; }
        public SQLiteQuery(IDbContext dbcontext = null)
        {
            _prefix = "@";
            _dbcontext = dbcontext;
            _param = new Dictionary<string, object>();
        }
        public SQLiteQuery(Dictionary<string, object> param)
        {
            _prefix = "@";
            _param = param;
        }
        #endregion

        #region implement
        public IQueryable<T> With(string lockType, bool condition = true)
        {
            if (condition)
            {
                _lock.Append(lockType);
            }
            return this;
        }
        public IQueryable<T> With(LockType lockType, bool condition = true)
        {
            if (condition)
            {
                if (lockType == LockType.FOR_UPADTE)
                {
                    With("FOR UPDATE");
                }
                else if (lockType == LockType.LOCK_IN_SHARE_MODE)
                {
                    With("LOCK IN SHARE MODE");
                }
            }
            return this;
        }
        public IQueryable<T> Distinct(bool condition = true)
        {
            if (condition)
            {
                _distinctBuffer.Append("DISTINCT");
            }
            return this;
        }
        public IQueryable<T> Filter<TResult>(Expression<Func<T, TResult>> columns, bool condition = true)
        {
            if (condition)
            {
                _filters.AddRange(ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => s.Value));
            }
            return this;
        }
        public IQueryable<T> GroupBy(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_groupBuffer.Length > 0)
                {
                    _groupBuffer.Append(",");
                }
                _groupBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T> Having(string expression, bool condition = true)
        {
            if (condition)
            {
                _havingBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> Having(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T> OrderBy(string orderBy, bool condition = true)
        {
            if (condition)
            {
                if (_orderBuffer.Length > 0)
                {
                    _orderBuffer.Append(",");
                }
                _orderBuffer.Append(orderBy);
            }
            return this;
        }
        public IQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} DESC", s.Value))));
            }
            return this;
        }
        public IQueryable<T> Page(int index, int count, out long total, bool condition = true)
        {
            total = 0;
            if (condition)
            {
                Skip(count * (index - 1), count);
                total = Count();
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, ISubQuery subquery, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
                _setBuffer.AppendFormat("{0} = {1}", columns.Value, subquery.Build(_param, _prefix));
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
                var key = string.Format("{0}{1}", columns.Key, _param.Count);
                _param.Add(key, value);
                _setBuffer.AppendFormat("{0} = @{1}", columns.Value, key);
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> value, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columnName = ExpressionUtil.BuildColumn(column, _param, _prefix).First().Value;
                var expression = ExpressionUtil.BuildExpression(value, _param, _prefix);
                _setBuffer.AppendFormat("{0} = {1}", columnName, expression);
            }
            return this;
        }
        public IQueryable<T> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                pageIndex = index;
                pageCount = count;
            }
            return this;
        }
        public IQueryable<T> Take(int count, bool condition = true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }
        public IQueryable<T> Where(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> Where(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, _param, _prefix));
            }
            return this;
        }
        public int Delete(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildDelete();
                return _dbcontext.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public async Task<int> DeleteAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildDelete();
                return await _dbcontext.ExecuteAsync(sql, _param, timeout);
            }
            return 0;
        }
        public int Insert(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return _dbcontext.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return await _dbcontext.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public long InsertReturnId(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT LAST_INSERT_ROWID();", sql);
                return _dbcontext.ExecuteScalar<long>(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<long> InsertReturnIdAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT LAST_INSERT_ROWID();", sql);
                return await _dbcontext.ExecuteScalarAsync<long>(sql, entity, timeout);
            }
            return 0;
        }
        public int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return _dbcontext.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return await _dbcontext.ExecuteAsync(sql, entitys, timeout);
            }
            return 0;
        }
        public int Update(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return _dbcontext.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return await _dbcontext.ExecuteAsync(sql, _param, timeout);
            }
            return 0;
        }
        public int Update(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return _dbcontext.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return await _dbcontext.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return _dbcontext.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return await _dbcontext.ExecuteAsync(sql, entitys, timeout);
            }
            return 0;
        }
        public T Single(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<T> SingleAsync(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        public TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columnstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Single<TResult>(columnstr, buffered, timeout);
        }
        public Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columnstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SingleAsync<TResult>(columnstr, timeout);
        }
        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<T>(sql, _param, buffered, timeout);
            }
            return new List<T>();
        }
        public async Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<T>(sql, _param, timeout);
            }
            return new List<T>();
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<TResult>(sql, _param, timeout);
            }
            return new List<TResult>();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SelectAsync<TResult>(columstr, timeout);
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return _dbcontext.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public async Task<long> CountAsync(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return await _dbcontext.ExecuteScalarAsync<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return Count(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), condition, timeout);
        }
        public Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return CountAsync(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), condition, timeout);
        }
        public bool Exists(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildExists();
                return _dbcontext.ExecuteScalar<int>(sql, _param, timeout) > 0;
            }
            return false;
        }
        public async Task<bool> ExistsAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildExists();
                return await _dbcontext.ExecuteScalarAsync<int>(sql, _param, timeout) > 0;
            }
            return false;
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
                _sumBuffer.AppendFormat("{0}", column);
                if (_dbcontext != null)
                {
                    var sql = BuildSum();
                    return _dbcontext.ExecuteScalar<TResult>(sql, _param, timeout);
                }
            }
            return default;
        }
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
                _sumBuffer.AppendFormat("{0}", column);
                if (_dbcontext != null)
                {
                    var sql = BuildSum();
                    return await _dbcontext.ExecuteScalarAsync<TResult>(sql, _param, timeout);
                }
            }
            return default;
        }
        #endregion

        #region property
        public Dictionary<string, object> _param { get; set; }
        public StringBuilder _columnBuffer = new StringBuilder();
        public List<string> _filters = new List<string>();
        public StringBuilder _setBuffer = new StringBuilder();
        public StringBuilder _havingBuffer = new StringBuilder();
        public StringBuilder _whereBuffer = new StringBuilder();
        public StringBuilder _groupBuffer = new StringBuilder();
        public StringBuilder _orderBuffer = new StringBuilder();
        public StringBuilder _distinctBuffer = new StringBuilder();
        public StringBuilder _countBuffer = new StringBuilder();
        public StringBuilder _sumBuffer = new StringBuilder();
        public StringBuilder _lock = new StringBuilder();
        public EntityTable _table = EntityUtil.GetTable<T>();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region build
        public string BuildInsert()
        {
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                _table.TableName,
                string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => s.ColumnName))
                , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("@{0}", s.CSharpName))));
            return sql;
        }
        public string BuildUpdate(bool allColumn = true)
        {
            if (allColumn)
            {
                var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
                var colums = _table.Columns.FindAll(f => f.ColumnKey != ColumnKey.Primary && !_filters.Exists(e => e == f.ColumnName));
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    _table.TableName,
                    string.Join(",", colums.Select(s => string.Format("{0} = @{1}", s.ColumnName, s.CSharpName))),
                    _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
                return sql;
            }
            else
            {
                var sql = string.Format("UPDATE {0} SET {1}{2}",
                    _table.TableName,
                    _setBuffer,
                    _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
                return sql;
            }

        }
        public string BuildDelete()
        {
            var sql = string.Format("DELETE FROM {0}{1}",
                _table.TableName,
                _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
            return sql;
        }
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            if (_columnBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            }
            else
            {
                sqlBuffer.AppendFormat(" {0}", string.Join(",", _table.Columns.FindAll(f => !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("{0} AS {1}", s.ColumnName, s.CSharpName))));
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex != null && pageCount != null)
            {
                sqlBuffer.AppendFormat(" LIMIT {0},{1}", pageIndex, pageCount);
            }
            if (_lock.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _lock);
            }
            var sql = sqlBuffer.ToString();
            return sql;
        }
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {

                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        public string BuildExists()
        {
            var sqlBuffer = new StringBuilder();

            sqlBuffer.AppendFormat("SELECT 1 FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            var sql = string.Format("SELECT EXISTS({0})", sqlBuffer);
            return sql;
        }
        public string BuildSum()
        {
            var sqlBuffer = new StringBuilder();
            sqlBuffer.AppendFormat("SELECT SUM({0}) FROM {1}", _sumBuffer, _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            return sqlBuffer.ToString();
        }
        #endregion
    }
    public class SQLiteQuery<T1, T2> : IQueryable<T1, T2> where T1 : class where T2 : class
    {
        #region constructor
        public IDbContext _dbcontext { get; }
        public string _prefix { get; }
        public SQLiteQuery(IDbContext dbcontext = null)
        {
            _prefix = "@";
            _dbcontext = dbcontext;
            _param = new Dictionary<string, object>();
        }
        public SQLiteQuery(Dictionary<string, object> param)
        {
            _prefix = "@";
            _param = param;
        }
        #endregion

        #region implement
        public IQueryable<T1, T2> Distinct(bool condition = true)
        {
            if (condition)
            {
                _distinctBuffer.Append("DISTINCT");
            }
            return this;
        }
        public IQueryable<T1, T2> GroupBy(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_groupBuffer.Length > 0)
                {
                    _groupBuffer.Append(",");
                }
                _groupBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2> GroupBy<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T1, T2> Having(string expression, bool condition = true)
        {
            if (condition)
            {
                _havingBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2> Having(Expression<Func<T1, T2, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T1, T2> OrderBy(string orderBy, bool condition = true)
        {
            if (condition)
            {
                if (_orderBuffer.Length > 0)
                {
                    _orderBuffer.Append(",");
                }
                _orderBuffer.Append(orderBy);
            }
            return this;
        }
        public IQueryable<T1, T2> OrderBy<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2> OrderByDescending<TResult>(Expression<Func<T1, T2, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} DESC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2> Page(int index, int count, out long total, bool condition = true)
        {
            total = 0;
            if (condition)
            {
                Skip(count * (index - 1), count);
                total = Count();
            }
            return this;
        }
        public IQueryable<T1, T2> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                pageIndex = index;
                pageCount = count;
            }
            return this;
        }
        public IQueryable<T1, T2> Take(int count, bool condition = true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }
        public IQueryable<T1, T2> Where(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2> Where(Expression<Func<T1, T2, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, _param, _prefix, false));
            }
            return this;
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<TResult>(sql, _param, timeout);
            }
            return new List<TResult>();
        }
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        public TResult Single<TResult>(Expression<Func<T1, T2, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T1, T2, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SelectAsync<TResult>(columstr, timeout);
        }
        public async Task<TResult> SingleAsync<TResult>(Expression<Func<T1, T2, TResult>> columns, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return _dbcontext.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public async Task<long> CountAsync(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return await _dbcontext.ExecuteScalarAsync<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public IQueryable<T1, T2> Join(string expression)
        {
            if (_join.Length > 0)
            {
                _join.Append(" ");
            }
            _join.Append(expression);
            return this;
        }
        public IQueryable<T1, T2> Join(Expression<Func<T1, T2, bool?>> expression, JoinType join = JoinType.Inner)
        {
            var onExpression = ExpressionUtil.BuildExpression(expression, _param, _prefix, false);
            var table1Name = EntityUtil.GetTable<T1>().TableName;
            var table2Name = EntityUtil.GetTable<T2>().TableName;
            var joinType = string.Format("{0} JOIN", join.ToString().ToUpper());
            Join(string.Format("{0} {1} {2} ON {3}", table1Name, joinType, table2Name, onExpression));
            return this;
        }
        #endregion

        #region property
        public Dictionary<string, object> _param { get; set; }
        public StringBuilder _columnBuffer = new StringBuilder();
        public StringBuilder _havingBuffer = new StringBuilder();
        public StringBuilder _whereBuffer = new StringBuilder();
        public StringBuilder _groupBuffer = new StringBuilder();
        public StringBuilder _orderBuffer = new StringBuilder();
        public StringBuilder _distinctBuffer = new StringBuilder();
        public StringBuilder _countBuffer = new StringBuilder();
        public StringBuilder _join = new StringBuilder();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region build
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex != null && pageCount != null)
            {
                sqlBuffer.AppendFormat(" LIMIT {0},{1}", pageIndex, pageCount);
            }
            var sql = sqlBuffer.ToString();
            return sql;
        }
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {

                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        #endregion
    }
    public class SQLiteQuery<T1, T2, T3> : IQueryable<T1, T2, T3> where T1 : class where T2 : class where T3 : class
    {
        #region constructor
        public IDbContext _dbcontext { get; }
        public string _prefix { get; }
        public SQLiteQuery(IDbContext dbcontext = null)
        {
            _prefix = "@";
            _dbcontext = dbcontext;
            _param = new Dictionary<string, object>();
        }
        public SQLiteQuery(Dictionary<string, object> param)
        {
            _prefix = "@";
            _param = param;
        }
        #endregion

        #region implement
        public IQueryable<T1, T2, T3> Distinct(bool condition = true)
        {
            if (condition)
            {
                _distinctBuffer.Append("DISTINCT");
            }
            return this;
        }
        public IQueryable<T1, T2, T3> GroupBy(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_groupBuffer.Length > 0)
                {
                    _groupBuffer.Append(",");
                }
                _groupBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3> GroupBy<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Having(string expression, bool condition = true)
        {
            if (condition)
            {
                _havingBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Having(Expression<Func<T1, T2, T3, bool?>> expression, bool condition = true)
        {
            Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)), condition);
            return this;
        }
        public IQueryable<T1, T2, T3> OrderBy(string orderBy, bool condition = true)
        {
            if (condition)
            {
                if (_orderBuffer.Length > 0)
                {
                    _orderBuffer.Append(",");
                }
                _orderBuffer.Append(orderBy);
            }
            return this;
        }
        public IQueryable<T1, T2, T3> OrderBy<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2, T3> OrderByDescending<TResult>(Expression<Func<T1, T2, T3, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} DESC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Page(int index, int count, out long total, bool condition = true)
        {
            total = 0;
            if (condition)
            {
                Skip(count * (index - 1), count);
                total = Count();
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                pageIndex = index;
                pageCount = count;
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Take(int count, bool condition = true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Where(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, _param, _prefix, false));
            }
            return this;
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<TResult>(sql, _param, timeout);
            }
            return new List<TResult>();
        }
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        public TResult Single<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SelectAsync<TResult>(columstr, timeout);
        }
        public async Task<TResult> SingleAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> columns, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return _dbcontext.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public async Task<long> CountAsync(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return await _dbcontext.ExecuteScalarAsync<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public IQueryable<T1, T2, T3> Join(string expression)
        {
            if (_join.Length > 0)
            {
                _join.Append(" ");
            }
            _join.Append(expression);
            return this;
        }
        public IQueryable<T1, T2, T3> Join<E1, E2>(Expression<Func<E1, E2, bool?>> expression, JoinType join = JoinType.Inner) where E1 : class where E2 : class
        {
            var onExpression = ExpressionUtil.BuildExpression(expression, _param, _prefix, false);
            var table1Name = EntityUtil.GetTable<E1>().TableName;
            var table2Name = EntityUtil.GetTable<E2>().TableName;
            var joinType = string.Format("{0} JOIN", join.ToString().ToUpper());
            if (_tables.Count == 0)
            {
                _tables.Add(table1Name);
                _tables.Add(table2Name);
                Join(string.Format("{0} {1} {2} ON {3}", table1Name, joinType, table2Name, onExpression));
            }
            else if (_tables.Exists(a => table1Name == a))
            {
                _tables.Add(table2Name);
                Join(string.Format("{0} {1} ON {2}", joinType, table2Name, onExpression));
            }
            else
            {
                _tables.Add(table1Name);
                Join(string.Format("{0} {1} ON {2}", joinType, table1Name, onExpression));
            }
            return this;
        }
        #endregion

        #region property
        public Dictionary<string, object> _param { get; set; }
        public StringBuilder _columnBuffer = new StringBuilder();
        public StringBuilder _havingBuffer = new StringBuilder();
        public StringBuilder _whereBuffer = new StringBuilder();
        public StringBuilder _groupBuffer = new StringBuilder();
        public StringBuilder _orderBuffer = new StringBuilder();
        public StringBuilder _distinctBuffer = new StringBuilder();
        public StringBuilder _countBuffer = new StringBuilder();
        public StringBuilder _join = new StringBuilder();
        public List<string> _tables = new List<string>();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region build
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex != null && pageCount != null)
            {
                sqlBuffer.AppendFormat(" LIMIT {0},{1}", pageIndex, pageCount);
            }
            var sql = sqlBuffer.ToString();
            return sql;
        }
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {

                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        #endregion
    }
    public class SQLiteQuery<T1, T2, T3, T4> : IQueryable<T1, T2, T3, T4> where T1 : class where T2 : class where T3 : class where T4 : class
    {
        #region constructor
        public IDbContext _dbcontext { get; }
        public string _prefix { get; }
        public SQLiteQuery(IDbContext dbcontext = null)
        {
            _prefix = "@";
            _dbcontext = dbcontext;
            _param = new Dictionary<string, object>();
        }
        public SQLiteQuery(Dictionary<string, object> param)
        {
            _prefix = "@";
            _param = param;
        }
        #endregion

        #region implement
        public IQueryable<T1, T2, T3, T4> Distinct(bool condition = true)
        {
            if (condition)
            {
                _distinctBuffer.Append("DISTINCT");
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> GroupBy(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_groupBuffer.Length > 0)
                {
                    _groupBuffer.Append(",");
                }
                _groupBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> GroupBy<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Having(string expression, bool condition = true)
        {
            if (condition)
            {
                _havingBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Having(Expression<Func<T1, T2, T3, T4, bool?>> expression, bool condition = true)
        {
            Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => s.Value)), condition);
            return this;
        }
        public IQueryable<T1, T2, T3, T4> OrderBy(string orderBy, bool condition = true)
        {
            if (condition)
            {
                if (_orderBuffer.Length > 0)
                {
                    _orderBuffer.Append(",");
                }
                _orderBuffer.Append(orderBy);
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> OrderBy<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> OrderByDescending<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix, false).Select(s => string.Format("{0} DESC", s.Value))));
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Page(int index, int count, out long total, bool condition = true)
        {
            total = 0;
            if (condition)
            {
                Skip(count * (index - 1), count);
                total = Count();
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                pageIndex = index;
                pageCount = count;
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Take(int count, bool condition = true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Where(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, _param, _prefix, false));
            }
            return this;
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<TResult>(sql, _param, timeout);
            }
            return new List<TResult>();
        }
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        public TResult Single<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix, false).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SelectAsync<TResult>(columstr, timeout);
        }
        public async Task<TResult> SingleAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> columns, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return _dbcontext.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public async Task<long> CountAsync(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return await _dbcontext.ExecuteScalarAsync<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public IQueryable<T1, T2, T3, T4> Join(string expression)
        {
            if (_join.Length > 0)
            {
                _join.Append(" ");
            }
            _join.Append(expression);
            return this;
        }
        public IQueryable<T1, T2, T3, T4> Join<E1, E2>(Expression<Func<E1, E2, bool?>> expression, JoinType join = JoinType.Inner) where E1 : class where E2 : class
        {
            var onExpression = ExpressionUtil.BuildExpression(expression, _param, _prefix, false);
            var table1Name = EntityUtil.GetTable<E1>().TableName;
            var table2Name = EntityUtil.GetTable<E2>().TableName;
            var joinType = string.Format("{0} JOIN", join.ToString().ToUpper());
            if (_tables.Count == 0)
            {
                _tables.Add(table1Name);
                _tables.Add(table2Name);
                Join(string.Format("{0} {1} {2} ON {3}", table1Name, joinType, table2Name, onExpression));
            }
            else if (_tables.Exists(a => table1Name == a))
            {
                _tables.Add(table2Name);
                Join(string.Format("{0} {1} ON {2}", joinType, table2Name, onExpression));
            }
            else
            {
                _tables.Add(table1Name);
                Join(string.Format("{0} {1} ON {2}", joinType, table1Name, onExpression));
            }
            return this;
        }
        #endregion

        #region property
        public Dictionary<string, object> _param { get; set; }
        public StringBuilder _columnBuffer = new StringBuilder();
        public StringBuilder _havingBuffer = new StringBuilder();
        public StringBuilder _whereBuffer = new StringBuilder();
        public StringBuilder _groupBuffer = new StringBuilder();
        public StringBuilder _orderBuffer = new StringBuilder();
        public StringBuilder _distinctBuffer = new StringBuilder();
        public StringBuilder _countBuffer = new StringBuilder();
        public StringBuilder _join = new StringBuilder();
        public List<string> _tables = new List<string>();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region build
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex != null && pageCount != null)
            {
                sqlBuffer.AppendFormat(" LIMIT {0},{1}", pageIndex, pageCount);
            }
            var sql = sqlBuffer.ToString();
            return sql;
        }
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {

                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _join);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        #endregion
    }
}
