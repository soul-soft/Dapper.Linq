using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public class ServiceFactory
    {
        private static ProxyGenerator ProxyGenerator = new ProxyGenerator();
        private static IInterceptor TransactionIntercept = new TransactionIntercept();
        public static void ConfigDataSource(string connectionString)
        {
            //开启静态代理
            SessionFactory.StaticProxy = true;
            SessionFactory.DataSources.Add(new DataSource()
            {
                Source = () => new MySql.Data.MySqlClient.MySqlConnection(connectionString),
                Name="mysql",
                Type=DataSourceType.MYSQL,
            });
        }

        public static T GetService<T>()where T : class
        {
            return ProxyGenerator.CreateClassProxy<T>(TransactionIntercept);
        }
    }

    public class TransactionIntercept : IInterceptor
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
                #region 注入会话\日志
                var target = invocation.InvocationTarget;
                if (target is IService)
                {
                    var service = target as IService;
                    //注入事务
                    if (service.Session == null)
                    {
                        session = SessionFactory.GetSession();
                        service.Session = session;
                        session.Open(false);
                    }
                }
                #endregion

                #region 执行目标方法
                //执行目标方法
                invocation.Proceed();
                #endregion

                #region 提交会话
                //提交会话
                if (session != null)
                {
                    session.Commit();
                }
                #endregion

            }
            catch (Exception e)
            {
                #region 关闭事物
                if (session != null)
                {
                    session.Rollback();
                }
                #endregion

                #region 响应信息
                if (invocation.Method.ReturnType is IActionResult)
                {
                    invocation.ReturnValue = new ActionResult()
                    {
                        Message = e is ServiceException ? e.Message : "请稍后再试",
                    };
                }
                #endregion
            }
            finally
            {
                #region 关闭会话
                //关闭会话
                if (session != null)
                {
                    session.Close();
                    session = null;
                }
                #endregion
            }
        }
    }
}
