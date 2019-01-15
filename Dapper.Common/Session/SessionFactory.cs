using System;
using System.Configuration;
using System.Data.Common;

namespace Dapper.Common
{

    /// <summary>
    /// 会话工厂
    /// </summary>
    public static class SessionFactory
    {
        /// <summary>
        /// 是否使用会话代理
        /// </summary>
        public static bool SessionProxy { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public static Func<DbConnection> DataSource { get; set; }
        /// <summary>
        /// 设置下划线不敏感
        /// </summary>
        /// <param name="flag"></param>
        public static bool MatchNamesWithUnderscores
        {
            set
            {
                DefaultTypeMap.MatchNamesWithUnderscores = value;
            }
        }
        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="useProxy">是否使用会话代理</param>
        /// <returns></returns>
        public static ISession GetSession()
        {
            ISession session = null;
            var connection = DataSource();
            if (SessionProxy)
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
    }

}
