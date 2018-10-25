using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MintCyclingService.alipay
{
    /// <summary>
    /// 生成App支付订单信息 输入参数模型
    /// </summary>
    public class AlipayTradeModel
    {
        /// <summary>
        /// 对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        [XmlElement("body")]
        public string Body { get; set; }
 
        /// <summary>
        /// 商品的标题/交易标题/订单标题/订单关键字等。
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        ///// <summary>
        ///// 商户网站唯一订单号
        ///// </summary>
        //[XmlElement("out_trade_no")]
        //public string OutTradeNo { get; set; }
 
        ///// <summary>
        ///// 销售产品码，商家和支付宝签约的产品码
        ///// </summary>
        //[XmlElement("product_code")]
        //public string ProductCode { get; set; }

        ///// <summary>
        ///// 收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID
        ///// </summary>
        //[XmlElement("seller_id")]
        //public string SellerId { get; set; }


        /// <summary>
        /// 该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。
        /// </summary>
        //[XmlElement("timeout_express")]
        //public string TimeoutExpress { get; set; }

        /// <summary>
        /// 订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }


        /// <summary>
        /// 充值的类别1：押金充值；2：余额充值
        /// </summary>
        [XmlElement("TypeName")]
        public int TypeName { get; set; }


        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        

    }


    /// <summary>
    /// 用户交易退款请求输入参数 
    /// </summary>
    public class UserDepositRefundModel
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
    }


    /// <summary>
    /// 交易退款请求支付宝参数 
    /// </summary>
    public class AlipayTradeRefundModel
    {

        /// <summary>
        /// 订单支付时传入的商户订单号,不能和 trade_no同时为空
        /// </summary>
        [XmlElement("out_trade_no")]
        public string out_trade_no { get; set; }


        /// <summary>
        /// 支付宝交易号，和商户订单号不能同时为空
        /// </summary>
        [XmlElement("trade_no")]
        public string trade_no { get; set; }

        /// <summary>
        /// 需要退款的金额，该金额不能大于订单金额,单位为元，支持两位小数
        /// 必填
        /// </summary>
        [XmlElement("refund_amount")]
        public decimal refund_amount { get; set; }
 
    }




}
