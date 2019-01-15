using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// 事物会话基础对象
    /// </summary>
    public class Session : ISession
    {
        #region Constructor
        public Session() : this(null)
        {

        }
        public Session(DbConnection connection)
        {
            Connection = connection;
            State = SessionState.Close;
        }
        #endregion

        #region Propertiy
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection Connection { get; set; }
        /// <summary>
        /// 数据库事物
        /// </summary>
        public DbTransaction Transaction { get; set; }
        /// <summary>
        /// 会话状态
        /// </summary>
        public SessionState State { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 查询缓存
        /// </summary>
        public bool Buffered { get; set; }
        #endregion

        #region SqlFrom
        /// <summary>
        /// 构建SQL语句的操作对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IFrom<T> From<T>(ISession session, string from) where T : class, new()
        {
            return new MysqlFrom<T>(session, from);
        }
        public IFrom<T> From<T>() where T : class, new()
        {
            return From<T>(this, null);
        }
        public IFrom<object> From(string from)
        {
            return From<object>(this, from);
        }
        #endregion

        #region Execute
        /// <summary>
        /// 执行SQL语句并返会影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Execute(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.Execute(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 异步执行SQL语句并返会影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteAsync(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalar<T>(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalar(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 异步查询单个数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalarAsync(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 异步执行查询单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, Transaction, Timeout, text);
        }
        #endregion

        #region Query

        /// <summary>
        /// 执行SQL语句并返会查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.Query<T>(sql, param, Transaction, Buffered, Timeout, text);
        }
        /// <summary>
        /// 异步执行SQL语句并返会查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryAsync<T>(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> Query(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.Query(sql, param, Transaction, Buffered, Timeout, text);
        }
        /// <summary>
        /// 异步执行查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryAsync(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 执行多个SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public SqlMapper.GridReader QueryMultiple(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultiple(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// 异步执行多个SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultipleAsync(sql, param, Transaction, Timeout, text);
        }
        #endregion

        #region ExecuteReader
        public IDataReader ExecuteReader(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteReader(sql, param, Transaction, Timeout, text);
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteReaderAsync(sql, param, Transaction, Timeout, text);
        }
        #endregion

        #region ADO.NET

        /// <summary>
        /// 开启会话
        /// </summary>
        /// <param name="autocommit">是否自动提交</param>
        public void Open(bool autoCommit)
        {
            if (Connection != null && State == SessionState.Close)
            {
                Connection.Open();
                Transaction = autoCommit ? null : Connection.BeginTransaction();
                State = SessionState.Open;
            }
        }
        /// <summary>
        /// 异步开启会话
        /// </summary>
        /// <param name="auto"></param>
        public Task OpenAsync(bool auto)
        {
            Task task = null;

            if (Connection != null && State == SessionState.Close)
            {
                task = Connection.OpenAsync();
                Transaction = auto ? null : Connection.BeginTransaction();
                State = SessionState.Open;
            }
            return task;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (Transaction != null && State == SessionState.Open)
            {
                Transaction.Commit();
                State = SessionState.Commit;
            }
        }
        /// <summary>
        /// 会滚事物
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null && State == SessionState.Open)
            {
                Transaction.Rollback();
                State = SessionState.Rollback;
            }
        }
        /// <summary>
        /// 关闭事物
        /// </summary>
        public void Close()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }
            if (Connection != null && State != SessionState.Close)
            {
                Connection.Close();
                Connection.Dispose();
            }
            State = SessionState.Close;
        }
        #endregion

        #region Logger
        public string Logger()
        {
            return "Please Set SessionFactory.SessionProxy = true";
        }
        #endregion

    }

}
