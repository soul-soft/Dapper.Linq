using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper.Linq
{
    public interface IDbContext : IDisposable
    {
        bool? Buffered { get; set; }
        int? Timeout { get; set; }
        List<Logger> Loggers { get; }
        IQueryable<T> From<T>() where T : class;
        IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class;
        IQueryable<T1, T2, T3> From<T1, T2, T3>() where T1 : class where T2 : class where T3 : class;
        IQueryable<T1, T2, T3, T4> From<T1, T2, T3, T4>() where T1 : class where T2 : class where T3 : class where T4 : class;
        GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        DatasourceType SourceType { get; }
        DbContextState State { get; }
        void Open(bool beginTransaction, IsolationLevel? isolationLevel = null);
        Task OpenAsync(bool beginTransaction, IsolationLevel? isolationLevel = null);
        void Commit();
        void Rollback();
        void Close();
    }
    public class DbContext : IDbContext
    {
        public DatasourceType SourceType { get; set; }
        public List<Logger> Loggers { get; set; }
        public IDbTransaction Transaction { get; private set; }
        public IDbConnection Connection { get; }
        public bool? Buffered { get; set; }
        public int? Timeout { get; set; }
        public DbContextState State { get; private set; }
        public DbContext(IDbConnection connection, DatasourceType sourceType)
        {
            Connection = connection;
            SourceType = sourceType;
            State = DbContextState.Closed;
        }
        public void Open(bool beginTransaction, IsolationLevel? level = null)
        {
            if (!beginTransaction)
            {
                Connection.Open();
            }
            else
            {
                Connection.Open();
                Transaction = level == null ? Connection.BeginTransaction() : Connection.BeginTransaction(level.Value);
            }
            State = DbContextState.Open;
        }
        public async Task OpenAsync(bool beginTransaction, IsolationLevel? level = null)
        {
            State = DbContextState.Open;
            if (!(Connection is DbConnection))
            {
                throw new InvalidOperationException("Async operations require use of a DbConnection or an already-open IDbConnection");
            }
            if (!beginTransaction)
            {
                await (Connection as DbConnection).OpenAsync();
            }
            else
            {
                await (Connection as DbConnection).OpenAsync();
                Transaction = level == null ? Connection.BeginTransaction() : Connection.BeginTransaction(level.Value);
            }
        }
        public void Close()
        {
            Connection?.Close();
            State = DbContextState.Closed;
        }
        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                State = DbContextState.Commit;
            }
        }
        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                State = DbContextState.Rollback;
            }
        }
        public void Dispose()
        {
            Close();
        }
        public GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultiple(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultipleAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.Execute(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalar<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public IQueryable<T> From<T>() where T : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3> From<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2, T3>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2, T3>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2, T3>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3, T4> From<T1, T2, T3, T4>() where T1 : class where T2 : class where T3 : class where T4 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2, T3, T4>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2, T3, T4>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2, T3, T4>(this);
            }
            throw new NotImplementedException();
        }
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T>(sql, param, Transaction, Buffered != null ? Buffered.Value : buffered, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryAsync<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query(sql, param, Transaction, Buffered != null ? Buffered.Value : buffered, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
    }
    public class DbContextProxy : IDbContext
    {
        public IDbContext _target = null;
        public DbContextProxy(IDbContext target)
        {
            _target = target;
            Loggers = new List<Logger>();
        }
        public List<Logger> Loggers { get; set; }
        public IDbConnection Connection => _target.Connection;
        public IDbTransaction Transaction => _target.Transaction;
        public DbContextState State => _target.State;
        public DatasourceType SourceType => _target.SourceType;
        public bool? Buffered { get => _target.Buffered; set => _target.Buffered = value; }
        public int? Timeout { get => _target.Timeout; set => _target.Timeout = value; }
        public void Close()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Close();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = nameof(DbContextProxy.Close),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Commit()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Commit();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = nameof(Commit),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Dispose()
        {
            _target.Dispose();
        }
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Execute(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteAsync(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteScalar<T>(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteScalarAsync<T>(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IQueryable<T> From<T>() where T : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2>(this);
            }
            else if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3> From<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2, T3>(this);
            }
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2, T3>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2, T3>(this);
            }
            throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3, T4> From<T1, T2, T3, T4>() where T1 : class where T2 : class where T3 : class where T4 : class
        {
            if (SourceType == DatasourceType.MYSQL)
            {
                return new MySqlQuery<T1, T2, T3, T4>(this);
            }
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlQuery<T1, T2, T3, T4>(this);
            }
            else if (SourceType == DatasourceType.SQLITE)
            {
                return new SQLiteQuery<T1, T2, T3, T4>(this);
            }
            throw new NotImplementedException();
        }
        public void Open(bool beginTransaction, IsolationLevel? level = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Open(beginTransaction, level);
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = string.Format("{0}:Transaction:{1}", nameof(Open), beginTransaction ? "ON" : "OFF"),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public async Task OpenAsync(bool beginTransaction, IsolationLevel? level = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                await _target.OpenAsync(beginTransaction, level);
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = string.Format("{0}:Transaction:{1}", nameof(Open), beginTransaction ? "ON" : "OFF"),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Query<T>(sql, param, buffered, commandTimeout, commandType);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryAsync<T>(sql, param, commandTimeout, commandType);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Query(sql, param, buffered, commandTimeout, commandType);

            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryAsync(sql, param, commandTimeout, commandType);

            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryMultiple(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryMultipleAsync(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Rollback()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Rollback();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Text = nameof(Rollback),
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
    }
    public enum DbContextState
    {
        Closed = 0,
        Open = 1,
        Commit = 2,
        Rollback = 3,
    }
    public class DataSource
    {
        public Func<IDbConnection> ConnectionFacotry { get; set; }
        public DatasourceType DatasourceType { get; set; }
        public string DatasourceName { get; set; }
        public bool UseProxy { get; set; }
        public bool Default { get; set; }
    }
    public enum DatasourceType
    {
        MYSQL,
        SQLSERVER,
        ORACLE,
        SQLITE,
        NONE
    }
}
