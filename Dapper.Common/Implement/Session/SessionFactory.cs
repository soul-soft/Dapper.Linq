using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly static Dictionary<string, Func<DbConnection>> DataSouces = new Dictionary<string, Func<DbConnection>>(32);
        /// <summary>
        /// 静态代理
        /// </summary>
        public static bool StaticProxy { get; set; }
        /// <summary>
        /// 天加数据源
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        public static void AddDataSource(string name,Func<DbConnection> func)
        {
            DataSouces.Add(name,func);
        }
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
        /// <param name="name"></param>
        /// <returns></returns>
        public static ISession GetSession(string name = null)
        {
            ISession session = null;
            var connection = string.IsNullOrEmpty(name) ? DataSouces.First().Value() : DataSouces[name]();
            if (StaticProxy)
            {
                session = new SessionProxy(new Session(connection));
                session.Timeout = connection.ConnectionTimeout;
            }
            else
            {
                session = new Session(connection);
                session.Timeout = connection.ConnectionTimeout;
            }
            return session;
        }
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(string name = null)
        {
           return string.IsNullOrEmpty(name) ? DataSouces.First().Value() : DataSouces[name]();
        }
    }
   
}
