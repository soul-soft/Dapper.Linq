using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Common
{
    public class SessionLogger
    {
        public string Sql { get; set; }
        public object Value { get; set; }
        public DateTime Time { get; set; }
        public long Watch { get; set; }
        public string Text { get; set; }
        public string LoggerFormat()
        {
            var buffer = new StringBuilder();
            if (Value is DynamicParameters)
            {
                var args = Value as DynamicParameters;
                foreach (var name in args.ParameterNames)
                {
                    var arg = args.Get<object>(name);
                    var type = arg.GetType();
                    buffer.AppendFormat("SET @{0} = {1}{2}{1};\n", name, type.IsValueType && type != typeof(DateTime) ? "" : "'", arg);
                }
            }
            return buffer.ToString();
        }
    }

}
