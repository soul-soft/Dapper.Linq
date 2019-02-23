using Dapper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public static class DbFun
    {
        /// <summary>
        /// 通过经纬度计算距离
        /// </summary>
        /// <param name="lng1"></param>
        /// <param name="lat1"></param>
        /// <param name="lng2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        [Function]
        public static double Dist_Len(double? lng1, double? lat1, double? lng2, double? lat2)
        {
            return 0;
        }
        /// <summary>
        /// 格式化日期去掉时分秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [Function]
        public static DateTime? Date(DateTime? dateTime)
        {
            return dateTime;
        }
        [Function]
        public static DateTime? Date_Add(DateTime? dateTime,[KeyParameter] string format)
        {
            return dateTime;
        }
        /// <summary>
        /// 条件函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        [Function]
        public static double If<T>(Expression<Func<T, bool>> expression, object val1, object val2)
        {
            return 0;
        }
        /// <summary>
        /// 聚合函数：计数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static int? Count(object val)
        {
            return 0;
        }
        /// <summary>
        /// 聚合函数：汇总
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static decimal? Sum(object val)
        {
            return 0;
        }
        /// <summary>
        /// 聚合函数：汇总
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static int? Sum(int? val)
        {
            return 0;
        }
        /// <summary>
        /// 聚合函数：最大值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static decimal? Max(object val)
        {
            return 0;
        }
        /// <summary>
        /// 聚合函数：最大值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static DateTime? Max(DateTime? val)
        {
            return val;
        }
        /// <summary>
        /// 去重
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Function]
        public static decimal? Distinct(object val)
        {
            return 0;
        }
        [Function]
        public static string Replace(object val,string old,string tar)
        {
            return null;
        }
    }
}
