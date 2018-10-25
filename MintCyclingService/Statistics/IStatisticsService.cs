using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.Statistics
{
    public interface IStatisticsService
    {

        /// <summary>
        /// 平台运营统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetStatisticsData();



        /// <summary>
        /// 根据条件搜索平台收益情况统计数据 
        /// </summary>
        /// <returns></returns>
        ResultModel GetStatisticsDataByCondition(GetCondition_PM model);


        }
}