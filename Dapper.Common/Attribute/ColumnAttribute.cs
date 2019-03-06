using System;

namespace Dapper.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Remove { get; set; }
        public ColumnAttribute()
        {

        }
        public ColumnAttribute(string columnName,bool primaryKey=false)
        {
            Name = columnName;
            PrimaryKey = primaryKey;
            Remove = false;
        }
        public ColumnAttribute(bool remove)
        {
            Remove = true;
        }
    }
}
