

using System;
using Dapper.Extension;

namespace Standard.Model
{

	
	/// <summary>
    /// 
	/// 更新时间：2019-04-14 18:25:10
    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(11), IsNull：NO, Default：NULL
		/// JsName:id
		/// </summary>
		[Column("ID",ColumnKey.Primary,isIdentity:true)]
		public int? Id { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：varchar(225), IsNull：YES, Default：NULL
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME",ColumnKey.None,isColumn:false)]
		public string NickName { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：datetime, IsNull：YES, Default：NULL
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME",ColumnKey.None)]
		public DateTime? CreateTime { get; set; }
			
		/// <summary>
		/// 
		/// ColumnType：decimal(10,0), IsNull：YES, Default：NULL
		/// JsName:balance
		/// </summary>
		[Column("BALANCE",ColumnKey.None)]
		public decimal? Balance { get; set; }
	}

}



