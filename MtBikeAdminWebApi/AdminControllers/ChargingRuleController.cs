using Autofac;
using MintCyclingService.Admin;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.ChargingRules;
using MintCyclingService.Utils;
using MtBikeAdminWebApi;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 计费规则控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class ChargingRuleController : ApiController
    {
        private IAdminService _adminService;
        private ResultModel _adminModel = null;
        private IAdminAccessCodeService _adminAccessCodeService;
        private IChargingRuleService _chargingService;
        
        /// <summary>
        /// 计费规则构造函数
        /// </summary>
        public ChargingRuleController()
        {
            _adminService = AutoFacConfig.Container.Resolve<IAdminService>();
            _adminModel = WebApiApplication.GetAdminUserData();
            _adminAccessCodeService = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();
            _chargingService = AutoFacConfig.Container.Resolve<IChargingRuleService>();
        }


        /// <summary>
        /// 计费规则列表 complete TOM
        /// DATE：2017-02-26
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetChargingRuleList()
        {
            return _chargingService.GetChargingRuleList();
        }




        /// <summary>
        /// 添加或者编辑计费规则  complete TOM
        /// DATE：2017-02-26
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddChargingRule([FromBody]ChargingRule_PM model)
        {
            return _chargingService.AddChargingRule(model);
        }


        /// <summary>
        ///删除计费规则信息 complete TOM
        ///DATE：2017-05-26
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteChargingRuleByServiceGuid([FromBody] DeleteChargingRule_PM model)
        {
            return _chargingService.DeleteChargingRuleByServiceGuid(model);
        }

    }
}