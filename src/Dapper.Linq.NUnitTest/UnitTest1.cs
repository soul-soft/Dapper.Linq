using Daper.Entitys;
using NUnit.Framework;

namespace Dapper.Linq.NUnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var db = new DbContext(new DbContextBuilder
            {
                Connection = new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;")
            });
            db.Open();
            var student = db.From<Student>().Get(1);
            var list = db.From<Student>()
                .Where(a => a.Id > 1)
                .Select();
            Assert.Pass();
        }
        [Test]
        public void Test2()
        {
            //一个应用只需要创建一个实例
            //可以指定xml路径批量加载
            GlobalSettings.XmlCommandsProvider.Load(@"D:\Dapper.Linq\src\Dapper.Linq.NUnitTest\student.xml");
            var db = new DbContext(new DbContextBuilder
            {
                Connection = new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"),
            });
            db.Open();
            var student = db.From<Student>().Get(1);
            var list = db.From<Student>()
                .Where(a => a.Id > 1)
                .Select();
            //由于底层解析表达式是基于表达式树实现的，因此Id和Age必须满足一下条件
            //1.必须可以为null（因为表达式是Id!=null,如果是Id>0，则Id可以是int类型）
            //2.必须是public的
            try
            {
                using (var multi = db.From("student.list", new { Id = (int?)null, Name = (string)null }).ExecuteMultiQuery())
                {
                    //执行第一个sql
                    var list2 = multi.GetList<Student>();
                    //执行第二个sql
                    var count = multi.Get<int>();
                    var mylist = db.ExecuteQuery("select * from student");
                }
            }
            catch (System.Exception E)
            {

                throw;
            }
            
        }
    }
}