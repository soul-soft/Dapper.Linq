using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
                    Source = () => new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"),
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
            session.Open(false);
            session.From<Student>().Select(s=>new Student()
            {
                Age=s.Age,
                Name=s.Name
            });
            session.Commit();
        }
    }
    public class Tas
    {
        public object Max { get; set; }
        public object Min { get; set; }
    }
}
