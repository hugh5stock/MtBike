using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.ChargingRules
{
    /// <summary>
    /// 后台计费规则列表 返回参数模型
    /// </summary>
    public class GetChargingRuleList_OM 
    {
        /// <summary>
        /// 编号
        /// </summary>

        public int RuleID { get; set; }

        /// <summary>
        /// 免费时长（分钟）
        /// </summary>
        public int? FreeTime { get; set; }

        /// <summary>
        /// 计费单位时间（分钟）
        /// </summary>
        public int? ChargingTime { get; set; }

        /// <summary>
        /// 计费价格单位(元)
        /// </summary>
        public decimal? Price { get; set; }


        /// <summary>
        /// 单日收费上限（元）
        /// </summary>
        public decimal? PriceMax { get; set; }

        /// <summary>
        ///  描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        ///  创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
 

    /// <summary>
    /// 计费规则输入参数模型
    /// </summary>
    public class ChargingRule_PM
    {
        /// <summary>
        /// 编号
        /// </summary>

        public int RuleID { get; set; }

        /// <summary>
        /// 免费时长（分钟）
        /// </summary>
        public int FreeTime { get; set; }

        /// <summary>
        /// 计费单位时间（分钟）
        /// </summary>
        public int ChargingTime { get; set; }

        /// <summary>
        /// 计费价格单位(元)
        /// </summary>
        public decimal Price { get; set; }


        /// <summary>
        /// 单日收费上限（元）
        /// </summary>
        public decimal PriceMax { get; set; }

        /// <summary>
        ///  描述
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 计费规则输入参数模型
    /// </summary>
    public class DeleteChargingRule_PM
    {
        /// <summary>
        /// 编号
        /// </summary>

        public int RuleID { get; set; }


        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid AdminGuid { get; set; }


    }


}
