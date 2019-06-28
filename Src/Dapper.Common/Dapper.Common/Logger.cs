using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common
{
    public class Logger
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public long ExecuteTime { get; set; }
        public int? Timeout { get; set; }
        public bool? Buffered { get; set; }
    }
}
