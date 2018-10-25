using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.ChargingRules
{
    public interface IChargingRuleService
    {

        /// <summary>
        ///计费规则列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetChargingRuleList();


        /// <summary>
        /// 添加计费规则
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddChargingRule(ChargingRule_PM model);


        /// <summary>
        /// 删除计费规则信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteChargingRuleByServiceGuid(DeleteChargingRule_PM Model);

    }
}