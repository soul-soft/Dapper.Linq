// See https://aka.ms/new-console-template for more information
using Dapper.Linq;
using Dapper.Linq.Test;

Console.WriteLine("Hello, World!");
int s = 20;
var context = new DbContext(null);
var sql = context.From<Student>("pro")
    .Where(a=>a.Id == s)
  .Sum(a => a.Id);
