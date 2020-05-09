using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Expressions
{
    /// <summary>
    /// 修改数据时并发冲突
    /// </summary>
    public class DbUpdateConcurrencyException : Exception
    {
        public DbUpdateConcurrencyException(string message)
            : base(message)
        {

        }
    }
}
