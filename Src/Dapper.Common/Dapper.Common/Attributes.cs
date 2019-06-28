using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public TableAttribute(string name=null)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public ColumnKey Key { get; set; }
        public bool IsColumn { get; set; }
        public bool IsIdentity { get; set; }
        public ColumnAttribute(string name = null, ColumnKey key = ColumnKey.None,bool isIdentity = false, bool isColumn = true)
        {
            Name = name;
            Key = key;
            IsColumn = isColumn;
            IsIdentity = isIdentity;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {

    }
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {

    }
    public enum ColumnKey
    {
        None,
        Primary,
        Quniue,
        Unique,
        Foreign
    }
}
