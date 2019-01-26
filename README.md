# Dapper.Common


    QQ群：642555086
    一、基本结构，此处可用委托，或动态代理完成
    class Program
    {
        //类加载时配置一次
        static Program()
        {
            //配置数据源为mysql
            SessionFactory.DataSource = ()=>new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;");
            //下划线不敏感,默认不区分大小写
            SessionFactory.MatchNamesWithUnderscores = true;
            //Session使用静态代理,记录会话日志,生产模式设置false
            SessionFactory.SessionProxy = true;

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
    /// 对应表student,同名可以省略，字段名不分大小写，不分下划线，但除此之外都要匹配
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
