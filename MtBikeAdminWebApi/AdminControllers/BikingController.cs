using Autofac;
using MintCyclingService.BicLock;
using MintCyclingService.ChargingRules;
using MintCyclingService.Cycling;
using MintCyclingService.Remote;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 车辆管理控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class BikingController : ApiController
    {
        ICyclingService _cyclingService;
        IChargingRuleService _chargingRuleService;
        IBicLockService _BicLockService;
        IRemoteService _RemoteService;

        /// <summary>
        /// 初始化单车控制器
        /// </summary>
        public BikingController()
        {
            _cyclingService = AutoFacConfig.Container.Resolve<ICyclingService>();
            _chargingRuleService = AutoFacConfig.Container.Resolve<IChargingRuleService>();
            _BicLockService = AutoFacConfig.Container.Resolve<IBicLockService>();
            _RemoteService = AutoFacConfig.Container.Resolve<IRemoteService>();
 
        }


        /// <summary>
        /// 添加或者修改车辆基本信息 complete TOM
        /// DATE：2017-02-16
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrUpdateBicycleBase([FromBody] AddBicycleOrUpdate_PM model)
        {
            return _BicLockService.AddAndUpdateBicycleBase(model);
        }


        /// <summary>
        /// 删除车辆基本信息 complete TOM
        /// DATE：2017-02-15
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteBicycleBaseByBicycleGuid([FromBody] BicycleDelete_PM model)
        {
            return _BicLockService.DeleteBicycleByBicycleGuid(model);
        }

        /// <summary>
        /// 根据查询条件搜索后台车辆列表  complete TOM
        /// DATE：2017-02-15
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBicycleBaseListByCondition([FromUri]GetBicycleBaseList_PM model)
        {
            return _BicLockService.GetBicycleBaseList(model);
        }


        /// <summary>
        /// 批量分配客户车锁 complete TOM
        /// DATE：2017-06-20
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrUpdateCustomerBicycleLock([FromBody] AddCustomerBicycleLockJson_PM model)
        {
            return _BicLockService.AddOrUpdateCustomerBicycleLock(model);
        }

        /// <summary>
        /// 根据查询条件搜索分配客户车锁列表  complete TOM
        /// DATE：2017-06-21
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel CustomerBicycleLockList([FromUri]CustomerBicycleLocklist_PM model)
        {
            return _BicLockService.CustomerBicycleLockList(model);
        }

        /// <summary>
        /// 批量生成车牌号  complete TOM
        /// DATE：2017-06-02
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetBicycleProduceBicNumber([FromBody]GetRandomNumModel_PM model)
        {
            return _BicLockService.GetBicycleProduceBicNumber(model);

        }
 
        /// <summary>
        /// 根据查询条件搜索批量生成车牌号列表  complete TOM
        /// DATE：2017-06-02
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBicycleNumberListByCondition([FromUri]GetBicycleNumberList_PM model)
        {
            return _BicLockService.GetBicycleNumberListByCondition(model);
        }




        /// <summary>
        /// 发送远程升级锁程序包  complete TOM
        /// DATE:2017-07-07
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel SendUpgradeBicLockProgram([FromBody]RemoteUpgradeBicLockProgram_PM model)
        {
            return _RemoteService.SendUpgradeBicLockProgram(model);
        }

        /// <summary>
        /// 接受远程升级锁程序包完成后锁的应答  complete TOM
        /// DATE:2017-07-18
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel ReceiveUpgradeFinish([FromBody]ReceiveUpgradeFinish_PM model)
        {
            return _RemoteService.ReceiveUpgradeFinish(model);
        }


    }
}
