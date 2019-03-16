using System;

namespace Dapper.Common
{
    /// <summary>
    /// 属性映射到字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否移除字段,如果表中不存在该列则设置为true
        /// </summary>
        public bool Remove { get; set; }
        /// <summary>
        /// 字段映射
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="isPrimarykey">是否为主键</param>
        public ColumnAttribute(string name,bool isPrimarykey=false)
        {
            Name = name;
            IsPrimaryKey = isPrimarykey;
            Remove = false;
        }
        /// <summary>
        /// 移除字段
        /// </summary>
        /// <param name="remove">是否移除</param>
        public ColumnAttribute(bool remove)
        {
            Remove = true;
        }
    }
}
