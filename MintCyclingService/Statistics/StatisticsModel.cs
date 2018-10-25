using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Statistics
{
    /// <summary>
    /// 统计数据返回参数模型
    /// </summary>
    public class GetDataModel_OM
    {
        /// <summary>
        /// 平台用户注册总数
        /// </summary>

        public int TotalUserCount { get; set; }

        /// <summary>
        /// 本月用户注册总数
        /// </summary>

        public int TotalMonthUserCount { get; set; }


        /// <summary>
        /// 本年用户注册总数
        /// </summary>

        public int TotalYearUserCount { get; set; }

        /// <summary>
        /// 总故障数
        /// </summary>

        public int TotalBreakCount { get; set; }


        /// <summary>
        /// 本月故障总数
        /// </summary>

        public int TotalMonthBreakCount { get; set; }

        /// <summary>
        /// 本年故障数
        /// </summary>

        public int TotalYearBreakCount { get; set; }


        /// <summary>
        /// 押金充值总金额
        /// </summary>

        public decimal TotalDepositAmount { get; set; }



        /// <summary>
        /// 本月押金充值总金额
        /// </summary>

        public decimal TotalMonthDepositAmount { get; set; }


        /// <summary>
        /// 本年押金充值总金额
        /// </summary>

        public decimal TotalYearDepositAmount { get; set; }



        /// <summary>
        /// 余额充值总金额
        /// </summary>

        public decimal TotalAccountAmount { get; set; }


        /// <summary>
        /// 本月余额充值总金额
        /// </summary>

        public decimal TotalMonthAccountAmount { get; set; }

        /// <summary>
        /// 本年余额充值总金额
        /// </summary>

        public decimal TotalYearAccountAmount { get; set; }


        /// <summary>
        /// 总的车辆数
        /// </summary>

        public int TotalBicycleCount { get; set; }

        /// <summary>
        /// 车辆开锁总数
        /// </summary>

        public int TotalOpenCount { get; set; }

        /// <summary>
        /// 车辆总故障数
        /// </summary>

        public int TotalRepairCount { get; set; }

        /// <summary>
        /// 电子围栏总数
        /// </summary>

        public int TotalElectronicCount { get; set; }


        /// <summary>
        /// 单车开锁率=当天的开锁次数/车辆数总数
        /// </summary>

        public decimal OpenRate { get; set; }


    }


    /// <summary>
    /// 返回集合
    /// </summary>
    public class GetDataList_OM
    {
        public List<GetDataModel_OM> dataList { get; set; }

    }

    /// <summary>
    /// 按照条件查询统计
    /// </summary>
    public class GetCondition_PM
    {
        /// <summary>
        /// 查询的类别1当天；2昨天；3本月；4上月；5本年；6去年
        /// </summary>

        public int TypeID { get; set; }


    }


    /// <summary>
    /// 按照条件查询统计返回输出模型
    /// </summary>
    public class GetCondition_OM
    {
        /// <summary>
        /// 总收入=查询累计充值押金总金额+查询累计充值余额总金额;
        /// </summary>

        public decimal TotalAmounts { get; set; }
 
        /// <summary>
        /// 查询累计充值押金总金额
        /// </summary>

        public decimal TotalDAmount { get; set; }

        /// <summary>
        /// 查询累计充值余额总金额
        /// </summary>

        public decimal TotalAAmount { get; set; }


        /// <summary>
        /// 按条件查询押金充值总金额
        /// </summary>

        public decimal TotalDepositAmount { get; set; }


        /// <summary>
        /// 按条件查询余额充值总金额
        /// </summary>

        public decimal TotalAccountAmount { get; set; }

        /// <summary>
        /// 用户退押金总金额
        /// </summary>

        public decimal TotalReturnAmount { get; set; }

 

    }



    /// <summary>
    /// 按条件查询返回集合
    /// </summary>
    public class GetDataCondition_OM
    {
        public List<GetCondition_OM> ConditionList { get; set; }

    }


}
