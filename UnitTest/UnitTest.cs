using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Common.Model;
using Dapper;
using Dapper.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Initialize()
        {
            //配置数据源为mysql
            SessionFactory.DataSources
                .Add(new DataSource()
                {
                    Name = "mysql2",
                    Source = () => new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"),//() => new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"),
                    Type = DataSourceType.MYSQL,
                });
            //下划线不铭感
            //Session使用静态代理,记录会话日志,生产模式设置false
            SessionFactory.StaticProxy = true;
        }

        [TestMethod]
        public void TestMethod2()
        {
            var session = SessionFactory.GetSession();
            try
            {
                //var age = 100;
                session.Open(false);
                var id1 = 53;
                var stopwatch1 = new Stopwatch();
                stopwatch1.Start();
               var list1 = session.From<Student>()
                    .Where(a => a.Id == id1)
                    .Select();
                stopwatch1.Stop();
                var id2 = 53;
                var stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                var list2 = session.From<Student>()
                   .Where(a => a.Id == id2)
                   .Select();
                stopwatch2.Stop();
                var id3 = 53;
                var stopwatch3 = new Stopwatch();
                stopwatch3.Start();
                var list3 = session.From<Student>()
                   .Where(a => a.Id == id3)
                   .Select();
                stopwatch3.Stop();

            }
            catch (Exception e)
            {

                throw e;
            }
            session.Commit();
        }
    }
    public class Tas
    {
        public object Max { get; set; }
        public object Min { get; set; }
    }
}
