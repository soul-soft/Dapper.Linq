// See https://aka.ms/new-console-template for more information
using Dapper.Linq;
using Dapper.Linq.Test;

Console.WriteLine("Hello, World!");
var context = new DbContext(null);
SqlBuilder sql = context.From<Student>()
    .Where(a => a.Id > 10)
    .Build();
var ff = sql.Build("/**where**/");
Console.WriteLine(ff);