using System;

namespace MintCyclingService.UserRecharge
{
    /// <summary>
    /// 用户充值余额记录 输入参数模型
    /// </summary>
    public class UserRechargeRecordModel_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 金额
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
        /// 支付宝交易号
        /// </summary>
        public string Trade_no { get; set; }

        /// <summary>
        /// 商户唯一的订单号码
        /// </summary>
        public string Out_trade_no { get; set; }

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
    /// 暂时没用
    /// </summary>
    public class AccountInfoModel_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 充值方式
        /// </summary>
        public int RechargeType { get; set; }

        /// <summary>
        /// 充值是否成功
        /// </summary>
        public int Status { get; set; }
    }



    







}