using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper
{
    public enum DbContextState
    {
        Closed = 0,
        Open = 1,
        Commit = 2,
        Rollback = 3,
    }
}
