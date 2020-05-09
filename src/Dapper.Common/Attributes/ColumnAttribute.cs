using System;

namespace Dapper.Attributes
{
    /// <summary>
    /// 字段映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        internal string Name { get; set; }
        /// <summary>
        /// 属性字段映射
        /// </summary>
        /// <param name="name">数据库字段名</param>
        public ColumnAttribute(string name = null)
        {
            Name = name;
        }
    }
}
