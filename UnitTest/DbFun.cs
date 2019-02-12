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
        public static DateTime Max(ValueType max)
        {
            return DateTime.Now;
        }
        [Function]
        public static DateTime? Date_Add(DateTime? date,[KeyParameter] string foramt)
        {
            return date;
        }
        [Function]
        public static double Dist_len(double? lng1,double? lat1 , double? lng2, double? lat2)
        {
            return 0;
        }
    }
}
