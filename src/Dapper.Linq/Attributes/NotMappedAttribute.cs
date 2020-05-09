using System;

namespace Dapper.Attributes
{
    /// <summary>
    /// 忽略映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotMappedAttribute: Attribute
    {

    }
}
