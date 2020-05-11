using System.Data;

namespace Dapper
{
    public class DbContextBuilder
    {
        public IDbConnection Connection { get; set; }
        public DbContextType DbContextType { get; set; }
    }
}
