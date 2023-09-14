using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    public interface ISqlBuilder
    {
        string Build(DynamicParameters values, string prefix);
    }
    public interface ISubQuery : ISqlBuilder
    {
    }
}
