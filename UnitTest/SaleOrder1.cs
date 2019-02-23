
using Dapper.Common;
using System;

namespace UnitTest
{
    /// <summary>
    /// 商品销售订单BASE TABLE
    /// 更新时间：2019-02-14 09:04:13
    /// </summary>
    [Table("sale_order")]
    public class SaleOrder
    {
        /// <summary>
        /// 
        /// Type:int(11) IsNull:NO Default:null
        /// </summary>
        [Column("ID", true)]
        public int? Id { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(100) IsNull:YES Default:null
        /// </summary>
        [Column("OPEN_ID", false)]
        public string OpenId { get; set; }
        /// <summary>
        /// 用户CODE
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("MEMBER_NO", false)]
        public string MemberNo { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("NICK_NAME", false)]
        public string NickName { get; set; }
        /// <summary>
        /// 用户头像
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("HEAD_IMG", false)]
        public string HeadImg { get; set; }
        /// <summary>
        /// 订单编号
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("ORDER_NO", false)]
        public string OrderNo { get; set; }
        /// <summary>
        /// 
        /// Type:int(11) IsNull:YES Default:null
        /// </summary>
        [Column("STATE", false)]
        public int? State { get; set; }
        /// <summary>
        /// 店铺编号
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_NO", false)]
        public string ShopNo { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_NAME", false)]
        public string ShopName { get; set; }
        /// <summary>
        /// 店铺代理人
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_MEMBER_NO", false)]
        public string ShopMemberNo { get; set; }
        /// <summary>
        /// 提货地点编号
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_ADDR_NO", false)]
        public string ShopAddrNo { get; set; }
        /// <summary>
        /// 
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_PROFIT", false)]
        public decimal? ShopProfit { get; set; }
        /// <summary>
        /// 提货地点
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_ADDR_STREET", false)]
        public string ShopAddrStreet { get; set; }
        /// <summary>
        /// 产品名称多个
        /// Type:varchar(500) IsNull:YES Default:null
        /// </summary>
        [Column("PRODUCT_NAME", false)]
        public string ProductName { get; set; }
        /// <summary>
        /// 产品总数量
        /// Type:int(11) IsNull:YES Default:null
        /// </summary>
        [Column("PRODUCT_COUNT", false)]
        public int? ProductCount { get; set; }
        /// <summary>
        /// 商品费用
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("PRODUCT_FEE", false)]
        public decimal? ProductFee { get; set; }
        /// <summary>
        /// 配送方式：0快递，1自提
        /// Type:int(11) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_TYPE", false)]
        public int? DeliverType { get; set; }
        /// <summary>
        /// 订单运费
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_FEE", false)]
        public decimal? DeliverFee { get; set; }
        /// <summary>
        /// 配送省份
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_PROVINCE", false)]
        public string DeliverProvince { get; set; }
        /// <summary>
        /// 配送城市
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_CITY", false)]
        public string DeliverCity { get; set; }
        /// <summary>
        /// 配送小区
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_DISTRICT", false)]
        public string DeliverDistrict { get; set; }
        /// <summary>
        /// 街道
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("DELIVER_STREET", false)]
        public string DeliverStreet { get; set; }
        /// <summary>
        /// 优惠金额
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("DISCOUNT_FEE", false)]
        public decimal? DiscountFee { get; set; }
        /// <summary>
        /// 余额支付
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("BALANCE_FEE", false)]
        public decimal? BalanceFee { get; set; }
        /// <summary>
        /// 微信支付
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("WECHAT_FEE", false)]
        public decimal? WechatFee { get; set; }
        /// <summary>
        /// 应付金额
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("TOTAL_FEE", false)]
        public decimal? TotalFee { get; set; }
        /// <summary>
        /// 实付金额
        /// Type:decimal(18,2) IsNull:YES Default:null
        /// </summary>
        [Column("REAL_PAYMENT", false)]
        public decimal? RealPayment { get; set; }
        /// <summary>
        /// 订单备注
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_REMARK", false)]
        public string ShopRemark { get; set; }
        /// <summary>
        /// 快递公司
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("EXPRESS_COMPANY", false)]
        public string ExpressCompany { get; set; }
        /// <summary>
        /// 快递号
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("EXPRESS_NUMBER", false)]
        public string ExpressNumber { get; set; }
        /// <summary>
        /// 优惠券
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("COUPON_NO", false)]
        public string CouponNo { get; set; }
        /// <summary>
        /// 用户留言
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("MEMBER_REMARK", false)]
        public string MemberRemark { get; set; }
        /// <summary>
        /// 在线支付交易流水
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("TRANSACTION_ID", false)]
        public string TransactionId { get; set; }
        /// <summary>
        /// 下单时间
        /// Type:datetime IsNull:YES Default:null
        /// </summary>
        [Column("CREATE_TIME", false)]
        public DateTime? CreateTime { get; set; }

    }

    /// <summary>
    /// 店铺转换记录BASE TABLE
    /// 更新时间：2019-02-13 10:54:49
    /// </summary>
    [Table("shop_convert")]
    public class ShopConvert
    {
        /// <summary>
        /// 
        /// Type:int(11) IsNull:NO Default:null
        /// </summary>
        [Column("ID", true)]
        public int? Id { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("SHOP_NO", false)]
        public string ShopNo { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(50) IsNull:YES Default:null
        /// </summary>
        [Column("MEMBER_NO", false)]
        public string MemberNo { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("NICK_NAME", false)]
        public string NickName { get; set; }
        /// <summary>
        /// 
        /// Type:varchar(225) IsNull:YES Default:null
        /// </summary>
        [Column("HEAD_IMG", false)]
        public string HeadImg { get; set; }
        /// <summary>
        /// 浏览次数
        /// Type:int(11) IsNull:YES Default:null
        /// </summary>
        [Column("BROWSE_COUNT", false)]
        public int? BrowseCount { get; set; }
        /// <summary>
        /// 更新时间
        /// Type:datetime IsNull:YES Default:null
        /// </summary>
        [Column("UPDATE_TIME", false)]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// Type:datetime IsNull:YES Default:null
        /// </summary>
        [Column("CREATE_TIME", false)]
        public DateTime? CreateTime { get; set; }

    }

    /// <summary>
    /// 店铺日销售
    /// </summary>
    public class ShopDaySale
    {
        public string ShopNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? MemberCount { get; set; }
        public int? OrderCount { get; set; }
        public decimal? ShopProfit { get; set; }
        public decimal? UnShopProfit { get; set; }
        public decimal? SaleAmount { get; set; }
        public int? ProductCount { get; set; }
    }
}

