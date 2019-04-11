

using System;
using Dapper.Common;

namespace  Dapper.Common.Example
{

	/// <summary>
    /// 
	/// 更新时间：2019-04-10 15:30:38
    /// </summary>
	public partial class MemberModel :MvcModel
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：NO Default：NULL
		/// JsName:id
		/// </summary>
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(225) IsNull：YES Default：NULL
		/// JsName:account
		/// </summary>
		public string Account { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：YES Default：NULL
		/// JsName:password
		/// </summary>
		public int? Password { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime IsNull：YES Default：NULL
		/// JsName:createTime
		/// </summary>
		public DateTime? CreateTime { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(18,2) IsNull：YES Default：NULL
		/// JsName:balance
		/// </summary>
		public decimal? Balance { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(255) IsNull：YES Default：NULL
		/// JsName:nickName
		/// </summary>
		public string NickName { get; set; }
			
		public Member NewInstance()
		{
			return new Member()
			{
    			Id = this.Id,
    			Account = this.Account,
    			Password = this.Password,
    			CreateTime = this.CreateTime,
    			Balance = this.Balance,
    			NickName = this.NickName,
    		};
		}

		public Queryable<Member> Query = new Queryable<Member>();
	}
	/// <summary>
    /// 
	/// 更新时间：2019-04-10 15:30:38
    /// </summary>
	public partial class MemberOrderModel :MvcModel
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11) IsNull：NO Default：NULL
		/// JsName:id
		/// </summary>
		public int? Id { get; set; }
			
		public MemberOrder NewInstance()
		{
			return new MemberOrder()
			{
    			Id = this.Id,
    		};
		}

		public Queryable<MemberOrder> Query = new Queryable<MemberOrder>();
	}

}



