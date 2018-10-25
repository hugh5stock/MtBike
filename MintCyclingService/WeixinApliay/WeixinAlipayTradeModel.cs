using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.WeixinApliay
{
    /// <summary> 
    /// 商户发起生成预付单App请求输入 参数模型
    /// </summary>
    public class WeixinAlipayTradeModel
    {
        ///// <summary>
        ///// 商户号/微信支付分配的商户号
        ///// </summary>
        //public string mch_id { get; set; }


        ///// <summary>
        ///// 应用ID
        ///// </summary>
        //public string appid { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public int total_fee { get; set; }

        ///// <summary>
        ///// 订单号--服务端生成
        ///// </summary>
        //public string OrderNumber { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }


        ///// <summary>
        ///// 通知地址/接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。
        ///// </summary>
        //public string notify_url { get; set; }



        /// <summary>
        /// 充值的类别1：押金充值；2：余额充值
        /// </summary>
        public int TypeName { get; set; }


        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
    }


    public class WeixinAlipayTradeOrderModel
    {

        /// <summary>
        /// 订单总金额
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }


        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OrderNumber { get; set; }
        

        /// <summary>
        /// 充值的类别1：押金充值；2：余额充值
        /// </summary>
        public int TypeName { get; set; }


        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
    }


    /// <summary> 
    /// 返回预付单结果参数给App 参数模型
    /// </summary>
    public class ResultWeinXinTradeModel
    {

        /// <summary>
        /// 应用ID
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 商户号/微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string prepay_id { get; set; }

        /// <summary>
        /// 扩展字段--暂填写固定值Sign=WXPay
        /// </summary>
        public string package { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 微信返回的签名
        /// </summary>
        public string sign { get; set; }


    }



    /// <summary> 
    /// 返回支付结果通知参数 模型
    /// </summary>
    public class NotifyWeinXinResultModel
    {

        /// <summary>
        /// 应用ID
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 商户号/微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 微信返回的签名
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }


        /// <summary>
        /// 用户标识--用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }


        /// <summary>
        /// 交易类型:APP
        /// </summary>
        public string trade_type { get; set; }



        /// <summary>
        /// 付款银行-字符型银行编码
        /// </summary>
        public string bank_type { get; set; }


        /// <summary>
        /// 总金额
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 现金支付金额--现金支付金额订单现金支付金额
        /// </summary>
        public int cash_fee { get; set; }


        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }


        /// <summary>
        /// 商户订单号--商户系统的订单号，与请求一致。
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string time_end { get; set; }

        /// <summary>
        /// 交易状态描述 
        /// </summary>
        //public string trade_state_desc { get; set; }

        /// <summary>
        /// 交易状态
        /// SUCCESS—支付成功;REFUND—转入退款；NOTPAY—未支付;CLOSED—已关闭;REVOKED—已撤销（刷卡支付;USERPAYING--用户支付中;PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        public string trade_state { get; set; }


        //扩展字段--是否充值成功
        public int Status { get; set; }






    }

    /// <summary>
    /// 修改订单状态是否成功输入 参数模型
    /// </summary>
    public class WeixinIsSuccessOrFail
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 充值的类别1：支付宝充值押金；2：微信充值押金
        /// </summary>
        public int TypeName { get; set; }


    }


    /// <summary>
    /// 微信退款申请参数模型
    /// </summary>
    public class WeixinRefundRequest
    {
        /// <summary>
        /// 微信退款申请
        /// </summary>
        public WeixinRefundRequest()
        {

        }

        /// <summary>
        /// 公众账号ID    
        /// </summary>
        /// <remarks>
        /// 微信分配的公众账号ID（企业号corpid即为此appId）
        /// </remarks>
        public string appid { get; set; }


        /// <summary>
        /// 商户号    
        /// </summary>
        /// <remarks>
        /// 微信支付分配的商户号
        /// </remarks>
        public string mch_id { get; set; }

        /// <summary>
        /// 设备号    
        /// </summary>
        /// <remarks>
        /// 终端设备号
        /// </remarks>
        public string device_info { get; set; }

        /// <summary>
        /// 随机字符串    
        /// </summary>
        /// <remarks>
        /// 随机字符串，不长于32位。推荐随机数生成算法
        /// </remarks>
        public string nonce_str { get; set; }

        /// <summary>
        /// 签名    
        /// </summary>
        /// <remarks>
        /// 签名，详见签名生成算法
        /// </remarks>
        public string sign { get; set; }

        /// <summary>
        /// 签名类型    
        /// </summary>
        /// <remarks>
        /// HMAC-SHA256 签名类型，目前支持HMAC-SHA256和MD5，默认为MD5
        /// </remarks>
        public string sign_type { get; set; }

        /// <summary>
        /// 微信订单号    
        /// </summary>
        /// <remarks>
        /// 微信生成的订单号，在支付通知中有返回
        /// </remarks>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号    
        /// </summary>
        /// <remarks>
        /// 商户侧传给微信的订单号
        /// </remarks>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商户退款单号    
        /// </summary>
        /// <remarks>
        /// 商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔
        /// </remarks>
        public string out_refund_no { get; set; }

        /// <summary>
        /// 订单金额    
        /// </summary>
        /// <remarks>
        /// 订单总金额，单位为分，只能为整数，详见支付金额
        /// </remarks>
        public string total_fee { get; set; }

        /// <summary>
        /// 退款金额    
        /// </summary>
        /// <remarks>
        /// 退款总金额，订单总金额，单位为分，只能为整数，详见支付金额
        /// </remarks>
        public string refund_fee { get; set; }

        /// <summary>
        /// 货币种类    
        /// </summary>
        /// <remarks>
        /// CNY 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </remarks>
        public string refund_fee_type { get; set; }

        /// <summary>
        /// 操作员    
        /// </summary>
        /// <remarks>
        /// 作员帐号, 默认为商户号
        /// </remarks>
        public string op_user_id { get; set; }

        /// <summary>
        /// 退款资金来源    
        /// </summary>
        /// <remarks>
        ///  REFUND_SOURCE_RECHARGE_FUNDS 仅针对老资金流商户使用
        ///  REFUND_SOURCE_UNSETTLED_FUNDS---未结算资金退款（默认使用未结算资金退款）
        ///  REFUND_SOURCE_RECHARGE_FUNDS---可用余额退款(限非当日交易订单的退款）
        /// </remarks>
        public string refund_account { get; set; }
    }

    /// <summary>
    /// 微信退款申请返回参数模型
    /// </summary>
    public class WeixinRefundResult
    {
        /// <summary>
        /// 返回状态码    
        /// </summary>
        /// <remarks>
        /// SUCCESS/FAIL
        /// </remarks>
        public string return_code { get; set; }

        /// <summary>
        /// 返回信息    
        /// </summary>
        /// <remarks>
        /// 返回信息，如非空，为错误原因. 签名失败 参数格式校验错误
        /// </remarks>
        public string return_msg { get; set; }

        /// <summary>
        /// 业务结果    
        /// </summary>
        /// <remarks>
        /// SUCCESS SUCCESS/FAIL
        /// SUCCESS退款申请接收成功，结果通过退款查询接口查询
        /// FAIL 提交业务失败
        /// </remarks>
        public string result_code { get; set; }

        /// <summary>
        /// 错误代码描述    
        /// </summary>
        /// <remarks>
        ///SYSTEMERROR 列表详见错误码列表
        /// </remarks>
        public string err_code { get; set; }

        /// <summary>
        /// 错误代码描述    
        /// </summary>
        /// <remarks>
        /// 系统超时 结果信息描述
        /// </remarks>
        public string err_code_des { get; set; }

        /// <summary>
        /// 公众账号ID    
        /// </summary>
        /// <remarks>
        /// 微信支付分配的商户号
        /// </remarks>
        public string appid { get; set; }

        /// <summary>
        /// 商户号    
        /// </summary>
        /// <remarks>
        /// 微信支付分配的商户号
        /// </remarks>
        public string mch_id { get; set; }

        /// <summary>
        /// 设备号    
        /// </summary>
        /// <remarks>
        /// 微信支付分配的终端设备号，与下单一致
        /// </remarks>
        public string device_info { get; set; }

        /// <summary>
        /// 随机字符串    
        /// </summary>
        /// <remarks>
        /// 随机字符串，不长于32位
        /// </remarks>
        public string nonce_str { get; set; }

        /// <summary>
        /// 签名    
        /// </summary>
        /// <remarks>
        /// 签名，详见签名算法
        /// </remarks>
        public string sign { get; set; }

        /// <summary>
        /// 微信订单号    
        /// </summary>
        /// <remarks>
        /// 微信订单号
        /// </remarks>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号    
        /// </summary>
        /// <remarks>
        /// 商户系统内部的订单号
        /// </remarks>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商户退款单号    
        /// </summary>
        /// <remarks>
        /// 商户退款单号
        /// </remarks>
        public string out_refund_no { get; set; }

        /// <summary>
        /// 微信退款单号    
        /// </summary>
        /// <remarks>
        /// 微信退款单号
        /// </remarks>
        public string refund_id { get; set; }

        /// <summary>
        /// 退款金额    
        /// </summary>
        /// <remarks>
        /// 退款总金额,单位为分,可以做部分退款
        /// </remarks>
        public int refund_fee { get; set; }

        /// <summary>
        /// 应结退款金额    
        /// </summary>
        /// <remarks>
        /// 去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额小于等于申请退款金额
        /// </remarks>
        public int settlement_refund_fee { get; set; }

        /// <summary>
        /// 标价金额    
        /// </summary>
        /// <remarks>
        /// 订单总金额，单位为分，只能为整数，详见支付金额
        /// </remarks>
        public int total_fee { get; set; }

        /// <summary>
        /// 应结订单金额    
        /// </summary>
        /// <remarks>
        /// 去掉非充值代金券金额后的订单总金额，应结订单金额=订单金额-非充值代金券金额，应结订单金额 小于等于 订单金额。 
        /// </remarks>
        public int settlement_total_fee { get; set; }

        /// <summary>
        /// 标价币种    
        /// </summary>
        /// <remarks>
        /// CNY 订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型 
        /// </remarks>
        public string fee_type { get; set; }

        /// <summary>
        /// 现金支付金额    
        /// </summary>
        /// <remarks>
        /// 现金支付金额，单位为分，只能为整数，详见支付金额 
        /// </remarks>
        public int cash_fee { get; set; }

        /// <summary>
        /// 现金支付币种    
        /// </summary>
        /// <remarks>
        ///货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </remarks>
        public string cash_fee_type { get; set; }

        /// <summary>
        /// 现金退款金额    
        /// </summary>
        /// <remarks>
        ///现金退款金额，单位为分，只能为整数，详见支付金额
        /// </remarks>
        public int cash_refund_fee { get; set; }


        //业务结果 result_code 是 String(16) SUCCESS SUCCESS/FAIL SUCCESS退款申请接收成功，结果通过退款查询接口查询
        //                            FAIL 提交业务失败
        //错误代码 err_code 否 String(32) SYSTEMERROR 列表详见错误码列表
        //错误代码描述 err_code_des 否 String(128) 系统超时 结果信息描述
        //公众账号ID appid 是 String(32) wx8888888888888888 微信分配的公众账号ID
        //商户号 mch_id 是 String(32) 1900000109 微信支付分配的商户号
        //设备号 device_info 否 String(32) 013467007045764 微信支付分配的终端设备号，与下单一致
        //随机字符串 nonce_str 是 String(32) 5K8264ILTKCH16CQ2502SI8ZNMTM67VS 随机字符串，不长于32位
        //签名 sign 是 String(32) 5K8264ILTKCH16CQ2502SI8ZNMTM67VS 签名，详见签名算法
        //微信订单号 transaction_id 是 String(28) 4007752501201407033233368018 微信订单号
        //商户订单号 out_trade_no 是 String(32) 33368018 商户系统内部的订单号
        //商户退款单号 out_refund_no 是 String(64) 121775250 商户退款单号
        //微信退款单号 refund_id 是 String(32) 2007752501201407033233368018 微信退款单号
        //退款金额 refund_fee 是 Int 100 退款总金额,单位为分,可以做部分退款
        //应结退款金额 settlement_refund_fee 否 Int 100 去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额<=申请退款金额
        //标价金额 total_fee 是 Int 100 订单总金额，单位为分，只能为整数，详见支付金额
        //应结订单金额 settlement_total_fee 否 Int 100  去掉非充值代金券金额后的订单总金额，应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额。  
        //标价币种 fee_type 否 String(8) CNY 订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型

        //现金支付金额 cash_fee 是 Int 100 现金支付金额，单位为分，只能为整数，详见支付金额
        //现金支付币种  cash_fee_type 否  String(16)  CNY 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        //现金退款金额 cash_refund_fee 否 Int 100 现金退款金额，单位为分，只能为整数，详见支付金额

        //代金券类型  coupon_type_$n 否  String(8) CASH CASH--充值代金券
//NO_CASH---非充值代金券
//订单使用代金券时有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_0
//代金券退款总金额 coupon_refund_fee 否 Int 100 代金券退款金额<=退款金额，退款金额-代金券或立减优惠退款金额为现金，说明详见代金券或立减优惠
//单个代金券退款金额 coupon_refund_fee_$n 否 Int 100 代金券退款金额<=退款金额，退款金额-代金券或立减优惠退款金额为现金，说明详见代金券或立减优惠
//退款代金券使用数量 coupon_refund_count 否 Int 1 退款代金券使用数量
//退款代金券ID coupon_refund_id_$n 否 String(20)  10000  退款代金券ID, $n为下标，从0开始编号

    }

    /// <summary>
    /// 用户退款申请
    /// </summary>
    public class WeixinRefundAgrs
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
    }
}