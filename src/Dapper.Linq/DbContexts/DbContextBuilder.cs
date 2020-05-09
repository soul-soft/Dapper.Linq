using System.Data;

namespace Dapper
{
    public class DbContextBuilder
    {
        public IXmlResovle XmlResovle { get; set; }
        public IDbConnection Connection { get; set; }
        public DbContextType DbContextType { get; set; }
        public IEntityMapper TypeMapper { get; set; }
    }
}
