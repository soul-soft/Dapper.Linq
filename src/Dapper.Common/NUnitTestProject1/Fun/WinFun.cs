using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Common;
using Dapper.Common.Util;

namespace NUnitTestProject1
{
    /*
     * Dapper.common doesn't care how you implement it, it only concerns the result of build.
     */
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
}
