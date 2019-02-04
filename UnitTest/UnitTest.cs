using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            SessionFactory.AddDataSource("mysql1", () => new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"));
            SessionFactory.AddDataSource("mysql2", () => new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"));
            //下划线不铭感
            //Session使用静态代理,记录会话日志,生产模式设置false
            SessionFactory.StaticProxy = true;
        }
        [TestMethod]
        public void TestMethod1()
        {
            var sesion = SessionFactory.GetSession();
            /*****************INSERT*******************/
            //Dapper
            var row1 = sesion.Execute("insert into student(Age,NAME)values(@Age,@MeName)", new { Age = 20, MeName = "Dapper" });
            //扩展
            var row2 = sesion.From<Student>().Insert(new Student()
            {
                Name = "Dapper.Common",
                Age = 50,
                CreateTime = DateTime.Now
            });
            var identity = sesion.From<Student>().InsertById(new Student()
            {
                Age = 20,
                Name = "Identity"
            });
            //list
            var list = new List<Student>();
            list.Add(new Student()
            {
                Name = "Dapper.Common",
                Age = 50,
                CreateTime = DateTime.Now
            });
            var row3 = sesion.From<Student>().Insert(list);

            /*****************UPDATE*******************/
            //根据主键修改全部列
            var row4 = sesion.From<Student>().Update(new Student()
            {
                Id = 27,
                Name = "Update"
            });
            //list
            var list2 = new List<Student>();
            list2.Add(new Student()
            {
                Name = "Update List",
                Id = 27
            });
            list2.Add(new Student()
            {
                Name = "Update List",
                Id = 28
            });
            var row5 = sesion.From<Student>().Update(list2);
            //修改部分列+条件更新
            var entity = new
            {
                Age = 20,
                Name = "admin"
            };
            var row6 = sesion.From<Student>()
                //如果第一个条件为true，则更新MeName为entity.Name
                .Set(!string.IsNullOrEmpty(entity.Name), a=>a.Name.Eq("hahah"))
                //Age在原来的基础上加20
                .Set(a => a.Age.Eq(a.Age+100))
                //条件ID=30
                .Where(a => a.Id == 1)
                //要执行的操作
                .Update();
            /*****************UPDATE*******************/
            //更新实体ID删除
            var row7 = sesion.From<Student>().Delete(new Student() { Id = 30 });
            //条件删除
            var row8 = sesion.From<Student>()
                .Where(a => a.Age > 20)
                .Delete();
            /*****************Select*******************/
            //查询单个
            var student = sesion.From<Student>().Single();
            var list1 = sesion.From<Student>().Select();
            //复杂查询
            list = sesion.From<Student>()
                //查询条件
                .Where(a => a.Age > 20 && a.Id.In(new int[] { 1, 2, 3 }.ToList()))
                //排序
                .Desc(a => a.Id)
                //悲观锁
                .XLock()
                //分页
                .Limit(1, 10)
                //部分列
                .Select(s => new { s.Name });
            //分页查询，返回总记录数
            var total = 10;
            list = sesion.From<Student>()
                .Skip(1, 5, out total)
                .Select();
            /*****************动态查询*******************/
            var query = new WhereQuery<Student>();
            query
                .And(a => a.Name.Like("aa"))
                .Or(a => a.Id > 0)
                .Or(a => a.Id < 10)
                .Or(a => a.Id.In(new[] { 1, 2, 3 }))
                .And(a => a.Name.NotLike("aa"));
            var res = sesion.From<Student>().Where(query).Exists();
            var rows6 = sesion.From<Student>().Where(a=>a.Id==29).DeleteAsync();
            /*****************会话日志*******************/
            var aa = sesion.Logger();

        }

        [TestMethod]
        public void TestMethod2()
        {
            var session = SessionFactory.GetSession();
            var list = session.From<Student>()
                .GroupBy(s => new
                {
                    s.Age,
                    Nsme = DbFun.Date_Add(s.CreateTime, "INTERVAL 1 DAY")
                })
                .Having(s=> DbFun.Max(DbFun.Date_Add(s.CreateTime, "INTERVAL 1 DAY")) > new DateTime(2019,2,4))
                .Select<dynamic>(s=> new
                {
                    s.Age ,
                    Nsme = DbFun.Date_Add(s.CreateTime, "INTERVAL 1 DAY")
                });
           
        }
    }
}
