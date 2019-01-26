using Dapper.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    /// <summary>
    /// 对应表student,同名可以省略，字段名不分大小写，不分下划线，但除此之外都要匹配
    /// </summary>
    [Table("student")]
    public class Student
    {
        /// <summary>
        /// 对应字段ID,主键标识列identity=true
        /// </summary>
        [Column("ID", true)]
        public int? Id { get; set; }
        /// <summary>
        /// 对应字段ME_NAME,通过Column校正为Name,
        /// </summary>
        [Column("NAME", false)]
        public string Name { get; set; }
        /// <summary>
        /// 对应数据字段AGE无需校正
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// 对应字段CREATE_TIME,不分大小写,下划线不敏感
        /// </summary>
        [Column("CREATE_TIME", false)]
        public DateTime? CreateTime { get; set; }
    }
}
