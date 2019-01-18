# Dapper.Common


    QQ群：642555086
    一、基本结构
    class Program
    {
        //类加载时配置一次
        static Program()
        {
            //配置数据源为mysql
            SessionFactory.DataSource = ()=>new MySqlConnection("server=47.94.59.37;user id=mammothcode;password=mammothcode;database=test;");
            //下划线不铭感
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
