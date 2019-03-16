using System;

namespace Dapper.Common
{
    /// <summary>
    /// 类映射表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]

    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 映射表名
        /// </summary>
        /// <param name="name">表名</param>
        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
}
