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
* set
``` C#
 //param
 var age = 20;
 DateTime? time = null;
 var sid = 1;

 //subquery
 var subquery = new SubQuery<School>()
     .Where(a => a.Id == sid)
     .Select(s => s.Name);

 var row = context.From<Student>()
     .Set(a => a.Age, a => a.Age + age)
     .Set(a => a.Name, subquery)
     .Set(a => a.CreateTime, time, time != null)
     .Where(a => a.Id == 16)
     .Update();

```
  
