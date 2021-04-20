# Dapper.Linq

##

### 免责说明：使用前请先测试和阅读源代码并自觉遵守开源协议

## 新版本的linq支持

  1. [SqlBatis.Extensions.Dapper](https://www.nuget.org/packages/SqlBatis.Extensions.Dapper/)

## 3.x版本说明
  1. 移除dapper，无需依赖dapper，内置一个简约的对象映射器，用于替代dapper  
  2. 新增简单的java.ibatis的xml功能
  3. 新版文档参考：[新版参考文档](https://github.com/1448376744/SqlBatis)，建议通过单测和源码学习
  4. 建议尽快从Dapper.Linq和Dapper.Common移植到[SqlBatis](https://github.com/1448376744/SqlBatis)
  sqlBatis代码可维护性极好，实体映射规则可定制化，移植成本非常小。
  5. 免责说明：使用前先测试
## About author
  1. Email:1448376744@qq.com
  2. QQ:1448376744
  3. QQGroup:642555086
  4. [Document](https://github.com/1448376744/Dapper.Linq/wiki)

## Config
``` C#
 //"LinqTypeMap" file in unit test project 
 Dapper.SqlMapper.TypeMapProvider = (type) => new LinqTypeMap(type);
 DbContextFactory.AddDataSource(new DataSource()
 {
     Default = true,
     Name = "mysql",
     ConnectionFacotry = () => new MySql.Data.MySqlClient.MySqlConnection("server=localhost;user id=root;password=1024;database=test;"),
     DatasourceType = DatasourceType.MYSQL,
     UseProxy = true//use static proxy,for logger
 });

```

## Insert
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

## Update
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

    //entity update by primary key
    var row3 = context.From<Student>()
        .Filter(a => a.SchoolId)
        .Update(new Student()
        {
            Id = 2,
            CreateTime = DateTime.Now
        });
     //reset update where
     var row3 = context.From<Student>()
        .Where(a => a.Id = 2 && a.Version=oldVersion)
        .Update(new Student()
        {            
            Id = 2,
            Version=Guid.NewGuid().ToString(),
            CreateTime = DateTime.Now
        });

```
## Delete
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
## Transaction

``` C#
  IDbContext dbContext = null;
  try
  {
      dbContext = DbContextFactory.GetDbContext();
      dbContext.Open(true);
      dbContext.From<Student>().Insert(new Student()
      {
          Name="stduent1"
      });
      //throw new Exception("rollback");
      dbContext.From<School>().Insert(new School()
      {
          Name = "school1"
      });
      dbContext.Commit();
  }
  catch (Exception)
  {
      dbContext?.Rollback();
      throw;
  }
  finally
  {
      dbContext?.Close();
  }

```

## Anonymous

``` C#
// Custom Mapper Handles the Problem that Anonymous Types Can't Match Constructors
//Copy "DefaultTypeMap" from "dapper" and modify this method
  public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            var constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (ConstructorInfo ctor in constructors.OrderBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1)).ThenBy(c => c.GetParameters().Length))
            {
                ParameterInfo[] ctorParameters = ctor.GetParameters();
                if (ctorParameters.Length == 0)
                    return ctor;

                if (ctorParameters.Length != types.Length)
                    continue;

                int i = 0;
                for (; i < ctorParameters.Length; i++)
                {
                    if (!string.Equals(ctorParameters[i].Name, names[i], StringComparison.OrdinalIgnoreCase))
                        break;
                    if (types[i] == typeof(byte[]) && ctorParameters[i].ParameterType.FullName == "System.Data.Linq.Binary")
                        continue;
                    var unboxedType = Nullable.GetUnderlyingType(ctorParameters[i].ParameterType) ?? ctorParameters[i].ParameterType;
                    //if ((unboxedType != types[i] && !SqlMapper.HasTypeHandler(unboxedType))
                    //    && !(unboxedType.IsEnum && Enum.GetUnderlyingType(unboxedType) == types[i])
                    //    && !(unboxedType == typeof(char) && types[i] == typeof(string))
                    //    && !(unboxedType.IsEnum && types[i] == typeof(string)))
                    //{
                    //    break;
                    //}
                }

                if (i == ctorParameters.Length)
                    return ctor;
            }

            return null;
        }
  
 SqlMapper.TypeMapProvider = (type) => new LinqTypeMap();
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
## Group by
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
## Dynamic query
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
## Task page
``` C#
 var students = context.From<Student>()
     .Page(1, 10, out long total)
     .Select();
```
## Join
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
## Other query
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
//lock
var students4 = context.From<Student>()
   .With(LockType.FOR_UPADTE)
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
## Custom Function

* step1
``` C#
 public static class MysqlFun
 {
     [Function]
     public static string REPLACE(string column,string oldstr,string newstr)
     {
         return string.Empty;
     }
     [Function]
     public static T Count<T>(T column)
     {
         return default;
     }
   
 }

```
* step2

``` C#
 var students = context.From<Student>()
     .GroupBy(a => a.Age)
     .Having(a => MysqlFun.Count(1L) > 2)
     .Select(s => new
     {
         Count = MysqlFun.Count(1L),
         s.Age,
     });
```

## Expression To Sql

``` C#
var prefix = "@";
var values = new Dictionary<string, object>();
Expression<Func<Student,bool>> expression = s => s.Age > 40; 
var expression = ExpressionUtil.BuildExpression(expression, values, prefix);

```
## Object to Sql

### DEMO.1 Case When Then Else

#### step1: implement
``` C#
//Dapper.common doesn't care how you implement it, it only concerns the result of build.
public class Case<T> : ISqlBuilder
{
    private List<Expression> _whens = new List<Expression>();
    private List<string> _thens = new List<string>();
    string _else = null;
    public string Build(Dictionary<string, object> values, string prefix)
    {
        var sb = new StringBuilder();
        foreach (var item in _whens)
        {
            var express = ExpressionUtil.BuildExpression(item, values, prefix);
            sb.AppendFormat(" WHEN {0} THEN '{1}'", express, _thens[_whens.IndexOf(item)]);
        }
        if (_else != null)
        {
            sb.AppendFormat(" ELSE '{0}'", _else);
        }
        return string.Format("(CASE {0} END)", sb);
    }
    public static implicit operator string(Case<T> d) => string.Empty;
    public Case<T> When(Expression<Func<T, bool>> expression)
    {
        new Dictionary<string, object>();
        _whens.Add(expression);
        return this;
    }
    public Case<T> Then(string value)
    {
        _thens.Add(value);
        return this;
    }
    public Case<T> Else(string value)
    {
        _else = value;
        return this;
    }
}

```
#### step2: use

``` C#
//case
var caseWhen = new Case<Student>()
    .When(a => a.Age <= 18)
    .Then("children")
    .When(a => a.Age <= 40)
    .Then("Youth")
    .Else("Old");

//The "caseWhen" object is still an ISqlBuild instance at run time, not a string
//The engine passes in parameters and calls the "caseWhen.Build" method of the instance
var students1 = context.From<Student>()
    .Where(a => caseWhen == "Old" || caseWhen == "Youth")
    .Select(s => new
    {
        s.Id,
        GroupAge = (string)caseWhen
    });

```

### DEMO.2 Complex Function

#### step1: implement

``` C#
 public class DateAdd<T> : ISqlBuilder
 {
     public string Column { get; set; }
     public int Expr { get; set; }
     public string Unit { get; set; }
     public Dictionary<string, object> Values { get; set; }

     public string Build(Dictionary<string, object> values, string prefix)
     {
         return "DATE_ADD(" + Column + ",INTERVAL " + Expr + " " + Unit + ")";
     }
     public DateAdd(Expression<Func<T, DateTime?>> column, int expr, string unit)
     {
         this.Column = ExpressionUtil.BuildColumn(column, null, null).FirstOrDefault().Value;
         this.Expr = expr;
         this.Unit = unit;
     }
     public static bool operator <(DateTime? t1, DateAdd<T> t2)
     {
         return false;
     }
     public static bool operator <(DateAdd<T> t1, DateTime? t2)
     {
         return false;
     }
     public static bool operator >(DateTime? t1, DateAdd<T> t2)
     {
         return false;
     }
     public static bool operator >(DateAdd<T> t1, DateTime? t2)
     {
         return false;
     }
     public static explicit operator DateTime(DateAdd<T> d) => DateTime.Now;
 }

```

#### step2: use

``` C#
 var adddayfun = new DateAdd<Student>(a => a.CreateTime, 1, "day");

 //in columus
 var student1 = context.From<Student>()
     .Select(s => new
     {
         s.Id,
         DateTime = (DateTime)adddayfun //just for type inference
     });

 //in expression
 var student2 = context.From<Student>()
     .Where(a => adddayfun > DateTime.Now)
     .Select();

```
### DEMO.3 Window Function

#### step1: implement

``` C#
 public class WinFun<T> : ISqlBuilder
 {
     string _partition { get; set; }
     string _orderby { get; set; }
     private string _methodName { get; set; }
     public WinFun<T> ROW_NUMBER()
     {
         _methodName = nameof(ROW_NUMBER);
         return this;
     }
     public WinFun<T> PARTITION<TResult>(Expression<Func<T, TResult>> columns)
     {
         var cls = ExpressionUtil.BuildColumns(columns, null, null);
         _partition += string.Join(",", cls.Select(s => s.Value));
         return this;
     }
     public WinFun<T> ORDERBY<TResult>(Expression<Func<T, TResult>> columns, bool asc = true)
     {
         var cls = ExpressionUtil.BuildColumns(columns, null, null);
         _orderby += string.Join(",", cls.Select(s => s.Value));
         _orderby += !asc ? "DESC" : "ASC";
         return this;
     }
     /*If there are no parameters in the expression, there is no need to build in build-method*/
     public string Build(Dictionary<string, object> values, string prefix)
     {
         if (_methodName == nameof(ROW_NUMBER))
         {
             return string.Format("ROW_NUMBER()OVER(ORDER BY {0})", _orderby);
         }
         throw new NotImplementedException();
     }

     public static implicit operator ulong(WinFun<T> d) => 0;
 }

```
#### step2: use

``` C#
 var winFun = new WinFun<Student>()
     .ORDERBY(a => a.Age)
     .ROW_NUMBER();

 var student1 = context.From<Student>()
    .Select(s => new
    {
        s.Id,
        s.Name,
        s.Age,
        RowNum = (ulong)winFun
    });
    
```
### DEMO.4 Subquery

#### step1: implement

``` C#
public class SubQuery<T> : ISubQuery where T : class
{
    private Expression _where { get; set; }
    private Expression _column { get; set; }
    private string _method { get; set; }
    private bool _useSignTable = true;
    public string Build(Dictionary<string, object> values, string prefix)
    {
        var table = EntityUtil.GetTable<T>();
        var column = ExpressionUtil.BuildColumn(_column, values, prefix).SingleOrDefault().Value;
        var where = ExpressionUtil.BuildExpression(_where, values, prefix, _useSignTable);
        if (_method == nameof(this.Select))
        {
            return string.Format("(select {0} from {1} where {2})", column, table.TableName, where);
        }
        if (_method == nameof(this.Count))
        {
            return string.Format("(select count({0}) from {1} where {2})", column, table.TableName, where);
        }
        throw new NotImplementedException();
    }
    public SubQuery<T> Where(Expression<Func<T, bool>> expression)
    {
        _where = expression;
        return this;
    }
    public SubQuery<T> Where<T1, T2>(Expression<Func<T1, T2, bool>> expression)
    {
        _useSignTable = false;
        _where = expression;
        return this;
    }
    public SubQuery<T> Select<TResut>(Expression<Func<T, TResut>> expression)
    {
        _method = nameof(this.Select);
        _column = expression;
        return this;
    }
    public SubQuery<T> Count<TResut>(Expression<Func<T, TResut>> expression)
    {
        _method = nameof(this.Count);
        _column = expression;
        return this;
    }

    public override bool Equals(object obj)
    {
        return obj is SubQuery<T> query &&
               EqualityComparer<Expression>.Default.Equals(_where, query._where) &&
               EqualityComparer<Expression>.Default.Equals(_column, query._column) &&
               _method == query._method;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_where, _column, _method);
    }

    public static bool operator <(object t1, SubQuery<T> t2)
    {
        return false;
    }
    public static bool operator ==(object t1, SubQuery<T> t2)
    {
        return false;
    }
    public static bool operator !=(object t1, SubQuery<T> t2)
    {
        return false;
    }
    public static bool operator <=(object t1, SubQuery<T> t2)
    {
        return false;
    }
    public static bool operator >=(object t1, SubQuery<T> t2)
    {
        return false;
    }
    public static bool operator >(object t1, SubQuery<T> t2)
    {
        return false;
    }

    public static explicit operator string(SubQuery<T> v)=> string.Empty;
    
}


```

#### step2: use

``` C#

//in where
var subquery1 = new SubQuery<Student>()
    .Where(a => a.Id <= 15)
    .Select(s => s.Age);

var student1 = context.From<Student>()
    .Where(a=>a.Age>=Operator.Any(subquery1))
    .Select();

//in columns
var subquery2 = new SubQuery<School>()
   .Where<Student,School>((a,b) => a.SchoolId==b.Id)
   .Select(s => s.Name);

var student2 = context.From<Student>()
    .Select(s=>new
    {
        s.Id,
        StudentName = s.Name,
        SchoolName = (string)subquery2//just for build
    });

```

