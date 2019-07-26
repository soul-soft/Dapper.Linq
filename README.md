# Dapper.Common

## 联系作者
  1. Email:1448376744@qq.com
  2. QQ:1448376744
  3. QQGroup:642555086

## CONFIG
``` C#
 DbContextFactory.AddDataSource(new DataSource()
 {
     Default = true,
     Name = "mysql",
     ConnectionFacotry = () => new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;password=1024;database=test;"),
     DatasourceType = DatasourceType.MYSQL,
     UseProxy = true//use static proxy,for logger
 });

```

## INSERET
``` C#
IDbContext context = null;
try
{
    context = DbContextFactory.GetDbContext();
    //because set "id[isIdentity=true]"，so not set "id" value
    var row1 = context.From<Student>().Insert(new Student()
    {
        Grade = Grade.A,
        CreateTime = DateTime.Now,
        Name = "jack",
    });
    //batch added
    var row2 = context.From<Student>().Insert(new List<Student>()
    {
        new Student()
        {
            Grade = Grade.C,
            CreateTime = DateTime.Now,
            Name = "tom",
        },
         new Student()
        {
            Grade = Grade.F,
            CreateTime = DateTime.Now,
            Name = "jar",
        },
    });
}
catch (Exception e)
{
    //debug sql logger
    Console.WriteLine(context.Loggers);
}
finally
{
    context.Close();
}

```

## UPDATE
``` C#
using (var context = DbContextFactory.GetDbContext())
{
    //param
    var age = 20;
    DateTime? time = null;
    var sid = 1;

    //subquery
    var subquery = new SubQuery<School>()
        .Where(a => a.Id == sid)
        .Select(s => s.Name);

    var row1 = context.From<Student>()
        .Set(a => a.Age, a => a.Age + age)
        .Set(a => a.Name, subquery)
        .Set(a => a.CreateTime, time, time != null)
        .Where(a => a.Id == 16)
        .Update();

    //function
    context.From<Student>()
        .Set(a => a.Name, a => MysqlFun.REPLACE(a.Name, "a", "b"))
        .Where(a => a.Id == 14)
        .Update();

    //lock
    var student = context.From<Student>()
        .Where(a => a.Id == 16)
        .Single();
    var row2 = context.From<Student>()
        .Set(a => a.Age, 80)
        .Set(a => a.Version, Guid.NewGuid().ToString())
        .Where(a => a.Id == 16 && a.Version == student.Version)
        .Update();

    //entity
    var row3 = context.From<Student>()
        .Filter(a => a.SchoolId)
        .Update(new Student()
        {
            Id = 2,
            CreateTime = DateTime.Now
        });

```
## DELETE
``` C#
using (var context = DbContextFactory.GetDbContext())
{
    var row1 = context.From<Student>()
         .Where(a => a.Id == 16)
         .Delete();

    var subquery = new SubQuery<School>()
        .Where(a => a.Id >= 0)
        .Select(a => a.Id);

    var row2 = context.From<Student>()
         .Where(a => Operator.In(a.Id, subquery))
         .Delete();
}
```
## SELECT 
``` C#
 //single
 var student = context.From<Student>()
     .Where(a => a.Id == 19)
     .Single();

 //subquery
 var id = 0;
 var age = 50;
 var subquery = new SubQuery<School>()
    .Where(a => a.Id >= id)
    .Select(a => a.Id);

 //Verify that subquery parameters are written to the current query
 var students2 = context.From<Student>()
     .OrderBy(a => a.Age)
     .Where(a => a.Id >= Operator.Any(subquery) && a.Age > age)
     .Select();

 //Partial columns
 var students3 = context.From<Student>()
    .Select(s => new
    {
        s.Id,
        s.Age
    });

```
## GROUPBY
```C#
  var students = context.From<Student>()
      .GroupBy(a => a.Age)
      .Having(a => MysqlFun.Count(1L) > 2)
      .Select(s => new
      {
          Count = MysqlFun.Count(1L),
          s.Age,
      });

```
## DYNAMIC QUERY
``` C#
var param = new Student()
{
    Name = "zs",
    Grade = Grade.B,
    SchoolId = null,
    Id = null,
    Type = 5
};

//Multiple Where Connections with AND
var students = context.From<Student>()
    .Where(a => a.Id == param.Id, param.Id != null)
    .Where(a => Operator.Contains(a.Name, param.Name), param.Name != null)
    .Where(a => a.Grade == param.Grade, param.Grade != null)
    .Where(a => a.Id > 2 || a.Age < 80, param.Type == 5)
    .Select();


var students2 = context.From<Student>()
    .Where(a => a.Id == param.Id, param.Id != null)
    .Where(a => a.Grade == param.Grade, param.Grade != null)
    .Where(a => Operator.StartsWith(a.Name, param.Name), param.Name != null)
    .Where(a => a.Id > 2 || a.Age > 20, param.Type == 8)
    .Select();
```
## TAKE PAGE
```
 var students = context.From<Student>()
     .Page(1, 10, out long total)
     .Select();
```
## JOIN
``` C#
 var students = context.From<Student, School>()
     .Join((a, b) => a.SchoolId == b.Id)
     .Select((a, b) => new
     {
         a.Id,
         StuName = a.Name,
         SchName = b.Name
     });

```
## OTHER QUERY
``` C#
//limit 0,10
var students1 = context.From<Student>()
    .Take(10)
    .Select();

//limit 10,20 
var students2 = context.From<Student>()
   .Skip(10, 20)
   .Select();

//Calling functions in expressions is not recommended, but n-tier attribute access is supported
var student3 = context.From<Student>()
    .Where(a => a.CreateTime == DateTime.Now.Date)
    .Select();

//exists1
var flag1 = context.From<Student>()
    .Where(a => a.Id > 50)
    .Exists();

//exists2
var subquery = new SubQuery<School>()
    .Where(a => a.Id >= 2)
    .Select(a => a.Id);
var flag2 = context.From<Student>()
    .Where(a => Operator.Exists(subquery))
    .Count();

//count
var count = context.From<Student>()
   .Where(a => a.Id > 50)
   .Count();

//sum
var sum = context.From<Student>()
 .Where(a => a.Id > 50)
 .Sum(s => s.Id * s.Age);

//distinct
var disinct = context.From<Student>()
    .Distinct()
    .Select(s => s.Name);

```
