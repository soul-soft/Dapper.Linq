using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Dapper.Extension.Util;

namespace Dapper.Extension.SqlServer
{
    public class SqlQuery<T> : IQueryable<T> where T : class
    {
        #region constructor
        public ISession _session { get; }
        public SqlQuery(ISession session = null)
        {
            _session = session;
            _param = new Dictionary<string, object>();
        }
        public SqlQuery(Dictionary<string, object> param)
        {
            _param = param;
        }
        #endregion

        #region implement
        public IQueryable<T> With(string locks, bool condition = true)
        {
            if (condition)
            {
                _lock.Append(locks);
            }
            return this;
        }

        public IQueryable<T> With(Lock locks, bool condition = true)
        {
            if (condition)
            {
                var temp = string.Empty;
                if (locks == Lock.UPDLOCK)
                {
                    With("UPDLOCK");
                }
                else if (locks == Lock.NOLOCK)
                {
                    With("NOLOCK");
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
                _filters.AddRange(ExpressionUtil.BuildColumns<T>(columns, _param).Select(s => s.Value));
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
            GroupBy(string.Join(",", ExpressionUtil.BuildColumns<T>(expression, _param).Select(s => s.Value)),condition);
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
        public IQueryable<T> Having(Expression<Func<T, bool>> expression, bool condition = true)
        {
            if (condition)
            {
                Having(string.Join(",", ExpressionUtil.BuildColumns<T>(expression, _param).Select(s => s.Value)));
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
            OrderBy(string.Join(",", ExpressionUtil.BuildColumns<T>(expression, _param).Select(s => string.Format("{0} ASC", s.Value))), condition);
            return this;
        }
        public IQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            OrderBy(string.Join(",", ExpressionUtil.BuildColumns<T>(expression, _param).Select(s => string.Format("{0} DESC", s.Value))), condition);
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
        public IQueryable<T> Set(string expression, Action<Dictionary<string, object>> action = null, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                action?.Invoke(_param);
                _setBuffer.AppendFormat(expression);
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
                var columns = ExpressionUtil.BuildColumn<T>(column, _param).First();
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
                var columnName = ExpressionUtil.BuildColumn<T>(column, _param).First().Value;
                var expression = ExpressionUtil.BuildExpression<T>(value, _param);
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
        public IQueryable<T> Take(int count)
        {
            Skip(0, count);
            return this;
        }
        public IQueryable<T> Where(string expression, Action<Dictionary<string, object>> action = null, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", ExtensionUtil.GetCondition(ExpressionType.AndAlso));
                }
                action?.Invoke(_param);
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> Where(Expression<Func<T, bool>> expression, bool condition = true)
        {
            Where(ExpressionUtil.BuildExpression<T>(expression, _param),null,condition);
            return this;
        }
        public int Delete(bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildDelete();
                return _session.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public int Insert(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildInsert();
                return _session.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public long InsertReturnId(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return _session.ExecuteScalar<long>(sql, entity, timeout);
            }
            return 0;
        }
        public int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildInsert();
                return _session.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public int Update(bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildUpdate();
                return _session.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public int Update(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildUpdate();
                return _session.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildUpdate();
                return _session.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public T Single(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns<T>(columns, _param);
            return Single<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key))),
                buffered,
                timeout);
        }
        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_session != null)
            {
                var sql = BuildSelect();
                return _session.Query<T>(sql, _param, buffered, timeout);
            }
            return new List<T>();
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_session != null)
            {
                var sql = BuildSelect();
                return _session.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns<T>(columns, _param);
            return Select<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key)))
                , buffered, timeout);
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_session != null)
                {
                    var sql = BuildCount();
                    return _session.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                return Count(string.Join(",", ExpressionUtil.BuildColumns<T>(expression, _param).Select(s => s.Value)), condition, timeout);
            }
            return 0;
        }
        public bool Exists(bool condition = true, int? timeout = null)
        {
            if (condition && _session != null)
            {
                var sql = BuildExists();
                return _session.ExecuteScalar<int>(sql, _param, timeout) > 0;
            }
            return false;
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildColumn<T>(expression, _param).First();
                _sumBuffer.AppendFormat("{0}", column.Value);
                if (_session != null)
                {
                    var sql = BuildSum();
                    return _session.ExecuteScalar<TResult>(sql, _param, timeout);
                }
            }
            return default(TResult);
        }
        #endregion

        #region property
        public Dictionary<string, object> _param { get; set; }
        public Dictionary<string, string> _columns = new Dictionary<string, string>();
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
        public Table _table = EntityUtil.GetTable<T>();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region build
        public string BuildInsert()
        {
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                _table.TableName,
                string.Join(",", _table.Columns.FindAll(f => f.Identity == false).Select(s => s.ColumnName))
                , string.Join(",", _table.Columns.FindAll(f => f.Identity == false).Select(s => string.Format("@{0}", s.CSharpName))));
            return sql;
        }
        public string BuildUpdate()
        {
            if (_setBuffer.Length == 0)
            {
                var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
                var colums = _table.Columns.FindAll(f => f.ColumnKey != ColumnKey.Primary && !_filters.Exists(e => e == f.ColumnName));
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    _table.TableName,
                    string.Join(",", colums.Select(s => string.Format("{0}=@{1}", s.ColumnName, s.CSharpName))),
                    string.Format("{0}=@{1}", keyColumn.ColumnName, keyColumn.CSharpName)
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
            var sqlBuffer = new StringBuilder();
            if (pageIndex == 0 && pageCount > 0)
            {
                sqlBuffer.AppendFormat("SELECT TOP {0}", pageCount);
            }
            else
            {
                sqlBuffer.AppendFormat("SELECT");
            }
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer.Length > 0 ? _distinctBuffer.ToString() : "");
            }
            if (_columnBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            }
            else
            {
                sqlBuffer.AppendFormat(" {0}", string.Join(",", _table.Columns.FindAll(f => !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("{0} AS {1}", s.ColumnName, s.CSharpName))));
            }
            if (pageIndex > 0)
            {
                sqlBuffer.AppendFormat(",ROW_NUMBER()OVER(ORDER BY {0}) AS RowNum",
                    _orderBuffer.Length > 0 ? _orderBuffer.ToString() :
                    _groupBuffer.Length > 0 ? _groupBuffer.ToString() :
                    _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary).ColumnName);
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_lock.Length > 0)
            {
                sqlBuffer.AppendFormat(" WITH({0})", _lock);
            }
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
            if (_orderBuffer.Length > 0 && (pageIndex == null || pageIndex == 0))
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex > 0)
            {
                return string.Format("SELECT TOP {0} {1} FROM ({2}) AS T WHERE RowNum>{3}", pageCount,
                    _columns.Count > 0 ? string.Join(",", _columns.Keys) : "*",
                    sqlBuffer,
                    pageIndex);
            }
            else
            {
                return sqlBuffer.ToString();
            }
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
            var sql = string.Format("SELECT 1 WHERE EXISTS({0})", sqlBuffer);
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
}
