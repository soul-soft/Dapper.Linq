using System;
using Dapper.Extension;

namespace Standard.Model
{	/// <summary>
    /// 学生表
	/// 更新时间：    /// </summary>
	[Table("student")]
	public partial class Student
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:id
		/// </summary>
		[Column("ID", ColumnKey.None, true)]
		public int? Id { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(225), IsNull：NO
		/// JsName:realName
		/// </summary>
		[Column("REAL_NAME", ColumnKey.None, false)]
		public string RealName { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:gander
		/// </summary>
		[Column("GANDER", ColumnKey.None, false)]
		public int? Gander { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(50), IsNull：NO
		/// JsName:mobile
		/// </summary>
		[Column("MOBILE", ColumnKey.None, false)]
		public string Mobile { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：datetime(8), IsNull：NO
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME", ColumnKey.None, false)]
		public DateTime? CreateTime { get; set; }
			
	}
	/// <summary>
    /// 用户表
	/// 更新时间：    /// </summary>
	[Table("member")]
	public partial class Member
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:id
		/// </summary>
		[Column("ID", ColumnKey.Primary, true)]
		public int? Id { get; set; }
				
		/// <summary>
		/// 昵称
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:nickName
		/// </summary>
		[Column("NICK_NAME", ColumnKey.None, false)]
		public string NickName { get; set; }
				
		/// <summary>
		/// 创建时间
		/// ColumnType：datetime(8), IsNull：NO
		/// JsName:createTime
		/// </summary>
		[Column("CREATE_TIME", ColumnKey.None, false)]
		public DateTime? CreateTime { get; set; }
				
		/// <summary>
		/// 开发ID
		/// ColumnType：varchar(225), IsNull：NO
		/// JsName:openId
		/// </summary>
		[Column("OPEN_ID", ColumnKey.None, false)]
		public string OpenId { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：decimal(9), IsNull：NO
		/// JsName:balance
		/// </summary>
		[Column("BALANCE", ColumnKey.None, false)]
		public decimal? Balance { get; set; }
			
	}
	/// <summary>
    /// 
	/// 更新时间：    /// </summary>
	[Table("t_sczx")]
	public partial class TSczx
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:id
		/// </summary>
		[Column("ID", ColumnKey.Primary, true)]
		public int? Id { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxPgd
		/// </summary>
		[Column("SCZX_PGD", ColumnKey.None, false)]
		public string SczxPgd { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxDdh
		/// </summary>
		[Column("SCZX_DDH", ColumnKey.None, false)]
		public string SczxDdh { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxWlmc
		/// </summary>
		[Column("SCZX_WLMC", ColumnKey.None, false)]
		public string SczxWlmc { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxCpxh
		/// </summary>
		[Column("SCZX_CPXH", ColumnKey.None, false)]
		public string SczxCpxh { get; set; }
				
		/// <summary>
		/// 物料编号
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxWlbh
		/// </summary>
		[Column("SCZX_WLBH", ColumnKey.None, false)]
		public string SczxWlbh { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:sczxGx
		/// </summary>
		[Column("SCZX_GX", ColumnKey.None, false)]
		public string SczxGx { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：datetime(8), IsNull：NO
		/// JsName:sczxJhwcDate
		/// </summary>
		[Column("SCZX_JHWC_DATE", ColumnKey.None, false)]
		public DateTime? SczxJhwcDate { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：bigint(8), IsNull：NO
		/// JsName:sczxJhwcDatetime
		/// </summary>
		[Column("SCZX_JHWC_DATETIME", ColumnKey.None, false)]
		public int? SczxJhwcDatetime { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:sczxJhCount
		/// </summary>
		[Column("SCZX_JH_COUNT", ColumnKey.None, false)]
		public int? SczxJhCount { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:sczxWcCount
		/// </summary>
		[Column("SCZX_WC_COUNT", ColumnKey.None, false)]
		public int? SczxWcCount { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:sczxFgCount
		/// </summary>
		[Column("SCZX_FG_COUNT", ColumnKey.None, false)]
		public int? SczxFgCount { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:sczxBlCount
		/// </summary>
		[Column("SCZX_BL_COUNT", ColumnKey.None, false)]
		public int? SczxBlCount { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:sczxStatus
		/// </summary>
		[Column("SCZX_STATUS", ColumnKey.None, false)]
		public int? SczxStatus { get; set; }
			
	}
	/// <summary>
    /// demo视图
	/// 更新时间：    /// </summary>
	[Table("v_sczx")]
	public partial class VSczx
	{
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:a
		/// </summary>
		[Column("A", ColumnKey.None, true)]
		public int? A { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:b
		/// </summary>
		[Column("B", ColumnKey.None, false)]
		public string B { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:c
		/// </summary>
		[Column("C", ColumnKey.None, false)]
		public string C { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:d
		/// </summary>
		[Column("D", ColumnKey.None, false)]
		public string D { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:e
		/// </summary>
		[Column("E", ColumnKey.None, false)]
		public string E { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:f
		/// </summary>
		[Column("F", ColumnKey.None, false)]
		public string F { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：varchar(250), IsNull：NO
		/// JsName:g
		/// </summary>
		[Column("G", ColumnKey.None, false)]
		public string G { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：datetime(8), IsNull：NO
		/// JsName:h
		/// </summary>
		[Column("H", ColumnKey.None, false)]
		public DateTime? H { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：bigint(8), IsNull：NO
		/// JsName:i
		/// </summary>
		[Column("I", ColumnKey.None, false)]
		public int? I { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:j
		/// </summary>
		[Column("J", ColumnKey.None, false)]
		public int? J { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:k
		/// </summary>
		[Column("K", ColumnKey.None, false)]
		public int? K { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:l
		/// </summary>
		[Column("L", ColumnKey.None, false)]
		public int? L { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:m
		/// </summary>
		[Column("M", ColumnKey.None, false)]
		public int? M { get; set; }
				
		/// <summary>
		/// 
		/// ColumnType：int(4), IsNull：NO
		/// JsName:n
		/// </summary>
		[Column("N", ColumnKey.None, false)]
		public int? N { get; set; }
			
	}
}


