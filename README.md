# Dapper.Common

## 联系作者
  1. Email:1448376744@qq.com
  2. QQ:1448376744
  3. QQGroup:642555086

## CONFIG


## INSERET
```
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
  
