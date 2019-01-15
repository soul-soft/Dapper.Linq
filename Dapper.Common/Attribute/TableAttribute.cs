using System;

namespace Dapper.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {
        public string Name { get; set; }
        public TableAttribute()
        {

        }
        public TableAttribute(string name)
        {
            this.Name = name;
        }
    }
}
