using Dapper.Common;
using System;
using System.Data.SQLite;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = "sqlite/sku.sqlite";
            SessionFactory.AddDataSource(new DataSource()
            {
                Default = true,
                Name = "sqlite",
                Source = () => new SQLiteConnection(sb.ToString()),
                UseProxy = true,
                SourceType = DataSourceType.SQLITE
            });

            var session = SessionFactory.GetSession();
            try
            {
                var m = 50;
                var fff = new { F = new { FF = 50 } };

                session.Open(true);
                var list = session.From<Member>().Where(a => a.CreateTime==null).Select();
                session.Commit();
                  
                
            }
            catch (Exception e)
            {

            }
           
        }
    }
    [Table("member")]
    public class Member
    {
        [Column("id", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("balance")]
        public double Balance { get; set; }
        [Column("nick_name")]
        public string NickName { get; set; }
        [Column("create_time")]
        public DateTime? CreateTime { get; set; }
    }
    [Table("school")]
    public class School
    {
        [Column("id", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("mid")]
        public int? MId { get; set; }
        [Column("sname")]
        public string SName { get; set; }
    }
    public class Fun
    {
        [Function]
        public static long Count<T>(T o)
        {
            return 0;
        }
    }
}
