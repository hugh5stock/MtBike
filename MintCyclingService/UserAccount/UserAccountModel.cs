using System;

namespace MintCyclingService.UserAccount
{
    /// <summary>
    /// 账户钱包余额 输入模型
    /// </summary>
    public class MyAccountUsableAmount_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
    }

    /// <summary>
    /// 账户钱包余额  输出模型
    /// </summary>
    public class MyAccountUsableAmount_OM
    {
        ///// <summary>
        ///// 押金的Guid
        ///// </summary>
        //public Guid DepositGuid { get; set; }
        
        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal? UsableAmount { get; set; }

        /// <summary>
        /// 押金
        /// </summary>
        public decimal? YAmount { get; set; }


    }

  



    /// <summary>
    /// 用户充押金 输入模型
    /// </summary>
    public class AddDeposit_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 押金
        /// </summary>
        public decimal? Amount { get; set; }


        /// <summary>
        /// 充值的方式：1支付宝；2微信
        /// </summary>
        public int RechargeType { get; set; }

        /// <summary>
        /// 充值是否成功 0:表示充值失败；1表示充值成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 唯一的订单号码
        /// </summary>
        public string Out_trade_no { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string Trade_no { get; set; }

 
        /// <summary>
        /// 签名：异步返回结果的验签
        /// </summary>
        public string Sign { get; set; }


        /// <summary>
        /// 签名类型:商户生成签名字符串所使用的签名算法类型，目前支持RSA2和RSA，推荐使用RSA2
        /// </summary>
        public string Sign_type { get; set; }

    }


    /// <summary>
    /// 退押金 输入模型
    /// </summary>
    public class RefundDeposit_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 充值的方式：1支付宝；2微信
        /// </summary>
        public int RechargeType { get; set; }

        /// <summary>
        /// 退款是否成功 0:表示充值失败；1表示充值成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 唯一的订单号码
        /// </summary>
        public string Out_trade_no { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string Trade_no { get; set; }


        /// <summary>
        /// 签名：异步返回结果的验签
        /// </summary>
        public string Sign { get; set; }


        /// <summary>
        /// 签名类型:商户生成签名字符串所使用的签名算法类型，目前支持RSA2和RSA，推荐使用RSA2
        /// </summary>
        public string Sign_type { get; set; }
    }



    /// <summary>
    /// 用户充值押金和退押金 输入模型
    /// </summary>
    public class UserReDepositUpdate_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 充值的方式：1支付宝；2微信
        /// </summary>
        public int RechargeType { get; set; }

        /// <summary>
        /// 退款是否成功 0:表示充值失败；1表示充值成功
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 唯一的订单号码
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime gmt_payment { get; set; }
 
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string trade_no { get; set; }


        /// <summary>
        /// 签名：异步返回结果的验签
        /// </summary>
        public string sign { get; set; }


        /// <summary>
        /// 签名类型:商户生成签名字符串所使用的签名算法类型，目前支持RSA2和RSA，推荐使用RSA2
        /// </summary>
        public string sign_type { get; set; }


        /// <summary>
        /// 押金
        /// </summary>
        public decimal? total_amount { get; set; }


    }

}