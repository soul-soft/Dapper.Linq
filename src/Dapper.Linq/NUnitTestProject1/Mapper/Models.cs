using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Linq;

namespace NUnitTestProject1
{
    public enum Grade
    {
        A = 0,
        B = 1,
        C = 2,
        E = 3,
        F = 4,
    }

    [Table("`student`")]
    public class Student
    {
        [Column("`id`", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("`name`")]
        public string Name { get; set; }
        [Column("`version`")]
        public string Version { get; set; }
        [Column("`school_id`")]
        public int? SchoolId { get; set; }
        [Column("`grade`")]
        public Grade? Grade { get; set; }
        [Column("`age`")]
        public int? Age { get; set; }
        [Column("`create_time`")]
        public DateTime? CreateTime { get; set; }
        [Column(isColumn:false)]
        public int? Type { get; set; }
        [Column("`is_delete`")]
        public bool? IsDelete { get; set; }
    }


    [Table("`school`")]
    public class School
    {
        [Column("`id`", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("`name`")]
        public string Name { get; set; }
        [Column("`address`")]
        public string Address { get; set; }
    }


    [Table("`school`")]
    public class Goods
    {
        [Column("`id`", ColumnKey.Primary, true)]
        public int? Id { get; set; }
        [Column("`name`")]
        public string Name { get; set; }
        [Column("`address`")]
        public string Address { get; set; }
    }
}
