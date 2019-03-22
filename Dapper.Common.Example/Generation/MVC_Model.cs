

using System;
using Dapper.Common;

namespace  Dapper.Common.Example
{

	/// <summary>
    /// 
	/// 更新时间：2019-03-22 18:49:50
    /// </summary>
	public partial class MemberModel :MvcModel
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：NO Default：0
		/// JsName:id
		/// </summary>
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50) IsNull：YES Default：NULL
		/// JsName:nickName
		/// </summary>
		public string NickName { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：YES Default：NULL
		/// JsName:gander
		/// </summary>
		public int? Gander { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50) IsNull：YES Default：NULL
		/// JsName:account
		/// </summary>
		public string Account { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(50) IsNull：YES Default：NULL
		/// JsName:password
		/// </summary>
		public string Password { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime IsNull：YES Default：NULL
		/// JsName:createTime
		/// </summary>
		public DateTime? CreateTime { get; set; }
			
		public Member NewInstance()
		{
			return new Member()
			{
    			Id = this.Id,
    			NickName = this.NickName,
    			Gander = this.Gander,
    			Account = this.Account,
    			Password = this.Password,
    			CreateTime = this.CreateTime,
    		};
		}

		public WhereQuery<Member> Query = new WhereQuery<Member>();
	}
	/// <summary>
    /// 
	/// 更新时间：2019-03-22 18:49:50
    /// </summary>
	public partial class MemberOrderModel :MvcModel
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：NO Default：0
		/// JsName:id
		/// </summary>
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：YES Default：NULL
		/// JsName:memberId
		/// </summary>
		public int? MemberId { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(18,2) IsNull：YES Default：NULL
		/// JsName:totalAmount
		/// </summary>
		public decimal? TotalAmount { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime IsNull：YES Default：NULL
		/// JsName:createTime
		/// </summary>
		public DateTime? CreateTime { get; set; }
			
		public MemberOrder NewInstance()
		{
			return new MemberOrder()
			{
    			Id = this.Id,
    			MemberId = this.MemberId,
    			TotalAmount = this.TotalAmount,
    			CreateTime = this.CreateTime,
    		};
		}

		public WhereQuery<MemberOrder> Query = new WhereQuery<MemberOrder>();
	}

}



