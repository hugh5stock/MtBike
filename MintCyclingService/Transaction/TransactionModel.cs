using MintCyclingService.Common;
using System;

namespace MintCyclingService.User
{
    /// <summary>
    /// 当前用户交易记录交易输入参数
    /// </summary>
    public class UserTransaction_PM : Paging_Model
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车锁编号
        /// </summary>
        public string DeviceNo { get; set; }


        ///// <summary>
        ///// 开始时间
        ///// </summary>
        //public DateTime StartTime { get; set; }


        ///// <summary>
        ///// 结束时间
        ///// </summary>
        //public DateTime EndTime { get; set; }

    }



    /// <summary>
    /// 当前用户交易记录或者历史交易记录列表   返回参数模型
    /// </summary>
    public class UserTransactionInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserinfoGuid { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicycleNumber { get; set; }


        /// <summary>
        /// 租车时间
        /// </summary>
        public DateTime? StartTime { get; set; }


        /// <summary>
        /// 还车时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal? TotalAmout { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal? UsableAmount { get; set; }

        /// <summary>
        /// 交易状态   （0手动还车、1自动还车）
        /// </summary>
        public string ReturnStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }


    /// <summary>
    /// 用户历史交易记录输入参数
    /// </summary>
    public class HistoryUserTransaction_PM : Paging_Model
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车锁编号
        /// </summary>
        public string DeviceNo { get; set; }


        /// <summary>
        /// 交易状态(0手动还车1自动还车）
        /// </summary>
        public string ReturnStatus { get; set; }


    }





    /// <summary>
    /// 手动还车详细返回参数模型
    /// </summary>
    public class UserTransactionList_PM
    {
        /// <summary>
        /// OrderGuid
        /// </summary>
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// 交易订单号
        /// </summary>
        public string OutTradeNo { get; set; }


        /// <summary>
        /// 骑行总时间
        /// </summary>
        public decimal TotalMinSpan { get; set; }

        /// <summary>
        /// 骑行总距离
        /// </summary>
        public decimal TotalDistance { get; set; }

        /// <summary>
        /// 消耗的总的卡路里（kg）或者运动成就
        /// </summary>
        public decimal TotalCalorieExpend { get; set; }


        /// <summary>
        /// 骑行节约碳排放量
        /// </summary>
        public decimal TotalCarbon { get; set; }



        /// <summary>
        /// 骑行总费用
        /// </summary>
        public decimal TotalAmount { get; set; }


        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车锁编号
        /// </summary>
        public string DeviceNo { get; set; }




    }

    /// <summary>
    /// 手动还车输入参数模型
    /// </summary>
    public class RetrunBicycle_PM
    {

        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

    }

    #region 用户充值或者退押金实体


    /// <summary>
    /// 用户充押金交易记录输入参数
    /// </summary>
    public class UserDepositRecharge_PM : Paging_Model
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 充值类别：1支付宝；2微信
        /// </summary>
        public int RechargeType { get; set; }

        ///// <summary>
        ///// 切换列表类别：1押金充值记录列表；2余额充值记录列表;3退款交易记录列表
        ///// </summary>
        //public int ClickType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 充值是否成功：1充值成功；2正在充值；
        /// </summary>
        public int IsRecharge { get; set; }

    }

    /// <summary>
    /// 用户充押金交易记录返回参数模型
    /// </summary>
    public class UserDepositRechargeList_OM
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid UserRechargeGuid { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 充值方式：1支付宝；2微信
        /// </summary>
        public String RechargeType { get; set; }

        /// <summary>
        /// 交易类别：1充值；2退款
        /// </summary>
        public string MoneyType { get; set; }

        /// <summary>
        /// 充值是否成功：1充值成功；2正在充值；
        /// </summary>
        public string RechargeStatus { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string Trade_no { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 支付完成日期
        /// </summary>
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }



    }

    #endregion



}