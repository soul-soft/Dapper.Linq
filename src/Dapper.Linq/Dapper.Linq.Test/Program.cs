// See https://aka.ms/new-console-template for more information
using Dapper.Linq;
using Dapper.Linq.Test;

Console.WriteLine("Hello, World!");
int s = 20;
var context = new DbContext(null);
var sb = new SqlBuilder();
sb.Where("1>1");
sb.Where("2>2");
var where = context.From<Student>().Where("aa>10").Build() ;
Console.WriteLine(  );

