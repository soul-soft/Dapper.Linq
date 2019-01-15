using System;
using Dapper.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Initialize()
        {
            var connectionString = "server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=1;maxpoolsize=10;connectiontimeout=3600;";
            SessionFactory.DataSource = () => new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            //是否代理
            SessionFactory.SessionProxy = true;
            SessionFactory.MatchNamesWithUnderscores = true;
        }
        [TestMethod]
        public void TestMethod1()
        {
            var session1 = SessionFactory.GetSession();
            var asc = session1.From<Student>().Where(a => a.Age > 1000).Exists();
           
        }
    }
}
