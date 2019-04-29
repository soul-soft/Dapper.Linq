using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Extension
{
    public class EntityUtil
    {
        private static List<Table> _database = new List<Table>();
        private static Table Build(Type type)
        {
            if (!_database.Exists(e => e.CSharpType == type))
            {
                var properties = type.GetProperties();
                var columns = new List<Column>();
                foreach (var item in properties)
                {
                    var columnName = item.Name;
                    var identity = false;
                    var key = ColumnKey.None;
                    if (item.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0)
                    {
                        var attribute = (item.GetCustomAttributes(typeof(ColumnAttribute), true)[0] as ColumnAttribute);
                        if (!attribute.IsColumn)
                        {
                            continue;
                        }
                        key = attribute.Key;
                        identity = attribute.IsIdentity;
                        columnName = attribute.Name;
                    }
                    var column = new Column()
                    {
                        ColumnKey = key,
                        ColumnName = columnName,
                        CSharpName = item.Name,
                        Identity = identity,
                    };
                    columns.Add(column);
                }
                if (columns.Count>0&&!columns.Exists(e=>e.ColumnKey==ColumnKey.Primary))
                {
                    columns[0].ColumnKey = ColumnKey.Primary;
                }
                var tableName = type.Name;
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                {
                    tableName = (type.GetCustomAttributes(typeof(TableAttribute), true)[0] as TableAttribute).Name;
                }
                var table = new Table()
                {
                    CSharpName = type.Name,
                    CSharpType = type,
                    TableName = tableName,
                    Columns = columns,
                };
                lock (_database)
                {
                    if (!_database.Exists(e => e.CSharpType == type))
                    {
                        _database.Add(table);
                    }
                }
            }
            return _database.Find(f => f.CSharpType == type);
        }
        public static Table GetTable<T>() where T : class
        {
            return Build(typeof(T));
        }
        public static Column GetColumn<T>(Func<Column, bool> func)
        {
            return Build(typeof(T)).Columns.Find(f => func(f));
        }
        public static Column GetColumn(Type type,Func<Column, bool> func)
        {
            return Build(type).Columns.Find(f => func(f));
        }
    }
    public class Table
    {
        public Type CSharpType { get; set; }
        public string TableName { get; set; }
        public string CSharpName { get; set; }
        public List<Column> Columns { get; set; }
    }
    public class Column
    {
        public string ColumnName { get; set; }
        public string CSharpName { get; set; }
        public bool Identity { get; set; }
        public ColumnKey ColumnKey { get; set; }
    }
}
