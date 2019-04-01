using System;
using Dapper.Common;
using Dapper.Common.Example;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        ISession Session { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            ServiceFactory.ConfigDataSource("server=127.0.0.1;user id=root;password=1024;database=test;");
            Session = SessionFactory.GetSession();
        }
        [TestMethod]
        public void TestMethod1()
        {
            Session.Close();
            var exis = Session.From<Member>().Where(a => a.Balance > 50).Exists();

            //模拟控制器：发起请求
            var service = ServiceFactory.GetService<MemberService>();

            var result = service.MemberRegister(new MemberModel()
            {
                Account = "admin",
                Password = "1024",
                NickName = "Dapper"
            }).ToJson();
        }
      
    }
}
