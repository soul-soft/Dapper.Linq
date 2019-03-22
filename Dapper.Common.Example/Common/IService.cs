using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common.Example
{
    public interface IService
    {
        ISession Session { get; set; }
    }
}
