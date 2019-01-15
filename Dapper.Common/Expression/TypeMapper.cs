using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Common
{
    public static class TypeMapper
    {
        #region Cache

        /// <summary>
        /// 映射缓存
        /// </summary>
        static Dictionary<Type, DbTable> Tables = new Dictionary<Type, DbTable>();

        /// <summary>
        /// 缓存策略
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static DbTable Cache(Type type)
        {
            if (!Tables.ContainsKey(type))
            {
                var table = new DbTable();

                #region 如果存在类型注解则使用字段注解配置，否则使用属性名，并将第一个属性作为标识列
                var tableAttrs = type.GetCustomAttributes(typeof(TableAttribute), false);
                table.TableName = tableAttrs != null && tableAttrs.Length > 0 ? (tableAttrs[0] as TableAttribute).Name : type.Name;
                #endregion

                #region 如果存在字段注解则使用字段注解配置，否则使用属性名，并将第一个属性作为标识列
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var propAttrs = prop.GetCustomAttributes(typeof(ColumnAttribute), false);
                    var name =  propAttrs.Length > 0 ? (propAttrs[0] as ColumnAttribute).Name : prop.Name;
                    var isIdentity = propAttrs.Length > 0 && !table.Columns.Exists(e=>e.Identity) && (propAttrs[0] as ColumnAttribute).Identity ? true : prop == props.First();
                    table.Columns.Add(new DbTable.DbColumn()
                    {
                        FieldName = prop.Name,
                        Identity = isIdentity,
                        ColumnName = name,
                    });
                }
                #endregion

                Tables.Add(type, table);

            }
            return Tables[type];
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
            return Cache(typeof(T)).TableName;
        }
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetColumnName<T>(string fieldName)
        {
            return Cache(typeof(T)).Columns.Find(f => f.FieldName == fieldName).ColumnName;
        }
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetColumnName(Type type, string fieldName)
        {
            return Cache(type).Columns.Find(f => f.FieldName == fieldName).ColumnName;
        }
        /// <summary>
        /// 获取列定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<DbTable.DbColumn> GetColumns<T>()
        {
            return Cache(typeof(T)).Columns;
        }
        /// <summary>
        /// 获取所有列名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetColumnNames<T>()
        {
            return Cache(typeof(T)).Columns.Select(c => c.ColumnName).ToArray();
        }
        /// <summary>
        /// 通过属性名列表获取字段名列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileds"></param>
        /// <returns></returns>
        public static string[] GetColumnNames<T>(List<string> fileds)
        {
            return Cache(typeof(T)).Columns.FindAll(f => fileds.Contains(f.FieldName)).Select(c => c.ColumnName).ToArray();
        }
        /// <summary>
        /// 通过字段名获取属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetFieldName<T>(string column)
        {
            return Cache(typeof(T)).Columns.Find(f => f.ColumnName == column).FieldName;
        }

        /// <summary>
        /// 获取所有属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetFieldNames<T>()
        {
            return Cache(typeof(T)).Columns.Select(c => c.FieldName).ToArray();
        }

        /// <summary>
        /// 获取标识列的属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetIdentityFieldName<T>()
        {
            return Cache(typeof(T)).Columns.Find(f => f.Identity == true).FieldName;
        }
        #endregion

        #region Model
        public class DbTable
        {
            public string TableName { get; set; }

            public List<DbColumn> Columns = new List<DbColumn>();
            public class DbColumn
            {
                public string FieldName { get; set; }
                public string ColumnName { get; set; }
                public bool Identity { get; set; }
            }
        }
        #endregion
    }
}
