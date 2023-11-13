using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Dapper.Linq.Util;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    public class MySqlQuery<T> : IQueryable<T> where T : class
    {
        #region constructor
        public DbContext _context { get; }
        public string Prefix { get; }
        public string View { get; }
        public string Alias {get;}
        public MySqlQuery(DbContext dbcontext = null, string view = null, string alias = null, DynamicParameters parameters = null)
        {
            View = view;
            Prefix = "@";
            _context = dbcontext;
            Alias = alias;
            Param = parameters ?? new DynamicParameters();
        }
        public MySqlQuery(DynamicParameters param)
        {
            Prefix = "@";
            Param = param;
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
                _filters.AddRange(ExpressionUtil.BuildColumns(columns, Param, Prefix).Select(s => s.Value));
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
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T> Having(string expression, object param = null, bool condition = true)
        {
            if (condition)
            {
                if (_havingBuffer.Length > 0)
                {
                    _havingBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _havingBuffer.Append(expression);
                AddParam(param);
            }
            return this;
        }
        public IQueryable<T> Having(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Having(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => s.Value)));
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
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => string.Format("{0} DESC", s.Value))));
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
                var columns = ExpressionUtil.BuildColumn(column, Param, Prefix).First();
                _setBuffer.AppendFormat("{0} = {1}", columns.Value, subquery.Build(Param, Prefix));
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
                var columns = ExpressionUtil.BuildColumn(column, Param, Prefix).First();
                var key = string.Format("{0}{1}", columns.Key, Param.ParameterNames.Count());
                Param.Add(key, value);
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
                var columnName = ExpressionUtil.BuildColumn(column, Param, Prefix, Alias).First().Value;
                var expression = ExpressionUtil.BuildExpression(value, Param, Prefix, Alias);
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
        public IQueryable<T> Where(string expression, object param = null, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
                AddParam(param);
            }
            return this;
        }
        public IQueryable<T> Where(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, Param, Prefix, Alias));
            }
            return this;
        }
        public int Delete(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildDelete();
                return _context.Execute(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<int> DeleteAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildDelete();
                return await _context.ExecuteAsync(sql, Param, timeout);
            }
            return 0;
        }
        public int Insert(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert(expression);
                return _context.Execute(sql, Param, timeout);
            }
            return 0;
        }
        public long InsertReturnId(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert(expression);
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return _context.ExecuteScalar<long>(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert(expression);
                return await _context.ExecuteAsync(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<long> InsertReturnIdAsync(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert(expression);
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return await _context.ExecuteScalarAsync<long>(sql, Param, timeout);
            }
            return 0;
        }
        public int Insert(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                return _context.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                return await _context.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public long InsertReturnId(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return _context.ExecuteScalar<long>(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<long> InsertReturnIdAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return await _context.ExecuteScalarAsync<long>(sql, entity, timeout);
            }
            return 0;
        }
        public int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                return _context.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildInsert();
                return await _context.ExecuteAsync(sql, entitys, timeout);
            }
            return 0;
        }
        public int Update(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return _context.Execute(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return await _context.ExecuteAsync(sql, Param, timeout);
            }
            return 0;
        }
        public int Update(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate();
                return _context.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public int Update(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate(expression);
                return _context.Execute(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(Expression<Func<T, T>> expression, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate(expression);
                return await _context.ExecuteAsync(sql, Param, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate();
                return await _context.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate();
                return _context.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildUpdate();
                return await _context.ExecuteAsync(sql, entitys, timeout);
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
                ExpressionUtil.BuildColumns(columns, Param, Prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Single<TResult>(columnstr, buffered, timeout);
        }
        public Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columnstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, Param, Prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SingleAsync<TResult>(columnstr, timeout);
        }

        public List<T> ToList()
        {
            var sql = BuildSelect();
            return _context.Query<T>(sql, Param, false);
        }

        public Task<List<T>> ToListAsync()
        {
            var sql = BuildSelect();
            return _context.QueryAsync<T>(sql, Param);
        }

        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_context != null)
            {
                var sql = BuildSelect();
                return _context.Query<T>(sql, Param, buffered, timeout);
            }
            return new List<T>();
        }

        public async Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_context != null)
            {
                var sql = BuildSelect();
                return await _context.QueryAsync<T>(sql, Param, timeout);
            }
            return new List<T>();
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_context != null)
            {
                var sql = BuildSelect();
                return _context.Query<TResult>(sql, Param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_context != null)
            {
                var sql = BuildSelect();
                return await _context.QueryAsync<TResult>(sql, Param, timeout);
            }
            return new List<TResult>();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, Param, Prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, Param, Prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
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
                if (_context != null)
                {
                    var sql = BuildCount();
                    return _context.ExecuteScalar<long>(sql, Param, timeout);
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
                if (_context != null)
                {
                    var sql = BuildCount();
                    return await _context.ExecuteScalarAsync<long>(sql, Param, timeout);
                }
            }
            return 0;
        }
        public long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return Count(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => s.Value)), condition, timeout);
        }
        public Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return CountAsync(string.Join(",", ExpressionUtil.BuildColumns(expression, Param, Prefix).Select(s => s.Value)), condition, timeout);
        }
        public bool Exists(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildExists();
                return _context.ExecuteScalar<int>(sql, Param, timeout) > 0;
            }
            return false;
        }
        public async Task<bool> ExistsAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _context != null)
            {
                var sql = BuildExists();
                return await _context.ExecuteScalarAsync<int>(sql, Param, timeout) > 0;
            }
            return false;
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, Param, Prefix,Alias);
                _sumBuffer.AppendFormat("{0}", column);
                if (_context != null)
                {
                    var sql = BuildSum();
                    return _context.ExecuteScalar<TResult>(sql, Param, timeout);
                }
            }
            return default;
        }
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, Param, Prefix, Alias);
                _sumBuffer.AppendFormat("{0}", column);
                if (_context != null)
                {
                    var sql = BuildSum();
                    return await _context.ExecuteScalarAsync<TResult>(sql, Param, timeout);
                }
            }
            return default;
        }
        #endregion

        #region property
        public DynamicParameters Param { get; set; }
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
        public string GetTableName()
        {
            if (!string.IsNullOrEmpty(View))
            {
                var alias = string.IsNullOrEmpty(Alias) ? "t" : Alias;
                return $"({View}) AS {alias}";
            }
            else if(!string.IsNullOrEmpty(Alias))
            {
                return $"{_table.TableName} AS {Alias}";
            }
            else
            {
                return $"{_table.TableName}";
            }
        }
        public void AddParam(object param)
        {
            if (param == null)
            {
                return;
            }
            if (param is Dictionary<string, object> values)
            {
                foreach (var item in values)
                {
                    Param.Add(item.Key, item.Value);
                }
                return;
            }
            var propertities = param.GetType().GetProperties();
            foreach (var item in propertities)
            {
                var name = item.Name;
                var value = item.GetValue(param);
                Param.Add(name, value);
            }
        }
        public string BuildInsert(Expression expression = null)
        {
            if (expression == null)
            {
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})"
                    , GetTableName()
                    , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => s.ColumnName))
                    , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("@{0}", s.CSharpName))));
                return sql;
            }
            else
            {
                var columns = ExpressionUtil.BuildColumnAndValues(expression, Param, Prefix);
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})"
                    , GetTableName()
                    , string.Join(",", columns.Keys)
                    , string.Join(",", columns.Values));
                return sql;
            }
        }
        public string BuildUpdate(bool allColumn = true)
        {
            if (allColumn)
            {
                var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
                var columns = _table.Columns.FindAll(f => f.ColumnKey != ColumnKey.Primary && !_filters.Exists(e => e == f.ColumnName));
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}"
                    , GetTableName()
                    , string.Join(",", columns.Select(s => string.Format("{0} = @{1}", s.ColumnName, s.CSharpName)))
                    , _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
                return sql;
            }
            else
            {
                var sql = string.Format("UPDATE {0} SET {1}{2}"
                    , GetTableName()
                    , _setBuffer
                    , _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
                return sql;
            }

        }
        public string BuildUpdate(Expression expression)
        {
            var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
            var columns = ExpressionUtil.BuildColumnAndValues(expression, Param, Prefix).Where(a => a.Key != keyColumn.ColumnName);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    GetTableName(),
                    string.Join(",", columns.Select(s => string.Format("{0} = {1}", s.Key, s.Value))),
                    _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
            return sql;
        }
        public string BuildDelete()
        {
            var sql = string.Format("DELETE FROM {0}{1}",
                GetTableName(),
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
            sqlBuffer.AppendFormat(" FROM {0}", GetTableName());
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

            sqlBuffer.AppendFormat(" FROM {0}", GetTableName());
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

            sqlBuffer.AppendFormat("SELECT 1 FROM {0}", GetTableName());
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
            sqlBuffer.AppendFormat("SELECT SUM({0}) FROM {1}", _sumBuffer, GetTableName());
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            return sqlBuffer.ToString();
        }

        public SqlBuilder Build()
        {
            var sb = new SqlBuilder(Param);
            if (_whereBuffer.Length>0)
            {
                sb.Where(_whereBuffer.ToString());
            }
            if (_havingBuffer.Length>0)
            {
                sb.Having(_havingBuffer.ToString());
            }
            return sb;
        }
        #endregion
    }
}
