using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Common
{
    internal class SessionLogger
    {
        public string Command { get; set; }
        public object Param { get; set; }
        public DateTime Time { get; set; }
        public long Sec { get; set; }
        public string Method { get; set; }
        public int? ChangeRow { get; set; }
        /// <summary>
        /// 参数格式化
        /// </summary>
        /// <returns></returns>
        public string ParamToFormat()
        {
            var loggerbuffer = new StringBuilder();
            if (Param is DynamicParameters)
            {
                var parameters = Param as DynamicParameters;
                foreach (var name in parameters.ParameterNames)
                {
                    dynamic param = ((SqlMapper.IParameterLookup)parameters)[name];
                    string value = "";
                    if (param is DBNull)
                    {
                        value = "NULL";
                    }
                    else if (param is string)
                    {
                        value = string.Format("'{0}'", param);
                    }
                    else if (param is DateTime)
                    {
                        value = string.Format("'{0}'", param);
                    }
                    else if (param.GetType().IsArray)
                    {
                        var array = param as Array;
                        var objs = array.Cast<dynamic>()
                            .Select(s => IsStringType(s.GetType()) ? string.Format("'{0}'", s) : s);
                        value = string.Format("{0}", string.Join(",", objs));
                    }
                    else if (param is IEnumerable)
                    {
                        var enumerable = param as IEnumerable;
                        var objs = enumerable
                            .Cast<dynamic>()
                            .Select(s => IsStringType(s.GetType()) ? string.Format("'{0}'", s) : s);
                        value = string.Format("({0})", string.Join(",", objs));
                    }
                    else
                    {
                        value = param.ToString();
                    }
                    loggerbuffer.AppendFormat("SET @{0} = {1};\n", name, value);
                }
            }
            return loggerbuffer.ToString();
        }
        /// <summary>
        /// 检查是否为字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsStringType(Type type)
        {
            if (type != typeof(DateTime) && type.IsValueType)
            {
                return false;
            }
            if (type == typeof(DBNull))
            {
                return false;
            }
            return true;

        }
    }

}
