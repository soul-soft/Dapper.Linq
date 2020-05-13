using Dapper.Expressions;
using System.Data;

namespace Dapper
{
    public class DbContextBuilder
    {
        /// <summary>
        /// 设置要托管的数据库连接
        /// </summary>
        public IDbConnection Connection { get; set; }
        /// <summary>
        /// 设置数据库类型
        /// </summary>
        public DbContextType DbContextType { get; set; }
    }
}
