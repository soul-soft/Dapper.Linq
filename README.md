# Dapper.Common QQ群：642555086

#### 性能:基于Dapper
* Dapper性能：[https://github.com/StackExchange/Dapper/blob/master/Readme.md]
* Dapper.Common SQL构建性能：
* PS：常量直接获取，如果是变量将采用反射，否则将采用动态编译，性能：常量>变量>函数
```

//这段代码将构建一条基础查询语句，2W次不超过400ms！！！基本等价与手写SQL
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

* MysqlQuery：构建mysql查询的实例，实现IQueryable接口，直接创建对象，不设置Session可只构建sql

* SqlQuery：构建sqlserver查询的实例，实现IQueryable接口

* ISession：数据库会话，一个Session对应一个Connection对象，代表一次会话，实现IDisposable接口，using只负责关闭连接不处理事务，事务请try...catch提交或回滚

* Session：实现ISession接口，Timeout和Buffered如果设置则优先使用Session中的配置

* SessionProxy:实现ISession对Session进行静态代理，记录日志信息，如果有异常便于调试，由于VS出现异常将退出程序，请在catch分析session实例中的Loggers

* SessionFactor:用于配置数据源，创建会话，

* Attribute:中包含对表，字段，函数的注解，用于解决：字段映射，自定义函数

* 绝大部分接口都可以传递一个condition以决定是否执行

#### 配置：多数据源
```
//可以配置多个数据源，UseProxy将开启代理，记录sql，查询耗时，name是获取数据源的标识，只需要执行一次，请写在静态代码块或者Asp启动事件中
 SessionFactory.AddDataSource(new DataSource()
    {
        SourceType = DataSourceType.MYSQL,
        Source = () => new MySql.Data.MySqlClient.MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=1;maxpoolsize=10;connectiontimeout=180;"),
        UseProxy = true,
        Name = "mysql",
    });
var session = SessionFactor.GetSession("mysql");
```
#### Insert
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
#### Update
* 根据主键，整体更新
```

//这将更新Member类的所有字段，假设还有Balance,此时将更新成NULL，支持更新一个List<Member>
session.From<Member>().Update(new Member
{
    Id = 1,
    Name = "Dapper"
});

//Filter指定Name,Balance列不更新，或只过滤余额 f=>f.Balance，select的时候可以过滤掉不想查询的列
session.From<Member>()
  .Filter(f=>new 
  {
    f.Name,
    f.Balance
  })
  .Update(new Member
  {
    Id=1,
    Name="Dapper"
  });
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
    .Update(true);//绝大部分接口都可以传递一个condition，已决定是否执行
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
#### Delete
```
//delete all
var row = session.From<Member>().Delete();

var row = session.From<Member>()
    .Where(a=>a.Id ==2)
    .Delete();
    
var row = session.From<Member>()
    .Where(a=>a.Id.In(new int[]{1,2,3}))
    .Delete();
```

#### Transaction
* 如果不使用事务:推荐使用AOP完成，不懂加群
 ```
  using(var session = SessionFactory.GetSession())
  {
    session.From<Member>().Insert(new Member()
    {
        NickName="dapper"
    });
  }
 ```
* 如果使用事务
```
  using(vaar session = SessionFactory.GetSession())
  {
    try
    {
     //打开事物
     session.Open(true);
     session.From<Member>().Insert(new Member()
      {
          NickName="dapper"
      });
      session.From<Member>().Where(a=>a.Id=2).Delete();
      session.Commit();
    }
    catch
    {
      //一定要通过异常机制来处理事物
      session.Rollback();
    }   
  }
```

#### Count
```
var count = session.From<Member>().Where(a=>a.id>2).Count();

var count = session.From<Member>().Count(s=>new 
{
   s.NickName,
   s.Balance
});

var count = session.Frm<Member>()
.Distinct()
.Count(s=>new 
{
   s.NickName,
   s.Balance
});

```

#### Sum
```
var total = session.From<Member>().Where(a=>a.Id>0).Sum(s=>s.Balace);

var total = session.From<Member>().Sum(s=>s.Balace*10*s.Id);

```
#### Single

```
var member = session.From<Member>().Where(a=>a.Id==1).Single();

var balance = session.From<Member>().Where(a=>a.Id==1).Single(s=>s.Balace);

var info = session.From<Member>().Where().Single(s=>new {s.NickName,s.Gander});

```

#### Page
```
var param=
{
 NickName="zs",
 Id = null,
 Index = 1,
 Count = 10
}

var list = session.From<Member>()
  //当param.Id!=null成立时，执行a.Id==param条件
  .Where(a=>a.Id==param.Id,param.Id!=null)
  .Where(a=>a.NickName.Like(param.NickName),!string.IsNull(param.NickName))
  .OrderBy(a=>a.Balance)
  .OrderByDescending(a=>a.Id)
  //分页一定要写在group,where，having 之后
  .Page(param.Index,param.Count,out long total)
  .Select();
```

#### Group By

```
 var list = select.From<Member>()
   .GroupBy(a=>a.NickName)
   .Having(a=>DbFun.Count(NickName)>2)
   //支持匿名类型但不建议使用
   .Select(a=>new 
   {
     a.NickName,
     SUM = DbFun.Sum(a.Balace)，
     Count = DbFun.Count(1L)
   });
 
```

#### Take
 ```
 //获取前8个
 var list = session.From<Member>().Take(8).Select();
 ```
#### Skip
```
 //从下标未5开始获取十个，等价于MYSQL中的LIMIT
 var list = session.From<Member>().Skip(5,10).Select();
```
#### Join

```
var list = session.From<MemberBill, Member, MemberOrder>()
        .Join<Member,MemberBill>((a, b) => a.Id == b.MemberId)
        .Join<Member, MemberOrder>((a, b) => a.Id == b.MemberId,JoinType.Left)
        .Select((a, b, c) => new
        {
            b.Id,
            b.NickName,
            c.OrderName
        });
```

#### Mapper
```
 TableAttribute("t_member")
 public class Member
 {
    //分别标识：字段名、主键，自增列，SQLSERVER，一定要标识自增列，否则INSERT时将向自增赋值，而引发异常
    [ColumnAttribu("ID",ColumnKey.Primary,true)]
    public int? Id { get; set; }
    //通过`COLUMN_NAME`，来映射关键词
    [ColumnAttribu("`GROUP`",ColumnKey.None)]
    public int? Group { get; set;}
 }
```

#### Function

```
//注意虽然可以使用匿名类型，但是dapper对匿名类型映射不友好，
public static DBFUN
{
    [Function]//必须用该特性标识为数据库函数    
    public long? COUNT<T>(T column)
    {
       return default(T);
    }
    [Function]//必须用该特性标识为数据库函数 
    //ParameterAttribute:标记特殊参数：关键字参数
    public T COUNT<T>([Parameter]string distinct,T column)
    {
       return default(T);
    }
    [Function]//必须用该特性标识为数据库函数,尽量返回大的数据类型 
    public decimal Sum(object o)
    {
       return default(T);
    }
}
//DEMO:可以标识该字段为特殊参数：特殊参数之后不加逗号
session.From<Member>().GroupBy(g=>g.NickName).Select(s=>new 
{
   s.NickName,
   Count=DBFUN.COUNT("DISTINCT ,",1)
})

```
#### SqlString
```
   //where,orderby,groupby,having，都可以插入sql片段
   //如果case返回的是0，1，2，3，4，5数字类型
   //则PriceRange也应该是int类型（应该用int64）,此时PriceRange=range，PriceRange类型推断为string类型
   //解决方案：PriceRange = Convert.ToInt64(range),只支持conver来将sql片段进行转换
   var range = @"(CASE WHEN sale_price <= 10 THEN '0' 
                    WHEN sale_price <= 20 THEN '1'
                    WHEN sale_price <= 30 THEN '2'
                    WHEN sale_price <= 50 THEN '3'
                    WHEN sale_price <= 100 THEN '4'
                    ELSE 5 END)";
                       
    var list = Session.From<SaleOrderItem>()
        .Where("id=@id",p=>p.Add("@id",1))//为了防止sql注入应该使用参数化
        .GroupBy(range)
        .Select(s => new
        {
            PriceRange = range,
            GoodsCount = DbFun.Sum(s.GoodsNum)
        });
```
#### Extension

* IQueryable
```
public class MVCBase
{
  public int PageIndex {get;set;}
  public int PageCount {get;set;}
  public int PageTotal {get;set;}
  public int QueryAll {get;set;}
}
public static CustomExtension
{
    IQueryable<T> ToPage(this IQueryable<T> queryable,MVCBase mvc)
    {
        //condition:mvc.QueryAll!=1
        queryable.Page(mvc.PageIndex,mvc.PageCount,out long total,mvc.QueryAll!=1)
        //返回总页数
        mvc.PageTotal = (int)(mvc.PageIndex+mvc.PageCount-1)*mvc.PageCount;
        return this;
    }   
}
```

* Lambda Expression

```
//1.
  Expression<Member,boll> expr = a=>a.NickName=='%132%' || a.Id.In(new []{1,8});
  var param = new Dictionary<string,>
  var exprstr = ExpressionUtil.BuildExpression(expr,param);
  var lsit = session.From<Member>().Where(exprestr).Select();
```

