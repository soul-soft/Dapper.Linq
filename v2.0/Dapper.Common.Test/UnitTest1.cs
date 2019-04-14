using System;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using Standard.Model;
using System.Data.SqlClient;

namespace Dapper.Extension.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SessionFactory.AddDataSource(new DataSource()
            {
                SourceType = DataSourceType.MYSQL,
                Source = () => new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=1;maxpoolsize=10;connectiontimeout=180;"),
                UseProxy = true,
                Name = "mysql",
            });
            var sb = new SqlConnectionStringBuilder();
            sb.InitialCatalog = "test";
            sb.Pooling = true;
            sb.Password = "1024";
            sb.UserID = "sa";
            sb.FailoverPartner = @"PC033\SQLEXPRESS";
            SessionFactory.AddDataSource(new DataSource()
            {
                SourceType = DataSourceType.SQLSERVER,
                Source = () => new SqlConnection(@"Data Source=PC033\SQLEXPRESS;Initial Catalog=test;User ID=sa;Password=1024"),
                UseProxy = true,
                Name = "sqlserver",
            });

            using (var session = SessionFactory.GetSession("mysql"))
            {
                var list = session.From<Member>().GroupBy(g => g.NickName).Paging(1,2,out long total).Select(s=>s.NickName);
                var ins = session.From<Member>().InsertReturnId(new Member() { NickName="ff"});
            }
            var stopwatch = new Stopwatch();
            var sql = string.Empty;
            stopwatch.Start();
            for (int i = 0; i < 20000; i++)
            {
                var query = new MysqlQuery<Member>();
                query.Where(a => a.Id == i).Single();
                sql = query.BuildSelect();
            }
            stopwatch.Stop();
        }

    }

}
