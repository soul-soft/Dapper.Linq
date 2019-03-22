using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.Common
{
    internal static class TypeMapper
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
                    if (attribute != null && attribute.Remove)
                    {
                        continue;
                    }
                    var propertyName = attribute == null ? property.Name : attribute.Name;
                    var isIdentity = attribute != null && attribute.IsPrimaryKey ? true : property == properties.First();
                    table.Columns.Add(new DbColumn()
                    {
                        FieldName = property.Name,
                        PrimaryKey = isIdentity,
                        ColumnName = propertyName,
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
        /// 获取列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetColumnName<T>(string fieldName)
        {
            return MapperCache(typeof(T)).Columns.Find(f => f.FieldName == fieldName).ColumnName;
        }
        /// <summary>
        /// 获取列定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<DbColumn> GetDbColumn<T>()
        {
            return MapperCache(typeof(T)).Columns;
        }
        /// <summary>
        /// 获取所有列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetColumnNames<T>()
        {
            return MapperCache(typeof(T)).Columns.Select(c => c.ColumnName).ToArray();
        }

        /// <summary>
        /// 通过字段名获取属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetFieldName<T>(string column)
        {
            return MapperCache(typeof(T)).Columns.Find(f => f.ColumnName == column).FieldName;
        }

        /// <summary>
        /// 获取所有属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetFieldNames<T>()
        {
            return MapperCache(typeof(T)).Columns.Select(c => c.FieldName).ToArray();
        }
        /// <summary>
        /// 获取标识列的属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetIdentityFieldName<T>()
        {
            return MapperCache(typeof(T)).Columns.Find(f => f.PrimaryKey == true).FieldName;
        }
        #endregion

        #region Model
        internal class DbColumn
        {
            public string FieldName { get; set; }
            public string ColumnName { get; set; }
            public bool PrimaryKey { get; set; }
        }
        internal class DbTable
        {
            public string TableName { get; set; }

            public List<DbColumn> Columns = new List<DbColumn>();
        }
        #endregion
    }
}
