

using System;
using Dapper.Common;

namespace Dapper.Common.Example
{

	
	/// <summary>
    /// 会员
	/// 更新时间：2019-04-01 10:07:51
    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID",true)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 用户CODE
		/// ColumnType：varchar(50), IsNull：YES, Default：NULL
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME",false)]
		public string NickName { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50), IsNull：YES, Default：NULL
		/// JsName:gander
		/// </summary>
		[Column("GANDER",false)]
		public string Gander { get; set; }
			
		/// <summary>
		/// 昵称
		/// ColumnType：varchar(225), IsNull：YES, Default：NULL
		/// JsName:account
		/// </summary>
		[Column("ACCOUNT",false)]
		public string Account { get; set; }
			
		/// <summary>
		/// 头像
		/// ColumnType：varchar(225), IsNull：YES, Default：NULL
		/// JsName:password
		/// </summary>
		[Column("PASSWORD",false)]
		public string Password { get; set; }
			
		/// <summary>
		/// 余额
		/// ColumnType：decimal(18,2), IsNull：YES, Default：NULL
		/// JsName:balance
		/// </summary>
		[Column("BALANCE",false)]
		public decimal? Balance { get; set; }
			
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
	/// 更新时间：2019-04-01 10:07:51
    /// </summary>
	[Table("member_order")]
	public partial class MemberOrder
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：0
		/// JsName:id
		/// </summary>
		[Column("ID",true)]
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
	
	/// <summary>
    /// 
	/// 更新时间：2019-04-01 10:07:51
    /// </summary>
	[Table("order")]
	public partial class Order
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID",true)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：YES, Default：NULL
		/// JsName:orderId
		/// </summary>
		[Column("ORDER_ID",false)]
		public int? OrderId { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：YES, Default：NULL
		/// JsName:orderItemId
		/// </summary>
		[Column("ORDER_ITEM_ID",false)]
		public int? OrderItemId { get; set; }
	}

}



