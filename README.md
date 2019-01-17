# Dapper.Common


    QQ群：642555086
     public class UnitTest1
     {
       


        [TestMethod]
        public void TestMethod1()
        {
            //SessionFactory 只需要配置一次，应写在程序启动，或者静态代码块中
             //配置会话工厂
            var connectionString = "server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=1;maxpoolsize=100;connectiontimeout=180;";
            //配置数据源
            SessionFactory.DataSource = () => new MySqlConnection(connectionString);
            //开启代理
            SessionFactory.SessionProxy = true;
            //下划线不敏感
            SessionFactory.MatchNamesWithUnderscores = true;         
            //获取一个代理会话
            var session = SessionFactory.GetSession();
            //开启事物/关闭自动提交
            
            
            session.Open(true);
            var row = 0;
            //Inset
            row = session.From<T_SYSTEM_MENUS>().Insert(new T_SYSTEM_MENUS()
            {
                IsChild = 1,
                MuName = "Main",
            });

            //Update:根据主键字段更新,及更新Id1==3的
            session.From<T_SYSTEM_MENUS>().Update(new T_SYSTEM_MENUS()
            {
                Id = 3,
                MuName = "Root"
            });
            //更新所有字节点的MuName和MuDesc为Child
            session.From<T_SYSTEM_MENUS>()
                .Set(s => s.MuName, "Child")
                .Set(s => s.MuDesc, "Child")
                .Where(s => s.IsChild == 1)
                .Update();
             //乐观锁
             var row = session.From<T_USER>()
                .Set(s => s.Balance, 100)
                .Set(s => s.Version, Guid.NewGuid().ToString())//设置新的版本号
                .Where(s => s.Id == 1&&a.Version==user.Version)//老的版本号
                .Update();
             if(row==0)
             {
                throw new Exception("乐观锁");
             }
            //动态条件更新
            session.From<T_SYSTEM_MENUS>()
                .Set(1>2,s => s.MuName, "Child")//当条件成立时
                .Set(s => s.MuDesc, "Child")//必须更新
                .Where(s => s.IsChild == 1)
                .Update();
            //引用更新
            session.From<T_SYSTEM_MENUS>()
                .Set(1>2,s => s.Age.Eq(s.Age+10))/Age=Age+10              
                .Where(s => s.IsChild == 1)
                .Update();
            //Delete:根据Id删除，及删除Id==2的
            row = session.From<T_SYSTEM_MENUS>().Delete(new T_SYSTEM_MENUS()
            {
                Id = 2
            });
            //Delete:删除所有子节点
            row = session.From<T_SYSTEM_MENUS>()
                .Where(s => s.IsChild == 1)
                .Delete();

            //Select ALL:查询菜单子节点并且id在1~23之间的,按Id升序,Sort降序查第一页所有数据
            var list = session.From<T_SYSTEM_MENUS>().Lock().Select();//悲观锁 for update
            var list = session.From<T_SYSTEM_MENUS>()
                .Where(m => m.IsChild == 1 && m.Id.Between(1, 23) && m.MuType.In(new int[] { 1, 2, 3 }))
                .Asc(s => s.Id)
                .Desc(s => s.Sort)
                .Limit(0, 10)
                .Select();

            //Select Single
            var entity = session.From<T_SYSTEM_MENUS>()
               .Where(m => m.IsChild == 1)
               .Asc(s => s.Id)
               .Desc(s => s.Sort)
               .Single();
            
            //dynamic Select 动态查询
            //前台请求参数:分页
            var total = 0;
            var req = new SystemMenusModel()
            {
                PageIndex = 1,
                PageSize = 10,
                MuName = "cc"
            };
            //分页查询:必须条件IsChild==1，动态条件MuName不为空则MuName必须包含cc
            var query = new SqlQuery()
                .And(s => s.IsChild == 1)
                .And(req.MuName != null, s => s.MuName.Like(req.MuName));
                
            list = session.From<T_SYSTEM_MENUS>()
                .Where(query)
                .Asc(s => s.MuType)
                .Skip(1,10,out total)
                .Select();
           //Sql查询
           var list1 = session.From("student")
                .Where("Age>@Age1")
                .GroupBy("GANDER")
                .Having("SUM(AGE)>@Age2")
                .Param(new {Age1=10,Age2=100})
                .Top(10)
                .Select("GANDER,SUM(AGE) AS AGE");
                
            //会话结束
            session.ToString();//打印日志，前提：SessionFactory.SessionProxy = true;
            session.Commit();
            session.Rollback();//应该写在catch里
            session.Close();//应该写在finally里
            //或者使用dapper.enhance.transaction 来代理事务的开启，提交回滚，关闭
        }       
    }

    //Session代理，自动完成创建、提交、回滚
    using Autofac;
    using Autofac.Extras.DynamicProxy;
    using Castle.DynamicProxy;
    using Dapper.Enhance;
    using Mammothcode.Service.Common;
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    namespace Mammothcode.MerchWeb
    {
        /// <summary>
        /// 代理配置
        /// </summary>
        public class ProxyConfig
        {
            /// <summary>
            /// 服务容器
            /// </summary>
            public static IContainer Container { get; set; }
            /// <summary>
            /// 注册服务
            /// </summary>
            public static void RegisterProxy()
            {
                //配置代理
                ContainerBuilder builder = new ContainerBuilder();
                //注册拦截器
                builder.RegisterType<ServiceIntercept>()
                        .SingleInstance();
                //注册组件服务
                builder.RegisterAssemblyTypes(System.Reflection.Assembly.Load("Mammothcode.Service"))
                        .Where(t => t.GetInterfaces().ToList().Exists(e => e == typeof(IService)))
                        .InstancePerLifetimeScope()
                        .InterceptedBy(typeof(ServiceIntercept))
                        .EnableClassInterceptors();
                //构建容器
                Container = builder.Build();

                //配置会话工厂
                var connectionString = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
                //配置数据源
                SessionFactory.DataSource = () => new MySqlConnection(connectionString);
                //开启代理
                SessionFactory.SessionProxy = true;
                //下划线不敏感
                SessionFactory.MatchNamesWithUnderscores = true;

            }
        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ResolveService<T>()
        {
            T instance = default(T);
            using (var scope = Container.BeginLifetimeScope())
            {
                instance = scope.Resolve<T>();
            }
            return instance;
        }
    }

    public class ServiceIntercept : IInterceptor
    {
        /// <summary>
        /// 事务拦截器
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            ISession session = null;
            try
            {
                //注入会话
                var target = invocation.InvocationTarget;
                if (target is IService)
                {
                    var sevice = target as IService;
                    if (sevice.Session == null)
                    {
                        //注入事务
                        session = SessionFactory.GetSession();
                        sevice.Session = session;
                        session.Open(false);
                    }
                }
                //执行目标方法
                invocation.Proceed();

                //提交会话
                if (session != null)
                {
                    session.Commit();
                }
            }
            catch (Exception e)
            {
                if (session != null)
                {
                    session.Rollback();
                }
                if (invocation.Method.ReturnType == typeof(ActionModel))
                {
                    invocation.ReturnValue = new ActionModel()
                    {
                        description="服务器错误",
                        err_msg=e.Message,
                        issuccess=false,
                    };
                }
            }
            finally
            {
                //关闭会话
                if (session != null)
                {
                    session.Close();
                }
            }
        }
    }


