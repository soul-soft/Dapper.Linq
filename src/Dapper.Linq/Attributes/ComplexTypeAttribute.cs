using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Attributes
{
    /// <summary>
    /// 计算列，如果字段一个是计算列则新增和修改的时候不会处理
    /// </summary>
    public class ComplexTypeAttribute : Attribute
    {

    }
}
