using System;

namespace Dapper.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        public string Name { get; set; }
        public bool Identity { get; set; }
        public ColumnAttribute()
        {

        }
        public ColumnAttribute(string name,bool identity=false)
        {
            Name = name;
            Identity = identity;
        }
    }
}
