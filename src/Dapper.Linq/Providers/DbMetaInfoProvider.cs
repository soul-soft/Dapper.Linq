using Dapper.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dapper
{
    /// <summary>
    /// 数据库元信息提供程序
    /// </summary>
    public interface IDbMetaInfoProvider
    {
        /// <summary>
        /// 获取表的元信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        DbTableMetaInfo GetTable(Type type);
        /// <summary>
        /// 获取字段的元信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<DbColumnMetaInfo> GetColumns(Type type);
    }
   
    /// <summary>
    /// 注解方案数据库元信息
    /// </summary>
    public class AnnotationDbMetaInfoProvider: IDbMetaInfoProvider
    {
        private static readonly ConcurrentDictionary<Type, DbTableMetaInfo> _tables
            = new ConcurrentDictionary<Type, DbTableMetaInfo>();

        private static readonly ConcurrentDictionary<Type, List<DbColumnMetaInfo>> _columns
            = new ConcurrentDictionary<Type, List<DbColumnMetaInfo>>();

        public DbTableMetaInfo GetTable(Type type)
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
                var table = new DbTableMetaInfo()
                {
                    TableName = name,
                    CsharpName = t.Name
                };
                return table;
            });
        }

        public List<DbColumnMetaInfo> GetColumns(Type type)
        {
            return _columns.GetOrAdd(type, t =>
            {
                var list = new List<DbColumnMetaInfo>();
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
                    list.Add(new DbColumnMetaInfo()
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
    public class DbTableMetaInfo
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
    public class DbColumnMetaInfo
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
