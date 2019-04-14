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
        public int? Change { get; set; }
      
    }

}
