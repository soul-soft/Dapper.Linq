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
                Pooling = true,
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

            var session1 = SessionFactory.GetSession("sqlserver");
            var ss = session1.From<Member>().Take(4).Select(s => new Member()
            {
                NickName = s.NickName,
                Balance = s.Balance
            });

        }

    }

}
