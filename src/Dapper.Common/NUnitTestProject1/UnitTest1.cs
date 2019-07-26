using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Common;
using Dapper.Common.Util;
using System.Collections.Generic;
using NUnitTestProject1;
using System.Diagnostics;

namespace Tests
{
    public class Tests
    {

        #region SetUp：初始化配置[Initialization configuration]
        [SetUp]
        public void Setup()
        {
            DbContextFactory.AddDataSource(new DataSource()
            {
                Default = true,
                Name = "mysql",
                ConnectionFacotry = () => new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;password=1024;database=test;"),
                DatasourceType = DatasourceType.MYSQL,
                UseProxy = true//use static proxy,for logger
            });
        }
        #endregion

        #region Insert：测试新增[Test Add]
        [Test]
        public void Insert()
        {
            IDbContext context = null;
            try
            {
                context = DbContextFactory.GetDbContext();
                //because set "id[isIdentity=true]"，so not set "id" value
                var row1 = context.From<Student>().Insert(new Student()
                {
                    Grade = Grade.A,
                    CreateTime = DateTime.Now,
                    Name = "jack",
                });
                //batch added
                var row2 = context.From<Student>().Insert(new List<Student>()
                {
                    new Student()
                    {
                        Grade = Grade.C,
                        CreateTime = DateTime.Now,
                        Name = "tom",
                    },
                     new Student()
                    {
                        Grade = Grade.F,
                        CreateTime = DateTime.Now,
                        Name = "jar",
                    },
                });
            }
            catch (Exception e)
            {
                //debug sql logger
                Console.WriteLine(context.Loggers);
            }
            finally
            {
                context.Close();
            }
        }
        #endregion

        #region Update：测试修改[Test Update]
        [Test]
        public void Update()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                //param
                var age = 20;
                DateTime? time = null;
                var sid = 1;

                //subquery
                var subquery = new SubQuery<School>()
                    .Where(a => a.Id == sid)
                    .Select(s => s.Name);

                var row1 = context.From<Student>()
                    .Set(a => a.Age, a => a.Age + age)
                    .Set(a => a.Name, subquery)
                    .Set(a => a.CreateTime, time, time != null)
                    .Where(a => a.Id == 16)
                    .Update();

                //function
                context.From<Student>()
                    .Set(a => a.Name, a => MysqlFun.REPLACE(a.Name, "a", "b"))
                    .Where(a => a.Id == 14)
                    .Update();

                //lock
                var student = context.From<Student>()
                    .Where(a => a.Id == 16)
                    .Single();
                var row2 = context.From<Student>()
                    .Set(a => a.Age, 80)
                    .Set(a => a.Version, Guid.NewGuid().ToString())
                    .Where(a => a.Id == 16 && a.Version == student.Version)
                    .Update();

                //entity
                var row3 = context.From<Student>()
                    .Filter(a => a.SchoolId)
                    .Update(new Student()
                    {
                        Id = 2,
                        CreateTime = DateTime.Now
                    });
            }
        }
        #endregion

        #region Delete：测试删除[Test Delete]
        [Test]
        public void Delete()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var row1 = context.From<Student>()
                     .Where(a => a.Id == 16)
                     .Delete();

                var subquery = new SubQuery<School>()
                    .Where(a => a.Id >= 0)
                    .Select(a => a.Id);

                var row2 = context.From<Student>()
                     .Where(a => Operator.In(a.Id, subquery))
                     .Delete();
            }
        }
        #endregion

        #region 测试事务：[Test Transaction]
        [Test]
        public void Transaction()
        {
            IDbContext dbContext = null;
            try
            {
                dbContext = DbContextFactory.GetDbContext();
                dbContext.From<Student>().Insert(new Student()
                {
                    Name="stduent1"
                });
                //throw new Exception("rollback");
                dbContext.From<School>().Insert(new School()
                {
                    Name = "school1"
                });
                dbContext.Commit();
            }
            catch (Exception)
            {
                dbContext?.Rollback();
                throw;
            }
            finally
            {
                dbContext?.Close();
            }
        }
        #endregion

        #region Select：测试基本查询[Test Base Query]
        [Test]
        public void Select()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                //single
                var student = context.From<Student>()
                    .Where(a => a.Id == 19)
                    .Single();

                //subquery
                var id = 0;
                var age = 50;
                var subquery = new SubQuery<School>()
                   .Where(a => a.Id >= id)
                   .Select(a => a.Id);

                //Verify that subquery parameters are written to the current query
                var students2 = context.From<Student>()
                    .OrderBy(a => a.Age)
                    .Where(a => a.Id >= Operator.Any(subquery) && a.Age > age)
                    .Select();

                //Partial columns
                var students3 = context.From<Student>()
                   .Select(s => new
                   {
                       s.Id,
                       s.Age
                   });
            }
        }
        #endregion

        #region Groupby：测试Group查询[Test Group Query]
        [Test]
        public void GroupBy()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var students = context.From<Student>()
                    .GroupBy(a => a.Age)
                    .Having(a => MysqlFun.Count(1L) > 2)
                    .Select(s => new
                    {
                        Count = MysqlFun.Count(1L),
                        s.Age,
                    });
            }
        }
        #endregion

        #region DynamicQuery：测试动态查询[Test Dynamic Query]
        [Test]
        public void DynamicQuery()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var param = new Student()
                {
                    Name = "zs",
                    Grade = Grade.B,
                    SchoolId = null,
                    Id = null,
                    Type = 5
                };

                //多个where成立用AND连接
                var students = context.From<Student>()
                    .Where(a => a.Id == param.Id, param.Id != null)
                    .Where(a => Operator.Contains(a.Name, param.Name), param.Name != null)
                    .Where(a => a.Grade == param.Grade, param.Grade != null)
                    .Where(a => a.Id > 2 || a.Age < 80, param.Type == 5)
                    .Select();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var students2 = context.From<Student>()
                    .Where(a => a.Id == param.Id, param.Id != null)
                    .Where(a => a.Grade == param.Grade, param.Grade != null)
                    .Where(a => Operator.StartsWith(a.Name, param.Name), param.Name != null)
                    .Where(a => a.Id > 2 || a.Age > 20, param.Type == 8)
                    .Select();
                stopwatch.Stop();
            }
        }
        #endregion

        #region TakePage：测试分页查询[Test paging]
        [Test]
        public void TakePage()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var students = context.From<Student>()
                    .Page(1, 10, out long total)
                    .Select();
            }
        }
        #endregion

        #region Join：测试连接[Test Join]
        [Test]
        public void Join()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var students = context.From<Student, School>()
                    .Join((a, b) => a.SchoolId == b.Id)
                    .Select((a, b) => new
                    {
                        a.Id,
                        StuName = a.Name,
                        SchName = b.Name
                    });
            }
        }
        #endregion

        #region Other Query
        [Test]
        public void Other()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                //limit 0,10
                var students1 = context.From<Student>()
                    .Take(10)
                    .Select();

                //limit 10,20 
                var students2 = context.From<Student>()
                   .Skip(10, 20)
                   .Select();

                //Calling functions in expressions is not recommended, but n-tier attribute access is supported
                var student3 = context.From<Student>()
                    .Where(a => a.CreateTime == DateTime.Now.Date)
                    .Select();

                //lock
                var students4 = context.From<Student>()
                   .With(LockType.FOR_UPADTE)
                   .Select();

                //exists1
                var flag1 = context.From<Student>()
                    .Where(a => a.Id > 50)
                    .Exists();

                //exists2
                var subquery = new SubQuery<School>()
                    .Where(a => a.Id >= 2)
                    .Select(a => a.Id);
                var flag2 = context.From<Student>()
                    .Where(a => Operator.Exists(subquery))
                    .Count();

                //count
                var count = context.From<Student>()
                   .Where(a => a.Id > 50)
                   .Count();

                //sum
                var sum = context.From<Student>()
                 .Where(a => a.Id > 50)
                 .Sum(s => s.Id * s.Age);

                //distinct
                var disinct = context.From<Student>()
                    .Distinct()
                    .Select(s => s.Name);
            }
        }
        #endregion

        #region demo1：测试Case When[Test Case When]
        [Test]
        public void Demo1_Case_When()
        {
            using (var context = DbContextFactory.GetDbContext())
            {

                //case
                var caseWhen = new Case<Student>()
                    .When(a => a.Age <= 18)
                    .Then("children")
                    .When(a => a.Age <= 40)
                    .Then("Youth")
                    .Else("Old");

                //The "caseWhen" object is still an ISqlBuild instance at run time, not a string
                //The engine passes in parameters and calls the "caseWhen.Build" method of the instance
                var students1 = context.From<Student>()
                    .Where(a => caseWhen == "Old" || caseWhen == "Youth")
                    .Select(s => new
                    {
                        s.Id,
                        GroupAge = (string)caseWhen
                    });
            }
        }
        #endregion

        #region demo2：测试复杂函数[Test Complex Function]
        [Test]
        public void Demo2_Complex_Function()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                var adddayfun = new DateAdd<Student>(a => a.CreateTime, 1, "day");

                //in columus
                var student1 = context.From<Student>()
                    .Select(s => new
                    {
                        s.Id,
                        DateTime = (DateTime)adddayfun //just for type inference
                    });

                //in expression
                var student2 = context.From<Student>()
                    .Where(a => adddayfun > DateTime.Now)
                    .Select();
            }
        }
        #endregion

        #region demo3：测试窗口函数[Test Window Function]
        [Test]
        public void Demo3_Window_Function()
        {

            using (var context = DbContextFactory.GetDbContext())
            {
                try
                {
                    var winFun = new WinFun<Student>()
                        .ORDERBY(a => a.Age)
                        .ROW_NUMBER();

                    var student1 = context.From<Student>()
                       .Select(s => new
                       {
                           s.Id,
                           s.Name,
                           s.Age,
                           RowNum = (ulong)winFun
                       });

                }
                catch (Exception e)
                {

                    throw;
                }
            }


        }
        #endregion

        #region demo4：测试子查询[Test subquery]
        [Test]
        public void Demo4_Subquery()
        {
            using (var context = DbContextFactory.GetDbContext())
            {
                //in where
                var subquery1 = new SubQuery<Student>()
                    .Where(a => a.Id <= 15)
                    .Select(s => s.Age);

                var student1 = context.From<Student>()
                    .Where(a=>a.Age>=Operator.Any(subquery1))
                    .Select();

                //in columns
                var subquery2 = new SubQuery<School>()
                   .Where<Student,School>((a,b) => a.SchoolId==b.Id)
                   .Select(s => s.Name);


                var student2 = context.From<Student>()
                    .Select(s=>new
                    {
                        s.Id,
                        StudentName = s.Name,
                        SchoolName = (string)subquery2//just for build
                    });
            }
        }
        #endregion
    }

}