using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.Common
{
    internal static class Mapper
    {
        #region Cache

        /// <summary>
        /// 映射缓存
        /// </summary>
        static Dictionary<Type, DbTable> CacheTables = new Dictionary<Type, DbTable>();

        private static DbTable MapperCache(Type type)
        {
            if (!CacheTables.ContainsKey(type))
            {
                var table = new DbTable();

                #region 如果存在类型注解则使用字段注解配置，否则使用属性名，并将第一个属性作为标识列
                var tableAttrs = type.GetCustomAttributes(typeof(TableAttribute), false);
                table.TableName = tableAttrs != null && tableAttrs.Length > 0 ? (tableAttrs[0] as TableAttribute).Name : type.Name;
                #endregion

                #region 如果存在字段注解则使用字段注解配置，否则使用属性名，并将第一个属性作为标识列
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(typeof(ColumnAttribute), false);
                    ColumnAttribute attribute = attributes.Length > 0 ? (ColumnAttribute)attributes[0] : null;
                    if (attribute != null && !attribute.IsColumn)
                    {
                        continue;
                    }
                    var columnName = attribute == null ? property.Name : attribute.ColumnName;
                    var columnKey = attribute == null ? ColumnKey.None : attribute.ColumnKey;
                    table.Columns.Add(new DbColumn()
                    {
                        PropertyName = property.Name,
                        ColumnKey = columnKey,
                        ColumnName = columnName,
                    });
                }
                #endregion

                CacheTables.Add(type, table);

            }

            return CacheTables[type];
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTableName<T>()
        {
            return MapperCache(typeof(T)).TableName;
        }
      
        /// <summary>
        /// 获取列定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<DbColumn> GetColumnList<T>()
        {
            return MapperCache(typeof(T)).Columns;
        }      

        /// <summary>
        /// 获取标识列的属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DbColumn GetColumn<T>(Func<DbColumn, bool> func)
        {
            return MapperCache(typeof(T)).Columns.Find(f => func(f));
        }
        #endregion

        #region Model
        internal class DbColumn
        {
            public string PropertyName { get; set; }
            public string ColumnName { get; set; }
            public ColumnKey ColumnKey { get; set; }
        }
        internal class DbTable
        {
            public string TableName { get; set; }

            public List<DbColumn> Columns = new List<DbColumn>();
        }
        #endregion
    }
}
