using System;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;
using Standard.Model;

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
                SourceType = DataSourceType.MYSQL,
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
                Source = () => new SqlConnection(@"Data Source=DESKTOP-9IS2HA6\SQLEXPRESS;Initial Catalog=test;User ID=sa;Password=1024"),
                UseProxy = true,
                Name = "sqlserver",
            });

            var session = SessionFactory.GetSession("mysql");
            session.Open(true);
            try
            {
                var list = session.From<Member, MemberBill>()
                    .Join((a, b) => a.Id == b.MemberId)
                    .Select((a, b) => new
                    {
                        a.Id,
                        a.NickName,
                        b.Fee
                    });
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
        [Column("ID", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("NICK_NAME", ColumnKey.None, false)]
        public string NickName { get; set; }
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
}
