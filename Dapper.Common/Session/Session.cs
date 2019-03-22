using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// 事物会话基础对象
    /// </summary>
    internal class Session : ISession
    {
        #region Constructor
        /// <summary>
        /// 创建数会话
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dataSourceType"></param>
        public Session(IDbConnection connection,DataSourceType dataSourceType)
        {
            Connection = connection;
            DataSourceType = dataSourceType;
            State = SessionState.Closed;
        }
        #endregion

        #region Propertiy
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection Connection { get; set; }
        /// <summary>
        /// 数据库事物
        /// </summary>
        public IDbTransaction Transaction { get; set; }
        /// <summary>
        /// 会话状态
        /// </summary>
        public SessionState State { get; private set; }
        /// <summary>
        /// 数据源类型
        /// </summary>
        private DataSourceType DataSourceType { get; set; }
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
        /// 返回构建sql的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IFrom<T> From<T>() where T : class, new()
        {
            if (DataSourceType==DataSourceType.MYSQL)
            {
                return new MysqlFrom<T>()
                {
                    Session=this,
                };
            }
            throw new Exception("Unrealized");
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
        /// <summary>
        /// ExecuteReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteReader(sql, param, Transaction, Timeout, text);
        }
        /// <summary>
        /// ExecuteReaderAsync
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>

        public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteReaderAsync(sql, param, Transaction, Timeout, text);
        }
        #endregion

        #region ADO.NET
        /// <summary>
        /// 开启会话
        /// </summary>
        /// <param name="autoCommit">是否自动提交</param>
        public void Open(bool autoCommit)
        {
            if (Connection != null && State == SessionState.Closed)
            {
                Connection.Open();
                Transaction = autoCommit ? null : Connection.BeginTransaction();
                State = SessionState.Open;
            }
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
            if (Connection != null && State != SessionState.Closed)
            {
                Connection.Close();
                Connection.Dispose();
            }
            State = SessionState.Closed;
        }
        #endregion

        #region Logger
        /// <summary>
        /// 输出会话日志
        /// </summary>
        /// <returns></returns>
        public string Logger()
        {
            return "Please Set SessionFactory.StaticProxy = true";
        }

        public void Dispose()
        {
            this.Close();
        }
        #endregion

    }

}
