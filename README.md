# Dapper.Common QQ群：642555086

        
        
一、Config
    //开启静态代理
    SessionFactory.StaticProxy = true;
    //配置数据源
    SessionFactory.DataSources.Add(new DataSource()
    {
    Source = () => new MySql.Data.MySqlClient.MySqlConnection(connectionString),
    Name="mysql",
    Type=DataSourceType.MYSQL,
    });
    //获取会话
    var session = SessionFactory.GetSession();

二、Insert
    //插入单个
    var row = Session.From<Member>().Insert(member);
    //批量插入
    var row = Session.From<Member>().Insert(memberList);
三、Update
    //根据主键更新
    var row = Session.From<Member>().Update(member);
    //根据条件更新部分列
    var row = Session.From<Member>()
        .Set(a => a.Password, 123)
        .Set(a => a.NickName, "aa")
        .Where(a => a.Id > 50)
        .Update();
    //在原来的基础上+100
    var row = Session.From<Member>()
        .Set(a => a.Balance, a => a.Balnace + 100)
        .Where(a=>a.Id=20)
        .Update();
四、Delete
    var row = Session.From<Member>()
        .Where(a=>a.Id>50)
        .Delete();
五、查询
     //查询单个
    var entity = Session.From<Member>().Where(a => a.Id == 10).Single();
    //查询全部
    var list = Session.From<Member>().Where(a => a.Id.In(new int[] { 1, 2, 3 })).Select();
    //查询单个列
    var accounts = Session.From<Member>().Select(s => s.Account);
    //查询部分列，不支持匿名类型
    var account = Session.From<Member>().Select(s => new MemberInfo()
    {
        Account = s.Account,
        NickName = s.NickName
    });
    //分页查询，返回总数
    var list = Session.From<Member>().Skip(1, 20, out int total).Select();

    //支持函数，完全自定义函数
    var list = Session.From<Member>()
        .GroupBy(a => a.Gander)
        .Having(a => DbFun.Count(1) > 10)
        .Select(s => new MemberGroupByGander
        {
            Gander = s.Gander,
            Count = DbFun.Count(1)
        });
六、自定义函数
    [Function]//需要用该特性标注，并写在静态类中
    public static int? Count(object val)
    {
         return 0;
    }

