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
## Select 
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
