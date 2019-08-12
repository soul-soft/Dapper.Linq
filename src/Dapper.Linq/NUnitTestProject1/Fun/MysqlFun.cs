using Dapper.Linq;
using Dapper.Linq.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NUnitTestProject1
{
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
}
