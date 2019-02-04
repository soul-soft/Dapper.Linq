using Dapper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public static  class DbFun
    {
        [Function]
        public static double Min(ValueType min)
        {
            return 0;
        }
        [Function]
        public static DateTime Max(ValueType max)
        {
            return DateTime.Now;
        }
        [Function]
        public static int Left(string str,int len)
        {
            return 0;
        }
        [Function]
        public static DateTime? Date_Add(DateTime? date,[KeyParameter] string foramt)
        {
            return date;
        }
    }
}
