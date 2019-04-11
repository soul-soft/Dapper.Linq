using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Dapper.Common
{
    /// <summary>
    /// 会话工厂
    /// </summary>
    public static class SessionFactory
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public readonly static List<DataSource> DataSources = new List<DataSource>(32);
        /// <summary>
        /// 直接设置Dapper下划线匹配
        /// </summary>
        static SessionFactory()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="name">数据源</param>
        /// <returns></returns>
        public static ISession GetSession(string name = null)
        {
            ISession session = null;
            var dataSource = GetDataSource(name);
            var connection = dataSource.Source();
            if (dataSource.StaticProxy)
            {
                session = new SessionProxy(new Session(dataSource.Source(),dataSource.Type))
                {
                    Timeout = connection.ConnectionTimeout,
                    Buffered = true,
                };
            }
            else
            {
                session = new Session(connection, dataSource.Type)
                {
                    Timeout = connection.ConnectionTimeout,
                    Buffered = true,
                };
            }
            return session;
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="name">数据源</param>
        /// <returns></returns>
        public static IDbConnection GetConnection(string name = null)
        {
            return GetDataSource(name).Source();
        }
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static DataSource GetDataSource(string name = null)
        {
            return string.IsNullOrEmpty(name) ? DataSources.First() : DataSources.Find(f => f.Name == name);
        }
    }
    /// <summary>
    /// 数据源
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataSourceType Type { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public Func<IDbConnection> Source { get; set; }
        /// <summary>
        /// 是否使用代理
        /// </summary>
        public bool StaticProxy { get; set; }
    }
    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DataSourceType
    {
        /// <summary>
        /// Mysql数据库
        /// </summary>
        MYSQL = 0,
    }
}
