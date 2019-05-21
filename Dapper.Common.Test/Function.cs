using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension.Test
{
    public static class DbFun
    {
        [Function]
        public static long? Count([Parameter] string expr, object column)
        {
            return 0;
        }
        [Function]
        public static decimal? Sum(object column)
        {
            return 0;
        }
        [Function]
        public static T Count<T>(long column)
        {
            return default(T);
        }
        [Function]
        public static string Replace(object column,string oldstr,string newstr)
        {
            return "";
        }
        [Function]
        public static DateTime DATE_ADD(object column,[Parameter] string expr)
        {
            return DateTime.Now;
        }
    }
}
