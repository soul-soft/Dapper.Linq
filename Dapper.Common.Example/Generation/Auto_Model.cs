

using System;
using Dapper.Common;

namespace Dapper.Common.Example
{

	
	/// <summary>
    /// 
	/// 更新时间：2019-03-22 18:42:15
    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：0
		/// JsName:id
		/// </summary>
		[Column("ID",false)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50), IsNull：YES, Default：NULL
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME",false)]
		public string NickName { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：YES, Default：NULL
		/// JsName:gander
		/// </summary>
		[Column("GANDER",false)]
		public int? Gander { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50), IsNull：YES, Default：NULL
		/// JsName:account
		/// </summary>
		[Column("ACCOUNT",false)]
		public string Account { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50), IsNull：YES, Default：NULL
		/// JsName:password
		/// </summary>
		[Column("PASSWORD",false)]
		public string Password { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime, IsNull：YES, Default：NULL
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME",false)]
		public DateTime? CreateTime { get; set; }
	}
	
	/// <summary>
    /// 
	/// 更新时间：2019-03-22 18:42:15
    /// </summary>
	[Table("member_order")]
	public partial class MemberOrder
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：0
		/// JsName:id
		/// </summary>
		[Column("ID",false)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：YES, Default：NULL
		/// JsName:memberId
		/// </summary>
		[Column("MEMBER_ID",false)]
		public int? MemberId { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(18,2), IsNull：YES, Default：NULL
		/// JsName:totalAmount
		/// </summary>
		[Column("TOTAL_AMOUNT",false)]
		public decimal? TotalAmount { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime, IsNull：YES, Default：NULL
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME",false)]
		public DateTime? CreateTime { get; set; }
	}

}



