using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper.Common
{
    /// <summary>
    /// 事物回话接口
    /// </summary>
    public interface ISession:IDisposable
    {
        #region SqlFrom
        /// <summary>
        /// 返回一个构建SQL的对象
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <returns></returns>
        IFrom<TTable> From<TTable>() where TTable : class, new();
        #endregion

        #region Execute
        /// <summary>
        /// 执行SQL语句并返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null, CommandType text = CommandType.Text);

        /// <summary>
        /// 异步执行SQL语句并返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string sql, object param = null, CommandType text = CommandType.Text);
        #endregion

        #region ExecuteReader
        /// <summary>
        /// 执行查询返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步查询返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, CommandType text = CommandType.Text);
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 查询单个对象并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 查询单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步查询单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType text = CommandType.Text);
        #endregion

        #region Query
        /// <summary>
        /// 执行SQL语句并返回查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 执行查询返回dynamic类型
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        IEnumerable<dynamic> Query(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 执行异步查询返回dynamic类型
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步执行SQL语句并返回查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 执行多SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        GridReader QueryMultiple(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步执行多SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<GridReader> QueryMultipleAsync(string sql, object param = null, CommandType text = CommandType.Text);
        #endregion

        #region ADO.NET
        /// <summary>
        /// 开启回话
        /// </summary>
        /// <param name="autoCommit">是否自动提交</param>
        void Open(bool autoCommit);
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 回滚事物
        /// </summary>
        void Rollback();
        /// <summary>
        /// 关闭事物
        /// </summary>
        void Close();
        /// <summary>
        /// 超时时间
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// 查询缓存
        /// </summary>
        bool Buffered { get; set; }
        /// <summary>
        /// 回话日志
        /// </summary>
        /// <returns></returns>
        string Logger();
        #endregion

    }

    /// <summary>
    /// 事物回话状态
    /// </summary>
    public enum SessionState
    {
        /// <summary>
        /// 回话关闭
        /// </summary>
        Closed = 0,
        /// <summary>
        /// 回话开启
        /// </summary>
        Open = 1,
        /// <summary>
        /// 回话关闭
        /// </summary>
        Commit = 2,
        /// <summary>
        /// 回话回滚
        /// </summary>
        Rollback = 3
    }

}
