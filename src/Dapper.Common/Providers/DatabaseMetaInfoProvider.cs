using Dapper.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.Expressions
{
    /// <summary>
    /// 数据库元信息提供程序
    /// </summary>
    public interface IDatabaseMetaInfoProvider
    {
        /// <summary>
        /// 获取表的元信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TableMetaInfo GetTable(Type type);
        /// <summary>
        /// 获取字段的元信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<ColumnMetaInfo> GetColumns(Type type);
    }
   
    /// <summary>
    /// 数据库元信息
    /// </summary>
    public class DatabaseMetaInfoProvider: IDatabaseMetaInfoProvider
    {
        private static readonly ConcurrentDictionary<Type, TableMetaInfo> _tables
            = new ConcurrentDictionary<Type, TableMetaInfo>();

        private static readonly ConcurrentDictionary<Type, List<ColumnMetaInfo>> _columns
            = new ConcurrentDictionary<Type, List<ColumnMetaInfo>>();

        public TableMetaInfo GetTable(Type type)
        {
            return _tables.GetOrAdd(type, t =>
            {
                var name = t.Name;
                if (t.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() != null)
                {
                    var attribute = t.GetCustomAttributes(typeof(TableAttribute), true)
                        .FirstOrDefault() as TableAttribute;
                    name = attribute.Name;
                }
                var table = new TableMetaInfo()
                {
                    TableName = name,
                    CsharpName = t.Name
                };
                return table;
            });
        }

        public List<ColumnMetaInfo> GetColumns(Type type)
        {
            return _columns.GetOrAdd(type, t =>
            {
                var list = new List<ColumnMetaInfo>();
                var properties = type.GetProperties();
                foreach (var item in properties)
                {
                    var columnName = item.Name;
                    var isPrimaryKey = false;
                    var isDefault = false;
                    var isIdentity = false;
                    var isNotMapped = false;
                    var isConcurrencyCheck = false;
                    var isComplexType = false;
                    if (item.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() != null)
                    {
                        var attribute = item.GetCustomAttributes(typeof(ColumnAttribute), true)
                            .FirstOrDefault() as ColumnAttribute;
                        columnName = attribute.Name;
                    }
                    if (item.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() != null)
                    {
                        isPrimaryKey = true;
                    }
                    if (item.GetCustomAttributes(typeof(IdentityAttribute), true).FirstOrDefault() != null)
                    {
                        isIdentity = true;
                    }
                    if (item.GetCustomAttributes(typeof(DefaultAttribute), true).FirstOrDefault() != null)
                    {
                        isDefault = true;
                    }
                    if (item.GetCustomAttributes(typeof(ConcurrencyCheckAttribute), true).FirstOrDefault() != null)
                    {
                        isConcurrencyCheck = true;
                    }
                    if (item.GetCustomAttributes(typeof(NotMappedAttribute), true).FirstOrDefault() != null)
                    {
                        isNotMapped = true;
                    }
                    if (item.GetCustomAttributes(typeof(ComplexTypeAttribute), true).FirstOrDefault() != null)
                    {
                        isComplexType = true;
                    }
                    list.Add(new ColumnMetaInfo()
                    {
                        CsharpType = item.PropertyType,
                        IsDefault = isDefault,
                        ColumnName = columnName,
                        CsharpName = item.Name,
                        IsPrimaryKey = isPrimaryKey,
                        IsIdentity = isIdentity,
                        IsNotMapped = isNotMapped,
                        IsConcurrencyCheck = isConcurrencyCheck,
                        IsComplexType = isComplexType
                    });
                }
                return list;
            });
        }

    }

    /// <summary>
    /// 表信息
    /// </summary>
    public class TableMetaInfo
    {
        /// <summary>
        /// 数据库表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Csharp表名称
        /// </summary>
        public string CsharpName { get; set; }
    }

    /// <summary>
    /// 字段信息
    /// </summary>
    public class ColumnMetaInfo
    {
        /// <summary>
        /// 是否并发检查
        /// </summary>
        public bool IsConcurrencyCheck { get; set; }
        /// <summary>
        /// 是否默认值约束
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否是数据库字段
        /// </summary>
        public bool IsNotMapped { get; set; }
        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Csharp字段名
        /// </summary>
        public string CsharpName { get; set; }
        /// <summary>
        /// Csharp类型
        /// </summary>
        public Type CsharpType { get; set; }
        /// <summary>
        /// 是否主键约束
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否是自增列
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否为计算列
        /// </summary>
        public bool IsComplexType { get; set; }
    }
}
