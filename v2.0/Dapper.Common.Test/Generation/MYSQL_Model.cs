using System;
using Dapper.Extension;

namespace Standard.Model
{
	
	/// <summary>
    /// 
	/// 更新时间：2019-04-29 11:39:57
    /// </summary>
	[Table("fds")]
	public partial class Fds
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID", ColumnKey.Primary, true)]
		public int? Id { get; set; }
	}
	
	/// <summary>
    /// 
	/// 更新时间：2019-04-29 11:39:57
    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID", ColumnKey.Primary, true)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(255), IsNull：YES, Default：NULL
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME", ColumnKey.None, false)]
		public string NickName { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(18,2), IsNull：YES, Default：NULL
		/// JsName:balance
		/// </summary>
		[Column("BALANCE", ColumnKey.None, false)]
		public decimal? Balance { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime, IsNull：YES, Default：NULL
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME", ColumnKey.None, false)]
		public DateTime? CreateTime { get; set; }
	}
}



