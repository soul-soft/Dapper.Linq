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
        public string ColumnName { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public ColumnKey ColumnKey { get; set; }
        /// <summary>
        /// 是否移除字段,如果表中不存在该列则设置为true
        /// </summary>
        public bool IsColumn { get; set; }
        /// <summary>
        /// 字段映射
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="primaryKey">是否为主键</param>
        public ColumnAttribute(string name, ColumnKey columnKey=ColumnKey.None)
        {
            ColumnName = name;
            ColumnKey = columnKey;
            IsColumn = true;
        }
        /// <summary>
        /// 移除字段
        /// </summary>
        /// <param name="isColumn">是否移除</param>
        public ColumnAttribute(bool isColumn)
        {
            IsColumn = isColumn;
        }
    }
    /// <summary>
    /// 列索引
    /// </summary>
    public enum ColumnKey
    {
        None,
        Primary,
        UniQue,
    }
}
