# Dapper.Common


    QQ群：642555086
    一、基本结构，此处可用委托，或动态代理完成
    class Program
    {
        //类加载时配置一次
        static Program()
        {
            //配置数据源为mysql
            SessionFactory.AddDataSource = ("mysql",()=>new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;"));
            //下划线不敏感,默认不区分大小写
           
            //Session使用静态代理,记录会话日志,生产模式设置false
            SessionFactory.StaticProxy = true;

        }
        static void Main(string[] args)
        {
            //变量声明
            ISession session = null;
            try
            {
                //开启一次数据会话
                session = SessionFactory.GetSession();
                //开启事物,取消自动提交
                session.Open(false);
                //事物操作1
                //事物操作2
                //事物操作3
                //.......
                //提交事物
                session.Commit();
            }
            catch (Exception e)
            {
                //异常回滚
                if (session!=null)
                {
                    session.Rollback();
                }
                throw e;
            }
            finally
            {
                //资源释放
                if (session!=null)
                {
                    session.Close();
                }
            }
        }
    }
    
    二、映射，此处可用自动模板完成
     /// <summary>
    /// 对应表student,同名可以省略，字段名不分大小写，不分下划线
    /// </summary>
    [Table("student")]
    public class Student
    {
        /// <summary>
        /// 对应字段ID,主键标识列identity=true
        /// </summary>
        [Column("ID", true)]
        public int? Id { get; set; }
        /// <summary>
        /// 对应字段ME_NAME,通过Column校正为Name,
        /// </summary>
        [Column("ME_NAME", false)]
        public string MeName { get; set; }
        /// <summary>
        /// 对应数据字段AGE无需校正
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// 对应字段CREATE_TIME,不分大小写,下划线不敏感
        /// </summary>
        [Column("CREATE_TIME", false)]
        public DateTime? CreateTime { get; set; }
    }
    三、常用API
         var sesion = SessionFactory.GetSession();
        /*****************INSERT*******************/
        //Dapper
        var row1 = sesion.Execute("insert into student(Age,ME_NAME)values(@Age,@MeName)", new { Age = 20, MeName = "Dapper" });
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
            .Set(!string.IsNullOrEmpty(entity.Name), a => a.Name, entity.Name)
            //Age在原来的基础上加20
            .Set(a => a.Age.Eq(a.Age + entity.Age))
            //条件ID=30
            .Where(a => a.Id == 30)
            //要执行的操作
            .Update();
        /*****************DELETE*******************/
        //更据实体ID删除
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
            .And(a => a.Name.Like("%aa%"))
            .Or(a => a.Id > 0)
            .Or(a => a.Id < 10)
            .Or(a => a.Id.In(new[] { 1, 2, 3 }))
            .And(a => 1 > 2 ? a.Name.Like("cc%") : a.Id > 100);
        var res = sesion.From<Student>().Where(query).Exists();
        /*****************会话日志*******************/
        var aa = sesion.Logger();
    四、自定义函数
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
        public static  class DbFun
        {
       
            [Function]
            public static DateTime Max(ValueType max)
            {
                return DateTime.Now;
            }
            [Function]//指定为数据库函数[KeyParameter]表示关键字
            public static DateTime? Date_Add(DateTime? date,[KeyParameter] string foramt)
            {
                return date;
            }
        }
