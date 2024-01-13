using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper.Linq
{
    public class DbContext : IDisposable
    {
        private IDbConnection _connection;

        public IDbConnection Connection => _connection;


        public DbContext(IDbConnection connection)
        {
            _connection = connection;
        }

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction CurrentTransaction => _currentTransaction;

        public void OpenDbConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void ColseDbConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                CurrentTransaction?.Dispose();
                _connection.Close();
            }
        }

        public IDbContextTransaction BeginTransaction(IDbTransaction transaction = null)
        {
            var autoCloase = false;
            if (_connection.State == ConnectionState.Closed)
            {
                OpenDbConnection();
                autoCloase = true;
            }
            transaction = transaction ?? _connection.BeginTransaction();
            _currentTransaction = new DbContextTransaction(transaction, () =>
            {
                _currentTransaction = null;
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            });
            return CurrentTransaction;
        }

        public IDbTransaction GetDbTransaction()
        {
            return CurrentTransaction?.DbTransaction;
        }

        public IQueryable<T> From<T>(DynamicParameters parameters = null) where T : class
        {
            return new MySqlQuery<T>(this, parameters);
        }

        public IQueryable<T> From<T>(SqlBuilder sqlBuilder, DynamicParameters parameters = null) where T : class
        {
            return new MySqlQuery<T>(this, sqlBuilder, parameters);
        }

        [Obsolete("有严重的性能问题")]
        public IQueryable<T> FromSql<T>(string sql, DynamicParameters parameters = null) where T : class
        {
            return new MySqlQuery<T>(this, sql, false, parameters);
        }

        public IQueryable<T> FromView<T>(string sql, DynamicParameters parameters = null) where T : class
        {
            return new MySqlQuery<T>(this, sql, true, parameters);
        }

        public void Dispose()
        {
            try
            {
                _currentTransaction?.Dispose();
            }
            finally
            {
                try
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                finally
                {
                    _connection = null;
                }
            }
        }

        public GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.QueryMultiple(sql, param, GetDbTransaction(), commandTimeout, text);
        }

        public Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.QueryMultipleAsync(sql, param, GetDbTransaction(), commandTimeout, text);
        }

        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.Execute(sql, param, GetDbTransaction(), commandTimeout, text);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.ExecuteAsync(sql, param, GetDbTransaction(), commandTimeout, text);
        }

        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.ExecuteScalar<T>(sql, param, GetDbTransaction(), commandTimeout, text);
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            Logging(sql);
            return Connection.ExecuteScalarAsync<T>(sql, param, GetDbTransaction(), commandTimeout, text);
        }


        public List<T> Query<T>(string sql, object param = null, bool buffered = false, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logging(sql);
            return Connection.Query<T>(sql, param, GetDbTransaction(), buffered, commandTimeout, commandType).ToList();
        }

        public async Task<List<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logging(sql);
            return (await Connection.QueryAsync<T>(sql, param, GetDbTransaction(), commandTimeout, commandType)).ToList();
        }

        public List<dynamic> Query(string sql, object param = null, bool buffered = false, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logging(sql);
            return Connection.Query(sql, param, GetDbTransaction(), buffered, commandTimeout, commandType).ToList();
        }

        public async Task<List<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logging(sql);
            return (await Connection.QueryAsync(sql, param, GetDbTransaction(), commandTimeout, commandType)).ToList();
        }

        protected virtual void Logging(string sql)
        {

        }
    }
}
