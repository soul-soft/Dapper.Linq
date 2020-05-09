using System;
using Dapper.Attributes;

namespace Daper.Entitys
{
	
	/// <summary>
    /// 
	/// 更新时间：2020-05-09 17:04:41
    /// </summary>
	[Table("student")]
	public partial class Student
	{
				
		/// <summary>
        /// 
		/// </summary>
		[Column("id")]
        [PrimaryKey]
[Identity]

        public int? Id { get; set; }
			
		/// <summary>
        /// 
		/// </summary>
		[Column("stu_name")]
        
        public string StuName { get; set; }
			
		/// <summary>
        /// 
		/// </summary>
		[Column("create_time")]
        [Default]

        public DateTime? CreateTime { get; set; }
	}
}



