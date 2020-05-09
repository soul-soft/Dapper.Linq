using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using Dapper.Attributes;
using System.Linq.Expressions;

namespace Dapper.Expressions
{
    /// <summary>
    /// 数据库元信息
    /// </summary>
    public static class DbMetaInfoCache
    {
        private static readonly ConcurrentDictionary<Type, TableInfo> _tables
            = new ConcurrentDictionary<Type, TableInfo>();

        private static readonly ConcurrentDictionary<Type, List<ColumnInfo>> _columns
            = new ConcurrentDictionary<Type, List<ColumnInfo>>();

        public static TableInfo GetTable(Type type)
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
                var table = new TableInfo()
                {
                    TableName = name,
                    CsharpName = t.Name
                };
                return table;
            });
        }

        public static List<ColumnInfo> GetColumns(Type type)
        {
            return _columns.GetOrAdd(type, t =>
            {
                var list = new List<ColumnInfo>();
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
                    list.Add(new ColumnInfo()
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
    public class TableInfo
    {
        public string TableName { get; set; }
        public string CsharpName { get; set; }
    }

    /// <summary>
    /// 字段信息
    /// </summary>
    public class ColumnInfo
    {
        public bool IsConcurrencyCheck { get; set; }
        public bool IsDefault { get; set; }
        public bool IsNotMapped { get; set; }
        public string ColumnName { get; set; }
        public string CsharpName { get; set; }
        public Type CsharpType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsComplexType { get; set; }
    }
}
