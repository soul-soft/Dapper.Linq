using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    /// <summary>
    /// 数据库函数:标识函数为数据库函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute:Attribute
    {

    }
    /// <summary>
    /// 关键字参数：标识该参数为关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class KeyParameterAttribute:Attribute
    {

    }
}
