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
    /// 事物会话接口
    /// </summary>
    public interface ISession
    {
        #region SqlFrom
        /// <summary>
        /// 返回一个通过模型构建SQL的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IFrom<T> From<T>(ISession session,string from=null) where T : class, new();
        /// <summary>
        /// 返回一个通过模型构建SQL的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IFrom<T> From<T>() where T : class, new();
        /// <summary>
        /// 指定表
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        IFrom<dynamic> From(string from);
        #endregion

        #region Execute
        /// <summary>
        /// 执行SQL语句并返会影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null, CommandType text = CommandType.Text);

        /// <summary>
        /// 异步执行SQL语句并返会影响行数
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
        /// 执行SQL语句并返会查询结果
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
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, CommandType text = CommandType.Text);
        /// <summary>
        /// 异步执行SQL语句并返会查询结果
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
        /// 开启会话
        /// </summary>
        /// <param name="auto">是否自动提交</param>
        void Open(bool auto);
        /// <summary>
        /// 异步打开连接
        /// </summary>
        /// <param name="auto"></param>
        Task OpenAsync(bool auto);
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 会滚事物
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
        /// 缓存
        /// </summary>
        bool Buffered { get; set; }
        /// <summary>
        /// 会话日志
        /// </summary>
        /// <returns></returns>
        string Logger();
        #endregion

    }

    /// <summary>
    /// 事物会话状态
    /// </summary>
    public enum SessionState
    {
        /// <summary>
        /// 会话关闭
        /// </summary>
        Close = 0,
        /// <summary>
        /// 会话开启
        /// </summary>
        Open = 1,
        /// <summary>
        /// 会话关闭
        /// </summary>
        Commit = 2,
        /// <summary>
        /// 会话会滚
        /// </summary>
        Rollback = 3
    }

}
