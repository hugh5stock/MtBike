using MintCyclingService.Utils;
using Autofac;
using MintCyclingService.ServicePerson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using MintCyclingService.Statistics;
using MtBikeAdminWebApi.Filter;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 运营统计控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class StatisticsController : ApiController
    {
        private IStatisticsService _StatisticsService;


        /// <summary>
        /// 维护人员构造函数
        /// </summary>
        public StatisticsController()
        {
            _StatisticsService = AutoFacConfig.Container.Resolve<IStatisticsService>();
        }


        /// <summary>
        /// 平台运营统计数据  complete TOM
        /// DATE：2017-02-27
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetStatisticsData()
        {
            return _StatisticsService.GetStatisticsData();
        }


        /// <summary>
        /// 根据条件搜索平台收益情况统计数据  complete TOM
        /// DATE：2017-02-27
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetStatisticsDataByCondition([FromUri]GetCondition_PM model)
        {
            return _StatisticsService.GetStatisticsDataByCondition(model);
        }






    }
}