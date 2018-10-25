using Autofac;
using MintCyclingService.BicLock;
using MintCyclingService.ChargingRules;
using MintCyclingService.Cycling;
using MintCyclingService.Supplier;
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
    /// 供应商管理控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class SupplierController : ApiController
    {
        ISupplierService _SupplierService;
        IBicLockService _BicLockService;

        /// <summary>
        /// 初始化单车控制器
        /// </summary>
        public SupplierController()
        {
            _SupplierService = AutoFacConfig.Container.Resolve<ISupplierService>();
            _BicLockService = AutoFacConfig.Container.Resolve<IBicLockService>();
        }


        /// <summary>
        ///添加或者修改供应商信息 complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrUpdateSupplier([FromBody] AddSupplierOrUpdate_PM model)
        {
            return _SupplierService.AddOrUpdateSupplier(model);
        }

        /// <summary>
        ///删除供应商信息 complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteSupplierBySupplierID([FromBody] SupplierDelete_PM model)
        {
            return _SupplierService.DeleteSupplierBySupplierID(model);
        }

        /// <summary>
        /// 根据查询条件搜索供应商管理列表  complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetSupplierInfoListByCondition([FromUri]GetSupplierList_PM model)
        {
            return _SupplierService.GetSupplierInfoList(model);
        }



        /// <summary>
        /// 根据查询条件搜索生产锁管理列表列表  complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetLockSupplierList([FromUri]BicLock_PM model)
        {
            return _BicLockService.GetLockSupplierList(model);
        }



    }
}