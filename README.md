# Dapper.Common

## 联系作者
  1. Email:1448376744@qq.com
  2. QQ:1448376744
  3. QQGroup:642555086

## CONFIG


## INSERET
``` C#
//由于id[isIdentity=true]，因此sql不会显示设置id值   
 var row1 = context.From<Student>().Insert(new Student()
{
    Status = Status.None,
    CreateTime = DateTime.Now,
    Name = "jack",
});
//批量新增
var row2 = context.From<Student>().Insert(new List<Student>()
{
    new Student()
    {
        Status = Status.None,
        CreateTime = DateTime.Now,
        Name = "tom",
    },
     new Student()
    {
        Status = Status.None,
        CreateTime = DateTime.Now,
        Name = "jar",
    },
});

```

## UPDATE
``` C#
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
    .Where(a => a.Id == 16 && a.Version = student.Version)
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
  
