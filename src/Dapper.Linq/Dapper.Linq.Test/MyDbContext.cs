using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Linq.Test
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(IDbConnection connection)
            : base(connection)
        {

        }

        protected override void Logging(string sql)
        {
            Console.WriteLine(sql);
        }
    }
}
