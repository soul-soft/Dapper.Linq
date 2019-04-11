

using System;
using Dapper.Common;

namespace Standard.Model
{

	
	/// <summary>
    /// 
	/// 更新时间：2019-04-10 15:30:38
    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID",ColumnKey.Primary)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(225), IsNull：YES, Default：NULL
		/// JsName:account
		/// </summary>
		[Column("ACCOUNT",ColumnKey.None)]
		public string Account { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：YES, Default：NULL
		/// JsName:password
		/// </summary>
		[Column("PASSWORD",ColumnKey.None)]
		public int? Password { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime, IsNull：YES, Default：NULL
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME",ColumnKey.None)]
		public DateTime? CreateTime { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(18,2), IsNull：YES, Default：NULL
		/// JsName:balance
		/// </summary>
		[Column("BALANCE",ColumnKey.None)]
		public decimal? Balance { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(255), IsNull：YES, Default：NULL
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME",ColumnKey.None)]
		public string NickName { get; set; }
	}
	
	/// <summary>
    /// 
	/// 更新时间：2019-04-10 15:30:38
    /// </summary>
	[Table("member_order")]
	public partial class MemberOrder
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID",ColumnKey.Primary)]
		public int? Id { get; set; }
	}

}



