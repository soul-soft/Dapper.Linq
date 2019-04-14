# Dapper.Common QQ群：642555086

#### 性能       

* Dapper性能：[https://github.com/StackExchange/Dapper/blob/master/Readme.md]
* Dapper.Common SQL构建性能：
* PS：常量直接获取，如果是变量将采用反射，否则将采用动态编译，性能：常量>变量>函数
```
/****
这段代码将一条基础查询语句构建，构建2W次耗时不超过400ms！！！

SELECT 
  ID AS Id,NICK_NAME AS NickName,CREATE_TIME AS CreateTime,BALANCE AS Balance 
FROM member 
  WHERE (ID=@Id0) LIMIT 0,1
 ****/
var query = new MysqlQuery<Member>();
query.Where(a => a.Id == i).Single();
sql = query.BuildSelect();
```
#### 架构
* ExpressionUtil：提供linq to sql 的构建工具类
* ExtensionUtil：扩展in，like，regexp等关键词，ID!=null,ID==null,将解析成为ID is null,ID is not null,切记列在左，值在右
* IQueryable:构建完整查询语句的抽象接口
* MysqlQuery:构建mysql查询的实例，实现IQueryable接口
* SqlQuery:构建sqlserver查询的实例，实现IQueryable接口
* ISession:数据库会话，一个Session对应一个Connection对象，代表一次会话，实现IDisposable接口，using只负责关闭连接不处理事务，事务请try...catch
