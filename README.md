# Dapper.Common QQ群：642555086

#### 性能:基于Dapper
* Dapper性能：[https://github.com/StackExchange/Dapper/blob/master/Readme.md]
* Dapper.Common SQL构建性能：
* PS：常量直接获取，如果是变量将采用反射，否则将采用动态编译，性能：常量>变量>函数
```

//这段代码将构建一条基础查询语句，2W次不超过400ms！！！
for(var i = 0;i < 20000; i++)
{
  var query = new MysqlQuery<Member>();
  query.Where(a => a.Id == i).Single();
  sql = query.BuildSelect();
}

```

#### 架构:高可维护性，可扩展性
* ExpressionUtil：提供linq to sql 的构建工具类

* ExtensionUtil：扩展in，like，regexp等关键词，ID!=null,ID==null,将解析成为ID is null,ID is not null,切记:列在左，值在右

* IQueryable：构建完整查询语句的抽象接口

* MysqlQuery：构建mysql查询的实例，实现IQueryable接口，直接创建对象，不设置Session可之构建sql

* SqlQuery：构建sqlserver查询的实例，实现IQueryable接口

* ISession：数据库会话，一个Session对应一个Connection对象，代表一次会话，实现IDisposable接口，using只负责关闭连接不处理事务，事务请try...catch提交或回滚

* Session：实现ISession接口，

* SessionProxy:实现ISession对Session进行静态代理，记录日志信息，如果有异常便于调试，由于VS出现异常将退出程序，请在catch分析session实例中的Loggers

* SessionFactor:用于配置数据源，创建会话，

* Attribute:中包含对表，字段，函数的注解，用于解决：字段映射，自定义函数

#### 配置：多数据源
```
//可以配置多个数据源，UseProxy将开启代理，记录sql，查询耗时，name是获取数据源的标识
 SessionFactory.AddDataSource(new DataSource()
    {
        SourceType = DataSourceType.MYSQL,
        Source = () => new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=1;maxpoolsize=10;connectiontimeout=180;"),
        UseProxy = true,
        Name = "mysql",
    });
var session = SessionFactor.GetSession("mysql");
```
#### INSERT
```
var session = SessionFactor.GetSession();
//新增单个
var row = session.From<Member>().Insert(new Member 
{
    Name="Dapper"
});

//新增多个
var row = session.From<Member>().Insert(new List<Member>()
{
    new Member()
    {
      Name="Dapper"
    },
    new Member()
    {
      Name="Common"
    }
});

//返回Identity
var id = session.From<Member>().InsertReturnId(new Member()
{
    Name="Dapper"
});

```
#### UPDATE
* 根据主键，整体更新
```

//这将更新Member类的所有字段，假设还有Balance,此时将更新成NULL，支持更新一个List<Member>
session.From<Member>().Update(new Member{Id=1,Name="Dapper"});

//Filter指定Name,Balance列不更新，或只过滤余额 f=>f.Balance
session.From<Member>().Filter(f=>new {f.Name,f.Balance}).Update(new Member{Id=1,Name="Dapper"});
```
* 更新部分列
```
var member = session.Where(a=>a.id==1).Signle();
//乐观锁
var row =  session.From<Member>()
    //column,value
    .Set(a => a.Balance,100)
    .Set(a => a.Version,Datetime.Now)
    .Where(a => a.Id == 1 && a.Version == member.Version)
    .Update();
if(row==0) 
  session.Rollbakc();

//value为一个表达式
var row = session.From<Member>()
     //column,expr
    .Set(a => a.Name,a => DbFun.Replace(a.Name, "ff", "cc"))
     //Balance在原来基础上加100,这种方法可以防止丢失更新
    .Set(a => a.Balance,a => a.Balance + 100)
    .Update();
    
```
#### DELETE
