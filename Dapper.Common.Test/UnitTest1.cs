using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.Extension.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //mysql
            SessionFactory.AddDataSource(new DataSource()
            {
                SourceType = DataSourceType.SQLSERVER,
                Source = () => new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"),
                UseProxy = true,
                Name = "mysql",
            });
            var sb = new SqlConnectionStringBuilder
            {
                InitialCatalog = "test",
                Pooling = false,
                Password = "1024",
                UserID = "sa",
                FailoverPartner = @"PC033\SQLEXPRESS"
            };

            //Sqlserver
            SessionFactory.AddDataSource(new DataSource()
            {
                SourceType = DataSourceType.SQLSERVER,
                Source = () => new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=test;Integrated Security=True"),
                UseProxy = true,
                Name = "sqlserver",
            });

            var session = SessionFactory.GetSession("mysql");
            session.Open(false);
            try
            {
                var ids = new int[] { 1,2,3,4};
                var f = new Member() { NickName="fff,cc"};
                session.From<Member>()
                   .Where(a => a.NickName.In(f.NickName.Split(',')))
                   .Select();
                session.Close();
            }
            catch (Exception e)
            {

                throw;
            }

        }

    }
    [Table("member")]
    public class Member
    {
        [Column("id", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("nick_name", ColumnKey.None, false)]
        public string NickName { get; set; }
        [Column("create_time", ColumnKey.None, false)]
        public DateTime? CreateTime { get; set; }
        [Column("balance", ColumnKey.None, false)]
        public decimal? Balance { get; set; }
    }
    [Table("member_bill")]
    public class MemberBill
    {
        [Column("ID", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("MEMBER_ID", ColumnKey.None, false)]
        public int? MemberId { get; set; }
        [Column("FEE", ColumnKey.None, false)]
        public decimal? Fee { get; set; }
    }
    [Table("member_order")]
    public class MemberOrder
    {
        [Column("ID", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("MEMBER_ID", ColumnKey.None, false)]
        public int? MemberId { get; set; }
        [Column("order_name", ColumnKey.None, false)]
        public string OrderName { get; set; }
    }
}
