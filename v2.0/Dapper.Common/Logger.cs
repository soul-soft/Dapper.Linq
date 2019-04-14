using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Extension
{
    public class Logger
    {
        public string Text { get; set; }
        public object Param { get; set; }
        public long ElapsedMilliseconds  { get; set; }
    }
}
